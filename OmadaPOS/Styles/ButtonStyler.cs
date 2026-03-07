using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

public static class ElegantButtonStyles
{
    // ─────────────────────────────────────────────
    // PALETA — alineada con AppColors (Slate Emerald Theme)
    // ─────────────────────────────────────────────

    // 30% — Slate (estructura, headers, teclado)
    public static readonly Color HeaderNavy      = AppColors.NavyDark;
    public static readonly Color HeaderDark      = AppColors.NavyDark;
    public static readonly Color Keypad          = AppColors.NavyBase;
    public static readonly Color KeypadHover     = AppColors.NavyLight;
    public static readonly Color ProductPrices   = AppColors.NavyBase;
    public static readonly Color PriceText       = AppColors.NavyBase;
    public static readonly Color DebitGray       = AppColors.SlateBlue;
    public static readonly Color RewardsButton   = AppColors.SlateBlue;
    public static readonly Color AlphabetButtons = Color.FromArgb(100, 116, 139); // slate-500

    // 10% — Emerald (acciones principales)
    public static readonly Color CashGreen        = AppColors.AccentGreen;
    public static readonly Color Buttonok         = AppColors.AccentGreen;
    public static readonly Color MoneyButtonGreen = AppColors.AccentGreenDark;
    public static readonly Color BackgroundLight  = AppColors.BackgroundPrimary;

    // Funcionales semánticos
    public static readonly Color AlertRed         = AppColors.Danger;
    public static readonly Color ButtonCacnel     = AppColors.Danger;
    public static readonly Color AlertRedGlass    = Color.FromArgb(80, 220, 38, 38);
    public static readonly Color WarningOrange    = AppColors.Warning;
    public static readonly Color EBTOrange        = AppColors.PaymentEBT;
    public static readonly Color EBTBalanceOrange = AppColors.Warning;
    public static readonly Color CreditBlue       = AppColors.PaymentCredit;
    public static readonly Color SplitBlueLight   = AppColors.PaymentSplit;
    public static readonly Color GiftPurple       = AppColors.PaymentGiftCard;

    // Texto y sombra
    public static readonly Color TextWhite    = AppColors.TextWhite;
    public static readonly Color ButtonShadow = Color.FromArgb(40, 0, 0, 0);
    public static readonly Color HoverOverlay = Color.FromArgb(30, 15, 23, 42);

    // HashSet para rastrear botones ya estilizados sin tocar su Tag
    private static readonly HashSet<Button> _styledButtons = new();

    // ─────────────────────────────────────────────
    // MÉTODO PRINCIPAL DE ESTILO DE BOTÓN
    // ─────────────────────────────────────────────
    public static void Style(Button button, Color? backColor = null, Color? foreColor = null, int radius = 10, float fontSize = 30F)
    {
        // Evitar suscripción duplicada al Paint/Resize si el botón ya fue estilizado
        if (_styledButtons.Contains(button))
        {
            UpdateButtonColor(button, backColor ?? HeaderNavy, foreColor ?? TextWhite, radius, fontSize);
            return;
        }

        var bg = backColor ?? HeaderNavy;
        var fg = foreColor ?? TextWhite;

        _styledButtons.Add(button);
        button.FlatStyle = FlatStyle.Flat;
        button.BackColor = Color.Transparent;
        button.ForeColor = fg;
        button.Font      = new Font("Montserrat", fontSize, FontStyle.Bold);
        button.TextAlign = ContentAlignment.MiddleCenter;
        button.FlatAppearance.BorderSize           = 0;
        button.FlatAppearance.MouseOverBackColor   = Color.Transparent;
        button.FlatAppearance.MouseDownBackColor   = Color.Transparent;
        button.Cursor  = Cursors.Hand;
        button.Padding = new Padding(16, 8, 16, 8);
        button.Height  = Math.Max(44, (int)(fontSize * 1.8f));
        button.TabStop = false;

        // Guardamos el color actual para usarlo en el Paint handler
        button.AccessibleDescription = ColorToHex(bg) + "|" + ColorToHex(fg) + "|" + radius;

        button.Paint   += ButtonPaintHandler;
        button.Resize  += (s, e) => (s as Button)?.Invalidate();
    }

