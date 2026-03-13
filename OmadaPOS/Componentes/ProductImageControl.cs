using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using OmadaPOS.Libreria.Extensions;
using OmadaPOS.Libreria.Models;

namespace OmadaPOS.Componentes
{
    public partial class ProductImageControl : UserControl
    {
        // ── API pública ───────────────────────────────────────────────────
        private ProductModel? _product;
        public event EventHandler? ProductClicked;
        public ProductModel? Product => _product;

        // ── Constantes de tamaño (deben declararse antes de los campos static) ──
        private const int CardW        = 185;
        private const int CardH        = 215;
        private const int Margin       = 6;
        private const int CornerRadius = 14;
        private const int ImageAreaH   = 120;
        private const int AccentH      = 4;

        // ── Colores — todos desde AppColors ──────────────────────────────
        private static readonly Color CardBackground  = AppColors.SurfaceCard;
        private static readonly Color ImageBackground = AppColors.BackgroundSecondary;
        private static readonly Color BorderNormal    = AppColors.SurfaceMuted;
        private static readonly Color NameForeground  = AppColors.TextPrimary;
        private static readonly Color PriceForeground = AppColors.AccentGreen;
        private static readonly Color AccentBar       = AppColors.AccentGreen;

        // ── Recursos GDI compartidos — una sola instancia para TODOS los controles ──
        private static readonly Font       FontTitle    = new("Segoe UI", 10F, FontStyle.Bold);
        private static readonly Font       FontPrice    = new("Consolas", 13F, FontStyle.Bold);

        // Brushes/Pens: reutilizados en cada Paint, sin allocations
        private static readonly SolidBrush CardBrush    = new(AppColors.SurfaceCard);
        private static readonly SolidBrush ShadowBrush1 = new(Color.FromArgb(6,  0, 0, 0));
        private static readonly SolidBrush ShadowBrush2 = new(Color.FromArgb(10, 0, 0, 0));
        private static readonly SolidBrush ShadowBrush3 = new(Color.FromArgb(14, 0, 0, 0));
        private static readonly Pen        PenNormal    = new(AppColors.SurfaceMuted, 1f);

        // ── Paths GDI pre-computados — tamaño fijo por las constantes ────
        // El static constructor los crea UNA vez para toda la vida de la app.
        // El Paint event los reutiliza: 0 allocations GDI por repintado.
        private static readonly GraphicsPath CardPath;
        private static readonly GraphicsPath ShadowPath1; // offset 3 (más alejada)
        private static readonly GraphicsPath ShadowPath2; // offset 2
        private static readonly GraphicsPath ShadowPath3; // offset 1 (más cercana)

        static ProductImageControl()
        {
            var b = new Rectangle(2, 2, CardW - 5, CardH - 5);
            CardPath    = CreateRoundedPath(b, CornerRadius);
            ShadowPath1 = CreateRoundedPath(new Rectangle(b.X + 3, b.Y + 3, b.Width, b.Height), CornerRadius);
            ShadowPath2 = CreateRoundedPath(new Rectangle(b.X + 2, b.Y + 2, b.Width, b.Height), CornerRadius);
            ShadowPath3 = CreateRoundedPath(new Rectangle(b.X + 1, b.Y + 1, b.Width, b.Height), CornerRadius);
        }

        // ── Caché de imágenes (app-lifetime) ─────────────────────────────
        // Evita re-descargar/re-leer la misma imagen al filtrar por letra.
        // Capped at MaxCacheSize entries; oldest entry is evicted when full.
        private const int MaxCacheSize = 200;
        private static readonly Dictionary<string, Image> _imageCache   = [];
        private static readonly Queue<string>             _cacheKeyQueue = new();

        private Region? _cardRegion;

        // ── Controles internos ────────────────────────────────────────────
        private PictureBox? pictureBoxImage;
        private Label?      labelTitle;
        private Label?      labelPrice;
        private Panel?      panelCard;
        private Panel?      panelImageArea;
        private Panel?      panelInfo;
        private Panel?      panelAccent;

        public ProductImageControl(ProductModel? product)
        {
            _product = product;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Double-buffer el UserControl exterior — elimina flickering al
            // actualizar el grid completo en el filtro por letra.
            this.SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint,
                true);

            this.BackColor = Color.Transparent;
            this.Size      = new Size(CardW + Margin * 2, CardH + Margin * 2);
            this.Cursor    = Cursors.Default;

            panelCard = new Panel
            {
                BackColor = CardBackground,
                Location  = new Point(Margin, Margin),
                Size      = new Size(CardW, CardH),
            };
            panelCard.Paint  += PanelCard_Paint;
            panelCard.Click  += OnCardClick;

            panelImageArea = new Panel
            {
                BackColor = ImageBackground,
                Location  = new Point(0, 0),
                Size      = new Size(CardW, ImageAreaH),
            };
            panelImageArea.Click += OnCardClick;

            pictureBoxImage = new PictureBox
            {
                Dock      = DockStyle.Fill,
                SizeMode  = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Padding   = new Padding(8),
            };
            pictureBoxImage.Click += OnCardClick;
            panelImageArea.SuspendLayout();
            panelImageArea.Controls.Add(pictureBoxImage);
            panelImageArea.ResumeLayout(false);

            panelAccent = new Panel
            {
                BackColor = AccentBar,
                Location  = new Point(0, ImageAreaH),
                Size      = new Size(CardW, AccentH),
            };

            int infoY = ImageAreaH + AccentH;
            int infoH = CardH - infoY;
            int nameH  = (int)(infoH * 0.55);
            int priceH = infoH - nameH;

