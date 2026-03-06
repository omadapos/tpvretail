using System;
using System.Drawing;
using System.Windows.Forms;

namespace OmadaPOS.Views
{
    public partial class frmPopupQuantity : Form
    {
        int number = 0;
        int productId = 0;

        public frmPopupQuantity(int number, int productId)
        {
            InitializeComponent();
            this.number    = number;
            this.productId = productId;
        }

        private void frmPopupQuantity_Load(object sender, EventArgs e)
        {
            AplicarEstiloVisual();
            keyPadControl1.Setdata("");
        }

        private void AplicarEstiloVisual()
        {
            ConfigurarTamano(anchoPorc: 0.42, altoPorc: 0.72);

            this.BackColor         = AppColors.BackgroundPrimary;
            this.FormBorderStyle   = FormBorderStyle.FixedDialog;
            this.StartPosition     = FormStartPosition.CenterParent;
            this.MaximizeBox       = false;
            this.MinimizeBox       = false;
            this.ControlBox        = false;

            tableLayoutPanel1.BackColor = AppColors.BackgroundPrimary;

            // ── Header contextual — Azul marino (operación de cantidad) ──
            AgregarHeaderContextual(
                colorHeader : AppColors.NavyBase,
                icono       : "#",
                titulo      : "Change Quantity",
                subtitulo   : "Enter the new quantity for this item"
            );

            ElegantButtonStyles.Style(buttonOK,     AppColors.AccentGreen, AppColors.TextWhite, fontSize: 18f);
            ElegantButtonStyles.Style(buttonCancel, AppColors.Danger,      AppColors.TextWhite, fontSize: 18f);
            buttonOK.Text     = "✔  CONFIRM";
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
                ForeColor = Color.FromArgb(200, 255, 255, 255),
                AutoSize  = true,
                Location  = new Point(16, 16),
                BackColor = Color.Transparent
            };

            var lblTitulo = new Label
            {
                Text      = titulo,
                Font      = new Font("Montserrat", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize  = true,
                Location  = new Point(60, 10),
                BackColor = Color.Transparent
            };

            var lblSub = new Label
            {
                Text      = subtitulo,
                Font      = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = Color.FromArgb(200, 255, 255, 255),
                AutoSize  = true,
                Location  = new Point(61, 38),
                BackColor = Color.Transparent
            };

            // Línea de acento en el borde inferior del header
            panel.Paint += (s, e) =>
            {
                using var pen = new Pen(AppColors.AccentGreen, 3);
                e.Graphics.DrawLine(pen, 0, panel.Height - 3, panel.Width, panel.Height - 3);
            };

            panel.Controls.Add(lblIcono);
            panel.Controls.Add(lblTitulo);
            panel.Controls.Add(lblSub);
            this.Controls.Add(panel);
            panel.BringToFront();
        }

        /// <summary>Ajusta el formulario a un porcentaje de la pantalla y lo centra.</summary>
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
            if (int.TryParse(keyPadControl1.Getdata(), out int quantity) && quantity > 0)
            {
                ((frmHome)Owner).ChangeQuantity(quantity, productId);
                this.Close();
            }
            else
            {
                MessageBox.Show("Please enter a valid quantity greater than 0.", "Invalid Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void buttonClose_Click(object sender, EventArgs e) => this.Close();
    }
}
