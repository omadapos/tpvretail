using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using OmadaPOS.Libreria.Extensions;
using OmadaPOS.Libreria.Models;

namespace OmadaPOS.Componentes
{
    public partial class ProductImageControl : UserControl
    {
        private ProductModel? _product;
        public event EventHandler? ProductClicked;
        public ProductModel? Product => _product;

        public ProductImageControl(ProductModel? product)
        {
            _product = product;
            InitializeComponent();
        }

        private PictureBox? pictureBoxImage;
        private Label? labelTitle;
        private Label? labelPrice;
        private TableLayoutPanel? tableLayoutPanel1;
        private Panel? panelContenedor;

        private void InitializeComponent()
        {
            tableLayoutPanel1 = new TableLayoutPanel();
            labelTitle = new Label();
            pictureBoxImage = new PictureBox();
            labelPrice = new Label();
            panelContenedor = new Panel();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxImage).BeginInit();
            panelContenedor.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.BackColor = Color.White;
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(labelTitle, 0, 0);
            tableLayoutPanel1.Controls.Add(pictureBoxImage, 0, 1);
            tableLayoutPanel1.Controls.Add(labelPrice, 0, 2);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Margin = new Padding(4);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableLayoutPanel1.Size = new Size(160, 180);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // labelTitle
            // 
            labelTitle.BackColor = Color.LightGray;
            labelTitle.Dock = DockStyle.Fill;
            labelTitle.Font = new Font("Roboto Mono", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelTitle.ForeColor = Color.Black;
            labelTitle.Location = new Point(0, 0);
            labelTitle.Margin = new Padding(0);
            labelTitle.Name = "labelTitle";
            labelTitle.Padding = new Padding(6, 4, 6, 4);
            labelTitle.Size = new Size(160, 36);
            labelTitle.TabIndex = 0;
            labelTitle.Text = "Título del producto";
            labelTitle.TextAlign = ContentAlignment.MiddleCenter;
            labelTitle.Click += labelTitle_Click;
            // 
            // pictureBoxImage
            // 
            pictureBoxImage.BackColor = Color.White;
            pictureBoxImage.Dock = DockStyle.Fill;
            pictureBoxImage.Location = new Point(6, 42);
            pictureBoxImage.Margin = new Padding(6, 6, 6, 2);
            pictureBoxImage.Name = "pictureBoxImage";
            pictureBoxImage.Size = new Size(148, 96);
            pictureBoxImage.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxImage.TabIndex = 1;
            pictureBoxImage.TabStop = false;
            pictureBoxImage.Click += pictureBoxImage_Click;
            // 
            // labelPrice
            // 
            labelPrice.BackColor = Color.FromArgb(44, 62, 80);
            labelPrice.Dock = DockStyle.Fill;
            labelPrice.Font = new Font("Roboto Mono SemiBold", 14.25F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            labelPrice.ForeColor = Color.White;
            labelPrice.Location = new Point(0, 140);
            labelPrice.Margin = new Padding(0);
            labelPrice.Name = "labelPrice";
            labelPrice.Padding = new Padding(4, 2, 4, 2);
            labelPrice.Size = new Size(160, 40);
            labelPrice.TabIndex = 2;
            labelPrice.Text = "$0.00";
            labelPrice.TextAlign = ContentAlignment.MiddleCenter;
            labelPrice.Click += labelPrice_Click;
            // 
            // panelContenedor
            // 
            panelContenedor.BackColor = Color.White;
            panelContenedor.Controls.Add(tableLayoutPanel1);
            panelContenedor.Location = new Point(4, 4);
            panelContenedor.Name = "panelContenedor";
            panelContenedor.Size = new Size(160, 180);
            panelContenedor.TabIndex = 0;
            panelContenedor.Paint += PanelContenedor_Paint;
            // 
            // ProductImageControl
            // 
            BackColor = Color.White;
            Controls.Add(panelContenedor);
            Name = "ProductImageControl";
            Size = new Size(168, 188);
            Load += ProductImageControl_Load;
            tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBoxImage).EndInit();
            panelContenedor.ResumeLayout(false);
            ResumeLayout(false);
        }

        // Método para dibujar bordes redondeados en el panel contenedor
        private void PanelContenedor_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = (Panel)sender;
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int borderRadius = 12;
            Rectangle bounds = new Rectangle(0, 0, panel.Width - 1, panel.Height - 1);

            // Crear path con esquinas redondeadas
            using (GraphicsPath path = CreateRoundedRectanglePath(bounds, borderRadius))
            {
                // Rellenar el fondo
                using (SolidBrush backgroundBrush = new SolidBrush(panel.BackColor))
                {
                    g.FillPath(backgroundBrush, path);
                }

                // Dibujar el borde
                using (Pen borderPen = new Pen(Color.FromArgb(200, 200, 200), 1))
                {
                    g.DrawPath(borderPen, path);
                }

                // Establecer la región para que los controles hijos respeten las esquinas redondeadas
                panel.Region = new Region(path.Clone() as GraphicsPath);
            }
        }

        // Método helper para crear un path con esquinas redondeadas
        private GraphicsPath CreateRoundedRectanglePath(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;

            // Esquina superior izquierda
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);

            // Esquina superior derecha
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);

            // Esquina inferior derecha
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);

            // Esquina inferior izquierda
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);

            path.CloseFigure();
            return path;
        }

        // Método para estados del producto (disponible/agotado)
        public void SetProductState(bool isAvailable)
        {
            if (isAvailable)
            {
                labelTitle.BackColor = Color.FromArgb(44, 62, 80);   // Azul normal
                labelPrice.BackColor = Color.FromArgb(39, 174, 96);  // Verde normal
                pictureBoxImage.BackColor = Color.FromArgb(250, 251, 252);
                this.Enabled = true;
            }
            else
            {
                labelTitle.BackColor = Color.FromArgb(149, 165, 166); // Gris deshabilitado
                labelPrice.BackColor = Color.FromArgb(149, 165, 166);
                pictureBoxImage.BackColor = Color.FromArgb(240, 240, 240);
                this.Enabled = false;
            }
        }

        // Método para agregar efectos hover (opcional)
        private void AddHoverEffects()
        {
            // Evento MouseEnter para efecto hover
            panelContenedor.MouseEnter += (s, e) =>
            {
                panelContenedor.BackColor = Color.FromArgb(248, 249, 250);  // Hover más claro
                panelContenedor.Invalidate(); // Redibujar para actualizar el fondo
            };

            // Evento MouseLeave para volver al estado normal
            panelContenedor.MouseLeave += (s, e) =>
            {
                panelContenedor.BackColor = Color.White;  // Color original
                panelContenedor.Invalidate(); // Redibujar para actualizar el fondo
            };
        }

        private void pictureBoxImage_Click(object sender, EventArgs e)
        {
            if (_product != null)
            {
                ProductClicked?.Invoke(this, EventArgs.Empty);
            }
        }

        private void ProductImageControl_Load(object sender, EventArgs e)
        {
            if (_product == null) return;

            pictureBoxImage.AccessibleName = _product.Id.ToString();
            labelTitle.Text = _product.Name ?? "No Title";
            labelPrice.Text = _product.Price?.ToString("C") ?? "No Price";
            pictureBoxImage.ImageLocation = _product.Image.ConvertUrlString();

            // Opcional: Activar efectos hover
            // AddHoverEffects();
        }

        private void labelPrice_Click(object sender, EventArgs e)
        {
            // Implementar lógica si es necesario
        }

        private void labelTitle_Click(object sender, EventArgs e)
        {

        }
    }
}