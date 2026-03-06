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
            this.number = number;
            this.productId = productId;
        }

        private void frmPopupQuantity_Load(object sender, EventArgs e)
        {
            AplicarEstiloVisual();
            keyPadControl1.Setdata("");
        }

        private void AplicarEstiloVisual()
        {
            // Fondo y estilo base
            this.BackColor = Color.FromArgb(245, 245, 245); // gris claro elegante

            // Estilo clásico de formulario con borde
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ControlBox = false; // opcional, para ocultar íconos de cerrar

            // Estilo botones
            ElegantButtonStyles.Style(buttonOK, ElegantButtonStyles.Buttonok, fontSize: 18f);
            ElegantButtonStyles.Style(buttonCancel, ElegantButtonStyles.ButtonCacnel, fontSize: 18f);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (int.TryParse(keyPadControl1.Getdata(), out int quantity) && quantity > 0)
            {
                if (quantity > 0)
                {
                    ((frmHome)Owner).ChangeQuantity(quantity, productId);

                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid quantity greater than 0.", "Invalid Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
