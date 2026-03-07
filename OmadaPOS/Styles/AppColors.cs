using System.Drawing;
using System.Drawing.Drawing2D;

/// <summary>
/// Slate Emerald Theme — Regla 60-30-10
/// 60% Gris-blanco frío  → fondos y superficies (limpio, aireado)
/// 30% Slate profundo     → headers, paneles, estructura (autoridad, premium)
/// 10% Emerald moderno    → acciones, confirmaciones, highlights (frescura digital)
///
/// Inspiración: Tailwind CSS slate/emerald — tendencia enterprise 2025-2026
/// Referencias: Stripe Dashboard, Linear App, Vercel — adaptado a POS supermercado
/// </summary>
public static class AppColors
{
    // ─────────────────────────────────────────────
    // 60% — FONDOS Y SUPERFICIES
    // ─────────────────────────────────────────────
    public static Color BackgroundPrimary   = Color.FromArgb(248, 250, 252); // slate-50  — base general
    public static Color BackgroundSecondary = Color.FromArgb(255, 255, 255); // blanco puro — cards / surfaces
    public static Color SurfaceCard         = Color.FromArgb(255, 255, 255); // blanco — áreas de contenido
    public static Color SurfaceMuted        = Color.FromArgb(238, 242, 246); // gris-azulado muy suave — filas alternas, separadores

    // ─────────────────────────────────────────────
    // 30% — SLATE (estructura, headers, navegación)
    // ─────────────────────────────────────────────
    public static Color NavyDark   = Color.FromArgb(15,  23,  42);  // slate-900 — header principal, fondo oscuro
    public static Color NavyBase   = Color.FromArgb(30,  58,  95);  // slate-800 warm — sub-headers, paneles
    public static Color NavyLight  = Color.FromArgb(51,  88, 140);  // slate-700 — elementos activos, hover
    public static Color SlateBlue  = Color.FromArgb(100, 116, 139); // slate-500 — bordes, texto secundario suave

    // ─────────────────────────────────────────────
    // 10% — EMERALD (acento principal — acción, éxito)
    // ─────────────────────────────────────────────
    public static Color AccentGreen      = Color.FromArgb(5,  150, 105);  // emerald-600 — CTA principal (más sutil que verde puro)
    public static Color AccentGreenLight = Color.FromArgb(52, 211, 153);  // emerald-400 — hover / success light
    public static Color AccentGreenDark  = Color.FromArgb(4,  120,  87);  // emerald-700 — pressed / activo

    // ─────────────────────────────────────────────
    // COLORES FUNCIONALES SEMÁNTICOS
    // ─────────────────────────────────────────────
    public static Color Success = Color.FromArgb(5,  150, 105);  // emerald-600
    public static Color Danger  = Color.FromArgb(220,  38,  38); // red-600 — más vivo y claro que el anterior
    public static Color Warning = Color.FromArgb(217, 119,   6); // amber-600 — dorado, menos naranja
    public static Color Info    = Color.FromArgb(37,  99,  235); // blue-600 — digital, moderno

    // ─────────────────────────────────────────────
    // MÉTODOS DE PAGO
    // ─────────────────────────────────────────────
    public static Color PaymentCash     = Color.FromArgb(5,  150, 105);  // emerald-600 — efectivo
    public static Color PaymentCredit   = Color.FromArgb(37,  99, 235);  // blue-600 — crédito
    public static Color PaymentDebit    = Color.FromArgb(100, 116, 139); // slate-500 — débito (neutro)
    public static Color PaymentEBT      = Color.FromArgb(180,  83,   9); // amber-800 — EBT (cálido)
    public static Color PaymentGiftCard = Color.FromArgb(109,  40, 217); // violet-700 — Gift Card
    public static Color PaymentSplit    = Color.FromArgb(6,  148, 162);  // cyan-600 — Split (diferenciador)

    // ─────────────────────────────────────────────
    // TEXTO
    // ─────────────────────────────────────────────
    public static Color TextPrimary   = Color.FromArgb(15,  23,  42);   // slate-900 — texto principal
    public static Color TextSecondary = Color.FromArgb(71,  85, 105);   // slate-600 — texto secundario
    public static Color TextMuted     = Color.FromArgb(148, 163, 184);  // slate-400 — placeholder / hint
    public static Color TextWhite     = Color.FromArgb(248, 250, 252);  // slate-50  — texto sobre fondos oscuros
    public static Color TextAccent    = Color.FromArgb(5,  150, 105);   // emerald-600 — precio, énfasis

    // ─────────────────────────────────────────────
    // GRADIENTES UTILITARIOS
    // ─────────────────────────────────────────────
    public static LinearGradientBrush HeaderGradient(Rectangle bounds) =>
        new LinearGradientBrush(bounds, NavyDark, NavyBase, LinearGradientMode.Vertical);

    public static LinearGradientBrush AccentGradient(Rectangle bounds) =>
        new LinearGradientBrush(bounds, AccentGreen, AccentGreenDark, LinearGradientMode.Vertical);

    public static LinearGradientBrush DangerGradient(Rectangle bounds) =>
        new LinearGradientBrush(bounds, Danger, Color.FromArgb(185, 28, 28), LinearGradientMode.Vertical);

    public static LinearGradientBrush SurfaceGradient(Rectangle bounds) =>
        new LinearGradientBrush(bounds, BackgroundSecondary, BackgroundPrimary, LinearGradientMode.Vertical);
}
