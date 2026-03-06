using OmadaPOS.Libreria.Services;
using OmadaPOS.Services;
using System.Drawing.Drawing2D;

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

            // Configurar pantalla completa
            ConfigureListView();

            bannerService = Program.GetService<IBannerService>();
            _shoppingCart = Program.GetService<IShoppingCart>();
            _shoppingCart.CartChanged += ShoppingCart_CartChanged;

            // Configuración del banner
            pictureBoxBanner.Height = 100;

            // Price, Total

            // Cargar datos del banner
            LoadData();

            // Configuración y arranque de timers
            ConfigureTimers();

            // Inicialización de unidades de peso
            labelWeight.Text = "0.0";

            // Cargar carrito inicialmente
            LoadCart();

            // Agregar tecla de escape para cerrar en caso de emergencia
            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                    this.Close();
            };

            labelWeight.Text = SharedData.WeightUnit;
            SharedData.WeightUnitChanged += OnWeightUnitChanged;

        }

        private async void LoadCart()
        {
            await _shoppingCart.LoadCartAsync();
        }

        private void OnWeightUnitChanged(string newWeightUnit)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => labelWeight.Text = newWeightUnit));
            }
            else
            {
                labelWeight.Text = newWeightUnit;
            }
        }

        private void ConfigureTimers()
        {
            // Timer para carrusel de banners
            timerCarrousel = new System.Windows.Forms.Timer();
            timerCarrousel.Interval = 3000;
            timerCarrousel.Tick += TimerCarrousel_Tick;
            timerCarrousel.Start();

            // Timer para el reloj
            clockTimer = new System.Windows.Forms.Timer();
            clockTimer.Interval = 1000;
            clockTimer.Tick += Timer_Tick;
            clockTimer.Start();
        }

        private void ShoppingCart_CartChanged(object? sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateCartDisplay()));
                return;
            }
            UpdateCartDisplay();
        }

        public void UpdateCartDisplay()
        {
            listViewCart.Items.Clear();

            totalGlobal = 0;

            foreach (var item in _shoppingCart.Items)
            {
                var listItem = new ListViewItem(
                [
                    item.Number.ToString(),
                    item.ProductName,
                    item.Quantity.ToString(),
                    item.UnitPrice.ToString("N2"),
                    item.Subtotal.ToString("N2")
                ])
                {
                    Tag = item.ProductId
                };

                listViewCart.Items.Add(listItem);

                totalGlobal += item.Subtotal;
            }

            UpdateTotals();
        }

        private void UpdateTotals()
        {
            labelTotal.Text = totalGlobal.ToString("N2");
        }

        private void TimerCarrousel_Tick(object sender, EventArgs e)
        {
            // Cambia a la siguiente imagen
            if (imageUrls != null && imageUrls.Length > 0)
            {
                LoadPicture();
                currentIndex = (currentIndex + 1) % imageUrls.Length;
            }
        }

        private async void LoadData()
        {
            try
            {
                var list = await bannerService!.LoadBanners();
                imageUrls = list.Select(b => b.Image).ToArray();
                if (imageUrls.Length > 0)
                {
                    LoadPicture();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar banners: {ex.Message}");
            }
        }

        private void ConfigureListView()
        {
            // Configurar columnas del ListView
            listViewCart.View = View.Details;
            listViewCart.FullRowSelect = true;
            listViewCart.GridLines = true;
            listViewCart.MultiSelect = false;

            // Agregar columnas
            listViewCart.Columns.Add("#", 80);
            listViewCart.Columns.Add("Product", 200);
            listViewCart.Columns.Add("Quantity", 80);
            listViewCart.Columns.Add("Price", 100);
            listViewCart.Columns.Add("Subtotal", 100);

            // Estilo visual
            listViewCart.BackColor = Color.White;
            listViewCart.Font = new Font("Montserrat", 18F, FontStyle.Regular); // Subir fuente

            // Subir fuente y estilo de encabezados
            foreach (ColumnHeader column in listViewCart.Columns)
            {
                column.TextAlign = HorizontalAlignment.Center;
            }
            listViewCart.OwnerDraw = true;
            listViewCart.DrawColumnHeader += (s, e) =>
            {
                using (var headerFont = new Font("Montserrat", 22F, FontStyle.Bold))
                using (var brush = new SolidBrush(Color.Black))
                using (var bgBrush = new SolidBrush(Color.Gainsboro))
                {
                    e.Graphics.FillRectangle(bgBrush, e.Bounds);
                    e.Graphics.DrawString(e.Header.Text, headerFont, brush, e.Bounds);
                }
            };
            listViewCart.DrawItem += (s, e) => e.DrawDefault = true;
            listViewCart.DrawSubItem += (s, e) => e.DrawDefault = true;

            // Ajustar el ancho de las columnas al ancho del ListView
            AdjustListViewColumns();
            listViewCart.Resize += (s, e) => AdjustListViewColumns();
        }

        private void AdjustListViewColumns()
        {
            // Sumar el ancho de los bordes y el scrollbar si es necesario
            int totalWidth = listViewCart.ClientSize.Width;
            int columnCount = listViewCart.Columns.Count;
            if (columnCount == 0) return;

            int columnWidth = totalWidth / columnCount;
            int lastColumnWidth = totalWidth - (columnWidth * (columnCount - 1));

            for (int i = 0; i < columnCount; i++)
            {
                listViewCart.Columns[i].Width = (i == columnCount - 1) ? lastColumnWidth : columnWidth;
            }
        }

        private void LoadPicture()
        {
            if (imageUrls != null && imageUrls.Length > 0)
            {
                try
                {
                    pictureBoxBanner.LoadAsync(imageUrls[currentIndex]);
                }
                catch
                {
                    // Manejo silencioso de errores de carga
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            labelHour.Text = DateTime.Now.ToString("dddd, dd/MM/yyyy HH:mm:ss");
        }

        private decimal CalculateItemTotal(dynamic row, double quantity, decimal price)
        {
            // Sin promoción
            if (string.IsNullOrEmpty(row.PromotionName))
            {
                return (decimal)quantity * price;
            }

            // Con promoción de precio
            if (row.PromotionName.Equals("Price"))
            {
                double quantityPromotion = row.PromotionValue;
                decimal precioPromocion = row.PromotionLimit;

                int vecesPromocion = (int)(quantity / quantityPromotion);
                int productosRestantes = (int)(quantity % quantityPromotion);

                return vecesPromocion * precioPromocion + productosRestantes * price;
            }

            // Otros tipos de promoción
            return (decimal)quantity * price;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Detener timers
            if (timerCarrousel != null) timerCarrousel.Stop();
            if (clockTimer != null) clockTimer.Stop();

            // Limpiar suscripciones
            SharedData.WeightUnitChanged -= OnWeightUnitChanged;

            base.OnFormClosing(e);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            // Obtener referencia al panel
            Panel panel = (Panel)sender;

            // Configurar calidad de gráficos
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Definir parámetros de diseño
            int borderRadius = 20;
            int borderSize = 1;
            int padding = 20; // Padding de 20
            Color borderColor = Color.FromArgb(30, 64, 175);      // Azul profundo para bordes
            Color fillColor = Color.White;                        // Fondo blanco limpio
            bool useGradient = true;
            Color gradientStart = Color.FromArgb(147, 197, 253);  // Azul claro suave
            Color gradientEnd = Color.FromArgb(59, 130, 246);     // Azul vibrante (intermedio/profesional)

            // Crear path para el panel redondeado
            GraphicsPath path = new GraphicsPath();
            Rectangle rect = panel.ClientRectangle;

            // Ajustar el rectángulo para el borde y el padding
            Rectangle drawRect = new Rectangle(
                rect.Left + padding + (borderSize / 2),
                rect.Top + padding + (borderSize / 2),
                rect.Width - (2 * padding) - borderSize,
                rect.Height - (2 * padding) - borderSize);

            // Crear esquinas redondeadas
            path.AddArc(drawRect.X, drawRect.Y, borderRadius, borderRadius, 180, 90);
            path.AddArc(drawRect.Right - borderRadius, drawRect.Y, borderRadius, borderRadius, 270, 90);
            path.AddArc(drawRect.Right - borderRadius, drawRect.Bottom - borderRadius, borderRadius, borderRadius, 0, 90);
            path.AddArc(drawRect.X, drawRect.Bottom - borderRadius, borderRadius, borderRadius, 90, 90);
            path.CloseFigure();

            // Aplicar región al panel (opcional - esto hará que el panel sea realmente redondeado)
            // panel.Region = new Region(path);

            // Rellenar fondo
            if (useGradient)
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    rect, gradientStart, gradientEnd, System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal))
                {
                    e.Graphics.FillPath(brush, path);
                }
            }
            else
            {
                using (SolidBrush brush = new SolidBrush(fillColor))
                {
                    e.Graphics.FillPath(brush, path);
                }
            }

            // Dibujar borde
            using (Pen pen = new Pen(borderColor, borderSize))
            {
                pen.Alignment = PenAlignment.Inset;
                e.Graphics.DrawPath(pen, path);
            }
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {
            // Obtener referencia al panel
            Panel panel = (Panel)sender;

            // Configurar calidad de gráficos para mejor renderizado
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // Definir parámetros de diseño mejorados
            int borderRadius = 25;
            int borderSize = 2;

            // Paleta de colores azul moderna
            Color borderColor = Color.FromArgb(70, 130, 180);        // Azul acero
            Color shadowColor = Color.FromArgb(40, 60, 90, 130);     // Sombra azul sutil
            Color gradientStart = Color.FromArgb(240, 248, 255);     // Azul hielo claro
            Color gradientMid = Color.FromArgb(200, 220, 240);       // Azul intermedio
            Color gradientEnd = Color.FromArgb(135, 170, 210);       // Azul profundo

            Rectangle rect = panel.ClientRectangle;

            // Crear sombra para efecto de profundidad
            Rectangle shadowRect = new Rectangle(
                rect.Left + 3,
                rect.Top + 3,
                rect.Width - 3,
                rect.Height - 3);

            GraphicsPath shadowPath = CreateRoundedRectanglePath(shadowRect, borderRadius);
            using (SolidBrush shadowBrush = new SolidBrush(shadowColor))
            {
                e.Graphics.FillPath(shadowBrush, shadowPath);
            }

            // Rectángulo principal ajustado para el borde
            Rectangle drawRect = new Rectangle(
                rect.Left + (borderSize / 2),
                rect.Top + (borderSize / 2),
                rect.Width - borderSize - 3,
                rect.Height - borderSize - 3);

            // Crear path para el panel redondeado
            GraphicsPath path = CreateRoundedRectanglePath(drawRect, borderRadius);

            // Aplicar gradiente triple para mayor profundidad
            using (LinearGradientBrush brush = new LinearGradientBrush(
                drawRect, gradientStart, gradientEnd, LinearGradientMode.ForwardDiagonal))
            {
                // Crear blend personalizado para gradiente más suave
                ColorBlend colorBlend = new ColorBlend(3);
                colorBlend.Colors = new Color[] { gradientStart, gradientMid, gradientEnd };
                colorBlend.Positions = new float[] { 0.0f, 0.5f, 1.0f };
                brush.InterpolationColors = colorBlend;

                e.Graphics.FillPath(brush, path);
            }

            // Agregar highlight interno para efecto glass
            Rectangle highlightRect = new Rectangle(
                drawRect.X + 2,
                drawRect.Y + 2,
                drawRect.Width - 4,
                drawRect.Height / 2);

            GraphicsPath highlightPath = CreateRoundedRectanglePath(highlightRect, borderRadius - 2);
            using (LinearGradientBrush highlightBrush = new LinearGradientBrush(
                highlightRect,
                Color.FromArgb(80, 255, 255, 255),
                Color.FromArgb(20, 255, 255, 255),
                LinearGradientMode.Vertical))
            {
                e.Graphics.FillPath(highlightBrush, highlightPath);
            }

            // Dibujar borde principal con efecto biselado
            using (Pen outerPen = new Pen(borderColor, borderSize))
            {
                outerPen.Alignment = PenAlignment.Inset;
                e.Graphics.DrawPath(outerPen, path);
            }

            // Borde interno más claro para efecto 3D
            Rectangle innerRect = new Rectangle(
                drawRect.X + 1,
                drawRect.Y + 1,
                drawRect.Width - 2,
                drawRect.Height - 2);

            GraphicsPath innerPath = CreateRoundedRectanglePath(innerRect, borderRadius - 2);
            using (Pen innerPen = new Pen(Color.FromArgb(100, 180, 210, 240), 1))
            {
                e.Graphics.DrawPath(innerPen, innerPath);
            }

            // Opcional: aplicar región al panel para hacerlo realmente redondeado
            // panel.Region = new Region(path);
        }

        // Método auxiliar para crear rectángulos redondeados
        private GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();

            // Esquina superior izquierda
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            // Esquina superior derecha
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            // Esquina inferior derecha
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            // Esquina inferior izquierda
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);

            path.CloseFigure();
            return path;
        }
    }
}