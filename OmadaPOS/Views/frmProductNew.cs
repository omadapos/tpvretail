using System.Drawing;
using System.Windows.Forms;

namespace OmadaPOS.Views
{
    public partial class frmProductNew : Form
    {
        bool bTax = false;

        public frmProductNew(bool _bTax)
        {
            this.bTax = _bTax;
            InitializeComponent();
            this.Load += frmProductNew_Load;
        }

        private void frmProductNew_Load(object sender, EventArgs e)
        {
            AplicarEstiloVisual();
        }

        private void AplicarEstiloVisual()
        {
            ConfigurarTamano(anchoPorc: 0.42, altoPorc: 0.72);

            this.BackColor = AppColors.BackgroundPrimary;
            tableLayoutPanel2.BackColor = AppColors.BackgroundPrimary;

            bool esTax = bTax;

            // ── Header contextual — Verde (venta / precio) ────────────
            AgregarHeaderContextual(
                colorHeader : esTax ? AppColors.Warning      : AppColors.AccentGreen,
                icono       : esTax ? "$+" : "$",
                titulo      : esTax ? "Quick Sale  +Tax"     : "Quick Sale",
                subtitulo   : esTax ? "Enter price — tax will be applied"
                                    : "Enter the custom product price"
            );

            ElegantButtonStyles.Style(buttonOk,     esTax ? AppColors.Warning : AppColors.AccentGreen,
                                                    AppColors.TextWhite, fontSize: 20f);
            ElegantButtonStyles.Style(buttonCancel, AppColors.Danger, AppColors.TextWhite, fontSize: 20f);

            buttonOk.Text     = esTax ? "$+  ADD +TAX" : "$  ADD ITEM";
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
                Font      = new Font("Montserrat", 22F, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 255, 255, 255),
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
                using var pen = new Pen(Color.FromArgb(80, 0, 0, 0), 2);
                e.Graphics.DrawLine(pen, 0, panel.Height - 2, panel.Width, panel.Height - 2);
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
            decimal valor = keyPadMoneyControl1.GetValue();
            if (valor > 0)
            {
                ((frmHome)Owner).addCustomProduct(bTax, valor);
                this.Close();
            }
            else
            {
                MessageBox.Show("Please enter a valid amount greater than zero.", "Invalid Amount",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void buttonClose_Click(object sender, EventArgs e) => this.Close();
    }
}
