

namespace OmadaPOS.Views
{
    public partial class frmKeyLookup : Form
    {
        public frmKeyLookup()
        {
            InitializeComponent();
            ElegantButtonStyles.Style(buttonOK, ElegantButtonStyles.Buttonok, fontSize: 18f);
            ElegantButtonStyles.Style(buttonCancel, ElegantButtonStyles.ButtonCacnel, fontSize: 18f);
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

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
