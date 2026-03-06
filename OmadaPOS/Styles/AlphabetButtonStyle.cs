using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

public static class ModernAlphabetButtonStyle
{
    // Paleta de colores moderna y profesional
    public static readonly Color BaseColor = Color.Transparent;        // Azul grisáceo moderno
    public static readonly Color HoverColor = Color.FromArgb(74, 144, 226);     // Azul vibrante hover
    public static readonly Color ClickColor = Color.FromArgb(41, 128, 185);     // Azul activo
    public static readonly Color ShadowColor = Color.FromArgb(30, 0, 0, 0);     // Sombra sutil
    public static readonly Color TextColor = Color.FromArgb(60, 60, 60);     // Blanco puro
    public static readonly Color BorderColor = Color.FromArgb(189, 195, 199);   // Borde sutil

    public static void Apply(Button button)
    {
        // Configuración básica del botón
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        button.BackColor = BaseColor;
        button.ForeColor = TextColor;
        button.Font = new Font("Montserrat", 15F, FontStyle.Bold);
        button.TextAlign = ContentAlignment.MiddleCenter;
        button.Size = new Size(50, 50);
        button.Margin = new Padding(3);
        button.Cursor = Cursors.Hand;

        // Desactivar el estilo por defecto para control total
        button.FlatAppearance.MouseOverBackColor = Color.Transparent;
        button.FlatAppearance.MouseDownBackColor = Color.Transparent;

        // Variables de estado
        bool isHovered = false;
        bool isPressed = false;

        // Eventos de mouse
        button.MouseEnter += (s, e) => { isHovered = true; button.Invalidate(); };
        button.MouseLeave += (s, e) => { isHovered = false; isPressed = false; button.Invalidate(); };
        button.MouseDown += (s, e) => { isPressed = true; button.Invalidate(); };
        button.MouseUp += (s, e) => { isPressed = false; button.Invalidate(); };

        // Renderizado personalizado con efectos modernos
        button.Paint += (sender, e) =>
        {
            Button btn = (Button)sender;
            Graphics g = e.Graphics;

            // Configuración de calidad de renderizado
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.CompositingQuality = CompositingQuality.HighQuality;

            Rectangle rect = new Rectangle(1, 1, btn.Width - 2, btn.Height - 2);
            int radius = 12;

            // Determinar color según estado
            Color currentColor = BaseColor;
            if (isPressed)
                currentColor = ClickColor;
            else if (isHovered)
                currentColor = HoverColor;

            using (GraphicsPath path = CreateRoundedPath(rect, radius))
            {
                // Sombra moderna (solo si no está presionado)
                if (!isPressed)
                {
                    Rectangle shadowRect = new Rectangle(rect.X + 2, rect.Y + 2, rect.Width, rect.Height);
                    using (GraphicsPath shadowPath = CreateRoundedPath(shadowRect, radius))
                    using (SolidBrush shadowBrush = new SolidBrush(ShadowColor))
                    {
                        g.FillPath(shadowBrush, shadowPath);
                    }
                }

                // Gradiente principal más sutil y moderno
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    rect,
                    LightenColor(currentColor, 0.15f),
                    DarkenColor(currentColor, 0.1f),
                    LinearGradientMode.Vertical))
                {
                    g.FillPath(brush, path);
                }

                // Borde sutil para definición
                using (Pen borderPen = new Pen(isHovered ? HoverColor : BorderColor, 1.5f))
                {
                    g.DrawPath(borderPen, path);
                }

                // Efecto de brillo sutil en la parte superior
                if (!isPressed)
                {
                    Rectangle glowRect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height / 3);
                    using (GraphicsPath glowPath = CreateRoundedPath(glowRect, radius))
                    using (LinearGradientBrush glowBrush = new LinearGradientBrush(
                        glowRect,
                        Color.FromArgb(40, 255, 255, 255),
                        Color.FromArgb(0, 255, 255, 255),
                        LinearGradientMode.Vertical))
                    {
                        g.FillPath(glowBrush, glowPath);
                    }
                }
            }

            // Texto con mejor renderizado
            Rectangle textRect = isPressed ?
                new Rectangle(rect.X + 1, rect.Y + 1, rect.Width, rect.Height) : rect;

            using (StringFormat sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;

                // Sombra del texto para mejor legibilidad
                using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(50, 0, 0, 0)))
                {
                    Rectangle shadowTextRect = new Rectangle(textRect.X + 1, textRect.Y + 1,
                                                           textRect.Width, textRect.Height);
                    g.DrawString(btn.Text, btn.Font, shadowBrush, shadowTextRect, sf);
                }

                // Texto principal
                using (SolidBrush textBrush = new SolidBrush(TextColor))
                {
                    g.DrawString(btn.Text, btn.Font, textBrush, textRect, sf);
                }
            }
        };
    }

    private static GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
    {
        GraphicsPath path = new GraphicsPath();
        int diameter = radius * 2;

        // Crear rectángulos para las esquinas redondeadas
        Rectangle arc = new Rectangle(rect.X, rect.Y, diameter, diameter);

        // Esquina superior izquierda
        path.AddArc(arc, 180, 90);

        // Esquina superior derecha
        arc.X = rect.Right - diameter;
        path.AddArc(arc, 270, 90);

        // Esquina inferior derecha
        arc.Y = rect.Bottom - diameter;
        path.AddArc(arc, 0, 90);

        // Esquina inferior izquierda
        arc.X = rect.Left;
        path.AddArc(arc, 90, 90);

        path.CloseFigure();
        return path;
    }

    private static Color LightenColor(Color color, float factor)
    {
        return Color.FromArgb(
            color.A,
            Math.Min(255, (int)(color.R + (255 - color.R) * factor)),
            Math.Min(255, (int)(color.G + (255 - color.G) * factor)),
            Math.Min(255, (int)(color.B + (255 - color.B) * factor))
        );
    }

    private static Color DarkenColor(Color color, float factor)
    {
        return Color.FromArgb(
            color.A,
            Math.Max(0, (int)(color.R * (1 - factor))),
            Math.Max(0, (int)(color.G * (1 - factor))),
            Math.Max(0, (int)(color.B * (1 - factor)))
        );
    }
}

// Versión alternativa con tema oscuro
public static class DarkAlphabetButtonStyle
{
    public static readonly Color BaseColor = Color.FromArgb(44, 47, 51);        // Gris oscuro moderno
    public static readonly Color HoverColor = Color.FromArgb(88, 101, 242);     // Púrpura vibrante
    public static readonly Color ClickColor = Color.FromArgb(78, 84, 200);      // Púrpura oscuro
    public static readonly Color ShadowColor = Color.FromArgb(40, 0, 0, 0);     // Sombra más pronunciada
    public static readonly Color TextColor = Color.FromArgb(220, 221, 222);     // Gris claro
    public static readonly Color BorderColor = Color.FromArgb(64, 68, 75);      // Borde oscuro

    // Usar la misma lógica de Apply() pero con estos colores
    public static void Apply(Button button)
    {
        // Misma implementación que ModernAlphabetButtonStyle.Apply()
        // pero usando los colores de tema oscuro definidos arriba
        ModernAlphabetButtonStyle.Apply(button);

        // Sobrescribir colores específicos para tema oscuro
        button.BackColor = BaseColor;
        button.ForeColor = TextColor;
    }
}