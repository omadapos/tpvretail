using OmadaPOS.Libreria.Services;

namespace OmadaPOS.Views
{
    public partial class frmCheckPrice : Form
    {
        private readonly ICategoryService _categoryService;

        public frmCheckPrice()
        {
            InitializeComponent();

            _categoryService = Program.GetService<ICategoryService>();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

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
                        string sPrice = upc.Substring(7, 4);

                        decimal price = decimal.Parse(sPrice) / 100;

                        string code = upc.Substring(1, 5);

                        //var plu = await categoryService.LoadPluByCode(code);
                        var plu = await _categoryService.LoadProductByUPC_Promotion(code);
                        if (plu != null)
                        {
                            labelName.Text = plu.Name;
                            labelPrice.Text = price.ToString("N2");

                            textUPC.Text = "";

                        }
                    }
                    catch (Exception)
                    {
                        var frm = new frmError("Error reading codebar");
                        frm.ShowDialog(this);
                    }
                }
                else
                {
                    var product = await _categoryService.LoadProductByUPC_Promotion(upc);

                    if (product != null)
                    {
                        labelName.Text = product.Name;
                        labelPrice.Text = (product.Price ?? 0m).ToString("N2"); 

                        textUPC.Text = "";

                    }
                
                }

            }

        }
    }
}
