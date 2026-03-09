using System.Drawing;

namespace OmadaPOS.Presentation.Styling;

/// <summary>
/// Border color tokens for the POS Design System — dark enterprise theme.
/// All borders are calibrated for contrast on dark (#0F172A / #1E293B) surfaces.
/// </summary>
public static class AppBorders
{
    // ─── On dark panel surfaces ──────────────────────────────────────────────

    /// <summary>Standard panel border — #475569 slate-600 — visible on dark cards.</summary>
    public static readonly Color PanelLight  = Color.FromArgb(71, 85, 105);

    /// <summary>Input/TextBox border — slightly lighter than panel for focus contrast.</summary>
    public static readonly Color InputLight  = Color.FromArgb(71, 85, 105);

    /// <summary>Focus/active border — emerald accent for focused inputs.</summary>
    public static readonly Color Focus       = AppColors.AccentGreen;

    // ─── On all dark backgrounds ─────────────────────────────────────────────

    /// <summary>Semi-transparent white hairline — 90 alpha — separator on dark panels.</summary>
    public static readonly Color SeparatorOnDark = Color.FromArgb(90, 255, 255, 255);

    /// <summary>Emerald bottom accent line — used in headers and cards.</summary>
    public static readonly Color AccentLine      = AppColors.AccentGreen;

    // ─── Widths ───────────────────────────────────────────────────────────────

    /// <summary>Standard 1px border.</summary>
    public const float Thin   = 1f;

    /// <summary>Accent bottom line — 2px.</summary>
    public const float Accent = 2f;

    /// <summary>Scan panel border — 1.5px.</summary>
    public const float Scan   = 1.5f;
}
