using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using OmadaPOS.Libreria.Extensions;
using OmadaPOS.Libreria.Models;

namespace OmadaPOS.Componentes
{
    public partial class ProductImageControl : UserControl
    {
        // ── API pública — sin cambios ─────────────────────────────────────
        private ProductModel? _product;
        public event EventHandler? ProductClicked;
        public ProductModel? Product => _product;

        // ── Colores de la tarjeta ─────────────────────────────────────────
        private static readonly Color CardBackground   = Color.White;
        private static readonly Color ImageBackground  = Color.White;
        private static readonly Color BorderNormal     = Color.FromArgb(220, 224, 230);
        private static readonly Color BorderHover      = AppColors.NavyBase;
        private static readonly Color NameForeground   = AppColors.TextPrimary;
        private static readonly Color PriceForeground  = AppColors.AccentGreen;
        private static readonly Color PriceBackground  = Color.FromArgb(240, 252, 245);
        private static readonly Color AccentBar        = AppColors.AccentGreen;

        private bool _isHovered;

        // Región redondeada de la tarjeta — se crea una sola vez en Load y se
        // reutiliza. Trackeada explícitamente para poder disponer el objeto GDI
        // anterior antes de asignar uno nuevo y en Dispose().
        private Region? _cardRegion;

        // ── Controles internos ────────────────────────────────────────────
        private PictureBox?      pictureBoxImage;
        private Label?           labelTitle;
        private Label?           labelPrice;
        private Panel?           panelCard;
        private Panel?           panelImageArea;
        private Panel?           panelInfo;
        private Panel?           panelAccent;

        // ── Constantes de tamaño ──────────────────────────────────────────
        private const int CardW        = 185;
        private const int CardH        = 215;
        private const int Margin       = 6;
        private const int CornerRadius = 14;
        private const int ImageAreaH   = 120;
        private const int AccentH      = 4;

        public ProductImageControl(ProductModel? product)
        {
            _product = product;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // ── Outer UserControl ────────────────────────────────────────
            this.BackColor = Color.Transparent;
            this.Size      = new Size(CardW + Margin * 2, CardH + Margin * 2);
            this.Cursor    = Cursors.Hand;

            // ── Card panel (con sombra + bordes redondeados via Paint) ────
            panelCard = new Panel
            {
                BackColor = CardBackground,
                Location  = new Point(Margin, Margin),
                Size      = new Size(CardW, CardH),
                Cursor    = Cursors.Hand,
            };
            panelCard.Paint      += PanelCard_Paint;
            panelCard.MouseEnter += OnMouseEnterCard;
            panelCard.MouseLeave += OnMouseLeaveCard;
            panelCard.Click      += OnCardClick;

            // ── Área de imagen ───────────────────────────────────────────
            panelImageArea = new Panel
            {
                BackColor = ImageBackground,
                Location  = new Point(0, 0),
                Size      = new Size(CardW, ImageAreaH),
                Cursor    = Cursors.Hand,
            };
            panelImageArea.MouseEnter += OnMouseEnterCard;
            panelImageArea.MouseLeave += OnMouseLeaveCard;
            panelImageArea.Click      += OnCardClick;

            pictureBoxImage = new PictureBox
            {
                Dock     = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Cursor   = Cursors.Hand,
                Padding  = new Padding(8),
            };
            pictureBoxImage.MouseEnter += OnMouseEnterCard;
            pictureBoxImage.MouseLeave += OnMouseLeaveCard;
            pictureBoxImage.Click      += OnCardClick;
            panelImageArea.Controls.Add(pictureBoxImage);

            // ── Barra de acento verde (separador imagen / info) ──────────
            panelAccent = new Panel
            {
                BackColor = AccentBar,
                Location  = new Point(0, ImageAreaH),
                Size      = new Size(CardW, AccentH),
            };

            // ── Área de texto (nombre + precio) ──────────────────────────
            int infoY = ImageAreaH + AccentH;
            int infoH = CardH - infoY;

            panelInfo = new Panel
            {
                BackColor = CardBackground,
                Location  = new Point(0, infoY),
                Size      = new Size(CardW, infoH),
                Cursor    = Cursors.Hand,
                Padding   = new Padding(10, 6, 10, 6),
            };
            panelInfo.MouseEnter += OnMouseEnterCard;
            panelInfo.MouseLeave += OnMouseLeaveCard;
            panelInfo.Click      += OnCardClick;

            int nameH  = (int)(infoH * 0.55);
            int priceH = infoH - nameH;

            labelTitle = new Label
            {
                AutoSize  = false,
                Dock      = DockStyle.None,
                Location  = new Point(8, 5),
                Size      = new Size(CardW - 16, nameH - 5),
                Font      = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = NameForeground,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor    = Cursors.Hand,
            };
            labelTitle.MouseEnter += OnMouseEnterCard;
            labelTitle.MouseLeave += OnMouseLeaveCard;
            labelTitle.Click      += OnCardClick;

            labelPrice = new Label
            {
                AutoSize  = false,
                Dock      = DockStyle.None,
                Location  = new Point(8, nameH),
                Size      = new Size(CardW - 16, priceH - 4),
                Font      = new Font("Montserrat", 13F, FontStyle.Bold),
                ForeColor = PriceForeground,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor    = Cursors.Hand,
            };
            labelPrice.MouseEnter += OnMouseEnterCard;
            labelPrice.MouseLeave += OnMouseLeaveCard;
            labelPrice.Click      += OnCardClick;

            panelInfo.Controls.Add(labelTitle);
            panelInfo.Controls.Add(labelPrice);

            panelCard.Controls.Add(panelImageArea);
            panelCard.Controls.Add(panelAccent);
            panelCard.Controls.Add(panelInfo);

            this.Controls.Add(panelCard);
            this.Load += ProductImageControl_Load;
        }

