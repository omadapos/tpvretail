namespace OmadaPOS.Views
{
    public partial class frmProductNew : Form
    {
        bool bTax = false;

        public frmProductNew(bool _bTax)
        {
            this.bTax = _bTax;

            InitializeComponent();
            ElegantButtonStyles.Style(buttonOk, ElegantButtonStyles.Buttonok, fontSize: 18f);
            ElegantButtonStyles.Style(buttonCancel, ElegantButtonStyles.ButtonCacnel, fontSize: 18f);
            this.Text += bTax ? " - Tax" : "";
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
                MessageBox.Show("Please enter a valid amount greater than zero.", "Invalid Amount", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
