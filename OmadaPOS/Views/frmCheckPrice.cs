using OmadaPOS.Libreria.Services;
using OmadaPOS.Services.Navigation;

namespace OmadaPOS.Views
{
    public partial class frmCheckPrice : Form
    {
        private readonly ICategoryService _categoryService;
        private readonly IWindowService _windowService;

        public frmCheckPrice(ICategoryService categoryService, IWindowService windowService)
        {
            InitializeComponent();

            _categoryService = categoryService;
            _windowService   = windowService;

            this.Load += frmCheckPrice_Load;
        }

        private void frmCheckPrice_Load(object sender, EventArgs e)
        {
            AplicarEstiloVisual();
            textUPC.Focus();
        }

        private void AplicarEstiloVisual()
        {
            PopupHeaderHelper.ConfigureSize(this, widthPct: 0.40, heightPct: 0.65);

            this.BackColor = AppColors.BackgroundPrimary;
            tableLayoutPanel1.BackColor = AppColors.BackgroundPrimary;

            PopupHeaderHelper.AddHeader(this,
                color    : AppColors.Info,
                icon     : "$",
                title    : "Check Price",
                subtitle : "Scan or enter UPC to look up the price");

            labelName.Font      = AppTypography.SectionTitle;
            labelName.ForeColor = AppColors.TextPrimary;
            labelName.BackColor = AppColors.BackgroundPrimary;

            labelPrice.Font      = AppTypography.AmountHero;
            labelPrice.ForeColor = AppColors.AccentGreen;
            labelPrice.BackColor = AppColors.BackgroundPrimary;

            textUPC.BackColor   = AppColors.NavyDark;
            textUPC.ForeColor   = AppColors.AccentGreen;
            textUPC.Font        = AppTypography.ScanInput;
            textUPC.BorderStyle = BorderStyle.None;

            ElegantButtonStyles.Style(buttonOK, AppColors.NavyBase, AppColors.TextWhite, fontSize: 22f);
            buttonOK.Text = "✔  CLOSE";
        }

        private void buttonOK_Click(object sender, EventArgs e) => this.Close();

        private void textUPC_TextChanged(object sender, EventArgs e)
        {
            SearchProduct(textUPC.Text);
        }

        public async void SearchProduct(string upc)
        {
            if (!string.IsNullOrEmpty(upc))
            {
                if (upc.Length > 2 && upc.Substring(0, 2) == "20")
                {
                    try
                    {
                        string  sPrice = upc.Substring(7, 4);
                        decimal price  = decimal.Parse(sPrice) / 100;
                        string  code   = upc.Substring(1, 5);

                        var plu = await _categoryService.LoadProductByUPC_Promotion(code);
                        if (plu != null)
                        {
                            labelName.Text  = plu.Name;
                            labelPrice.Text = price.ToString("C");
                            textUPC.Text    = "";
                        }
                    }
                    catch (Exception ex)
                    {
                        _windowService.OpenError(ex.Message);
                    }
                }
                else
                {
                    try
                    {
                        var plu = await _categoryService.LoadProductByUPC_Promotion(upc);
                        if (plu != null)
                        {
                            labelName.Text  = plu.Name;
                            labelPrice.Text = plu.Price?.ToString("C") ?? "—";
                            textUPC.Text    = "";
                        }
                    }
                    catch (Exception ex)
                    {
                        _windowService.OpenError(ex.Message);
                    }
                }
            }
        }
    }
}
