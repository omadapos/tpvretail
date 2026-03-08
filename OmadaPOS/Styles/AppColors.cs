using System.Drawing;
using System.Drawing.Drawing2D;

namespace OmadaPOS.Presentation.Styling;

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
    public static readonly Color BackgroundPrimary   = Color.FromArgb(248, 250, 252); // slate-50  — base general
    public static readonly Color BackgroundSecondary = Color.FromArgb(255, 255, 255); // blanco puro — cards / surfaces
    public static readonly Color SurfaceCard         = Color.FromArgb(255, 255, 255); // blanco — áreas de contenido
    public static readonly Color SurfaceMuted        = Color.FromArgb(238, 242, 246); // gris-azulado muy suave — filas alternas, separadores

    // ─────────────────────────────────────────────
    // 30% — SLATE (estructura, headers, navegación)
    // ─────────────────────────────────────────────
    public static readonly Color NavyDark   = Color.FromArgb(15,  23,  42);  // slate-900 — header principal, fondo oscuro
    public static readonly Color NavyBase   = Color.FromArgb(30,  58,  95);  // slate-800 warm — sub-headers, paneles
    public static readonly Color NavyLight  = Color.FromArgb(51,  88, 140);  // slate-700 — elementos activos, hover
    public static readonly Color SlateBlue  = Color.FromArgb(100, 116, 139); // slate-500 — bordes, texto secundario suave

    // ─────────────────────────────────────────────
    // 10% — EMERALD (acento principal — acción, éxito)
    // ─────────────────────────────────────────────
    public static readonly Color AccentGreen      = Color.FromArgb(5,  150, 105);  // emerald-600 — CTA principal (más sutil que verde puro)
    public static readonly Color AccentGreenLight = Color.FromArgb(52, 211, 153);  // emerald-400 — hover / success light
    public static readonly Color AccentGreenDark  = Color.FromArgb(4,  120,  87);  // emerald-700 — pressed / activo

    // ─────────────────────────────────────────────
    // COLORES FUNCIONALES SEMÁNTICOS
    // ─────────────────────────────────────────────
    public static readonly Color Danger  = Color.FromArgb(220,  38,  38); // red-600
    public static readonly Color Warning = Color.FromArgb(217, 119,   6); // amber-600
    public static readonly Color Info    = Color.FromArgb(37,  99,  235); // blue-600

    // ─────────────────────────────────────────────
    // MÉTODOS DE PAGO
    // ─────────────────────────────────────────────
    public static readonly Color PaymentCash     = Color.FromArgb(5,  150, 105);  // emerald-600 — efectivo
    public static readonly Color PaymentCredit   = Color.FromArgb(37,  99, 235);  // blue-600 — crédito
    public static readonly Color PaymentDebit    = Color.FromArgb(100, 116, 139); // slate-500 — débito (neutro)
    public static readonly Color PaymentEBT      = Color.FromArgb(180,  83,   9); // amber-800 — EBT (cálido)
    public static readonly Color PaymentGiftCard = Color.FromArgb(109,  40, 217); // violet-700 — Gift Card
    public static readonly Color PaymentSplit    = Color.FromArgb(6,  148, 162);  // cyan-600 — Split (diferenciador)

    // ─────────────────────────────────────────────
    // TEXTO
    // ─────────────────────────────────────────────
    public static readonly Color TextPrimary   = Color.FromArgb(15,  23,  42);   // slate-900 — texto principal
    public static readonly Color TextSecondary = Color.FromArgb(71,  85, 105);   // slate-600 — texto secundario
    public static readonly Color TextMuted     = Color.FromArgb(148, 163, 184);  // slate-400 — placeholder / hint
    public static readonly Color TextWhite     = Color.FromArgb(248, 250, 252);  // slate-50  — texto sobre fondos oscuros
    public static readonly Color TextAccent    = Color.FromArgb(5,  150, 105);   // emerald-600 — precio, énfasis

    // ─────────────────────────────────────────────
    // TEXTO SOBRE FONDOS OSCUROS (paneles navy)
    // ─────────────────────────────────────────────
    public static readonly Color TextOnDarkMuted     = Color.FromArgb(160, 200, 220); // text-muted sobre navy — hints, labels secundarios
    public static readonly Color TextOnDarkSecondary = Color.FromArgb(200, 230, 240); // texto secondary sobre navy — subtotales, tax

    // ─────────────────────────────────────────────
    // COLORES ADICIONALES (overlays, variantes semánticas)
    // ─────────────────────────────────────────────
    public static readonly Color OverlayLight    = Color.FromArgb(200, 255, 255, 255); // overlay blanco semitransparente
    public static readonly Color SeparatorOnDark = Color.FromArgb( 60, 255, 255, 255); // separador/borde sobre fondo oscuro
    public static readonly Color ShadowSubtle    = Color.FromArgb( 12,   0,   0,   0); // sombra suave — tarjetas / paneles
}
