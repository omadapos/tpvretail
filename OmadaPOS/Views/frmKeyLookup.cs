using OmadaPOS.Services.Navigation;

namespace OmadaPOS.Views
{
    public partial class frmKeyLookup : Form
    {
        private readonly IHomeInteractionService _homeInteractionService;

        public frmKeyLookup(IHomeInteractionService homeInteractionService)
        {
            InitializeComponent();
            _homeInteractionService = homeInteractionService;
            this.Load += frmKeyLookup_Load;
        }

        private void frmKeyLookup_Load(object sender, EventArgs e)
        {
            AplicarEstiloVisual();
        }

        private void AplicarEstiloVisual()
        {
            PopupHeaderHelper.ConfigureSize(this, widthPct: 0.42, heightPct: 0.72);

            this.BackColor = AppColors.BackgroundPrimary;
            tableLayoutPanel1.BackColor = AppColors.BackgroundPrimary;

            PopupHeaderHelper.AddHeader(this,
                color    : AppColors.Info,
                icon     : "⌕",
                title    : "Product Lookup",
                subtitle : "Scan or enter the UPC / barcode");

            ElegantButtonStyles.Style(buttonOK,     AppColors.Info,   AppColors.TextWhite, fontSize: 18f);
            ElegantButtonStyles.Style(buttonCancel, AppColors.Danger, AppColors.TextWhite, fontSize: 18f);
            buttonOK.Text     = "⌕  SEARCH";
            buttonCancel.Text = "✕  CANCEL";
        }

        private async void buttonOK_Click(object sender, EventArgs e)
        {
            try
            {
                var code = keyPadControl1.TextValue;
                if (!string.IsNullOrEmpty(code))
                {
                    await _homeInteractionService.RequestProductSearchAsync(code);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar producto:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e) => this.Close();
    }
}
