using System.Drawing;

namespace OmadaPOS.Presentation.Styling;

/// <summary>
/// Border color tokens for the POS Design System.
/// Covers both light-surface and dark-surface contexts.
/// </summary>
public static class AppBorders
{
    // ─── On light backgrounds ────────────────────────────────────────────────

    /// <summary>Default panel border on white/light surfaces.</summary>
    public static readonly Color PanelLight  = Color.FromArgb(200, 210, 225);

    /// <summary>Subtle input border on light surfaces.</summary>
    public static readonly Color InputLight  = Color.FromArgb(210, 218, 230);

    /// <summary>Hover/focus border on light surfaces.</summary>
    public static readonly Color Focus       = AppColors.AccentGreen;

    // ─── On dark backgrounds (navy panels) ──────────────────────────────────

    /// <summary>Semi-transparent separator on dark panels (60% alpha white).</summary>
    public static readonly Color SeparatorOnDark = Color.FromArgb(60, 255, 255, 255);

    /// <summary>Accent line at bottom of dark headers.</summary>
    public static readonly Color AccentLine      = AppColors.AccentGreen;

    // ─── Widths ───────────────────────────────────────────────────────────────

    /// <summary>Standard 1px border.</summary>
    public const float Thin   = 1f;

    /// <summary>Accent bottom line — 2px.</summary>
    public const float Accent = 2f;

    /// <summary>Scan panel border — 1.5px.</summary>
    public const float Scan   = 1.5f;
}
