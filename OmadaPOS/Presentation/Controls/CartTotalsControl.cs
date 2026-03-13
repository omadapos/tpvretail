using System.Drawing.Drawing2D;
using System.Drawing.Text;
using OmadaPOS.Presentation.Styling;

namespace OmadaPOS.Presentation.Controls;

/// <summary>
/// Self-contained totals card drawn entirely in a single OnPaint call.
///
/// Design rationale:
///   The previous implementation used a RoundedPanel → TableLayoutPanel → 6 Labels
///   hierarchy, all with BackColor=Transparent. Every Label.Text change triggered
///   WM_ERASEBKGND bubbling through 4 transparent layers, forcing 3 independent
///   paint cycles (one per label) with 4 GDI+ allocations each — visible as white
///   flashes and horizontal artifact lines.
///
///   This control eliminates all children. OnPaint draws background, border, separator
///   and all three text rows in one atomic double-buffered operation. UpdateTotals()
///   calls a single Invalidate() regardless of how many values changed.
/// </summary>
public sealed class CartTotalsControl : UserControl
{
    // ── Static GDI+ resources — one allocation per process lifetime ───────────
    private static readonly SolidBrush _brushBg     = new(AppColors.NavyDark);
    private static readonly SolidBrush _brushShadow = new(Color.FromArgb(50, 0, 0, 0));
    private static readonly SolidBrush _brushMuted  = new(AppColors.TextOnDarkMuted);
    private static readonly SolidBrush _brushSec    = new(AppColors.TextOnDarkSecondary);
    private static readonly SolidBrush _brushWhite  = new(AppColors.TextWhite);
    private static readonly SolidBrush _brushGreen  = new(AppColors.AccentGreen);
    private static readonly Pen        _penBorder   = new(AppColors.SlateBlue, 1f);
    private static readonly Pen        _penSep      = new(Color.FromArgb(55, 255, 255, 255), 1f);

    private static readonly StringFormat _sfLeft  = new()
    {
        Alignment     = StringAlignment.Near,
        LineAlignment = StringAlignment.Center,
        FormatFlags   = StringFormatFlags.NoWrap,
        Trimming      = StringTrimming.EllipsisCharacter,
    };
    private static readonly StringFormat _sfRight = new()
    {
        Alignment     = StringAlignment.Far,
        LineAlignment = StringAlignment.Center,
        FormatFlags   = StringFormatFlags.NoWrap,
        Trimming      = StringTrimming.EllipsisCharacter,
    };

    // ── Cached GraphicsPath — rebuilt only when the control is resized ─────────
    private GraphicsPath? _pathMain;
    private GraphicsPath? _pathShadow;
    private Rectangle     _cachedBounds = Rectangle.Empty;

    // ── Cached display strings ─────────────────────────────────────────────────
    private string _subtotal = "$0.00";
    private string _tax      = "$0.00";
    private string _total    = "$0.00";

    // ── Constructor ───────────────────────────────────────────────────────────
    private CartTotalsControl()
    {
        Dock    = DockStyle.Fill;
        Margin  = new Padding(6, 4, 6, 4);
        Padding = new Padding(0);

        // AllPaintingInWmPaint: background + foreground painted in one message,
        // no separate WM_ERASEBKGND. OptimizedDoubleBuffer: rendered off-screen,
        // blitted atomically — zero visible intermediate states.
        SetStyle(
            ControlStyles.UserPaint            |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw          |
            ControlStyles.SupportsTransparentBackColor, true);

        BackColor = Color.Transparent;
    }

