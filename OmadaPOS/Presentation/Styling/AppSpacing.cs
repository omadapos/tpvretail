using System.Windows.Forms;

namespace OmadaPOS.Presentation.Styling;

/// <summary>
/// Spacing scale for the POS Design System.
/// Based on a 4-point grid — all values are multiples of 4.
/// Use these constants instead of Padding(n) or Margin(n) literals.
/// </summary>
public static class AppSpacing
{
    // ─── Base scale (pixels) ─────────────────────────────────────────────────

    /// <summary>4px — tight spacing between tightly related elements.</summary>
    public const int XS  = 4;

    /// <summary>8px — standard inner padding for compact controls.</summary>
    public const int SM  = 8;

    /// <summary>12px — default padding inside panels.</summary>
    public const int MD  = 12;

    /// <summary>16px — comfortable padding, section separation.</summary>
    public const int LG  = 16;

    /// <summary>24px — generous padding, card inner spacing.</summary>
    public const int XL  = 24;

    /// <summary>32px — large section gap.</summary>
    public const int XXL = 32;

    // ─── Common Padding presets ───────────────────────────────────────────────

    /// <summary>Uniform 0 — no padding.</summary>
    public static readonly Padding None       = new(0);

    /// <summary>4px all sides.</summary>
    public static readonly Padding Tight      = new(XS);

    /// <summary>8px all sides — compact panels.</summary>
    public static readonly Padding Compact    = new(SM);

    /// <summary>Horizontal 20px, vertical 8px — totals panel inner.</summary>
    public static readonly Padding TotalsInner = new(XL, SM, XL, SM);

    /// <summary>6px horizontal, 4px vertical — panel margins in grid.</summary>
    public static readonly Padding PanelMargin = new(6, XS, 6, XS);

    /// <summary>10px horizontal, 6px vertical — summary panel inner.</summary>
    public static readonly Padding SummaryInner = new(10, 6, 10, 6);

    /// <summary>6px all sides — payment column inner padding.</summary>
    public static readonly Padding PaymentColumn = new(6, 6, 6, XS);

    /// <summary>4px horizontal, 2px vertical — payment buttons padding.</summary>
    public static readonly Padding ButtonGroup  = new(XS, 2, XS, 6);

    /// <summary>6px all sides — weight/scale section.</summary>
    public static readonly Padding ScaleSection = new(6);

    /// <summary>14px left, 0 others — header title indent.</summary>
    public static readonly Padding HeaderTitle  = new(14, 0, 0, 0);

    /// <summary>4px horizontal, 5px vertical — scan input margin.</summary>
    public static readonly Padding ScanMargin   = new(XS, 5, XS, 5);

    /// <summary>16px horizontal, 8px vertical — button inner padding.</summary>
    public static readonly Padding ButtonInner  = new(LG, SM, LG, SM);
}
