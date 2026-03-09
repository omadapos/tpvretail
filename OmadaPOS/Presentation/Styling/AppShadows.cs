using System.Drawing;

namespace OmadaPOS.Presentation.Styling;

/// <summary>
/// Drop-shadow color tokens for the POS Design System — dark enterprise theme.
/// Shadows on dark surfaces need slightly higher opacity for perceptible depth.
/// </summary>
public static class AppShadows
{
    /// <summary>50 alpha black — visible depth for floating cards on dark backgrounds.</summary>
    public static readonly Color Subtle = Color.FromArgb(50, 0, 0, 0);
}
