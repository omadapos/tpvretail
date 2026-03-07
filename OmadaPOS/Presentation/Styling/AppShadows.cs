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
    public static readonly Color Subtle  = Color.FromArgb(12, 0, 0, 0);

    /// <summary>15 alpha — standard card elevation.</summary>
    public static readonly Color Medium  = Color.FromArgb(15, 0, 0, 0);

    /// <summary>40 alpha — strong shadow for modal dialogs.</summary>
    public static readonly Color Strong  = Color.FromArgb(40, 0, 0, 0);

    /// <summary>Button text shadow — subtle depth on rendered text.</summary>
    public static readonly Color ButtonText = Color.FromArgb(50, 0, 0, 0);

    /// <summary>Hover overlay on dark buttons — adds depth on mouse-over.</summary>
    public static readonly Color HoverOverlay = Color.FromArgb(30, 15, 23, 42);
}
