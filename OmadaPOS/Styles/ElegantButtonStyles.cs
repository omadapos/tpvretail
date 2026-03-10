using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace OmadaPOS.Presentation.Styling;

public static class ElegantButtonStyles
{
    // ─────────────────────────────────────────────
    // PALETA — alineada con AppColors
    // ─────────────────────────────────────────────

    public static readonly Color HeaderNavy  = AppColors.NavyDark;
    public static readonly Color Keypad      = AppColors.NavyBase;
    public static readonly Color DebitGray   = AppColors.SlateBlue;
    public static readonly Color CashGreen   = AppColors.AccentGreen;
    public static readonly Color Buttonok    = AppColors.AccentGreen;
    public static readonly Color AlertRed         = AppColors.Danger;
    public static readonly Color ButtonCacnel     = AppColors.Danger;
    public static readonly Color WarningOrange    = AppColors.Warning;
    public static readonly Color EBTOrange        = AppColors.PaymentEBT;
    public static readonly Color EBTBalanceOrange = AppColors.Warning;
    public static readonly Color CreditBlue       = AppColors.PaymentCredit;
    public static readonly Color SplitBlueLight   = AppColors.PaymentSplit;
    public static readonly Color GiftPurple       = AppColors.PaymentGiftCard;
    public static readonly Color TextWhite        = AppColors.TextWhite;

    // ─────────────────────────────────────────────
    // ESTADO POR BOTÓN — ConditionalWeakTable para evitar memory leaks.
    // Reemplaza el hack de AccessibleDescription + HexToColor()
    // eliminando el parseo de strings en cada repaint.
    // ─────────────────────────────────────────────

    private sealed class ButtonState
    {
        public Color BackColor { get; }
        public Color ForeColor { get; }
        public int   Radius    { get; }
        public ButtonState(Color bg, Color fg, int radius)
        { BackColor = bg; ForeColor = fg; Radius = radius; }
    }

    private static readonly ConditionalWeakTable<Button, ButtonState> _buttonStates = new();

    // ─────────────────────────────────────────────
    // MÉTODO PRINCIPAL
    // ─────────────────────────────────────────────

    public static void Style(Button button, Color? backColor = null, Color? foreColor = null, int radius = 10, float fontSize = 30F)
    {
        var bg = backColor ?? HeaderNavy;
        var fg = foreColor ?? TextWhite;

        if (_buttonStates.TryGetValue(button, out _))
        {
            UpdateButtonColor(button, bg, fg, radius, fontSize);
            return;
        }

        _buttonStates.Add(button, new ButtonState(bg, fg, radius));

        button.FlatStyle = FlatStyle.Flat;
        button.BackColor = Color.Transparent;
        button.ForeColor = fg;
        button.Font      = new Font("Segoe UI", fontSize, FontStyle.Bold);
        button.TextAlign = ContentAlignment.MiddleCenter;
        button.FlatAppearance.BorderSize         = 0;
        button.FlatAppearance.MouseOverBackColor = Color.Transparent;
        button.FlatAppearance.MouseDownBackColor = Color.Transparent;
        button.Cursor  = Cursors.Hand;
        button.Padding = new Padding(16, 8, 16, 8);
        button.Height  = Math.Max(44, (int)(fontSize * 1.8f));
        button.TabStop = false;

        button.Paint  += ButtonPaintHandler;
        button.Resize += (s, _) => (s as Button)?.Invalidate();
    }

    // ─────────────────────────────────────────────
    // ACTUALIZAR COLOR SIN RE-SUSCRIBIR EVENTOS
    // ─────────────────────────────────────────────

    private static void UpdateButtonColor(Button button, Color bg, Color fg, int radius, float fontSize)
    {
        _buttonStates.Remove(button);
        _buttonStates.Add(button, new ButtonState(bg, fg, radius));

        button.ForeColor = fg;
        // Dispose old font before assigning new one to prevent GDI font handle leak
        var oldFont = button.Font;
        button.Font = new Font("Segoe UI", fontSize, FontStyle.Bold);
        oldFont?.Dispose();
        button.Height = Math.Max(44, (int)(fontSize * 1.8f));
        button.Invalidate();
    }

