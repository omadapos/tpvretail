using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

public static class ElegantButtonStyles
{
    // 🎨 Colores base reorganizados y refinados
    public static readonly Color HeaderNavy = Color.FromArgb(44, 62, 80);        // Azul marino para encabezado
    public static readonly Color BackgroundLight = Color.FromArgb(248, 249, 250); // Fondo general gris muy claro
    public static readonly Color AlertRed = Color.FromArgb(231, 76, 60);          // Rojo para cancelar/eliminar
    public static readonly Color AlertRedGlass = Color.FromArgb(80, 192, 57, 43); // Rojo con transparencia
    public static readonly Color WarningOrange = Color.FromArgb(243, 156, 18);    // Naranja para advertencias
    public static readonly Color HeaderDark = Color.FromArgb(44, 62, 80);        // Header azul marino
    public static readonly Color CashGreen = Color.FromArgb(40, 180, 99);        // Verde para efectivo/pay cash
    public static readonly Color CreditBlue = Color.FromArgb(52, 152, 219);      // Azul para credit card
    public static readonly Color DebitGray = Color.FromArgb(84, 98, 112);      // Gris para debit card
    public static readonly Color GiftPurple = Color.FromArgb(142, 68, 173);      // Púrpura para gift card
    public static readonly Color EBTOrange = Color.FromArgb(230, 126, 34);       // Naranja para EBT
    public static readonly Color SplitBlueLight = Color.FromArgb(93, 173, 226);  // Azul claro para split pay
    public static readonly Color TextWhite = Color.White;                        // Texto blanco
    public static readonly Color Keypad = Color.FromArgb(52, 73, 94);        // Azul oscuro para teclado numérico
    public static readonly Color RewardsButton = Color.FromArgb(149, 165, 166);  // Gris medio para Rewards
    public static readonly Color AlphabetButtons = Color.FromArgb(189, 195, 199); // Gris claro para A,B,C...
    public static readonly Color ProductPrices = Color.FromArgb(44, 62, 80);     // Azul marino para precios
    public static readonly Color KeypadHover = Color.FromArgb(74, 111, 165);     // Hover para teclado numérico
    public static readonly Color MoneyButtonGreen = Color.FromArgb(39, 174, 96); // Verde oscuro para botones $10,$20,$50,$100
    public static readonly Color ButtonCacnel = Color.FromArgb(192, 57, 43);                // Fondo blanco para área productos
    public static readonly Color Buttonok = Color.FromArgb(39, 174, 96);  // Borde gris claro productos
    public static readonly Color PriceText = Color.FromArgb(44, 62, 80);         // Texto de precios azul oscuro
    public static readonly Color EBTBalanceOrange = Color.FromArgb(243, 156, 18); // Naranja para EBT Balance
    public static readonly Color ButtonShadow = Color.FromArgb(30, 0, 0, 0);     // Sombra sutil para botones
    public static readonly Color HoverOverlay = Color.FromArgb(30, 44, 62, 80);  // Overlay hover con transparencia


    public static void Style(Button button, Color? backColor = null, Color? foreColor = null, int radius = 10, float fontSize = 30F)
    {
        var bg = backColor ?? HeaderNavy;
        var fg = foreColor ?? TextWhite;

        button.FlatStyle = FlatStyle.Flat;
        button.BackColor = Color.Transparent;
        button.ForeColor = fg;

        button.Font = new Font("Montserrat", fontSize,FontStyle.Bold);
        button.TextAlign = ContentAlignment.MiddleCenter;
        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = Color.Transparent;
        button.FlatAppearance.MouseDownBackColor = Color.Transparent;
        button.Cursor = Cursors.Hand;
        button.Padding = new Padding(16, 8, 16, 8);
        button.Height = Math.Max(44, (int)(fontSize * 1.8f));
        button.TabStop = false;

        button.Paint += (sender, e) =>
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            var rect = new Rectangle(2, 2, button.Width - 4, button.Height - 4);
            using var path = RoundedPath(rect, radius);

            using var brush = new LinearGradientBrush(rect, Lighten(bg, 0.1f), Darken(bg, 0.1f), LinearGradientMode.Vertical);
        //    using var glass = new SolidBrush(GlassOverlay);
            g.FillPath(brush, path);
         //   g.FillPath(glass, path);

            using var border = new Pen(Color.FromArgb(80, 255, 255, 255), 1);
            g.DrawPath(border, path);

            DrawText(g, button.Text, button.Font, fg, rect);

        };

        button.Resize += (s, e) => button.Invalidate();
    }

    public static GraphicsPath RoundedPath(Rectangle r, int radius)
    {
        int d = radius * 2;
        var path = new GraphicsPath();
        path.AddArc(r.X, r.Y, d, d, 180, 90);
        path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }

    private static void DrawText(Graphics g, string text, Font font, Color color, Rectangle r)
    {
        using var shadow = new SolidBrush(Color.FromArgb(60, 0, 0, 0));
        using var textBrush = new SolidBrush(color);
        var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

        g.DrawString(text, font, shadow, new Rectangle(r.X + 1, r.Y + 1, r.Width, r.Height), format);
        g.DrawString(text, font, textBrush, r, format);
    }

    public static Color Lighten(Color c, float f)
    {
        return Color.FromArgb(c.A,
            Math.Min(255, (int)(c.R + (255 - c.R) * f)),
            Math.Min(255, (int)(c.G + (255 - c.G) * f)),
            Math.Min(255, (int)(c.B + (255 - c.B) * f)));
    }

    public static Color Darken(Color c, float f)
    {
        return Color.FromArgb(c.A,
            Math.Max(0, (int)(c.R * (1 - f))),
            Math.Max(0, (int)(c.G * (1 - f))),
            Math.Max(0, (int)(c.B * (1 - f))));
    }
    // Estilo para SplitContainer POS
    public static void StyleSplitContainer(SplitContainer splitContainer, int porcentajePanel1 = 50)
    {
        // Estilos generales
        splitContainer.SplitterWidth = 2;
        splitContainer.BackColor = Color.FromArgb(220, 220, 220);
        splitContainer.BorderStyle = BorderStyle.None;
        splitContainer.IsSplitterFixed = false;
        splitContainer.Orientation = Orientation.Vertical;

        // Evento Layout: garantiza que el Width ya está definido
        splitContainer.Layout += (s, e) =>
        {
            splitContainer.SplitterDistance = (int)(splitContainer.Width * (porcentajePanel1 / 100.0));
        };

        // Panel izquierdo
        splitContainer.Panel1.BackColor = Color.FromArgb(245, 245, 245);
        splitContainer.Panel1.Padding = new Padding(10);
        splitContainer.Panel1.AutoScroll = true;

        // Panel derecho
        splitContainer.Panel2.BackColor = Color.White;
        splitContainer.Panel2.Padding = new Padding(5);
    }



}
