using System.Drawing;

namespace OmadaPOS.Presentation.Styling;

/// <summary>
/// Centralised font tokens for the POS Design System.
///
/// Two-font system — optimised for 15" Elo touch POS terminal:
///
///   Segoe UI  — all UI text: labels, buttons, menus, headers, captions.
///               Microsoft's native Windows screen font; ClearType-tuned;
///               supports emoji; no installation required.
///
///   Consolas  — all numeric values: amounts, prices, totals, UPC/barcodes,
///               time displays, keypad input.
///               Monospaced — every digit takes identical width, so
///               currency columns stay perfectly aligned in the cart.
///               No installation required on any Windows machine.
///
/// Scale: base 10pt → labels 11pt → section titles 14pt → totals 20pt bold
/// At 144 DPI (150% scaling, typical 15" Elo) 10pt ≈ 13px physical — adequate for touch.
/// </summary>
public static class AppTypography
{
    // ─── Segoe UI — Display / Headers ────────────────────────────────────────

    /// <summary>Application header / logo bar — 13pt bold.</summary>
    public static readonly Font AppHeader      = new("Segoe UI", 13F, FontStyle.Bold);

    /// <summary>Panel section titles — 14pt semibold.</summary>
    public static readonly Font SectionTitle   = new("Segoe UI", 14F, FontStyle.Bold);

    /// <summary>Popup / dialog title — 16pt bold.</summary>
    public static readonly Font PopupTitle     = new("Segoe UI", 16F, FontStyle.Bold);

    /// <summary>Customer screen welcome greeting — 30pt bold.</summary>
    public static readonly Font Welcome        = new("Segoe UI", 30F, FontStyle.Bold);

    // ─── Segoe UI — Body & Labels ────────────────────────────────────────────

    /// <summary>Tender / Due labels in payment summary — 11pt semibold.</summary>
    public static readonly Font PaymentLabel   = new("Segoe UI", 11F, FontStyle.Bold);

    /// <summary>Cart list view rows, product lists — 11pt regular.</summary>
    public static readonly Font ListItem       = new("Segoe UI", 11F, FontStyle.Regular);

    /// <summary>Cart price/total cells — Consolas monospace for aligned numbers.</summary>
    public static readonly Font ListItemNumber = new("Consolas", 11F, FontStyle.Regular);

    /// <summary>Subtotal / tax row labels — 11pt regular.</summary>
    public static readonly Font RowLabel       = new("Segoe UI", 11F, FontStyle.Regular);

    /// <summary>General body — form labels, inputs — 10pt regular.</summary>
    public static readonly Font Body           = new("Segoe UI", 10F, FontStyle.Regular);

    /// <summary>Smaller body — secondary info — 10pt regular.</summary>
    public static readonly Font BodySmall      = new("Segoe UI", 10F, FontStyle.Regular);

    /// <summary>Column headers in list views — 10pt bold.</summary>
    public static readonly Font ColumnHeader   = new("Segoe UI", 10F, FontStyle.Bold);

    /// <summary>Alphabet / keyboard filter buttons — 10pt bold.</summary>
    public static readonly Font KeyboardButton = new("Segoe UI", 10F, FontStyle.Bold);

    /// <summary>Header icon buttons (⚙ ✕ glyphs) — 18pt.</summary>
    public static readonly Font HeaderIcon     = new("Segoe UI", 18F);

    /// <summary>Power / close icon in header — 20pt.</summary>
    public static readonly Font HeaderIconLg   = new("Segoe UI", 20F);

    /// <summary>Status lines, auxiliary hints — 9pt italic.</summary>
    public static readonly Font Caption        = new("Segoe UI", 9F, FontStyle.Italic);

    /// <summary>Small dropdown chevron glyph — 10pt regular.</summary>
    public static readonly Font ChevronIcon    = new("Segoe UI", 10F, FontStyle.Regular);

    // ─── Consolas — Numeric Display ──────────────────────────────────────────
    // Monospaced: every digit 0-9 is exactly the same width.
    // Essential for currency columns, totals, UPC codes.

    /// <summary>Grand total — customer-facing primary amount — 20pt bold.</summary>
    public static readonly Font AmountGrand    = new("Consolas", 20F, FontStyle.Bold);

    /// <summary>Payment tender / change display — 20pt bold.</summary>
    public static readonly Font AmountDisplay  = new("Consolas", 20F, FontStyle.Bold);

    /// <summary>Customer screen total hero — largest display — 52pt bold.</summary>
    public static readonly Font AmountHero     = new("Consolas", 52F, FontStyle.Bold);

    /// <summary>Weight display on payment and scale panels — 20pt bold.</summary>
    public static readonly Font WeightDisplay  = new("Consolas", 20F, FontStyle.Bold);

    /// <summary>Customer screen weight hero — 32pt bold.</summary>
    public static readonly Font WeightHero     = new("Consolas", 32F, FontStyle.Bold);

    /// <summary>Subtotal and tax values inside totals panel — 14pt regular.</summary>
    public static readonly Font AmountMono     = new("Consolas", 14F, FontStyle.Regular);

    /// <summary>UPC scan text box — 15pt bold for barcode readability.</summary>
    public static readonly Font ScanInput      = new("Consolas", 15F, FontStyle.Bold);

    /// <summary>Keypad numeric display (PIN / GiftCard / quantity entry) — 28pt bold.</summary>
    public static readonly Font KeypadDisplay  = new("Consolas", 28F, FontStyle.Bold);

    /// <summary>Clock / time display on customer screen — 26pt bold.</summary>
    public static readonly Font Clock          = new("Consolas", 26F, FontStyle.Bold);
}
