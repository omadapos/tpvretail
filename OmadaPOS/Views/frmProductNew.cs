using OmadaPOS.Services.Navigation;

namespace OmadaPOS.Views
{
    public partial class frmProductNew : Form
    {
        private readonly IHomeInteractionService _homeInteractionService;
        bool bTax = false;

        public frmProductNew(bool _bTax, IHomeInteractionService homeInteractionService)
        {
            this.bTax = _bTax;
            InitializeComponent();
            _homeInteractionService = homeInteractionService;
            this.Load += frmProductNew_Load;
        }

        private void frmProductNew_Load(object sender, EventArgs e)
        {
            AplicarEstiloVisual();
        }

        private void AplicarEstiloVisual()
        {
            PopupHeaderHelper.ConfigureSize(this, widthPct: 0.42, heightPct: 0.72);

            this.BackColor = AppColors.BackgroundPrimary;
            tableLayoutPanel2.BackColor = AppColors.BackgroundPrimary;

            PopupHeaderHelper.AddHeader(this,
                color    : bTax ? AppColors.Warning     : AppColors.AccentGreen,
                icon     : bTax ? "$+"                  : "$",
                title    : bTax ? "Quick Sale  +Tax"    : "Quick Sale",
                subtitle : bTax ? "Enter price — tax will be applied"
                                : "Enter the custom product price");

            ElegantButtonStyles.Style(buttonOk,     bTax ? AppColors.Warning : AppColors.AccentGreen,
                                                    AppColors.TextWhite, fontSize: 20f);
            ElegantButtonStyles.Style(buttonCancel, AppColors.Danger, AppColors.TextWhite, fontSize: 20f);

            buttonOk.Text     = bTax ? "$+  ADD +TAX" : "$  ADD ITEM";
            buttonCancel.Text = "✕  CANCEL";
        }

        private async void buttonOK_Click(object sender, EventArgs e)
        {
            try
            {
                decimal valor = keyPadMoneyControl1.ValueDecimal;
                if (valor > 0)
                {
                    await _homeInteractionService.RequestCustomProductAsync(bTax, valor);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Please enter a valid amount greater than zero.", "Invalid Amount",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar producto personalizado:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonClose_Click(object sender, EventArgs e) => this.Close();
    }
}
