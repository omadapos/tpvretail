using OmadaPOS.Services.Navigation;

namespace OmadaPOS.Views
{
    public partial class frmPopupQuantity : Form
    {
        private readonly IHomeInteractionService _homeInteractionService;
        int number = 0;
        int productId = 0;

        public frmPopupQuantity(int number, int productId, IHomeInteractionService homeInteractionService)
        {
            InitializeComponent();
            this.number    = number;
            this.productId = productId;
            _homeInteractionService = homeInteractionService;
        }

        private void frmPopupQuantity_Load(object sender, EventArgs e)
        {
            AplicarEstiloVisual();
            keyPadControl1.Reset();
        }

        private void AplicarEstiloVisual()
        {
            PopupHeaderHelper.ConfigureSize(this, widthPct: 0.42, heightPct: 0.72);

            this.BackColor         = AppColors.BackgroundPrimary;
            this.FormBorderStyle   = FormBorderStyle.FixedDialog;
            this.StartPosition     = FormStartPosition.CenterParent;
            this.MaximizeBox       = false;
            this.MinimizeBox       = false;
            this.ControlBox        = false;

            tableLayoutPanel1.BackColor = AppColors.BackgroundPrimary;

            PopupHeaderHelper.AddHeader(this,
                color    : AppColors.NavyBase,
                icon     : "#",
                title    : "Change Quantity",
                subtitle : "Enter the new quantity for this item");

            ElegantButtonStyles.Style(buttonOK,     AppColors.AccentGreen, AppColors.TextWhite, fontSize: 18f);
            ElegantButtonStyles.Style(buttonCancel, AppColors.Danger,      AppColors.TextWhite, fontSize: 18f);
            buttonOK.Text     = "✔  CONFIRM";
            buttonCancel.Text = "✕  CANCEL";
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (int.TryParse(keyPadControl1.TextValue, out int quantity) && quantity > 0)
            {
                _homeInteractionService.RequestQuantityChange(quantity, productId);
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
