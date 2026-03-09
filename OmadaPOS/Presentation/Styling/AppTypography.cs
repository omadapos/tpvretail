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
/// Scale: Caption (9) → Small (11) → Body (12-13) → Section (14) → Display (20-56)
/// </summary>
public static class AppTypography
{
    // ─── Segoe UI — Display / Headers ────────────────────────────────────────

    /// <summary>Application header / logo bar — 13px bold.</summary>
    public static readonly Font AppHeader      = new("Segoe UI", 13F, FontStyle.Bold);

    /// <summary>Panel section titles — 14px bold.</summary>
    public static readonly Font SectionTitle   = new("Segoe UI", 14F, FontStyle.Bold);

    /// <summary>Popup / dialog title — 16px bold.</summary>
    public static readonly Font PopupTitle     = new("Segoe UI", 16F, FontStyle.Bold);

    /// <summary>Customer screen welcome greeting — 30px bold.</summary>
    public static readonly Font Welcome        = new("Segoe UI", 30F, FontStyle.Bold);

    // ─── Segoe UI — Body & Labels ────────────────────────────────────────────

    /// <summary>Tender / Due labels in payment summary — 13px bold.</summary>
    public static readonly Font PaymentLabel   = new("Segoe UI", 13F, FontStyle.Bold);

    /// <summary>Cart list view rows, product lists — 13px regular.</summary>
    public static readonly Font ListItem       = new("Segoe UI", 13F, FontStyle.Regular);

    /// <summary>Subtotal / tax row labels — 14px regular.</summary>
    public static readonly Font RowLabel       = new("Segoe UI", 14F, FontStyle.Regular);

    /// <summary>General body — form labels, inputs — 12px regular.</summary>
    public static readonly Font Body           = new("Segoe UI", 12F, FontStyle.Regular);

    /// <summary>Smaller body — secondary info — 11px regular.</summary>
    public static readonly Font BodySmall      = new("Segoe UI", 11F, FontStyle.Regular);

    /// <summary>Column headers in list views — 12px bold.</summary>
    public static readonly Font ColumnHeader   = new("Segoe UI", 12F, FontStyle.Bold);

    /// <summary>Alphabet / keyboard filter buttons — 11px bold.</summary>
    public static readonly Font KeyboardButton = new("Segoe UI", 11F, FontStyle.Bold);

    /// <summary>Header icon buttons (⚙ ✕ glyphs) — 18px.</summary>
    public static readonly Font HeaderIcon     = new("Segoe UI", 18F);

    /// <summary>Power / close icon in header — 20px.</summary>
    public static readonly Font HeaderIconLg   = new("Segoe UI", 20F);

    /// <summary>Status lines, auxiliary hints — 9px italic.</summary>
    public static readonly Font Caption        = new("Segoe UI", 9F, FontStyle.Italic);

    /// <summary>Small dropdown chevron glyph — 10px regular.</summary>
    public static readonly Font ChevronIcon    = new("Segoe UI", 10F, FontStyle.Regular);

    // ─── Consolas — Numeric Display ──────────────────────────────────────────
    // Monospaced: every digit 0-9 is exactly the same width.
    // Essential for currency columns, totals, UPC codes.

    /// <summary>Grand total — customer-facing primary amount — 26px bold.</summary>
    public static readonly Font AmountGrand    = new("Consolas", 26F, FontStyle.Bold);

    /// <summary>Payment tender / change display — 22px bold.</summary>
    public static readonly Font AmountDisplay  = new("Consolas", 22F, FontStyle.Bold);

    /// <summary>Customer screen total hero — largest display — 52px bold.</summary>
    public static readonly Font AmountHero     = new("Consolas", 52F, FontStyle.Bold);

    /// <summary>Weight display on payment and scale panels — 20px bold.</summary>
    public static readonly Font WeightDisplay  = new("Consolas", 20F, FontStyle.Bold);

    /// <summary>Customer screen weight hero — 32px bold.</summary>
    public static readonly Font WeightHero     = new("Consolas", 32F, FontStyle.Bold);

    /// <summary>Subtotal and tax values inside totals panel — 16px regular.</summary>
    public static readonly Font AmountMono     = new("Consolas", 16F, FontStyle.Regular);

    /// <summary>UPC scan text box — 15px bold for barcode readability.</summary>
    public static readonly Font ScanInput      = new("Consolas", 15F, FontStyle.Bold);

    /// <summary>Keypad numeric display (PIN / GiftCard / quantity entry) — 28px bold.</summary>
    public static readonly Font KeypadDisplay  = new("Consolas", 28F, FontStyle.Bold);

    /// <summary>Clock / time display on customer screen — 26px bold.</summary>
    public static readonly Font Clock          = new("Consolas", 26F, FontStyle.Bold);
}
