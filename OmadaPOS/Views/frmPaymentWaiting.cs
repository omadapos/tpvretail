using OmadaPOS.Libreria.Utils;

namespace OmadaPOS.Views;

/// <summary>
/// Full-screen overlay shown while the PAX terminal processes a payment.
/// Strategy:
///   1. Captures a screenshot of the owner form.
///   2. Darkens it and uses it as the background (simulates "blur").
///   3. Draws a centered card with an animated spinner + amount + elapsed counter.
///   4. Disables all interaction until explicitly closed by the caller.
/// </summary>
public sealed class frmPaymentWaiting : Form
{
    // ── Config ────────────────────────────────────────────────────────────────
    private static readonly Color _overlay  = Color.FromArgb(200, 10, 16, 30);   // dark navy 78% opaque
    private static readonly Color _cardBg   = Color.FromArgb(255, 20, 30, 48);   // #141E30
    private static readonly Color _cardBord = Color.FromArgb(60,  255, 255, 255);
    private static readonly Color _spinFg   = AppColors.AccentGreen;
    private static readonly Color _spinBg   = Color.FromArgb(40,  16, 185, 129); // faint green ring

    private static readonly Font _fontType    = new("Segoe UI",  11F, FontStyle.Regular);
    private static readonly Font _fontAmount  = new("Consolas",  28F, FontStyle.Bold);
    private static readonly Font _fontMessage = new("Segoe UI",  13F, FontStyle.Regular);
    private static readonly Font _fontTimer   = new("Consolas",  11F, FontStyle.Regular);
    private static readonly Font _fontHint    = new("Segoe UI",   9F, FontStyle.Italic);

    // ── Shared GDI resources (allocated once) ────────────────────────────────
    private static readonly SolidBrush  _overlayBrush = new(_overlay);
    private static readonly StringFormat _sfCenter    = new() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

    // ── State ─────────────────────────────────────────────────────────────────
    private readonly decimal     _amount;
    private readonly PaymentType _paymentType;
    private readonly DateTime    _startTime = DateTime.Now;
    private readonly System.Windows.Forms.Timer _timer;

    private float   _spinAngle = 0f;
    private Bitmap? _backdrop;

    // Card geometry + pre-baked shadow paths (set in OnLoad, fixed thereafter)
    private Rectangle _card;
    private (System.Drawing.Drawing2D.GraphicsPath path, SolidBrush brush)[]? _shadowLayers;

    public frmPaymentWaiting(decimal amount, PaymentType paymentType)
    {
        _amount      = amount;
        _paymentType = paymentType;

        FormBorderStyle  = FormBorderStyle.None;
        StartPosition    = FormStartPosition.Manual;
        ShowInTaskbar    = false;
        DoubleBuffered   = true;
        BackColor        = Color.FromArgb(10, 16, 30);   // fallback if screenshot fails
        Cursor           = Cursors.WaitCursor;

        // Spinner + counter — 60 ms ≈ 16 fps, imperceptible on a payment terminal
        _timer = new System.Windows.Forms.Timer { Interval = 60 };
        _timer.Tick += (_, _) =>
        {
            _spinAngle = (_spinAngle + 9f) % 360f;   // 9° per tick keeps same visual speed at 16fps
            Invalidate();
        };
    }

    // ── Lifecycle ─────────────────────────────────────────────────────────────
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Cover the owner form exactly
        if (Owner != null)
        {
            Bounds = Owner.Bounds;
        }
        else
        {
            Bounds = Screen.PrimaryScreen?.Bounds ?? new Rectangle(0, 0, 1024, 768);
        }

        // Card: centered, fixed 480 × 280
        _card = new Rectangle(
            (Width - 480) / 2,
            (Height - 280) / 2,
            480, 280);

        // Pre-bake shadow paths — card geometry is fixed, so we compute once
        _shadowLayers = new (System.Drawing.Drawing2D.GraphicsPath, SolidBrush)[3];
        for (int i = 3; i >= 1; i--)
        {
            var shadow = Rectangle.Inflate(_card, i * 3, i * 3);
            shadow.Offset(0, i * 2);
            _shadowLayers[i - 1] = (RoundedRect(shadow, 18), new SolidBrush(Color.FromArgb(i * 12, 0, 0, 0)));
        }

        // Capture + darken the owner's current state
        _backdrop = CaptureAndDarken();

