using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace OmadaPOS.Componentes
{
    public class RoundedPanel : Panel
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int CornerRadius { get; set; } = 16;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color BorderColor { get; set; } = Color.FromArgb(220, 224, 228);
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color BackgroundStart { get; set; } = Color.FromArgb(248, 249, 250);
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color BackgroundEnd { get; set; } = Color.FromArgb(240, 242, 245);
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color ShadowColor { get; set; } = Color.FromArgb(15, 0, 0, 0);

        public RoundedPanel()
        {
            this.DoubleBuffered = true;
            this.Padding = new Padding(20);
            this.BackColor = Color.Transparent;

            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.ResizeRedraw |
                          ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.SupportsTransparentBackColor, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.CompositingQuality = CompositingQuality.HighQuality;

            Rectangle rect = this.ClientRectangle;
            int radius = CornerRadius;

            // Sombra
            Rectangle shadowRect = new Rectangle(rect.X + 3, rect.Y + 3, rect.Width - 6, rect.Height - 6);
            using (GraphicsPath shadowPath = CreateRoundedPath(shadowRect, radius))
            using (SolidBrush shadowBrush = new SolidBrush(ShadowColor))
            {
                g.FillPath(shadowBrush, shadowPath);
            }

            // Fondo principal con gradiente
            Rectangle mainRect = new Rectangle(rect.X, rect.Y, rect.Width - 3, rect.Height - 3);
            using (GraphicsPath path = CreateRoundedPath(mainRect, radius))
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    mainRect, BackgroundStart, BackgroundEnd, LinearGradientMode.Vertical))
                {
                    g.FillPath(brush, path);
                }

                using (Pen borderPen = new Pen(BorderColor, 1.5f))
                {
                    g.DrawPath(borderPen, path);
                }
            }
        }

        private GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            Rectangle arc = new Rectangle(rect.X, rect.Y, diameter, diameter);

            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}
