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

            this.Load += frmCheckPrice_Load;
        }

        private void frmCheckPrice_Load(object sender, EventArgs e)
        {
            AplicarEstiloVisual();
            textUPC.Focus();
        }

        private void AplicarEstiloVisual()
        {
            ConfigurarTamano(anchoPorc: 0.40, altoPorc: 0.65);

            this.BackColor = AppColors.BackgroundPrimary;
            tableLayoutPanel1.BackColor = AppColors.BackgroundPrimary;

            // ── Header contextual — Azul info (consulta de precio) ──
            AgregarHeader();

            // ── Labels de resultado ──
            labelName.Font      = new Font("Montserrat", 22F, FontStyle.Bold);
            labelName.ForeColor = AppColors.TextPrimary;
            labelName.BackColor = AppColors.BackgroundPrimary;

            labelPrice.Font      = new Font("Montserrat", 40F, FontStyle.Bold);
            labelPrice.ForeColor = AppColors.AccentGreen;
            labelPrice.BackColor = AppColors.BackgroundPrimary;

            // ── TextBox de escaneo ──
            textUPC.BackColor   = AppColors.NavyDark;
            textUPC.ForeColor   = AppColors.AccentGreen;
            textUPC.Font        = new Font("Consolas", 18F, FontStyle.Bold);
            textUPC.BorderStyle = BorderStyle.None;

            // ── Botón OK — NavyBase (cerrar/dismissal, no destructivo) ──
            ElegantButtonStyles.Style(buttonOK, AppColors.NavyBase, AppColors.TextWhite, fontSize: 22f);
            buttonOK.Text = "✔  CLOSE";
        }

        private void AgregarHeader()
        {
            var panel = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 70,
                BackColor = AppColors.Info,
            };

            var lblIcono = new Label
            {
                Text      = "$",
                Font      = new Font("Montserrat", 24F, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 255, 255, 255),
                AutoSize  = true,
                Location  = new Point(16, 12),
                BackColor = Color.Transparent
            };

            var lblTitulo = new Label
            {
                Text      = "Check Price",
                Font      = new Font("Montserrat", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize  = true,
                Location  = new Point(52, 10),
                BackColor = Color.Transparent
            };

            var lblSub = new Label
            {
                Text      = "Scan or enter UPC to look up the price",
                Font      = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = Color.FromArgb(200, 255, 255, 255),
                AutoSize  = true,
                Location  = new Point(53, 38),
                BackColor = Color.Transparent
            };

            panel.Paint += (s, e) =>
            {
                using var pen = new Pen(AppColors.AccentGreen, 3);
                e.Graphics.DrawLine(pen, 0, panel.Height - 3, panel.Width, panel.Height - 3);
            };

            panel.Controls.Add(lblIcono);
            panel.Controls.Add(lblTitulo);
            panel.Controls.Add(lblSub);
            this.Controls.Add(panel);
            panel.BringToFront();
        }

        private void ConfigurarTamano(double anchoPorc, double altoPorc)
        {
            var screen    = Screen.PrimaryScreen!.WorkingArea;
            this.Width    = (int)(screen.Width  * anchoPorc);
            this.Height   = (int)(screen.Height * altoPorc);
            this.Location = new Point(
                screen.X + (screen.Width  - this.Width)  / 2,
                screen.Y + (screen.Height - this.Height) / 2);
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
                        labelName.Text  = product.Name;
                        labelPrice.Text = (product.Price ?? 0m).ToString("C");
                        textUPC.Text    = "";
                    }
                }
            }
        }
    }
}
