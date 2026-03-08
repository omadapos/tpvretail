using System.Drawing;

namespace OmadaPOS.Presentation.Styling;

/// <summary>
/// Drop-shadow color tokens for the POS Design System.
/// Applied to RoundedPanel.ShadowColor.
/// All shadows use black with varying alpha transparency.
/// </summary>
public static class AppShadows
{
    /// <summary>12 alpha — barely visible depth for floating cards.</summary>
    public static readonly Color Subtle = Color.FromArgb(12, 0, 0, 0);
}