        _timer.Start();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _timer.Stop();
        _timer.Dispose();
        _backdrop?.Dispose();
        if (_shadowLayers != null)
            foreach (var (path, brush) in _shadowLayers) { path.Dispose(); brush.Dispose(); }
        base.OnFormClosed(e);
    }

    // ── Prevent user from closing the overlay ─────────────────────────────────
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (e.CloseReason == CloseReason.UserClosing)
            e.Cancel = true;   // only the caller (try/finally) may close this
        else
            base.OnFormClosing(e);
    }

    // ── Paint ─────────────────────────────────────────────────────────────────
    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode         = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.TextRenderingHint     = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        g.InterpolationMode     = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

        // 1. Darkened screenshot background
        if (_backdrop != null)
            g.DrawImage(_backdrop, Point.Empty);

        // 2. Full-screen dark overlay on top of screenshot
        g.FillRectangle(_overlayBrush, ClientRectangle);

        // 3. Card shadow
        DrawCardShadow(g);

        // 4. Card body
        using var cardBrush = new SolidBrush(_cardBg);
        using var cardPath  = RoundedRect(_card, 16);
        g.FillPath(cardBrush, cardPath);
        using var cardBorder = new Pen(_cardBord, 1f);
        g.DrawPath(cardBorder, cardPath);

        // 5. Content inside the card
        DrawCardContent(g);
    }

    private void DrawCardShadow(Graphics g)
    {
        if (_shadowLayers == null) return;
        foreach (var (path, brush) in _shadowLayers)
            g.FillPath(brush, path);
    }

    private void DrawCardContent(Graphics g)
    {
        int cx = _card.Left + _card.Width / 2;

        // ── Payment type badge ───────────────────────────────────────────────
        string badge = _paymentType switch
        {
            PaymentType.Credit     => "  CREDIT CARD",
            PaymentType.Debit      => "  DEBIT CARD",
            PaymentType.EBT        => "  EBT",
            PaymentType.EBTBalance => "  EBT BALANCE",
            _                      => "  CARD PAYMENT"
        };
        Color badgeColor = _paymentType switch
        {
            PaymentType.Credit  => AppColors.PaymentCredit,
            PaymentType.Debit   => AppColors.PaymentDebit,
            PaymentType.EBT     => AppColors.PaymentEBT,
            _                   => AppColors.SlateBlue,
        };

        using var badgeBrush = new SolidBrush(Color.FromArgb(30, badgeColor));
        var badgeRect = new Rectangle(_card.Left + 24, _card.Top + 22, _card.Width - 48, 28);
        using var badgePath = RoundedRect(badgeRect, 6);
        g.FillPath(badgeBrush, badgePath);

        using var badgeTextBrush = new SolidBrush(badgeColor);
        g.DrawString(badge, _fontType, badgeTextBrush, badgeRect, _sfCenter);

        // ── Spinner ──────────────────────────────────────────────────────────
        int spinD  = 72;
        var spinR  = new Rectangle(cx - spinD / 2, _card.Top + 62, spinD, spinD);

        using var bgPen = new Pen(_spinBg, 7f) { StartCap = System.Drawing.Drawing2D.LineCap.Round, EndCap = System.Drawing.Drawing2D.LineCap.Round };
        g.DrawEllipse(bgPen, spinR);

        using var fgPen = new Pen(_spinFg, 7f) { StartCap = System.Drawing.Drawing2D.LineCap.Round, EndCap = System.Drawing.Drawing2D.LineCap.Round };
        g.DrawArc(fgPen, spinR, _spinAngle, 100f);   // 100° arc rotating

        // ── Amount ───────────────────────────────────────────────────────────
        int amountY = _card.Top + 148;
        using var amtBrush = new SolidBrush(AppColors.TextWhite);
        g.DrawString(_amount.ToString("C"), _fontAmount, amtBrush,
            new RectangleF(_card.Left, amountY, _card.Width, 40), _sfCenter);

        // ── "Processing…" message ────────────────────────────────────────────
        using var msgBrush = new SolidBrush(AppColors.TextOnDarkSecondary);
        g.DrawString("Procesando pago…  Siga las instrucciones en el terminal.",
            _fontMessage, msgBrush,
            new RectangleF(_card.Left + 16, amountY + 44, _card.Width - 32, 32), _sfCenter);

        // ── Elapsed timer ────────────────────────────────────────────────────
        int elapsed = (int)(DateTime.Now - _startTime).TotalSeconds;
        using var timerBrush = new SolidBrush(AppColors.TextMuted);
        g.DrawString($"{elapsed}s",
            _fontTimer, timerBrush,
            new RectangleF(_card.Left, amountY + 80, _card.Width, 20), _sfCenter);

        // ── Hint ─────────────────────────────────────────────────────────────
        using var hintBrush = new SolidBrush(Color.FromArgb(120, 148, 163, 184));
        g.DrawString("No presione ningún botón. La transacción está en progreso.",
            _fontHint, hintBrush,
            new RectangleF(_card.Left, _card.Bottom - 28, _card.Width, 24), _sfCenter);
    }

    // ── Screenshot + darken ───────────────────────────────────────────────────
    private Bitmap? CaptureAndDarken()
    {
        try
        {
            var bmp = new Bitmap(Width, Height);
            if (Owner != null)
            {
                // Capture only the owner form area
                using var gfx = Graphics.FromImage(bmp);
                gfx.CopyFromScreen(Owner.Location, Point.Empty, Owner.Size);
            }

            // Darken by blending each pixel with the dark overlay colour
            // This is fast enough for a one-time capture on form load.
            using var gfxDark  = Graphics.FromImage(bmp);
            using var darkBrush = new SolidBrush(Color.FromArgb(160, 5, 10, 20));
            gfxDark.FillRectangle(darkBrush, 0, 0, bmp.Width, bmp.Height);

            return bmp;
        }
        catch
        {
            return null;   // fallback: solid dark background only
        }
    }

    // ── GDI+ helper ───────────────────────────────────────────────────────────
    private static System.Drawing.Drawing2D.GraphicsPath RoundedRect(Rectangle r, int radius)
    {
        int d    = radius * 2;
        var path = new System.Drawing.Drawing2D.GraphicsPath();
        path.AddArc(r.X,           r.Y,           d, d, 180, 90);
        path.AddArc(r.Right - d,   r.Y,           d, d, 270, 90);
        path.AddArc(r.Right - d,   r.Bottom - d,  d, d,   0, 90);
        path.AddArc(r.X,           r.Bottom - d,  d, d,  90, 90);
        path.CloseFigure();
        return path;
    }

    // ── Block keyboard while visible ──────────────────────────────────────────
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData) => true;
}
