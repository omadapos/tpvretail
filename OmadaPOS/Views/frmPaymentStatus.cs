namespace OmadaPOS.Views
{
    public partial class frmPaymentStatus : Form
    {
        private string msg = "";

        public frmPaymentStatus(string msg)
        {
            InitializeComponent();

            this.msg = msg;
        }

        private void frmPaymentStatus_Load(object sender, EventArgs e)
        {
            labelMsg.Text = msg;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
