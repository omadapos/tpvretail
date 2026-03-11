using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using System.ComponentModel;
using OmadaPOS.Presentation.Styling;

namespace OmadaPOS.Componentes
{
    public class RoundedPanel : Panel
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int CornerRadius { get; set; } = 16;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color BorderColor { get; set; } = AppColors.SurfaceMuted;

        /// <summary>Fill color for the panel background (flat — no gradient).</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color BackgroundStart { get; set; } = AppColors.SurfaceCard;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color ShadowColor { get; set; } = AppColors.ShadowSubtle;

        public RoundedPanel()
        {
            DoubleBuffered = true;
            Padding        = new Padding(20);
            BackColor      = Color.Transparent;

            SetStyle(ControlStyles.AllPaintingInWmPaint  |
                     ControlStyles.UserPaint             |
                     ControlStyles.ResizeRedraw           |
                     ControlStyles.OptimizedDoubleBuffer  |
                     ControlStyles.SupportsTransparentBackColor, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            g.SmoothingMode      = SmoothingMode.AntiAlias;
            g.TextRenderingHint  = TextRenderingHint.ClearTypeGridFit;
            g.CompositingQuality = CompositingQuality.HighQuality;

            var rect   = ClientRectangle;
            int radius = CornerRadius;

            // Shadow (offset rect)
            var shadowRect = new Rectangle(rect.X + 3, rect.Y + 3, rect.Width - 6, rect.Height - 6);
            using (var shadowPath  = CreateRoundedPath(shadowRect, radius))
            using (var shadowBrush = new SolidBrush(ShadowColor))
                g.FillPath(shadowBrush, shadowPath);

            // Background — flat SolidBrush (was LinearGradientBrush: same visual, far cheaper)
            var mainRect = new Rectangle(rect.X, rect.Y, rect.Width - 3, rect.Height - 3);
            using (var path = CreateRoundedPath(mainRect, radius))
            {
                using (var fillBrush = new SolidBrush(BackgroundStart))
                    g.FillPath(fillBrush, path);

                using (var borderPen = new Pen(BorderColor, 1f))
                    g.DrawPath(borderPen, path);
            }
        }

        private static GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
        {
            var path     = new GraphicsPath();
            int diameter = radius * 2;
            var arc      = new Rectangle(rect.X, rect.Y, diameter, diameter);

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
