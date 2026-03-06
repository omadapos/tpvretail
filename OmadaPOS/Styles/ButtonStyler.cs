using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

public static class ElegantButtonStyles
{
    // ─────────────────────────────────────────────
    // PALETA — alineada con AppColors (PremiumMarket Theme)
    // ─────────────────────────────────────────────

    // 30% — Azul marino (estructura, headers, teclado)
    public static readonly Color HeaderNavy      = AppColors.NavyDark;
    public static readonly Color HeaderDark      = AppColors.NavyDark;
    public static readonly Color Keypad          = AppColors.NavyBase;
    public static readonly Color KeypadHover     = AppColors.NavyLight;
    public static readonly Color ProductPrices   = AppColors.NavyBase;
    public static readonly Color PriceText       = AppColors.NavyBase;
    public static readonly Color DebitGray       = AppColors.SlateBlue;
    public static readonly Color RewardsButton   = AppColors.SlateBlue;
    public static readonly Color AlphabetButtons = Color.FromArgb(160, 174, 192); // Gris azulado suave

    // 10% — Verde fresco (acciones principales)
    public static readonly Color CashGreen          = AppColors.AccentGreen;
    public static readonly Color Buttonok           = AppColors.AccentGreen;
    public static readonly Color MoneyButtonGreen   = AppColors.AccentGreenDark;
    public static readonly Color BackgroundLight    = AppColors.BackgroundPrimary;

    // Funcionales semánticos
    public static readonly Color AlertRed        = AppColors.Danger;
    public static readonly Color ButtonCacnel    = AppColors.Danger;
    public static readonly Color AlertRedGlass   = Color.FromArgb(80, 197, 48, 48);
    public static readonly Color WarningOrange   = AppColors.Warning;
    public static readonly Color EBTOrange       = AppColors.PaymentEBT;
    public static readonly Color EBTBalanceOrange= AppColors.Warning;
    public static readonly Color CreditBlue      = AppColors.PaymentCredit;
    public static readonly Color SplitBlueLight  = AppColors.PaymentSplit;
    public static readonly Color GiftPurple      = AppColors.PaymentGiftCard;

    // Texto
    public static readonly Color TextWhite    = AppColors.TextWhite;
    public static readonly Color ButtonShadow = Color.FromArgb(40, 0, 0, 0);
    public static readonly Color HoverOverlay = Color.FromArgb(30, 13, 31, 45);

    // ─────────────────────────────────────────────
    // MÉTODO PRINCIPAL DE ESTILO DE BOTÓN
    // ─────────────────────────────────────────────
    public static void Style(Button button, Color? backColor = null, Color? foreColor = null, int radius = 10, float fontSize = 30F)
    {
        var bg = backColor ?? HeaderNavy;
        var fg = foreColor ?? TextWhite;

        button.FlatStyle = FlatStyle.Flat;
        button.BackColor = Color.Transparent;
        button.ForeColor = fg;
        button.Font = new Font("Montserrat", fontSize, FontStyle.Bold);
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
            using var brush = new LinearGradientBrush(rect, Lighten(bg, 0.10f), Darken(bg, 0.10f), LinearGradientMode.Vertical);

            g.FillPath(brush, path);

            using var border = new Pen(Color.FromArgb(60, 255, 255, 255), 1);
            g.DrawPath(border, path);

            DrawText(g, button.Text, button.Font, fg, rect);
        };

        button.Resize += (s, e) => button.Invalidate();
    }

    // ─────────────────────────────────────────────
    // HELPERS DE GEOMETRÍA Y TEXTO
    // ─────────────────────────────────────────────
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
        var format = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };
        g.DrawString(text, font, shadow, new Rectangle(r.X + 1, r.Y + 1, r.Width, r.Height), format);
        g.DrawString(text, font, textBrush, r, format);
    }

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
        splitContainer.SplitterWidth = 2;
        splitContainer.BackColor = AppColors.SurfaceMuted;
        splitContainer.BorderStyle = BorderStyle.None;
        splitContainer.IsSplitterFixed = false;
        splitContainer.Orientation = Orientation.Vertical;

        splitContainer.Layout += (s, e) =>
        {
            splitContainer.SplitterDistance = (int)(splitContainer.Width * (porcentajePanel1 / 100.0));
        };

        splitContainer.Panel1.BackColor = AppColors.BackgroundPrimary;
        splitContainer.Panel1.Padding = new Padding(10);
        splitContainer.Panel1.AutoScroll = true;

        splitContainer.Panel2.BackColor = AppColors.BackgroundSecondary;
        splitContainer.Panel2.Padding = new Padding(5);
    }
}
