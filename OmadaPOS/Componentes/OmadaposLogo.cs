using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace OmadaPOS.Controles
{
    public class WatermarkOmadaPOS : Control
    {
        public WatermarkOmadaPOS()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint, true);

            this.BackColor = Color.Transparent;
            this.Dock = DockStyle.Fill;
            this.Enabled = false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            string texto = "OmadaPOS";

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            Rectangle rect = this.ClientRectangle;

            using (Font fuente = new Font("Montserrat", 32F, FontStyle.Bold, GraphicsUnit.Point))
            using (SolidBrush sombra = new SolidBrush(Color.FromArgb(50, 0, 0, 0)))
            using (SolidBrush textoPrincipal = new SolidBrush(Color.FromArgb(40, 255, 255, 255))) // Azul pizarra elegante
            using (StringFormat formato = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            {
                Rectangle sombraRect = new Rectangle(rect.X + 2, rect.Y + 2, rect.Width, rect.Height);
                e.Graphics.DrawString(texto, fuente, sombra, sombraRect, formato);
                e.Graphics.DrawString(texto, fuente, textoPrincipal, rect, formato);
            }
        }
    }
}
