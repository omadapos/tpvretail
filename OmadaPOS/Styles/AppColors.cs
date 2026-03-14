using System.Drawing;

namespace OmadaPOS.Presentation.Styling;

/// <summary>
/// OmadaPOS — Paleta de colores profesional. POS empresarial moderno.
///
/// Estrategia visual:
///   • Canvas de la app          → blanco / gris muy claro #F4F6F8
///   • Chrome de header          → navy corporativo #1F4E79
///   • Paneles oscuros / totales → slate oscuro #1E293B
///   • Acento primario           → azul #3B82F6
///   • Éxito / Efectivo          → verde #16A34A
///   • Peligro / Cancelar        → rojo #DC2626
///   • Texto principal           → #0F172A (alta legibilidad)
/// </summary>
public static class AppColors
{
    // ─────────────────────────────────────────────────────────────────────────
    // FONDOS Y SUPERFICIES  — canvas claro
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Canvas principal — gris muy claro, casi blanco.</summary>
    public static readonly Color BackgroundPrimary   = Color.FromArgb(244, 246, 248);   // #F4F6F8

    /// <summary>Encabezados de tabla, áreas elevadas.</summary>
    public static readonly Color BackgroundSecondary = Color.FromArgb(233, 236, 239);   // #E9ECEF

    /// <summary>Tarjetas, listas, cuerpo de popups — blanco puro.</summary>
    public static readonly Color SurfaceCard         = Color.FromArgb(255, 255, 255);   // #FFFFFF

    /// <summary>Bordes y líneas separadoras sobre fondo claro.</summary>
    public static readonly Color SurfaceMuted        = Color.FromArgb(203, 213, 225);   // #CBD5E1

    /// <summary>Fila alterna en tablas — gris muy tenue.</summary>
    public static readonly Color AlternateRow        = Color.FromArgb(242, 244, 247);   // #F2F4F7

    // ─────────────────────────────────────────────────────────────────────────
    // CHROME OSCURO — header, login, paneles de navegación
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Header / nav bar — navy más oscuro.</summary>
    public static readonly Color NavyDark  = Color.FromArgb(15,  23,  42);    // #0F172A

    /// <summary>Sub-header / chrome secundario / dark summary panels.</summary>
    public static readonly Color NavyBase  = Color.FromArgb(30,  41,  59);    // #1E293B

    /// <summary>Estados activos dentro del chrome oscuro.</summary>
    public static readonly Color NavyLight = Color.FromArgb(51,  65,  85);    // #334155

    /// <summary>Bordes y accents dentro del chrome oscuro.</summary>
    public static readonly Color SlateBlue = Color.FromArgb(71,  85, 105);    // #475569

    // ─────────────────────────────────────────────────────────────────────────
    // HEADER CORPORATIVO — azul navy empresarial #1F4E79
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Header bar background principal — navy corporativo.</summary>
    public static readonly Color HeaderPrimary   = Color.FromArgb(31,  78, 121);    // #1F4E79

    /// <summary>Header bar background zona scan — navy ligeramente más oscuro.</summary>
    public static readonly Color HeaderSecondary = Color.FromArgb(22,  59,  109);   // #163B6D

    /// <summary>Header bar botones (menú) — navy más oscuro.</summary>
    public static readonly Color HeaderDark      = Color.FromArgb(15,  45,  86);    // #0F2D56

    // ─────────────────────────────────────────────────────────────────────────
    // ACENTO PRIMARIO — azul profesional Tailwind
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Botones de acción neutral: QTY, HOLD, LOOKUP, letras.</summary>
    public static readonly Color AccentBlue      = Color.FromArgb(59, 130, 246);    // #3B82F6

    /// <summary>Estado hover / selected del acento azul.</summary>
    public static readonly Color AccentBlueDark  = Color.FromArgb(37, 99,  235);    // #2563EB

    /// <summary>Estado pressed / borde del acento azul.</summary>
    public static readonly Color AccentBlueDarker = Color.FromArgb(29, 78,  216);   // #1D4ED8

    // ─────────────────────────────────────────────────────────────────────────
    // VERDE — efectivo, éxito, tender
    // ─────────────────────────────────────────────────────────────────────────

    public static readonly Color AccentGreen      = Color.FromArgb(22,  163,  74);  // #16A34A
    public static readonly Color AccentGreenLight = Color.FromArgb(34,  197, 94);   // #22C55E
    public static readonly Color AccentGreenDark  = Color.FromArgb(21,  128,  61);  // #15803D

    // ─────────────────────────────────────────────────────────────────────────
    // SEMÁNTICOS
    // ─────────────────────────────────────────────────────────────────────────

