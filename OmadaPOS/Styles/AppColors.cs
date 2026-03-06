using System.Drawing;
using System.Drawing.Drawing2D;

public static class AppColors
{
    // 🌙 Nueva Paleta Principal con colores pastel y ligeros
    public static Color PrimaryDark = Color.FromArgb(180, 210, 200);    // Verde menta claro
    public static Color PrimaryBase = Color.FromArgb(210, 235, 225);    // Turquesa suave
    public static Color PrimaryLight = Color.FromArgb(235, 250, 245);   // Menta blanquecino

    // 🎨 Colores Funcionales con tonos pastel
    public static Color Success = Color.FromArgb(180, 230, 190);       // Verde manzana claro
    public static Color Warning = Color.FromArgb(255, 220, 170);       // Melocotón suave
    public static Color Danger = Color.FromArgb(250, 190, 195);        // Rosa pálido
    public static Color Info = Color.FromArgb(190, 210, 240);          // Lavanda clara

    // 💳 Métodos de Pago con colores dulces
    public static Color PaymentCredit = Color.FromArgb(195, 235, 200); // Pistacho
    public static Color PaymentCash = Color.FromArgb(245, 230, 195);   // Vainilla
    public static Color PaymentDigital = Color.FromArgb(190, 220, 245); // Celeste claro
    public static Color PaymentVoucher = Color.FromArgb(225, 200, 240); // Lila suave
    public static Color PaymentGiftcard = Color.FromArgb(245, 200, 225);// Rosa algodón

    // 🏞 Fondos con tonos luminosos
    public static Color BackgroundPrimary = Color.FromArgb(255, 255, 252); // Blanco marfil
    public static Color BackgroundSecondary = Color.FromArgb(245, 245, 250);// Blanco lila
    public static Color SurfaceDark = Color.FromArgb(200, 200, 210);    // Gris lila claro

    // 📝 Textos con colores delicados
    public static Color TextPrimary = Color.FromArgb(100, 110, 120);    // Gris azulado medio
    public static Color TextSecondary = Color.FromArgb(150, 160, 170);  // Gris celeste claro
    public static Color TextWhite = Color.FromArgb(255, 255, 255);      // Blanco puro

    // ✨ Degradados con estética suave
    public static LinearGradientBrush PrimaryGradient(Rectangle bounds) =>
        new LinearGradientBrush(bounds, PrimaryBase, PrimaryLight, 60f);
    public static LinearGradientBrush SuccessGradient(Rectangle bounds) =>
        new LinearGradientBrush(bounds, Success, Color.FromArgb(210, 245, 220), 60f);
    public static LinearGradientBrush DangerGradient(Rectangle bounds) =>
        new LinearGradientBrush(bounds, Danger, Color.FromArgb(255, 220, 225), 60f);
}