using System.Drawing;
using System.Windows.Forms;

namespace OmadaPOS.Presentation.Styling;

/// <summary>
/// Estilo de botones del abecedario — nueva paleta clara.
/// Fondo blanco con borde #DEE2E6 y texto oscuro #212529.
/// </summary>
public static class ModernAlphabetButtonStyle
{
    /// <summary>Default background so <c>ResetearBotonActivo</c> can restore it.</summary>
    public static readonly Color DefaultBack = AppColors.SurfaceCard;

    public static void Apply(Button button)
    {
        button.FlatStyle  = FlatStyle.Flat;
        button.BackColor  = DefaultBack;
        button.ForeColor  = AppColors.TextPrimary;
        button.Font       = new Font("Segoe UI", 11F, FontStyle.Bold);
        button.TextAlign  = ContentAlignment.MiddleCenter;
        button.Cursor     = Cursors.Default;
        button.UseVisualStyleBackColor = false;

        button.FlatAppearance.BorderSize         = 1;
        button.FlatAppearance.BorderColor        = AppColors.SurfaceMuted;    // #DEE2E6
        button.FlatAppearance.MouseOverBackColor = AppColors.SurfaceCard;     // sin cambio (touchscreen)
        button.FlatAppearance.MouseDownBackColor = AppColors.SurfaceCard;     // sin cambio (touchscreen)
    }
}