        // ── Región redondeada — se crea una vez, no en cada Paint ────────
        private void ApplyCardRegion()
        {
            if (panelCard == null) return;

            var bounds = new Rectangle(2, 2, panelCard.Width - 5, panelCard.Height - 5);
            using var path     = CreateRoundedPath(bounds, CornerRadius);
            var       newRegion = new Region(path);

            // Disponer la región anterior ANTES de asignar la nueva.
            // Control.Region no hace Dispose automático del objeto anterior.
            var oldRegion = panelCard.Region;
            panelCard.Region = newRegion;
            oldRegion?.Dispose();

            _cardRegion = newRegion;
        }

        // ── Dibujar tarjeta: bordes redondeados + sombra suave ───────────
        private void PanelCard_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var bounds = new Rectangle(2, 2, panelCard!.Width - 5, panelCard.Height - 5);

            // Sombra (capas semi-transparentes desplazadas)
            for (int i = 3; i > 0; i--)
            {
                var shadowRect = new Rectangle(bounds.X + i, bounds.Y + i,
                                               bounds.Width, bounds.Height);
                using var shadowBrush = new SolidBrush(Color.FromArgb(18 - i * 4, 0, 0, 0));
                using var shadowPath  = CreateRoundedPath(shadowRect, CornerRadius);
                g.FillPath(shadowBrush, shadowPath);
            }

            // Fondo + borde — solo dibujo visual, sin tocar Region aquí.
            using var cardPath  = CreateRoundedPath(bounds, CornerRadius);
            using var cardBrush = new SolidBrush(CardBackground);
            g.FillPath(cardBrush, cardPath);

            var borderColor = _isHovered ? BorderHover : BorderNormal;
            var borderWidth = _isHovered ? 2f : 1f;
            using var borderPen = new Pen(borderColor, borderWidth);
            g.DrawPath(borderPen, cardPath);

            // La Region ya fue aplicada en Load (ApplyCardRegion).
            // No se asigna Region aquí para evitar el GDI leak por pintura repetida.
        }

        // ── Hover ────────────────────────────────────────────────────────
        private void OnMouseEnterCard(object? sender, EventArgs e)
        {
            _isHovered = true;
            panelCard!.Invalidate();
            panelImageArea!.BackColor = Color.FromArgb(240, 242, 245);
        }

        private void OnMouseLeaveCard(object? sender, EventArgs e)
        {
            // Solo salir del hover si el cursor realmente dejó la tarjeta
            var pos = panelCard!.PointToClient(Cursor.Position);
            if (!panelCard.ClientRectangle.Contains(pos))
            {
                _isHovered = false;
                panelCard.Invalidate();
                panelImageArea!.BackColor = ImageBackground;
            }
        }

        // ── Clic unificado en toda la tarjeta ────────────────────────────
        private void OnCardClick(object? sender, EventArgs e)
        {
            if (_product != null)
                ProductClicked?.Invoke(this, EventArgs.Empty);
        }

        // ── Carga de datos del producto ───────────────────────────────────
        private void ProductImageControl_Load(object sender, EventArgs e)
        {
            if (_product == null) return;

            pictureBoxImage!.AccessibleName = _product.Id.ToString();
            labelTitle!.Text  = _product.Name ?? "No Title";
            labelPrice!.Text  = _product.Price?.ToString("C") ?? "--";

            var url = _product.Image.ConvertUrlString();
            if (!string.IsNullOrEmpty(url))
                pictureBoxImage.ImageLocation = url;

            // Aplicar región redondeada una sola vez al cargar el control.
            // El tamaño de la tarjeta es fijo (CardW × CardH), por lo que
            // no es necesario recalcularla en cada repaint.
            ApplyCardRegion();
        }

        // ── Liberación de recursos GDI ────────────────────────────────────
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Disponer la Region GDI explícitamente — WinForms NO la libera
                // automáticamente al hacer Dispose() del control.
                _cardRegion?.Dispose();
                _cardRegion = null;
            }
            base.Dispose(disposing);
        }

        // ── Estado disponible / agotado — API pública sin cambios ────────
        public void SetProductState(bool isAvailable)
        {
            if (isAvailable)
            {
                panelImageArea!.BackColor = ImageBackground;
                labelTitle!.ForeColor     = NameForeground;
                labelPrice!.ForeColor     = PriceForeground;
                this.Enabled = true;
            }
            else
            {
                panelImageArea!.BackColor = Color.FromArgb(240, 240, 240);
                labelTitle!.ForeColor     = Color.FromArgb(160, 160, 160);
                labelPrice!.ForeColor     = Color.FromArgb(160, 160, 160);
                this.Enabled = false;
            }
        }

        // ── Helper: path con esquinas redondeadas ─────────────────────────
        private static GraphicsPath CreateRoundedPath(Rectangle bounds, int radius)
        {
            int d    = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(bounds.X,                   bounds.Y,                    d, d, 180, 90);
            path.AddArc(bounds.Right - d,            bounds.Y,                    d, d, 270, 90);
            path.AddArc(bounds.Right - d,            bounds.Bottom - d,           d, d,   0, 90);
            path.AddArc(bounds.X,                   bounds.Bottom - d,           d, d,  90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