    // ─────────────────────────────────────────────
    // PAINT HANDLER — optimizado (punto medio):
    //   ✓ Esquinas redondeadas conservadas
    //   ✓ SolidBrush en lugar de LinearGradientBrush  → cero allocations de heap
    //   ✓ AntiAlias solo para el borde (DrawPath)     → fill sin antialiasing (más rápido)
    //   ✓ Un solo DrawString sin sombra               → mitad del costo de texto
    //   ✓ Colores desde ButtonState                   → sin parseo de hex strings
    // ─────────────────────────────────────────────

    private static void ButtonPaintHandler(object? sender, PaintEventArgs e)
    {
        if (sender is not Button button) return;
        if (!_buttonStates.TryGetValue(button, out var state)) return;

        var g    = e.Graphics;
        var rect = new Rectangle(2, 2, button.Width - 4, button.Height - 4);

        using var path = RoundedPath(rect, state.Radius);

        // ── Fill — SolidBrush, sin antialiasing (rápido) ─────────────────────
        g.SmoothingMode = SmoothingMode.None;
        using var fill = new SolidBrush(state.BackColor);
        g.FillPath(fill, path);

        // ── Borde — AntiAlias solo aquí para curvas suaves ───────────────────
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var border = new Pen(AppColors.SeparatorOnDark, 1);
        g.DrawPath(border, path);
        g.SmoothingMode = SmoothingMode.None;

        // ── Texto — ClearType, un solo draw, sin sombra ──────────────────────
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        using var sf = new StringFormat
        {
            Alignment     = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            FormatFlags   = StringFormatFlags.NoWrap,
            Trimming      = StringTrimming.EllipsisCharacter,
        };
        using var textBrush = new SolidBrush(state.ForeColor);
        g.DrawString(button.Text, button.Font, textBrush, (RectangleF)rect, sf);
    }

    // ─────────────────────────────────────────────
    // HELPERS DE GEOMETRÍA — públicos (usados en POSDialog / NumericPadDialog)
    // ─────────────────────────────────────────────

    public static GraphicsPath RoundedPath(Rectangle r, int radius)
    {
        int d    = radius * 2;
        var path = new GraphicsPath();
        path.AddArc(r.X,         r.Y,          d, d, 180, 90);
        path.AddArc(r.Right - d, r.Y,          d, d, 270, 90);
        path.AddArc(r.Right - d, r.Bottom - d, d, d,   0, 90);
        path.AddArc(r.X,         r.Bottom - d, d, d,  90, 90);
        path.CloseFigure();
        return path;
    }

    // Utilidades de color — públicas para uso externo
    public static Color Lighten(Color c, float f) =>
        Color.FromArgb(c.A,
            Math.Min(255, (int)(c.R + (255 - c.R) * f)),
            Math.Min(255, (int)(c.G + (255 - c.G) * f)),
            Math.Min(255, (int)(c.B + (255 - c.B) * f)));

    public static Color Darken(Color c, float f) =>
        Color.FromArgb(c.A,
            Math.Max(0, (int)(c.R * (1 - f))),
            Math.Max(0, (int)(c.G * (1 - f))),
            Math.Max(0, (int)(c.B * (1 - f))));

    // ─────────────────────────────────────────────
    // ESTILO PARA SPLITCONTAINER
    // ─────────────────────────────────────────────

    public static void StyleSplitContainer(SplitContainer splitContainer, int porcentajePanel1 = 50)
    {
        splitContainer.SplitterWidth   = 2;
        splitContainer.BackColor       = AppColors.SurfaceMuted;
        splitContainer.BorderStyle     = BorderStyle.None;
        splitContainer.IsSplitterFixed = false;
        splitContainer.Orientation     = Orientation.Vertical;

        splitContainer.Layout += (s, e) =>
        {
            splitContainer.SplitterDistance = (int)(splitContainer.Width * (porcentajePanel1 / 100.0));
        };

        splitContainer.Panel1.BackColor  = AppColors.BackgroundPrimary;
        splitContainer.Panel1.Padding    = new Padding(10);
        splitContainer.Panel1.AutoScroll = true;

        splitContainer.Panel2.BackColor = AppColors.BackgroundSecondary;
        splitContainer.Panel2.Padding   = new Padding(5);
    }
}
