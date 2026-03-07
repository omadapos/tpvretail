using OmadaPOS.Componentes;
using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Services;

namespace OmadaPOS.Views
{
    public partial class frmProductNoExist :Form
    {
        private ICategoryService categoryService;
        string upc = "";
        string imageUrl = "";

        public frmProductNoExist(string upc)
        {
            InitializeComponent();
            ElegantButtonStyles.Style(buttonOk, ElegantButtonStyles.Buttonok, fontSize: 18f);
            ElegantButtonStyles.Style(buttonCancel, ElegantButtonStyles.ButtonCacnel, fontSize: 18f);
            this.upc = upc;

            categoryService = Program.GetService<ICategoryService>();
        }
        private async void frmProductNoExist_Load(object sender, EventArgs e)
        {
            try
            {
                textBoxUPC.Text = upc;
                var product = await categoryService.LoadProductInfoByUPC(upc);
                if (product != null)
                {
                    textBoxName.Text = product.Name ?? "cp";
                    imageUrl = product.Image ?? string.Empty;
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        picThumb.SizeMode = PictureBoxSizeMode.StretchImage;
                        picThumb.LoadAsync(imageUrl);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar información del producto:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                decimal dueValue = keyPadMoneyControl1.GetValue();
                if (dueValue == 0)
                {
                    MessageBox.Show("Por favor ingrese un precio válido.", "Precio requerido",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                buttonOk.Enabled = false;
                var prod = new ProductCreateModel
                {
                    Name           = textBoxName.Text,
                    Short_Name     = "...",
                    Description    = "...",
                    Price          = (double)dueValue,
                    Status         = 1,
                    BranchId       = SessionManager.BranchId,
                    Tax            = checkBoxTax.Checked ? 7 : 0,
                    CategoryId     = 761,
                    Display_Addons = false,
                    Display_Sides  = false,
                    Upc            = upc,
                    Ebt            = checkBoxEBT.Checked,
                    Wic            = false,
                    Stock          = 0,
                    Cost           = 0
                };

                if (!string.IsNullOrEmpty(imageUrl))
                    prod.Image = imageUrl;

                await categoryService.SaveProduct(prod);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el producto:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                buttonOk.Enabled = true;
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
