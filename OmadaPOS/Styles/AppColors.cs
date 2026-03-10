using System.Drawing;
using System.Drawing.Drawing2D;

namespace OmadaPOS.Presentation.Styling;

/// <summary>
/// Light Enterprise Theme — debug mode (all backgrounds forced to light).
/// Accent / semantic / payment colors unchanged.
/// </summary>
public static class AppColors
{
    // ─────────────────────────────────────────────
    // BACKGROUNDS & SURFACES  → todos claros
    // ─────────────────────────────────────────────
    /// <summary>Main canvas — white.</summary>
    public static readonly Color BackgroundPrimary   = Color.FromArgb(248, 250, 252);

    /// <summary>Elevated panels — very light gray.</summary>
    public static readonly Color BackgroundSecondary = Color.FromArgb(241, 245, 249);

    /// <summary>Content areas, list surfaces — light.</summary>
    public static readonly Color SurfaceCard         = Color.FromArgb(255, 255, 255);

    /// <summary>Muted inputs, alternate rows — light slate.</summary>
    public static readonly Color SurfaceMuted        = Color.FromArgb(226, 232, 240);

    // ─────────────────────────────────────────────
    // SLATE — headers, nav  → tonos medios claros
    // ─────────────────────────────────────────────
    /// <summary>Deepest nav — medium slate (not pure white, keeps contrast for headers).</summary>
    public static readonly Color NavyDark   = Color.FromArgb(71,  85, 105);

    /// <summary>Sub-headers — lighter slate.</summary>
    public static readonly Color NavyBase   = Color.FromArgb(100, 116, 139);

    /// <summary>Active states — blue-tinted slate.</summary>
    public static readonly Color NavyLight  = Color.FromArgb(148, 163, 184);

    /// <summary>Borders, secondary text.</summary>
    public static readonly Color SlateBlue  = Color.FromArgb(148, 163, 184);

    // ─────────────────────────────────────────────
    // EMERALD  (sin cambios)
    // ─────────────────────────────────────────────
    public static readonly Color AccentGreen      = Color.FromArgb(16,  185, 129);
    public static readonly Color AccentGreenLight = Color.FromArgb(52,  211, 153);
    public static readonly Color AccentGreenDark  = Color.FromArgb(5,   150, 105);

    // ─────────────────────────────────────────────
    // SEMANTIC  (sin cambios)
    // ─────────────────────────────────────────────
    public static readonly Color Danger  = Color.FromArgb(239,  68,  68);
    public static readonly Color Warning = Color.FromArgb(245, 158,  11);
    public static readonly Color Info    = Color.FromArgb(37,   99, 235);

    // ─────────────────────────────────────────────
    // PAYMENT COLORS  (sin cambios)
    // ─────────────────────────────────────────────
    public static readonly Color PaymentCash     = Color.FromArgb(16,  185, 129);
    public static readonly Color PaymentCredit   = Color.FromArgb(37,   99, 235);
    public static readonly Color PaymentDebit    = Color.FromArgb(71,   85, 105);
    public static readonly Color PaymentEBT      = Color.FromArgb(217, 119,   6);
    public static readonly Color PaymentGiftCard = Color.FromArgb(124,  58, 237);
    public static readonly Color PaymentSplit    = Color.FromArgb(8,   145, 178);

    // ─────────────────────────────────────────────
    // TEXT — ajustado para fondos claros
    // ─────────────────────────────────────────────
    public static readonly Color TextPrimary          = Color.FromArgb(15,  23,  42);
    public static readonly Color TextSecondary        = Color.FromArgb(71,  85, 105);
    public static readonly Color TextMuted            = Color.FromArgb(100, 116, 139);
    public static readonly Color TextWhite            = Color.FromArgb(248, 250, 252);
    public static readonly Color TextAccent           = Color.FromArgb(16,  185, 129);
    public static readonly Color TextOnDarkMuted      = Color.FromArgb(71,  85, 105);
    public static readonly Color TextOnDarkSecondary  = Color.FromArgb(30,  41,  59);

    // ─────────────────────────────────────────────
    // OVERLAYS & SEPARATORS
    // ─────────────────────────────────────────────
    public static readonly Color OverlayLight    = Color.FromArgb(200, 248, 250, 252);
    public static readonly Color SeparatorOnDark = Color.FromArgb(90,   0,   0,   0);
    public static readonly Color ShadowSubtle    = Color.FromArgb(30,   0,   0,   0);
}
