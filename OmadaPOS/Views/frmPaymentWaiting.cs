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
    private static readonly Color _overlay  = Color.FromArgb(200, AppColors.NavyDark.R,  AppColors.NavyDark.G,  AppColors.NavyDark.B);   // NavyDark 78% opaque
    private static readonly Color _cardBg   = Color.FromArgb(255, AppColors.NavyBase.R,  AppColors.NavyBase.G,  AppColors.NavyBase.B);   // NavyBase solid
    private static readonly Color _cardBord = Color.FromArgb(60,  255, 255, 255);
    private static readonly Color _spinFg   = AppColors.AccentGreen;
    private static readonly Color _spinBg   = Color.FromArgb(40, AppColors.AccentGreen.R, AppColors.AccentGreen.G, AppColors.AccentGreen.B); // faint green ring

    private static readonly Font _fontType    = new("Segoe UI",  11F, FontStyle.Regular);
    private static readonly Font _fontAmount  = new("Consolas",  28F, FontStyle.Bold);
    private static readonly Font _fontMessage = new("Segoe UI",  13F, FontStyle.Regular);
    private static readonly Font _fontTimer   = new("Consolas",  11F, FontStyle.Regular);
    private static readonly Font _fontHint    = new("Segoe UI",   9F, FontStyle.Italic);

    // ── Shared GDI resources (allocated once) ────────────────────────────────
    private static readonly SolidBrush  _overlayBrush = new(_overlay);
    private static readonly StringFormat _sfCenter    = new() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

    private const int TimeoutSeconds = 90;

    // ── State ─────────────────────────────────────────────────────────────────
    private readonly decimal     _amount;
    private readonly PaymentType _paymentType;
    private readonly DateTime    _startTime = DateTime.Now;
    private readonly System.Windows.Forms.Timer _timer;

    private float   _spinAngle       = 0f;
    private Bitmap? _backdrop;
    private bool    _timeoutFired    = false;
    private bool    _cancelRequested = false;
    private Button? _btnCancel;

    /// <summary>
    /// Raised on the UI thread when no response has been received from the
    /// terminal after <see cref="TimeoutSeconds"/> seconds. Subscribers should
    /// inform the user that the terminal timed out.
    /// The form closes itself immediately after raising this event.
    /// </summary>
    public event EventHandler? TimeoutElapsed;

    /// <summary>
    /// Raised when the cashier deliberately clicks "Force Cancel" after waiting.
    /// The caller is responsible for cancelling the payment task and closing the form.
    /// </summary>
    public event EventHandler? UserCancelRequested;

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

            double elapsed = (DateTime.Now - _startTime).TotalSeconds;

            // Show the Force Cancel button after 15 s so accidental taps don't abort.
            if (_btnCancel != null && !_btnCancel.Visible && elapsed >= 15)
                _btnCancel.Visible = true;

            // Auto-close after timeout so the cashier is never permanently stuck.
            if (!_timeoutFired && elapsed >= TimeoutSeconds)
            {
                _timeoutFired = true;
                TimeoutElapsed?.Invoke(this, EventArgs.Empty);
                BeginInvoke(Close);
            }
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

        // "Force Cancel" button — appears at the bottom of the card so the cashier
        // can abort if the terminal becomes unresponsive. It is initially invisible
        // and becomes visible after 15 s to avoid accidental taps at the start.
        _btnCancel = new Button
        {
            Text      = "✕  Force Cancel",
            FlatStyle = FlatStyle.Flat,
            Font      = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = Color.FromArgb(252, 165, 165),  // red-300
            BackColor = Color.FromArgb(60, 220, 38, 38),
            Cursor    = Cursors.Hand,
            Visible   = false,
            Size      = new Size(140, 30),
            Location  = new Point(
                _card.Left + (_card.Width - 140) / 2,
                _card.Bottom + 12),
        };
        _btnCancel.FlatAppearance.BorderColor = Color.FromArgb(100, 220, 38, 38);
        _btnCancel.FlatAppearance.BorderSize  = 1;
        _btnCancel.Click += (_, _) =>
        {
            _cancelRequested = true;
            UserCancelRequested?.Invoke(this, EventArgs.Empty);
            BeginInvoke(Close);
        };
        Controls.Add(_btnCancel);

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
        // Allow close only if triggered programmatically or by the Force Cancel button.
        // A plain UserClosing (Alt+F4, taskbar) is ignored so the overlay can't be
        // accidentally dismissed mid-transaction.
        if (e.CloseReason == CloseReason.UserClosing && !_cancelRequested)
            e.Cancel = true;
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
        g.DrawString("Processing payment…  Follow the instructions on the terminal.",
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
        g.DrawString("Do not press any button. Transaction is in progress.",
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