    // ── Paint ─────────────────────────────────────────────────────────────────
    protected override void OnPaint(PaintEventArgs e)
    {
        var g      = e.Graphics;
        var bounds = ClientRectangle;

        g.SmoothingMode      = SmoothingMode.AntiAlias;
        g.TextRenderingHint  = TextRenderingHint.ClearTypeGridFit;
        g.CompositingQuality = CompositingQuality.HighSpeed;

        // Rebuild rounded paths only when the control is resized
        if (bounds != _cachedBounds)
        {
            _cachedBounds = bounds;
            RebuildPaths(bounds);
        }

        // ── 1. Shadow ────────────────────────────────────────────────────────
        if (_pathShadow != null)
            g.FillPath(_brushShadow, _pathShadow);

        // ── 2. Background + border ───────────────────────────────────────────
        if (_pathMain != null)
        {
            g.FillPath(_brushBg,     _pathMain);
            g.DrawPath(_penBorder,   _pathMain);
        }

        // ── 3. Text content ──────────────────────────────────────────────────
        // Layout mirrors the original TableLayoutPanel:
        //   Padding  18px left/right, 10px top, 6px bottom
        //   Row 0    Subtotal  38px
        //   Row 1    Tax       34px
        //   Hairline separator
        //   Row 2    TOTAL     fills remaining

        const int px   = 18;
        const int pt   = 10;
        const int rH0  = 38;   // subtotal row height
        const int rH1  = 34;   // tax row height

        int areaX = bounds.X + px;
        int areaW = bounds.Width  - px * 2;
        int y0    = bounds.Y + pt;
        int col1W = (int)(areaW * 0.40f);   // 40% label | 60% value  (subtotal/tax)

        // ── Subtotal ─────────────────────────────────────────────────────────
        var lR0 = new RectangleF(areaX,          y0,      col1W,          rH0);
        var vR0 = new RectangleF(areaX + col1W,  y0,      areaW - col1W,  rH0);
        g.DrawString("Subtotal",  AppTypography.RowLabel,  _brushMuted, lR0, _sfLeft);
        g.DrawString(_subtotal,   AppTypography.AmountMono, _brushSec,  vR0, _sfRight);

        // ── Tax ───────────────────────────────────────────────────────────────
        var lR1 = new RectangleF(areaX,          y0 + rH0,  col1W,          rH1);
        var vR1 = new RectangleF(areaX + col1W,  y0 + rH0,  areaW - col1W,  rH1);
        g.DrawString("Tax",  AppTypography.RowLabel,   _brushMuted, lR1, _sfLeft);
        g.DrawString(_tax,   AppTypography.AmountMono, _brushMuted, vR1, _sfRight);

        // ── Hairline separator ────────────────────────────────────────────────
        int sepY = y0 + rH0 + rH1 + 2;
        g.DrawLine(_penSep, areaX, sepY, areaX + areaW, sepY);

        // ── TOTAL (hero) ──────────────────────────────────────────────────────
        int totalY = sepY + 6;
        int totalH = Math.Max(bounds.Bottom - totalY - 6, 0);
        int col1TW = (int)(areaW * 0.38f);   // 38% "TOTAL" | 62% amount

        var lR2 = new RectangleF(areaX,           totalY, col1TW,          totalH);
        var vR2 = new RectangleF(areaX + col1TW,  totalY, areaW - col1TW,  totalH);
        g.DrawString("TOTAL",  AppTypography.SectionTitle, _brushWhite, lR2, _sfLeft);
        g.DrawString(_total,   AppTypography.AmountGrand,  _brushGreen, vR2, _sfRight);
    }

    // ── Path management ───────────────────────────────────────────────────────
    private void RebuildPaths(Rectangle b)
    {
        _pathMain?.Dispose();
        _pathShadow?.Dispose();

        int r = AppRadii.Panel;
        _pathMain   = BuildRoundedPath(new Rectangle(b.X,     b.Y,     b.Width - 3, b.Height - 3), r);
        _pathShadow = BuildRoundedPath(new Rectangle(b.X + 3, b.Y + 3, b.Width - 6, b.Height - 6), r);
    }

    private static GraphicsPath BuildRoundedPath(Rectangle rect, int radius)
    {
        var path     = new GraphicsPath();
        int diameter = radius * 2;
        var arc      = new Rectangle(rect.X, rect.Y, diameter, diameter);

        path.AddArc(arc, 180, 90);
        arc.X = rect.Right - diameter;
        path.AddArc(arc, 270, 90);
        arc.Y = rect.Bottom - diameter;
        path.AddArc(arc, 0, 90);
        arc.X = rect.Left;
        path.AddArc(arc, 90, 90);
        path.CloseFigure();
        return path;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _pathMain?.Dispose();
            _pathShadow?.Dispose();
        }
        base.Dispose(disposing);
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Inserts a new <see cref="CartTotalsControl"/> into the specified
    /// TableLayoutPanel cell, removing any previous occupant.
    /// </summary>
    public static CartTotalsControl Attach(TableLayoutPanel parent, int column, int row)
    {
        ArgumentNullException.ThrowIfNull(parent);

        var existing = parent.GetControlFromPosition(column, row);
        if (existing != null)
            parent.Controls.Remove(existing);

        var ctrl = new CartTotalsControl();
        parent.Controls.Add(ctrl, column, row);
        return ctrl;
    }

    /// <summary>
    /// Updates the three displayed amounts.
    /// No-ops when all values are identical to what is already painted — zero redraws.
    /// When something changes, issues a single Invalidate() → one OnPaint call.
    /// </summary>
    public void UpdateTotals(decimal subtotal, decimal tax, decimal total)
    {
        var s = subtotal.ToString("C");
        var t = tax.ToString("C");
        var g = total.ToString("C");

        if (s == _subtotal && t == _tax && g == _total) return;

        _subtotal = s;
        _tax      = t;
        _total    = g;

        Invalidate();   // one call → one OnPaint → one blit
    }

    /// <summary>Resets amounts to $0.00. Triggers a single repaint only if needed.</summary>
    public void ResetTotals(bool includeGrandTotal)
    {
        bool changed = false;
        if (_subtotal != "$0.00") { _subtotal = "$0.00"; changed = true; }
        if (_tax      != "$0.00") { _tax      = "$0.00"; changed = true; }
        if (includeGrandTotal && _total != "$0.00") { _total = "$0.00"; changed = true; }
        if (changed) Invalidate();
    }
}
