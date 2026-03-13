using System.Drawing;

namespace OmadaPOS.Presentation.Styling;

/// <summary>
/// OmadaPOS — Paleta de colores profesional Bootstrap-inspired.
///
/// Estrategia:
///   • Cuerpo / canvas de la app  → blanco / gris muy claro
///   • Chrome de navegación       → navy oscuro (header, login)
///   • Acento primario            → azul #4A90E2
///   • Éxito / Efectivo           → verde #28A745
///   • Peligro / Cancelar         → rojo #DC3545
///   • Texto principal            → #212529 (alta legibilidad)
/// </summary>
public static class AppColors
{
    // ─────────────────────────────────────────────────────────────────────────
    // FONDOS Y SUPERFICIES  — canvas claro
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Canvas principal — gris muy claro, casi blanco.</summary>
    public static readonly Color BackgroundPrimary   = Color.FromArgb(248, 249, 250);   // #F8F9FA

    /// <summary>Encabezados de tabla, áreas elevadas.</summary>
    public static readonly Color BackgroundSecondary = Color.FromArgb(233, 236, 239);   // #E9ECEF

    /// <summary>Tarjetas, listas, cuerpo de popups — blanco puro.</summary>
    public static readonly Color SurfaceCard         = Color.FromArgb(255, 255, 255);   // #FFFFFF

    /// <summary>Bordes y líneas separadoras sobre fondo claro.</summary>
    public static readonly Color SurfaceMuted        = Color.FromArgb(222, 226, 230);   // #DEE2E6

    /// <summary>Fila alterna en tablas — gris muy tenue.</summary>
    public static readonly Color AlternateRow        = Color.FromArgb(242, 242, 242);   // #F2F2F2

    // ─────────────────────────────────────────────────────────────────────────
    // CHROME OSCURO — header, login, paneles de navegación
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Header / nav bar — navy más oscuro.</summary>
    public static readonly Color NavyDark  = Color.FromArgb(15,  23,  42);    // #0F172A

    /// <summary>Sub-header / chrome secundario.</summary>
    public static readonly Color NavyBase  = Color.FromArgb(30,  41,  59);    // #1E293B

    /// <summary>Estados activos dentro del chrome oscuro.</summary>
    public static readonly Color NavyLight = Color.FromArgb(51,  65,  85);    // #334155

    /// <summary>Bordes y accents dentro del chrome oscuro.</summary>
    public static readonly Color SlateBlue = Color.FromArgb(71,  85, 105);    // #475569

    // ─────────────────────────────────────────────────────────────────────────
    // ACENTO PRIMARIO — azul profesional
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Botones de acción neutral: QTY, HOLD, LOOKUP, letras.</summary>
    public static readonly Color AccentBlue      = Color.FromArgb(74, 144, 226);   // #4A90E2

    /// <summary>Estado hover / selected del acento azul.</summary>
    public static readonly Color AccentBlueDark  = Color.FromArgb(53, 122, 189);   // #357ABD

    /// <summary>Estado pressed / borde del acento azul.</summary>
    public static readonly Color AccentBlueDarker = Color.FromArgb(40,  96, 144);  // #286090

    // ─────────────────────────────────────────────────────────────────────────
    // VERDE — efectivo, éxito, tender
    // ─────────────────────────────────────────────────────────────────────────

    public static readonly Color AccentGreen      = Color.FromArgb(40, 167,  69);   // #28A745
    public static readonly Color AccentGreenLight = Color.FromArgb(72, 187, 120);   // #48BB78
    public static readonly Color AccentGreenDark  = Color.FromArgb(30, 126,  52);   // #1E7E34

    // ─────────────────────────────────────────────────────────────────────────
    // SEMÁNTICOS
    // ─────────────────────────────────────────────────────────────────────────

    public static readonly Color Danger  = Color.FromArgb(220,  53,  69);   // #DC3545
    public static readonly Color Warning = Color.FromArgb(255, 193,   7);   // #FFC107
    public static readonly Color Info    = Color.FromArgb(23, 162, 184);    // #17A2B8

    // ─────────────────────────────────────────────────────────────────────────
    // COLORES DE PAGO
    // ─────────────────────────────────────────────────────────────────────────

    public static readonly Color PaymentCash     = Color.FromArgb(40,  167,  69);   // verde
    public static readonly Color PaymentCredit   = Color.FromArgb(74,  144, 226);   // azul
    public static readonly Color PaymentDebit    = Color.FromArgb(108, 117, 125);   // gris #6C757D
    public static readonly Color PaymentEBT      = Color.FromArgb(217, 119,   6);   // ámbar
    public static readonly Color PaymentGiftCard = Color.FromArgb(111,  66, 193);   // violeta
    public static readonly Color PaymentSplit    = Color.FromArgb(23,  162, 184);   // cian

    // ─────────────────────────────────────────────────────────────────────────
    // TEXTO — dos contextos: fondo claro + chrome oscuro
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Texto primario sobre fondo claro — gris muy oscuro.</summary>
    public static readonly Color TextPrimary   = Color.FromArgb(33,  37,  41);    // #212529

    /// <summary>Texto secundario sobre fondo claro — gris medio.</summary>
    public static readonly Color TextSecondary = Color.FromArgb(108, 117, 125);   // #6C757D

    /// <summary>Texto muted / placeholder sobre fondo claro.</summary>
    public static readonly Color TextMuted     = Color.FromArgb(173, 181, 189);   // #ADB5BD

    /// <summary>Texto blanco para botones oscuros y chrome.</summary>
    public static readonly Color TextWhite     = Color.FromArgb(255, 255, 255);

    /// <summary>Texto secundario sobre chrome oscuro.</summary>
    public static readonly Color TextOnDarkSecondary = Color.FromArgb(203, 213, 225);   // #CBD5E1

    /// <summary>Texto muted sobre chrome oscuro.</summary>
    public static readonly Color TextOnDarkMuted = Color.FromArgb(148, 163, 184);       // #94A3B8

    /// <summary>Acento azul como texto (totales, importes destacados).</summary>
    public static readonly Color TextAccent = Color.FromArgb(74, 144, 226);   // #4A90E2

    // ─────────────────────────────────────────────────────────────────────────
    // OVERLAYS Y SEPARADORES
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Separador sutil sobre chrome oscuro (15% blanco).</summary>
    public static readonly Color SeparatorOnDark  = Color.FromArgb(38,  255, 255, 255);

    /// <summary>Separador / borde sobre fondo claro (#DEE2E6 con alpha).</summary>
    public static readonly Color SeparatorOnLight = Color.FromArgb(26,    0,   0,   0);

    /// <summary>Sombra de tarjeta — sutil (10% negro).</summary>
    public static readonly Color ShadowSubtle = Color.FromArgb(26,   0,   0,   0);

    /// <summary>Overlay modal.</summary>
    public static readonly Color OverlayLight = Color.FromArgb(200, 248, 249, 250);

    // ─────────────────────────────────────────────────────────────────────────
    // CAMBIO / SUCCESS HIGHLIGHT — fondo verde muy claro para importe de cambio
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Fondo de chip de cambio — verde muy claro.</summary>
    public static readonly Color SuccessLight = Color.FromArgb(212, 237, 218);   // #D4EDDA

    /// <summary>Texto del chip de cambio — verde oscuro.</summary>
    public static readonly Color SuccessDark  = Color.FromArgb(21,  87,  36);    // #155724
}
