using System.Drawing;

namespace OmadaPOS.Presentation.Styling;

/// <summary>
/// Centralised font tokens for the POS Design System.
///
/// Font families in use:
///   Montserrat — brand headlines, totals, actionable numbers (high visual weight)
///   Segoe UI   — body text, labels, column headers (system-native legibility)
///   Consolas   — monospaced amounts, scan input, UPC codes (digit alignment)
///
/// Scale: Caption (9) → Small (11) → Body (12-13) → Section (14) → Header (18-24)
/// </summary>
public static class AppTypography
{
    // ─── Brand / Display ─────────────────────────────────────────────────────

    /// <summary>Grand total amounts — customer-facing, maximum visual weight.</summary>
    public static readonly Font AmountGrand    = new("Montserrat", 24F, FontStyle.Bold);

    /// <summary>Payment tender / change display — large payment screen numbers.</summary>
    public static readonly Font AmountDisplay  = new("Montserrat", 22F, FontStyle.Bold);

    /// <summary>Customer screen total — largest display amount.</summary>
    public static readonly Font AmountHero     = new("Montserrat", 56F, FontStyle.Bold);

    /// <summary>Weight display on payment and customer panels.</summary>
    public static readonly Font WeightDisplay  = new("Montserrat", 20F, FontStyle.Bold);

    /// <summary>Customer screen weight hero.</summary>
    public static readonly Font WeightHero     = new("Montserrat", 32F, FontStyle.Bold);

    /// <summary>Application header / logo bar.</summary>
    public static readonly Font AppHeader      = new("Montserrat", 13F, FontStyle.Bold);

    // ─── Segoe UI — Section & Body ───────────────────────────────────────────

    /// <summary>Panel section titles with strong emphasis.</summary>
    public static readonly Font SectionTitle   = new("Segoe UI", 14F, FontStyle.Bold);

    /// <summary>Tender / Due labels in payment summary.</summary>
    public static readonly Font PaymentLabel   = new("Segoe UI", 13F, FontStyle.Bold);

    /// <summary>Cart list view rows, product lists.</summary>
    public static readonly Font ListItem       = new("Segoe UI", 13F, FontStyle.Regular);

    /// <summary>Subtotal / tax row labels.</summary>
    public static readonly Font RowLabel       = new("Segoe UI", 14F, FontStyle.Regular);

    /// <summary>General body — form labels, inputs.</summary>
    public static readonly Font Body           = new("Segoe UI", 12F, FontStyle.Regular);

    /// <summary>Smaller body — secondary information.</summary>
    public static readonly Font BodySmall      = new("Segoe UI", 11F, FontStyle.Regular);

    /// <summary>Alphabet / keyboard buttons.</summary>
    public static readonly Font KeyboardButton = new("Segoe UI", 11F, FontStyle.Bold);

    /// <summary>Column headers in list views.</summary>
    public static readonly Font ColumnHeader   = new("Segoe UI", 12F, FontStyle.Bold);

    /// <summary>Header icon buttons (emoji glyphs).</summary>
    public static readonly Font HeaderIcon     = new("Segoe UI", 18F);

    /// <summary>Close / power icon in header.</summary>
    public static readonly Font HeaderIconLg   = new("Segoe UI", 20F);

    /// <summary>Status lines, auxiliary hints.</summary>
    public static readonly Font Caption        = new("Segoe UI", 9F, FontStyle.Italic);

    /// <summary>Welcome / customer greeting.</summary>
    public static readonly Font Welcome        = new("Montserrat", 30F, FontStyle.Bold);

    /// <summary>Customer clock display.</summary>
    public static readonly Font Clock          = new("Montserrat", 26F, FontStyle.Bold);

    // ─── Consolas — Monospaced ────────────────────────────────────────────────

    /// <summary>UPC scan text box — monospaced for code readability.</summary>
    public static readonly Font ScanInput      = new("Consolas", 15F, FontStyle.Bold);

    /// <summary>Subtotal and tax values inside totals panel.</summary>
    public static readonly Font AmountMono     = new("Consolas", 16F, FontStyle.Regular);

    /// <summary>Keypad numeric display (PIN / GiftCard entry).</summary>
    public static readonly Font KeypadDisplay  = new("Consolas", 30F, FontStyle.Bold);

    // ─── Popup headers ───────────────────────────────────────────────────────

    /// <summary>Popup / dialog title — same weight as AppHeader but for modal context.</summary>
    public static readonly Font PopupTitle     = new("Montserrat", 16F, FontStyle.Bold);

    /// <summary>Small dropdown / flyout chevron glyph.</summary>
    public static readonly Font ChevronIcon    = new("Segoe UI", 10F, FontStyle.Regular);
}
