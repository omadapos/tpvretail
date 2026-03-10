using System.Drawing;

namespace OmadaPOS.Presentation.Styling;

/// <summary>
/// OmadaPOS — "Light Body / Dark Chrome" theme.
///
/// Palette strategy:
///   • App canvas (cart, products, forms)  → clean white / near-white
///   • Navigation chrome (header, popups)  → deep slate-navy
///   • Accent                              → emerald green #10B981
///   • All semantic / payment tokens       → unchanged
/// </summary>
public static class AppColors
{
    // ─────────────────────────────────────────────────────────────────────────
    // BACKGROUNDS & SURFACES  — light canvas
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Main app canvas — near-white blue-gray.</summary>
    public static readonly Color BackgroundPrimary   = Color.FromArgb(248, 250, 252);   // #F8FAFC

    /// <summary>Elevated panels, tab areas — slightly darker canvas.</summary>
    public static readonly Color BackgroundSecondary = Color.FromArgb(241, 245, 249);   // #F1F5F9

    /// <summary>Cards, list surfaces, popup bodies — pure white.</summary>
    public static readonly Color SurfaceCard         = Color.FromArgb(255, 255, 255);   // #FFFFFF

    /// <summary>Muted inputs, alternate rows, hairline dividers on light bg.</summary>
    public static readonly Color SurfaceMuted        = Color.FromArgb(226, 232, 240);   // #E2E8F0

    // ─────────────────────────────────────────────────────────────────────────
    // SLATE NAVY — headers, navigation, popup chrome
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Primary header / nav bar background — deepest slate.</summary>
    public static readonly Color NavyDark   = Color.FromArgb(15,  23,  42);    // #0F172A

    /// <summary>Sub-header / secondary nav background.</summary>
    public static readonly Color NavyBase   = Color.FromArgb(30,  41,  59);    // #1E293B

    /// <summary>Active/hover states inside dark chrome.</summary>
    public static readonly Color NavyLight  = Color.FromArgb(51,  65,  85);    // #334155

    /// <summary>Borders and secondary accents inside dark chrome.</summary>
    public static readonly Color SlateBlue  = Color.FromArgb(71,  85, 105);    // #475569

    // ─────────────────────────────────────────────────────────────────────────
    // EMERALD ACCENT
    // ─────────────────────────────────────────────────────────────────────────

    public static readonly Color AccentGreen      = Color.FromArgb(16,  185, 129);   // #10B981
    public static readonly Color AccentGreenLight = Color.FromArgb(52,  211, 153);   // #34D399
    public static readonly Color AccentGreenDark  = Color.FromArgb(5,   150, 105);   // #059669

    // ─────────────────────────────────────────────────────────────────────────
    // SEMANTIC
    // ─────────────────────────────────────────────────────────────────────────

    public static readonly Color Danger  = Color.FromArgb(239,  68,  68);   // #EF4444
    public static readonly Color Warning = Color.FromArgb(245, 158,  11);   // #F59E0B
    public static readonly Color Info    = Color.FromArgb(37,   99, 235);   // #2563EB

    // ─────────────────────────────────────────────────────────────────────────
    // PAYMENT COLORS
    // ─────────────────────────────────────────────────────────────────────────

    public static readonly Color PaymentCash     = Color.FromArgb(16,  185, 129);   // emerald
    public static readonly Color PaymentCredit   = Color.FromArgb(37,   99, 235);   // blue
    public static readonly Color PaymentDebit    = Color.FromArgb(71,   85, 105);   // slate
    public static readonly Color PaymentEBT      = Color.FromArgb(217, 119,   6);   // amber
    public static readonly Color PaymentGiftCard = Color.FromArgb(124,  58, 237);   // violet
    public static readonly Color PaymentSplit    = Color.FromArgb(8,   145, 178);   // cyan

    // ─────────────────────────────────────────────────────────────────────────
    // TEXT — two contexts: light backgrounds + dark chrome
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Primary body text — on light backgrounds.</summary>
    public static readonly Color TextPrimary   = Color.FromArgb(15,  23,  42);    // #0F172A  near-black

    /// <summary>Secondary body text — on light backgrounds.</summary>
    public static readonly Color TextSecondary = Color.FromArgb(71,  85, 105);    // #475569  medium slate

    /// <summary>Hints, placeholders, captions — on light backgrounds.</summary>
    public static readonly Color TextMuted     = Color.FromArgb(148, 163, 184);   // #94A3B8  light slate

    /// <summary>Text on dark chrome (headers, popups, buttons).</summary>
    public static readonly Color TextWhite     = Color.FromArgb(255, 255, 255);   // #FFFFFF  pure white

    /// <summary>Secondary text on dark chrome.</summary>
    public static readonly Color TextOnDarkSecondary = Color.FromArgb(203, 213, 225);   // #CBD5E1  light slate

    /// <summary>Muted / hint text on dark chrome.</summary>
    public static readonly Color TextOnDarkMuted = Color.FromArgb(148, 163, 184);  // #94A3B8

    /// <summary>Emerald accent text.</summary>
    public static readonly Color TextAccent = Color.FromArgb(16, 185, 129);

    // ─────────────────────────────────────────────────────────────────────────
    // OVERLAYS & SEPARATORS
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Thin separator line ON dark chrome backgrounds (15% white).</summary>
    public static readonly Color SeparatorOnDark  = Color.FromArgb(38,  255, 255, 255);  // 15% white

    /// <summary>Thin separator / border ON light backgrounds (10% black).</summary>
    public static readonly Color SeparatorOnLight = Color.FromArgb(26,    0,   0,   0);  // 10% black

    /// <summary>Card shadow — subtle (10% black).</summary>
    public static readonly Color ShadowSubtle = Color.FromArgb(26,   0,   0,   0);

    /// <summary>Modal overlay.</summary>
    public static readonly Color OverlayLight = Color.FromArgb(200, 248, 250, 252);
}
