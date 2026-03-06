using System.Drawing;
using System.Drawing.Drawing2D;

/// <summary>
/// PremiumMarket Theme — Regla 60-30-10
/// 60% Blanco/Gris claro  → fondos y superficies (neutro, limpio)
/// 30% Azul marino profundo → headers, paneles, labels (autoridad, confianza)
/// 10% Verde fresco          → acciones, confirmaciones, highlights (energía, frescura)
/// Inspirado en supermercados premium: Stop & Shop, Market 32, Whole Foods
/// </summary>
public static class AppColors
{
    // ─────────────────────────────────────────────
    // 60% — FONDOS Y SUPERFICIES (Blanco / Gris suave)
    // ─────────────────────────────────────────────
    public static Color BackgroundPrimary   = Color.FromArgb(247, 248, 250); // Gris blanquecino muy suave
    public static Color BackgroundSecondary = Color.FromArgb(255, 255, 255); // Blanco puro — tarjetas/surfaces
    public static Color SurfaceCard         = Color.FromArgb(255, 255, 255); // Blanco — áreas de contenido
    public static Color SurfaceMuted        = Color.FromArgb(237, 240, 245); // Gris azulado muy claro — separadores

    // ─────────────────────────────────────────────
    // 30% — AZUL MARINO (Headers, paneles, texto primario)
    // ─────────────────────────────────────────────
    public static Color NavyDark   = Color.FromArgb(13,  31,  45);  // Marino casi negro — header principal
    public static Color NavyBase   = Color.FromArgb(26,  58,  92);  // Marino medio — sub-headers, sidebar
    public static Color NavyLight  = Color.FromArgb(44,  82,  130); // Marino claro — elementos activos
    public static Color SlateBlue  = Color.FromArgb(74,  85,  104); // Slate — bordes, divisores

    // ─────────────────────────────────────────────
    // 10% — VERDE FRESCO (Acento principal — acción, éxito)
    // ─────────────────────────────────────────────
    public static Color AccentGreen      = Color.FromArgb(0,   166,  80);  // Verde vibrante — CTA principal
    public static Color AccentGreenLight = Color.FromArgb(72,  187, 120);  // Verde suave — hover/success
    public static Color AccentGreenDark  = Color.FromArgb(0,   110,  53);  // Verde oscuro — pressed/activo

    // ─────────────────────────────────────────────
    // COLORES FUNCIONALES SEMÁNTICOS
    // ─────────────────────────────────────────────
    public static Color Success = Color.FromArgb(0,   166,  80);  // Verde — confirmación, OK
    public static Color Danger  = Color.FromArgb(197,  48,  48);  // Rojo profundo — cancelar, eliminar
    public static Color Warning = Color.FromArgb(221, 107,  32);  // Ámbar — advertencia
    public static Color Info    = Color.FromArgb(43,  108, 176);  // Azul — información

    // ─────────────────────────────────────────────
    // MÉTODOS DE PAGO (con identidad semántica)
    // ─────────────────────────────────────────────
    public static Color PaymentCash     = Color.FromArgb(0,   166,  80);  // Verde — efectivo
    public static Color PaymentCredit   = Color.FromArgb(43,  108, 176);  // Azul — crédito
    public static Color PaymentDebit    = Color.FromArgb(74,  85,  104);  // Slate — débito
    public static Color PaymentEBT      = Color.FromArgb(192,  86,  33);  // Ámbar oscuro — EBT
    public static Color PaymentGiftCard = Color.FromArgb(107,  70, 193);  // Púrpura — Gift Card
    public static Color PaymentSplit    = Color.FromArgb(93,  173, 226);  // Azul claro — Split

    // ─────────────────────────────────────────────
    // TEXTO
    // ─────────────────────────────────────────────
    public static Color TextPrimary   = Color.FromArgb(26,  32,  44);   // Casi negro — texto principal
    public static Color TextSecondary = Color.FromArgb(74,  85,  104);  // Gris oscuro — texto secundario
    public static Color TextMuted     = Color.FromArgb(113, 128, 150);  // Gris suave — placeholder/hint
    public static Color TextWhite     = Color.FromArgb(255, 255, 255);  // Blanco — texto sobre fondos oscuros
    public static Color TextAccent    = Color.FromArgb(0,   166,  80);  // Verde — precio destacado, énfasis

    // ─────────────────────────────────────────────
    // DEGRADADOS UTILITARIOS
    // ─────────────────────────────────────────────
    public static LinearGradientBrush HeaderGradient(Rectangle bounds) =>
        new LinearGradientBrush(bounds, NavyDark, NavyBase, LinearGradientMode.Vertical);

    public static LinearGradientBrush AccentGradient(Rectangle bounds) =>
        new LinearGradientBrush(bounds, AccentGreen, AccentGreenDark, LinearGradientMode.Vertical);

    public static LinearGradientBrush DangerGradient(Rectangle bounds) =>
        new LinearGradientBrush(bounds, Danger, Color.FromArgb(155, 30, 30), LinearGradientMode.Vertical);

    public static LinearGradientBrush SurfaceGradient(Rectangle bounds) =>
        new LinearGradientBrush(bounds, BackgroundSecondary, BackgroundPrimary, LinearGradientMode.Vertical);
}