    // ─────────────────────────────────────────────
    // ACTUALIZAR COLOR SIN RE-SUSCRIBIR EVENTOS
    // ─────────────────────────────────────────────
    private static void UpdateButtonColor(Button button, Color bg, Color fg, int radius, float fontSize)
    {
        button.ForeColor = fg;
        button.Font      = new Font("Montserrat", fontSize, FontStyle.Bold);
        button.Height    = Math.Max(44, (int)(fontSize * 1.8f));
        button.AccessibleDescription = ColorToHex(bg) + "|" + ColorToHex(fg) + "|" + radius;
        button.Invalidate();
    }

    // ─────────────────────────────────────────────
    // HANDLER DE PAINT — único, sin closures con captura de color
    // ─────────────────────────────────────────────
    private static void ButtonPaintHandler(object? sender, PaintEventArgs e)
    {
        if (sender is not Button button) return;

        // Recuperar color del AccessibleDescription
        var parts  = (button.AccessibleDescription ?? "").Split('|');
        var bg     = parts.Length > 0 ? HexToColor(parts[0]) : HeaderNavy;
        var fg     = parts.Length > 1 ? HexToColor(parts[1]) : TextWhite;
        var radius = parts.Length > 2 && int.TryParse(parts[2], out int r) ? r : 10;

        var g = e.Graphics;
        g.SmoothingMode     = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        var rect = new Rectangle(2, 2, button.Width - 4, button.Height - 4);

        using var path   = RoundedPath(rect, radius);
        using var brush  = new LinearGradientBrush(rect, Lighten(bg, 0.10f), Darken(bg, 0.10f), LinearGradientMode.Vertical);

        g.FillPath(brush, path);

        using var border = new Pen(Color.FromArgb(60, 255, 255, 255), 1);
        g.DrawPath(border, path);

        using var sf = new StringFormat
        {
            Alignment     = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        DrawText(g, button.Text, button.Font, fg, rect, sf);
    }

    // ─────────────────────────────────────────────
    // HELPERS DE GEOMETRÍA Y TEXTO
    // ─────────────────────────────────────────────
    public static GraphicsPath RoundedPath(Rectangle r, int radius)
    {
        int d    = radius * 2;
        var path = new GraphicsPath();
        path.AddArc(r.X,          r.Y,           d, d, 180, 90);
        path.AddArc(r.Right - d,  r.Y,           d, d, 270, 90);
        path.AddArc(r.Right - d,  r.Bottom - d,  d, d,   0, 90);
        path.AddArc(r.X,          r.Bottom - d,  d, d,  90, 90);
        path.CloseFigure();
        return path;
    }

    private static void DrawText(Graphics g, string text, Font font, Color color, Rectangle r, StringFormat sf)
    {
        using var shadow    = new SolidBrush(Color.FromArgb(50, 0, 0, 0));
        using var textBrush = new SolidBrush(color);

        g.DrawString(text, font, shadow,    new Rectangle(r.X + 1, r.Y + 1, r.Width, r.Height), sf);
        g.DrawString(text, font, textBrush, r, sf);
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

    // Serialización de color para guardar en AccessibleDescription
    private static string ColorToHex(Color c) => $"{c.R:X2}{c.G:X2}{c.B:X2}";

    private static Color HexToColor(string hex)
    {
        try
        {
            if (hex.Length == 6)
                return Color.FromArgb(
                    Convert.ToInt32(hex[..2], 16),
                    Convert.ToInt32(hex[2..4], 16),
                    Convert.ToInt32(hex[4..6], 16));
        }
        catch { }
        return HeaderNavy;
    }

    // ─────────────────────────────────────────────
    // ESTILO PARA SPLITCONTAINER
    // ─────────────────────────────────────────────
    public static void StyleSplitContainer(SplitContainer splitContainer, int porcentajePanel1 = 50)
    {
        splitContainer.SplitterWidth = 2;
        splitContainer.BackColor     = AppColors.SurfaceMuted;
        splitContainer.BorderStyle   = BorderStyle.None;
        splitContainer.IsSplitterFixed = false;
        splitContainer.Orientation   = Orientation.Vertical;

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