            panelInfo = new Panel
            {
                BackColor = CardBackground,
                Location  = new Point(0, infoY),
                Size      = new Size(CardW, infoH),
                Padding   = new Padding(10, 6, 10, 6),
            };
            panelInfo.Click += OnCardClick;

            labelTitle = new Label
            {
                AutoSize  = false,
                Location  = new Point(8, 5),
                Size      = new Size(CardW - 16, nameH - 5),
                Font      = FontTitle,
                ForeColor = NameForeground,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
            };
            labelTitle.Click += OnCardClick;

            labelPrice = new Label
            {
                AutoSize  = false,
                Location  = new Point(8, nameH),
                Size      = new Size(CardW - 16, priceH - 4),
                Font      = FontPrice,
                ForeColor = PriceForeground,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
            };
            labelPrice.Click += OnCardClick;

            panelInfo.SuspendLayout();
            panelInfo.Controls.Add(labelTitle);
            panelInfo.Controls.Add(labelPrice);
            panelInfo.ResumeLayout(false);

            panelCard.SuspendLayout();
            panelCard.Controls.Add(panelImageArea);
            panelCard.Controls.Add(panelAccent);
            panelCard.Controls.Add(panelInfo);
            panelCard.ResumeLayout(false);

            this.SuspendLayout();
            this.Controls.Add(panelCard);
            this.ResumeLayout(false);
            this.Load += ProductImageControl_Load;
        }

        // ── Región redondeada — se aplica una vez en Load ────────────────
        private void ApplyCardRegion()
        {
            if (panelCard == null) return;

            var bounds = new Rectangle(2, 2, panelCard.Width - 5, panelCard.Height - 5);
            using var path = CreateRoundedPath(bounds, CornerRadius);
            var newRegion  = new Region(path);
            var old        = panelCard.Region;
            panelCard.Region = newRegion;
            old?.Dispose();
            _cardRegion = newRegion;
        }

        // ── Paint: CERO allocations GDI — todo es static pre-computado ───
        private void PanelCard_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Sombra: 3 capas con paths y brushes estáticos, sin new() aquí
            g.FillPath(ShadowBrush1, ShadowPath1);
            g.FillPath(ShadowBrush2, ShadowPath2);
            g.FillPath(ShadowBrush3, ShadowPath3);

            g.FillPath(CardBrush, CardPath);
            g.DrawPath(PenNormal, CardPath);
        }

        // ── Clic unificado ────────────────────────────────────────────────
        private void OnCardClick(object? sender, EventArgs e)
        {
            if (_product != null)
                ProductClicked?.Invoke(this, EventArgs.Empty);
        }

        // ── Carga de datos ────────────────────────────────────────────────
        private void ProductImageControl_Load(object sender, EventArgs e)
        {
            if (_product == null) return;

            pictureBoxImage!.AccessibleName = _product.Id.ToString();
            labelTitle!.Text = _product.Name ?? "No Title";
            labelPrice!.Text = _product.Price?.ToString("C") ?? "--";

            var url = _product.Image.ConvertUrlString();
            if (!string.IsNullOrEmpty(url))
                LoadProductImageAsync(url);

            ApplyCardRegion();
        }

        // ── Carga de imagen con caché ─────────────────────────────────────
        // Primera carga: LoadAsync (no bloquea el UI thread).
        // Siguiente carga (misma URL tras filtro de letra): desde cache, instantáneo.
        private void LoadProductImageAsync(string url)
        {
            if (_imageCache.TryGetValue(url, out var cached))
            {
                // PictureBox no gestiona el ciclo de vida de imágenes asignadas
                // directamente via .Image (solo lo hace con las que él mismo carga).
                pictureBoxImage!.Image = cached;
                return;
            }

            pictureBoxImage!.LoadCompleted += PictureBox_LoadCompleted;
            pictureBoxImage.LoadAsync(url);
        }

        private void PictureBox_LoadCompleted(object? sender, AsyncCompletedEventArgs e)
        {
            pictureBoxImage!.LoadCompleted -= PictureBox_LoadCompleted;

            if (e.Error == null && pictureBoxImage.Image is Image img)
            {
                var url = pictureBoxImage.ImageLocation;
                if (url is not null && !_imageCache.ContainsKey(url))
                {
                    // Clonamos: PictureBox es dueño de su copia (la destruirá al
                    // hacer Dispose). El cache guarda una copia independiente.
                    AddToCache(url, new Bitmap(img));
                }
            }
        }

        // ── Dispose ───────────────────────────────────────────────────────
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // NO disponer FontTitle/FontPrice/Brushes/Pens/Paths — son estáticos.
                // Solo disponer la Region que este control creó.
                _cardRegion?.Dispose();
                _cardRegion = null;
            }
            base.Dispose(disposing);
        }

        // ── Cache helpers ─────────────────────────────────────────────────
        private static void AddToCache(string key, Image image)
        {
            // Evict oldest entry when cap is reached
            while (_cacheKeyQueue.Count >= MaxCacheSize)
            {
                var evict = _cacheKeyQueue.Dequeue();
                if (_imageCache.TryGetValue(evict, out var old))
                {
                    old?.Dispose();
                    _imageCache.Remove(evict);
                }
            }
            _cacheKeyQueue.Enqueue(key);
            _imageCache[key] = image;
        }

        // ── Helper: path con esquinas redondeadas ─────────────────────────
        private static GraphicsPath CreateRoundedPath(Rectangle bounds, int radius)
        {
            int d    = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(bounds.X,         bounds.Y,          d, d, 180, 90);
            path.AddArc(bounds.Right - d,  bounds.Y,          d, d, 270, 90);
            path.AddArc(bounds.Right - d,  bounds.Bottom - d, d, d,   0, 90);
            path.AddArc(bounds.X,         bounds.Bottom - d, d, d,  90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
