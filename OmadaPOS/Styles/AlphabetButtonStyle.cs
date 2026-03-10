using System.Drawing;
using System.Windows.Forms;

namespace OmadaPOS.Presentation.Styling;

/// <summary>
/// Estilo de botones del abecedario — PremiumMarket Theme.
/// Usa FlatStyle.Flat para evitar el overhead de Paint override y
/// mantener consistencia con el resto del sistema.
/// </summary>
public static class ModernAlphabetButtonStyle
{
    /// <summary>Default background so <c>ResetearBotonActivo</c> can restore it.</summary>
    public static readonly Color DefaultBack = AppColors.SlateBlue;

    public static void Apply(Button button)
    {
        button.FlatStyle  = FlatStyle.Flat;
        button.BackColor  = DefaultBack;
        button.ForeColor  = AppColors.TextWhite;
        button.Font       = new Font("Segoe UI", 11F, FontStyle.Bold);
        button.TextAlign  = ContentAlignment.MiddleCenter;
        button.Cursor     = Cursors.Hand;
        button.UseVisualStyleBackColor = false;

        button.FlatAppearance.BorderSize            = 0;
        button.FlatAppearance.MouseOverBackColor    = Color.Transparent;
        button.FlatAppearance.MouseDownBackColor    = AppColors.AccentGreenDark;
    }
}
