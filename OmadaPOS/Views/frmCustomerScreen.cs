using OmadaPOS.Libreria.Services;
using OmadaPOS.Services;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace OmadaPOS.Views
{
    public partial class frmCustomerScreen : Form
    {
        private readonly IBannerService? bannerService;
        private readonly IShoppingCart _shoppingCart;

        private string[]? imageUrls;
        private int currentIndex = 0;
        decimal totalGlobal = 0;

        private System.Windows.Forms.Timer? timerCarrousel;
        private System.Windows.Forms.Timer? clockTimer;

        public frmCustomerScreen()
        {
            InitializeComponent();

            bannerService = Program.GetService<IBannerService>();
            _shoppingCart = Program.GetService<IShoppingCart>();
            _shoppingCart.CartChanged += ShoppingCart_CartChanged;

            // Diseño profesional — debe aplicarse antes de mostrar controles
            AplicarDisenoCliente();
            ConfigureListView();

            LoadData();
            ConfigureTimers();

            labelWeight.Text = SharedData.WeightUnit;
            SharedData.WeightUnitChanged += OnWeightUnitChanged;

            LoadCart();

            this.KeyPreview = true;
            this.KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) this.Close(); };
        }

        // ═══════════════════════════════════════════════════════════════
        // TEMA VISUAL — PremiumMarket Customer Screen
        // ═══════════════════════════════════════════════════════════════
        private void AplicarDisenoCliente()
        {
            // ── Fondo general ─────────────────────────────────────────────
            this.BackColor                 = AppColors.NavyDark;
            tableLayoutPanelMain.BackColor = AppColors.NavyDark;
            tableLayoutPanelMain.Padding   = new Padding(0);
            tableLayoutPanelMain.Margin    = new Padding(0);

            // ── Header bienvenida (label1) ────────────────────────────────
            label1.BackColor  = AppColors.NavyBase;
            label1.ForeColor  = AppColors.TextWhite;
            label1.Font       = new Font("Montserrat", 30F, FontStyle.Bold);
            label1.TextAlign  = ContentAlignment.MiddleCenter;
            label1.Margin     = new Padding(0);
            label1.Text       = "✦  WELCOME  —  DAILY STOP  ✦";

            // ── Columna izquierda — Lista ─────────────────────────────────
            tableLayoutPanelLeft.BackColor = AppColors.BackgroundPrimary;
            tableLayoutPanelLeft.Padding   = new Padding(10, 10, 10, 0);
            tableLayoutPanelLeft.Margin    = new Padding(0);

            listViewCart.BackColor = Color.White;
            listViewCart.ForeColor = AppColors.TextPrimary;
            listViewCart.Font      = new Font("Segoe UI", 18F, FontStyle.Regular);
            listViewCart.GridLines = false;

            // ── Banner (columna derecha) ───────────────────────────────────
            tableLayoutPanel2.BackColor = AppColors.NavyDark;
            tableLayoutPanel2.Padding   = new Padding(10);
            tableLayoutPanel2.Margin    = new Padding(0);
            pictureBoxBanner.BackColor  = AppColors.NavyDark;
            pictureBoxBanner.SizeMode   = PictureBoxSizeMode.StretchImage;

            // ── Panel de totales ──────────────────────────────────────────
            // BackColor del panel se dibuja via panel1_Paint (rediseñado abajo)
            panel1.Margin  = new Padding(10, 10, 5, 10);

            label2.Font      = new Font("Segoe UI", 20F, FontStyle.Bold);
            label2.ForeColor = AppColors.TextMuted;
            label2.BackColor = Color.Transparent;
            label2.Text      = "TOTAL";

            labelTotal.Font      = new Font("Montserrat", 56F, FontStyle.Bold);
            labelTotal.ForeColor = AppColors.AccentGreen;
            labelTotal.BackColor = Color.Transparent;
            labelTotal.Text      = "$0.00";

            label3.Font      = new Font("Segoe UI", 18F, FontStyle.Bold);
            label3.ForeColor = AppColors.TextMuted;
            label3.BackColor = Color.Transparent;
            label3.Text      = "WEIGHT";

            labelWeight.Font      = new Font("Montserrat", 32F, FontStyle.Bold);
            labelWeight.ForeColor = AppColors.Warning;
            labelWeight.BackColor = Color.Transparent;

            // ── Panel del reloj (panel3) ──────────────────────────────────
            panel3.BackColor = AppColors.NavyBase;
            panel3.Margin    = new Padding(5, 10, 10, 10);
            panel3.Paint    += Panel3_Paint;

            labelHour.Font      = new Font("Montserrat", 26F, FontStyle.Bold);
            labelHour.ForeColor = AppColors.TextWhite;
            labelHour.BackColor = Color.Transparent;
            labelHour.TextAlign = ContentAlignment.MiddleCenter;
        }

        // ── ListView — columnas temáticas ─────────────────────────────────
        private void ConfigureListView()
        {
            listViewCart.View         = View.Details;
            listViewCart.FullRowSelect = true;
            listViewCart.GridLines    = false;
            listViewCart.MultiSelect  = false;

            listViewCart.Columns.Add("#",        80);
            listViewCart.Columns.Add("Product",  200);
            listViewCart.Columns.Add("Qty",       80);
            listViewCart.Columns.Add("Price",    100);
            listViewCart.Columns.Add("Subtotal", 100);

            foreach (ColumnHeader col in listViewCart.Columns)
                col.TextAlign = HorizontalAlignment.Center;

            listViewCart.OwnerDraw = true;

            // Header navy con texto blanco — igual que frmHome
            listViewCart.DrawColumnHeader += (s, e) =>
            {
                var g = e.Graphics;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                using var bgBrush = new SolidBrush(AppColors.NavyBase);
                g.FillRectangle(bgBrush, e.Bounds);

                using var sep = new Pen(Color.FromArgb(60, 255, 255, 255), 1);
                g.DrawLine(sep, e.Bounds.Right - 1, e.Bounds.Top + 4,
                                e.Bounds.Right - 1, e.Bounds.Bottom - 4);

                using var hFont = new Font("Segoe UI", 14F, FontStyle.Bold);
                using var tBrush = new SolidBrush(AppColors.TextWhite);
                var sf = new StringFormat
                {
                    Alignment     = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    Trimming      = StringTrimming.EllipsisCharacter
                };
                g.DrawString(e.Header.Text, hFont, tBrush,
                    new Rectangle(e.Bounds.X + 2, e.Bounds.Y, e.Bounds.Width - 4, e.Bounds.Height), sf);
            };

            // Filas alternadas (zebra) para legibilidad
            listViewCart.DrawItem += (s, e) =>
            {
                bool isAlt = e.ItemIndex % 2 == 1;
                var bg     = isAlt ? Color.FromArgb(245, 248, 252) : Color.White;
                using var brush = new SolidBrush(bg);
                e.Graphics.FillRectangle(brush, e.Bounds);
                e.DrawDefault = true;
            };
            listViewCart.DrawSubItem += (s, e) => e.DrawDefault = true;

            AdjustListViewColumns();
            listViewCart.Resize += (s, e) => AdjustListViewColumns();
        }

        private void AdjustListViewColumns()
        {
            int total  = listViewCart.ClientSize.Width;
            int count  = listViewCart.Columns.Count;
            if (count == 0) return;

            int colW   = total / count;
            int lastW  = total - colW * (count - 1);
            for (int i = 0; i < count; i++)
                listViewCart.Columns[i].Width = (i == count - 1) ? lastW : colW;
        }

        // ── Paint: Panel de totales ───────────────────────────────────────
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            var panel = (Panel)sender;
            var g     = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var bounds = new Rectangle(1, 1, panel.Width - 3, panel.Height - 3);

            // Fondo navy oscuro
            using var bgBrush = new SolidBrush(AppColors.NavyBase);
            using var path    = RoundedRect(bounds, 18);
            g.FillPath(bgBrush, path);

            // Borde sutil
            using var borderPen = new Pen(Color.FromArgb(60, 255, 255, 255), 1f);
            g.DrawPath(borderPen, path);

            // Barra lateral verde de acento (izquierda)
            using var accentBrush = new SolidBrush(AppColors.AccentGreen);
            var accentRect = new Rectangle(bounds.X, bounds.Y + 16, 6, bounds.Height - 32);
            using var accentPath = RoundedRect(accentRect, 3);
            g.FillPath(accentBrush, accentPath);

            panel.Region = new Region(path);
        }

        // ── Paint: Panel del reloj ────────────────────────────────────────
        private void Panel3_Paint(object sender, PaintEventArgs e)
        {
            var panel = (Panel)sender;
            var g     = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var bounds = new Rectangle(1, 1, panel.Width - 3, panel.Height - 3);

            using var bgBrush = new SolidBrush(AppColors.NavyBase);
            using var path    = RoundedRect(bounds, 18);
            g.FillPath(bgBrush, path);

            using var borderPen = new Pen(Color.FromArgb(60, 255, 255, 255), 1f);
            g.DrawPath(borderPen, path);

            // Barra superior verde (acento)
            using var accentBrush = new SolidBrush(AppColors.AccentGreen);
            var accentRect = new Rectangle(bounds.X + 16, bounds.Y, bounds.Width - 32, 5);
            using var accentPath = RoundedRect(accentRect, 2);
            g.FillPath(accentBrush, accentPath);

            panel.Region = new Region(path);
        }

        // ── Carrito ───────────────────────────────────────────────────────
        private async void LoadCart() => await _shoppingCart.LoadCartAsync();

        private void ShoppingCart_CartChanged(object? sender, EventArgs e)
        {
            if (InvokeRequired) { Invoke(UpdateCartDisplay); return; }
            UpdateCartDisplay();
        }

        public void UpdateCartDisplay()
        {
            listViewCart.Items.Clear();
            totalGlobal = 0;

            foreach (var item in _shoppingCart.Items)
            {
                var li = new ListViewItem(new[]
                {
                    item.Number.ToString(),
                    item.ProductName,
                    item.Quantity.ToString(),
                    item.UnitPrice.ToString("N2"),
                    item.Subtotal.ToString("N2")
                }) { Tag = item.ProductId };

                listViewCart.Items.Add(li);
                totalGlobal += item.Subtotal;
            }

            UpdateTotals();
        }

        private void UpdateTotals()
        {
            labelTotal.Text = $"${totalGlobal:N2}";
        }

        // ── Peso ──────────────────────────────────────────────────────────
        private void OnWeightUnitChanged(string newWeightUnit)
        {
            if (InvokeRequired)
                Invoke(new Action(() => labelWeight.Text = newWeightUnit));
            else
                labelWeight.Text = newWeightUnit;
        }

        // ── Timers ────────────────────────────────────────────────────────
        private void ConfigureTimers()
        {
            timerCarrousel = new System.Windows.Forms.Timer { Interval = 4000 };
            timerCarrousel.Tick += TimerCarrousel_Tick;
            timerCarrousel.Start();

            clockTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            clockTimer.Tick += Timer_Tick;
            clockTimer.Start();
        }

        private void TimerCarrousel_Tick(object sender, EventArgs e)
        {
            if (imageUrls != null && imageUrls.Length > 0)
            {
                LoadPicture();
                currentIndex = (currentIndex + 1) % imageUrls.Length;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            labelHour.Text = DateTime.Now.ToString("dddd\ndd/MM/yyyy   HH:mm:ss");
        }

        // ── Banners ───────────────────────────────────────────────────────
        private async void LoadData()
        {
            try
            {
                var list   = await bannerService!.LoadBanners();
                imageUrls  = list.Select(b => b.Image).ToArray();
                if (imageUrls.Length > 0)
                    LoadPicture();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading banners: {ex.Message}");
            }
        }

        private void LoadPicture()
        {
            if (imageUrls != null && imageUrls.Length > 0)
            {
                try { pictureBoxBanner.LoadAsync(imageUrls[currentIndex]); }
                catch { /* fallo silencioso */ }
            }
        }

        // ── Cierre limpio ─────────────────────────────────────────────────
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            timerCarrousel?.Stop();
            clockTimer?.Stop();
            SharedData.WeightUnitChanged -= OnWeightUnitChanged;
            base.OnFormClosing(e);
        }

        // ── Helper: path con esquinas redondeadas ─────────────────────────
        private static GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            int d    = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(r.X,          r.Y,          d, d, 180, 90);
            path.AddArc(r.Right - d,  r.Y,          d, d, 270, 90);
            path.AddArc(r.Right - d,  r.Bottom - d, d, d,   0, 90);
            path.AddArc(r.X,          r.Bottom - d, d, d,  90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
