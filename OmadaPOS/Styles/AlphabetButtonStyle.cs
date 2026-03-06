using System.Drawing;
using System.Windows.Forms;

/// <summary>
/// Estilo de botones del abecedario — PremiumMarket Theme.
/// Usa FlatStyle.Flat para evitar el overhead de Paint override y
/// mantener consistencia con el resto del sistema.
/// </summary>
public static class ModernAlphabetButtonStyle
{
    public static void Apply(Button button)
    {
        button.FlatStyle  = FlatStyle.Flat;
        button.BackColor  = AppColors.NavyBase;
        button.ForeColor  = AppColors.TextWhite;
        button.Font       = new Font("Segoe UI", 11F, FontStyle.Bold);
        button.TextAlign  = ContentAlignment.MiddleCenter;
        button.Cursor     = Cursors.Hand;
        button.UseVisualStyleBackColor = false;

        button.FlatAppearance.BorderSize            = 1;
        button.FlatAppearance.BorderColor           = AppColors.NavyLight;
        button.FlatAppearance.MouseOverBackColor    = AppColors.AccentGreen;
        button.FlatAppearance.MouseDownBackColor    = AppColors.AccentGreenDark;
    }
}
