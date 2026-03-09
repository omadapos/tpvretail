using System.Drawing;
using System.Drawing.Drawing2D;

namespace OmadaPOS.Presentation.Styling;

/// <summary>
/// Dark Enterprise Theme — Square POS / Shopify POS inspired
/// Rule 60-30-10
/// 60% Deep slate   → backgrounds and surfaces   (#0F172A → #1E293B)
/// 30% Mid slate    → panels, structure, cards    (#334155 → #475569)
/// 10% Emerald blue → actions, CTA, highlights    (#10B981 / #2563EB)
///
/// Reference palette: Tailwind CSS slate + emerald, adapted for POS supermercado.
/// Inspirations: Square POS, Shopify POS, Toast POS, Lightspeed Retail.
/// </summary>
public static class AppColors
{
    // ─────────────────────────────────────────────
    // 60% — BACKGROUNDS & SURFACES  (dark base)
    // ─────────────────────────────────────────────
    /// <summary>#0F172A — slate-900 — deepest background (main canvas).</summary>
    public static readonly Color BackgroundPrimary   = Color.FromArgb(15,  23,  42);

    /// <summary>#1E293B — slate-800 — elevated panels, cards.</summary>
    public static readonly Color BackgroundSecondary = Color.FromArgb(30,  41,  59);

    /// <summary>#1E293B — same as secondary — content areas, list surfaces.</summary>
    public static readonly Color SurfaceCard         = Color.FromArgb(30,  41,  59);

    /// <summary>#334155 — slate-700 — muted inputs, alternate rows, dividers.</summary>
    public static readonly Color SurfaceMuted        = Color.FromArgb(51,  65,  85);

    // ─────────────────────────────────────────────
    // 30% — SLATE  (structure, headers, nav)
    // ─────────────────────────────────────────────
    /// <summary>#0F172A — slate-900 — deepest nav / header background.</summary>
    public static readonly Color NavyDark   = Color.FromArgb(15,  23,  42);

    /// <summary>#1E293B — slate-800 — sub-headers, panel sections.</summary>
    public static readonly Color NavyBase   = Color.FromArgb(30,  41,  59);

    /// <summary>#2D4A72 — blue-tinted slate — active states, hover panels.</summary>
    public static readonly Color NavyLight  = Color.FromArgb(45,  74, 114);

    /// <summary>#94A3B8 — slate-400 — borders, secondary text, quiet labels.</summary>
    public static readonly Color SlateBlue  = Color.FromArgb(148, 163, 184);

    // ─────────────────────────────────────────────
    // 10% — EMERALD  (primary action / success)
    // ─────────────────────────────────────────────
    /// <summary>#10B981 — emerald-500 — CTA, cash, confirm, positive.</summary>
    public static readonly Color AccentGreen      = Color.FromArgb(16,  185, 129);

    /// <summary>#34D399 — emerald-400 — hover / success light.</summary>
    public static readonly Color AccentGreenLight = Color.FromArgb(52,  211, 153);

    /// <summary>#059669 — emerald-600 — pressed / active deep.</summary>
    public static readonly Color AccentGreenDark  = Color.FromArgb(5,   150, 105);

    // ─────────────────────────────────────────────
    // SEMANTIC FUNCTIONAL COLORS
    // ─────────────────────────────────────────────
    /// <summary>#EF4444 — red-500 — danger, cancel, delete.</summary>
    public static readonly Color Danger  = Color.FromArgb(239,  68,  68);

    /// <summary>#F59E0B — amber-500 — warning, EBT balance, weighted items.</summary>
    public static readonly Color Warning = Color.FromArgb(245, 158,  11);

    /// <summary>#2563EB — blue-600 — info, primary action, credit.</summary>
    public static readonly Color Info    = Color.FromArgb(37,   99, 235);

    // ─────────────────────────────────────────────
    // PAYMENT METHOD COLORS
    // ─────────────────────────────────────────────
    public static readonly Color PaymentCash     = Color.FromArgb(16,  185, 129); // emerald-500
    public static readonly Color PaymentCredit   = Color.FromArgb(37,   99, 235); // blue-600
    public static readonly Color PaymentDebit    = Color.FromArgb(71,   85, 105); // slate-600
    public static readonly Color PaymentEBT      = Color.FromArgb(217, 119,   6); // amber-600
    public static readonly Color PaymentGiftCard = Color.FromArgb(124,  58, 237); // violet-600
    public static readonly Color PaymentSplit    = Color.FromArgb(8,   145, 178); // cyan-600

    // ─────────────────────────────────────────────
    // TEXT — light on dark surfaces
    // ─────────────────────────────────────────────
    /// <summary>#F8FAFC — slate-50 — primary text on dark backgrounds.</summary>
    public static readonly Color TextPrimary   = Color.FromArgb(248, 250, 252);

    /// <summary>#94A3B8 — slate-400 — secondary text, labels, captions.</summary>
    public static readonly Color TextSecondary = Color.FromArgb(148, 163, 184);

    /// <summary>#64748B — slate-500 — muted text, placeholder hints.</summary>
    public static readonly Color TextMuted     = Color.FromArgb(100, 116, 139);

    /// <summary>#F8FAFC — same as TextPrimary in dark mode — maximum legibility.</summary>
    public static readonly Color TextWhite     = Color.FromArgb(248, 250, 252);

    /// <summary>#10B981 — emerald accent — prices, totals, positive emphasis.</summary>
    public static readonly Color TextAccent    = Color.FromArgb(16,  185, 129);

    // ─────────────────────────────────────────────
    // TEXT ON DARK PANELS  (navy sections)
    // ─────────────────────────────────────────────
    /// <summary>#94A3B8 — readable muted label on navy panels (subtotals, tax).</summary>
    public static readonly Color TextOnDarkMuted     = Color.FromArgb(148, 163, 184);

    /// <summary>#CBD5E1 — slate-300 — clear secondary value on navy panels.</summary>
    public static readonly Color TextOnDarkSecondary = Color.FromArgb(203, 213, 225);

    // ─────────────────────────────────────────────
    // OVERLAYS & SEPARATORS
    // ─────────────────────────────────────────────
    /// <summary>Semi-transparent white overlay — 200/255 alpha.</summary>
    public static readonly Color OverlayLight    = Color.FromArgb(200, 248, 250, 252);

    /// <summary>Subtle white hairline on dark surfaces — 90/255 alpha.</summary>
    public static readonly Color SeparatorOnDark = Color.FromArgb(90,  255, 255, 255);

    /// <summary>Drop shadow — 50/255 alpha black for depth on dark cards.</summary>
    public static readonly Color ShadowSubtle    = Color.FromArgb(50,    0,   0,   0);
}
