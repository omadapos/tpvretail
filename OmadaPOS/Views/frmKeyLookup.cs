using System.Drawing;
using System.Windows.Forms;

namespace OmadaPOS.Views
{
    public partial class frmKeyLookup : Form
    {
        public frmKeyLookup()
        {
            InitializeComponent();
            this.Load += frmKeyLookup_Load;
        }

        private void frmKeyLookup_Load(object sender, EventArgs e)
        {
            AplicarEstiloVisual();
        }

        private void AplicarEstiloVisual()
        {
            ConfigurarTamano(anchoPorc: 0.42, altoPorc: 0.72);

            this.BackColor = AppColors.BackgroundPrimary;
            tableLayoutPanel1.BackColor = AppColors.BackgroundPrimary;

            // Forzar que el keypad llene su celda del TableLayout
            keyPadControl1.Anchor   = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            keyPadControl1.AutoSize = false;

            // ── Header contextual — Azul info (búsqueda de producto) ──
            AgregarHeaderContextual(
                colorHeader : AppColors.Info,
                icono       : "⌕",
                titulo      : "Product Lookup",
                subtitulo   : "Scan or enter the UPC / barcode"
            );

            ElegantButtonStyles.Style(buttonOK,     AppColors.Info,   AppColors.TextWhite, fontSize: 18f);
            ElegantButtonStyles.Style(buttonCancel, AppColors.Danger, AppColors.TextWhite, fontSize: 18f);
            buttonOK.Text     = "⌕  SEARCH";
            buttonCancel.Text = "✕  CANCEL";
        }

        private void AgregarHeaderContextual(Color colorHeader, string icono, string titulo, string subtitulo)
        {
            var panel = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 70,
                BackColor = colorHeader,
                Padding   = new Padding(16, 0, 16, 0)
            };

            var lblIcono = new Label
            {
                Text      = icono,
                Font      = new Font("Segoe UI", 22F, FontStyle.Bold),
                ForeColor = Color.FromArgb(200, 255, 255, 255),
                AutoSize  = true,
                Location  = new Point(16, 14),
                BackColor = Color.Transparent
            };

            var lblTitulo = new Label
            {
                Text      = titulo,
                Font      = new Font("Montserrat", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize  = true,
                Location  = new Point(62, 10),
                BackColor = Color.Transparent
            };

            var lblSub = new Label
            {
                Text      = subtitulo,
                Font      = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = Color.FromArgb(200, 255, 255, 255),
                AutoSize  = true,
                Location  = new Point(63, 38),
                BackColor = Color.Transparent
            };

            panel.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(180, 255, 255, 255), 3);
                e.Graphics.DrawLine(pen, 0, panel.Height - 3, panel.Width, panel.Height - 3);
            };

            panel.Controls.Add(lblIcono);
            panel.Controls.Add(lblTitulo);
            panel.Controls.Add(lblSub);
            this.Controls.Add(panel);
            panel.BringToFront();
        }

        private void ConfigurarTamano(double anchoPorc, double altoPorc)
        {
            var screen    = Screen.PrimaryScreen!.WorkingArea;
            this.Width    = (int)(screen.Width  * anchoPorc);
            this.Height   = (int)(screen.Height * altoPorc);
            this.Location = new Point(
                screen.X + (screen.Width  - this.Width)  / 2,
                screen.Y + (screen.Height - this.Height) / 2);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            var code = keyPadControl1.Getdata();
            if (!string.IsNullOrEmpty(code))
            {
                ((frmHome)Owner).SearchProduct(code);
                this.Close();
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e) => this.Close();
    }
}