    public static readonly Color Danger  = Color.FromArgb(220,  38,  38);    // #DC2626
    public static readonly Color Warning = Color.FromArgb(245, 158,  11);    // #F59E0B
    public static readonly Color Info    = Color.FromArgb(14,  165, 233);    // #0EA5E9

    // ─────────────────────────────────────────────────────────────────────────
    // CONTROLES DESHABILITADOS
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Fondo de control deshabilitado.</summary>
    public static readonly Color DisabledBackground = Color.FromArgb(229, 231, 235);   // #E5E7EB

    /// <summary>Texto de control deshabilitado.</summary>
    public static readonly Color DisabledText       = Color.FromArgb(148, 163, 184);   // #94A3B8

    // ─────────────────────────────────────────────────────────────────────────
    // COLORES DE PAGO
    // ─────────────────────────────────────────────────────────────────────────

    public static readonly Color PaymentCash     = Color.FromArgb(22,  163,  74);   // verde #16A34A
    public static readonly Color PaymentCredit   = Color.FromArgb(59,  130, 246);   // azul  #3B82F6
    public static readonly Color PaymentDebit    = Color.FromArgb(100, 116, 139);   // slate #64748B
    public static readonly Color PaymentEBT      = Color.FromArgb(245, 158,  11);   // ámbar #F59E0B
    public static readonly Color PaymentGiftCard = Color.FromArgb(111,  66, 193);   // violeta
    public static readonly Color PaymentSplit    = Color.FromArgb(14,  165, 233);   // cian  #0EA5E9

    // ─────────────────────────────────────────────────────────────────────────
    // TEXTO — dos contextos: fondo claro + chrome oscuro
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Texto primario sobre fondo claro — casi negro.</summary>
    public static readonly Color TextPrimary   = Color.FromArgb(15,  23,  42);     // #0F172A

    /// <summary>Texto secundario sobre fondo claro — slate.</summary>
    public static readonly Color TextSecondary = Color.FromArgb(71,  85, 105);     // #475569

    /// <summary>Texto muted / placeholder sobre fondo claro.</summary>
    public static readonly Color TextMuted     = Color.FromArgb(148, 163, 184);    // #94A3B8

    /// <summary>Texto blanco para botones oscuros y chrome.</summary>
    public static readonly Color TextWhite     = Color.FromArgb(255, 255, 255);

    /// <summary>Texto secundario sobre chrome oscuro.</summary>
    public static readonly Color TextOnDarkSecondary = Color.FromArgb(203, 213, 225);   // #CBD5E1

    /// <summary>Texto muted sobre chrome oscuro.</summary>
    public static readonly Color TextOnDarkMuted = Color.FromArgb(148, 163, 184);       // #94A3B8

    /// <summary>Acento azul como texto (totales, importes destacados).</summary>
    public static readonly Color TextAccent = Color.FromArgb(59, 130, 246);    // #3B82F6

    // ─────────────────────────────────────────────────────────────────────────
    // OVERLAYS Y SEPARADORES
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Separador sutil sobre chrome oscuro (15% blanco).</summary>
    public static readonly Color SeparatorOnDark  = Color.FromArgb(38,  255, 255, 255);

    /// <summary>Separador / borde sobre fondo claro.</summary>
    public static readonly Color SeparatorOnLight = Color.FromArgb(26,    0,   0,   0);

    /// <summary>Sombra de tarjeta — sutil (10% negro).</summary>
    public static readonly Color ShadowSubtle = Color.FromArgb(26,   0,   0,   0);

    /// <summary>Overlay modal.</summary>
    public static readonly Color OverlayLight = Color.FromArgb(200, 244, 246, 248);

    // ─────────────────────────────────────────────────────────────────────────
    // CAMBIO / SUCCESS HIGHLIGHT
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Fondo de chip de cambio — verde muy claro.</summary>
    public static readonly Color SuccessLight = Color.FromArgb(220, 252, 231);   // #DCFCE7

    /// <summary>Texto del chip de cambio — verde oscuro.</summary>
    public static readonly Color SuccessDark  = Color.FromArgb(21,  87,  36);    // #155724

    // ─────────────────────────────────────────────────────────────────────────
    // SELECCIÓN EN LISTVIEW (light blue — no invasivo sobre fondo claro)
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Fondo de fila seleccionada en listas — azul muy claro.</summary>
    public static readonly Color ListViewSelection     = Color.FromArgb(219, 234, 254);   // #DBEAFE

    /// <summary>Texto de fila seleccionada — oscuro (contraste sobre azul claro).</summary>
    public static readonly Color ListViewSelectionText = Color.FromArgb(15,  23,  42);    // #0F172A

    /// <summary>Línea de grilla en listview.</summary>
    public static readonly Color ListViewGridLine      = Color.FromArgb(226, 232, 240);   // #E2E8F0
}
