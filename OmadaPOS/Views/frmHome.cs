using Newtonsoft.Json;
using OmadaPOS.Componentes;
using OmadaPOS.Domain;
using OmadaPOS.Impresora;
using OmadaPOS.Libreria.Extensions;
using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Services;
using OmadaPOS.Libreria.Utils;
using OmadaPOS.Models;
using OmadaPOS.Services;

namespace OmadaPOS.Views
{
    public partial class frmHome : Form
    {
        private readonly ZebraScannerService? _zebraScannerService;

        // Flyout panel del usuario (Settings / Logout / Daily Close)
        private Panel? _panelUserMenu;
        private bool   _userMenuVisible = false;

        int orderId = 0;

        decimal totalGlobal = 0;
        int inputValue = 0;
        decimal dueValue = 0;
        decimal changeValue = 0;
        bool bDesc = false;
        double weight = 0.0;

        private TabPage? _selectedTab;
        public List<MenuCategoryModel> MenuCategories { get; } = new();
        public List<CategoryModel> Categories { get; } = new();
        public List<ProductModel> Products { get; } = new();

        private readonly IShoppingCart _shoppingCart;
        private readonly ICategoryService _categoryService;
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly IAdminSettingService _adminSettingService;
        private readonly IPaymentService _paymentService;
        private readonly IPaymentSplitService _paymentSplitService;
        private readonly IOrderApplicationService _orderApplicationService;
        private readonly IProductApplicationService _productApplicationService;
        private readonly IHoldService _holdService;

        public frmHome(ZebraScannerService zebraScannerService)
        {
            InitializeComponent();

            _categoryService = Program.GetService<ICategoryService>();
            _orderService = Program.GetService<IOrderService>();
            _userService = Program.GetService<IUserService>();
            _adminSettingService = Program.GetService<IAdminSettingService>();
            _paymentService = Program.GetService<IPaymentService>();
            _paymentSplitService = Program.GetService<IPaymentSplitService>();
            _orderApplicationService = Program.GetService<IOrderApplicationService>();
            _productApplicationService = Program.GetService<IProductApplicationService>();
            _holdService = Program.GetService<IHoldService>();

            _shoppingCart = Program.GetService<IShoppingCart>();
            _shoppingCart.CartChanged += ShoppingCart_CartChanged;

            ConfigureUI();
            ConfigureListView();

            _zebraScannerService = zebraScannerService;

            _zebraScannerService.OnBarcodeDataReceived += _zebraScannerService_OnBarcodeDataReceived;
            _zebraScannerService.OnWeightUpdated += _zebraScannerService_OnWeightUpdated;

            lblScalStatusDesc.Text = zebraScannerService.Initialize();
        }

        private void ConfigureUI()
        {
            // ═══════════════════════════════════════════════════════════
            // GRUPO 1 — Barra superior (header)
            // ═══════════════════════════════════════════════════════════
            ElegantButtonStyles.Style(buttonInvoice,    ElegantButtonStyles.Keypad,    fontSize: 16f);
            ElegantButtonStyles.Style(DualScreenButton, ElegantButtonStyles.Keypad,    fontSize: 16f);
            ElegantButtonStyles.Style(ButtonSettings,   ElegantButtonStyles.Keypad,    fontSize: 16f);
            ElegantButtonStyles.Style(labelCashier,     ElegantButtonStyles.Keypad,    fontSize: 16f);
            ElegantButtonStyles.Style(buttonClose,      ElegantButtonStyles.AlertRed,  fontSize: 16f);
            ElegantButtonStyles.Style(buttonCheckPrice, ElegantButtonStyles.Keypad,    fontSize: 16f);

            buttonInvoice.Text    = "🖨  PRINT";
            DualScreenButton.Text = "🖥  DUAL";
            ButtonSettings.Text   = "⚙  CONFIG";
            buttonClose.Text      = "⏻  EXIT";
            buttonCheckPrice.Text = "💲  PRICE";

            // ═══════════════════════════════════════════════════════════
            // GRUPO 2 — Acciones del carrito (texto + icono ya se asigna
            //           en EstilizarColumnaCarrito via AplicarEstiloVisual)
            // ═══════════════════════════════════════════════════════════
            ElegantButtonStyles.Style(buttonCancelOrder,    ElegantButtonStyles.AlertRed,    fontSize: 16f);
            ElegantButtonStyles.Style(buttonChangeQuantity, ElegantButtonStyles.WarningOrange, fontSize: 16f);
            ElegantButtonStyles.Style(buttonDeleteItem,     ElegantButtonStyles.AlertRed,    fontSize: 16f);
            ElegantButtonStyles.Style(buttonHold,           ElegantButtonStyles.WarningOrange, fontSize: 16f);

            // ═══════════════════════════════════════════════════════════
            // GRUPO 3 — Herramientas (Quick Sale / Lookup)
            // ═══════════════════════════════════════════════════════════
            // Quick Sale → verde: es una acción de venta directa (generar ingreso)
            ElegantButtonStyles.Style(buttonQsale,  ElegantButtonStyles.CashGreen, fontSize: 16f);
            // Lookup UPC → navy: es una consulta/búsqueda informativa
            ElegantButtonStyles.Style(buttonLookup, ElegantButtonStyles.Keypad,    fontSize: 16f);

            buttonQsale.Text  = "⚡  QUICK SALE";
            buttonLookup.Text = "🔍  LOOKUP UPC";

            // ── Fila 1: métodos de pago principales ──────────────────────
            ElegantButtonStyles.Style(buttonPayCash,       ElegantButtonStyles.CashGreen,       fontSize: 22f);
            ElegantButtonStyles.Style(buttonPayCreditCard, ElegantButtonStyles.CreditBlue,      fontSize: 18f);
            ElegantButtonStyles.Style(buttonPayDebitCard,  ElegantButtonStyles.DebitGray,       fontSize: 18f);
            ElegantButtonStyles.Style(buttonSplit,         ElegantButtonStyles.SplitBlueLight,  fontSize: 18f);

            // ── Fila 2: métodos secundarios ───────────────────────────────
            ElegantButtonStyles.Style(buttonEBTBalance,    ElegantButtonStyles.EBTBalanceOrange, fontSize: 16f);
            ElegantButtonStyles.Style(buttonEBTFood,       ElegantButtonStyles.EBTOrange,        fontSize: 16f);
            ElegantButtonStyles.Style(buttonOpenDrawer,    ElegantButtonStyles.HeaderNavy,       fontSize: 16f);
            ElegantButtonStyles.Style(buttonGiftCard,      ElegantButtonStyles.GiftPurple,       fontSize: 16f);

            // Textos con iconos — uniformes y reconocibles de un vistazo
            buttonPayCash.Text       = "💵  CASH";
            buttonPayCreditCard.Text = "💳  CREDIT";
            buttonPayDebitCard.Text  = "💳  DEBIT";
            buttonSplit.Text         = "⇌  SPLIT PAY";
            buttonEBTBalance.Text    = "⚖  EBT BAL";
            buttonEBTFood.Text       = "🌿  EBT FOOD";
            buttonOpenDrawer.Text    = "🗄  DRAWER";
            buttonGiftCard.Text      = "🎁  GIFT CARD";

            ElegantButtonStyles.StyleSplitContainer(splitContainerScale);

            EstilizarTabControl();
        }

        // ── Tab Control profesional con owner-drawing ─────────────────────
        private void EstilizarTabControl()
        {
            var tab = tabControlMenuCategories;

            // Fuente legible para las pestañas
            tab.Font     = new Font("Segoe UI", 13F, FontStyle.Bold);
            tab.DrawMode = TabDrawMode.OwnerDrawFixed;
            tab.SizeMode = TabSizeMode.FillToRight;
            tab.ItemSize = new Size(0, 54);
            tab.Padding  = new Point(18, 14);

            tab.DrawItem += TabControl_DrawItem;

            var productsBg = Color.FromArgb(248, 249, 252);
            tableLayoutPanelCategoria.BackColor = productsBg;
            tableLayoutPanelCategoria.Padding   = new Padding(0);

            tab.BackColor = productsBg;
        }

        // Fonts cacheados para el TabControl — se crean una sola vez
        private static readonly Font _tabFontSelected = new Font("Segoe UI", 12F, FontStyle.Bold);
        private static readonly Font _tabFontNormal   = new Font("Segoe UI", 12F, FontStyle.Regular);

        private void TabControl_DrawItem(object? sender, DrawItemEventArgs e)
        {
            var tab = tabControlMenuCategories;
            var g   = e.Graphics;
            g.SmoothingMode     = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            bool isSelected = e.Index == tab.SelectedIndex;
            var  bounds     = e.Bounds;
            var  page       = tab.TabPages[e.Index];

            // ── Fondo — tab activo navy, inactivo gris suave coincide con el área de productos
            Color back = isSelected ? AppColors.NavyBase : Color.FromArgb(225, 228, 235);
            using (var bgBrush = new SolidBrush(back))
                g.FillRectangle(bgBrush, bounds);

            // ── Acento o separador ────────────────────────────────────────
            if (isSelected)
            {
                using var accentBrush = new SolidBrush(AppColors.AccentGreen);
                g.FillRectangle(accentBrush, bounds.X, bounds.Y, bounds.Width, 4);
            }
            else
            {
                using var sep = new Pen(Color.FromArgb(200, 210, 220), 1);
                g.DrawLine(sep, bounds.Right - 1, bounds.Y + 6, bounds.Right - 1, bounds.Bottom - 6);
            }

            // ── Texto — reutiliza fonts cacheados ─────────────────────────
            Color fore     = isSelected ? Color.White : AppColors.SlateBlue;
            var   textFont = isSelected ? _tabFontSelected : _tabFontNormal;
            var   textRect = new Rectangle(bounds.X, bounds.Y + 4, bounds.Width, bounds.Height - 4);

            using var sf = new StringFormat
            {
                Alignment     = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming      = StringTrimming.EllipsisCharacter
            };
            using var foreBrush = new SolidBrush(fore);
            g.DrawString(page.Text, textFont, foreBrush, textRect, sf);
        }

        private void ConfigureListView()
        {
            listViewCart.View = View.Details;
            listViewCart.FullRowSelect = true;
            listViewCart.GridLines = true;
            listViewCart.MultiSelect = false;

            listViewCart.Columns.Add("#", 80);
            listViewCart.Columns.Add("Product", 200);
            listViewCart.Columns.Add("Quantity", 80);
            listViewCart.Columns.Add("Price", 100);
            listViewCart.Columns.Add("Subtotal", 100);

            listViewCart.BackColor = Color.White;
            listViewCart.Font      = new Font("Segoe UI", 13F, FontStyle.Regular);

            foreach (ColumnHeader column in listViewCart.Columns)
            {
                column.TextAlign = HorizontalAlignment.Center;
            }
            listViewCart.OwnerDraw = true;
            listViewCart.DrawColumnHeader += (s, e) =>
            {
                var g = e.Graphics;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                using var bgBrush   = new SolidBrush(AppColors.NavyBase);
                g.FillRectangle(bgBrush, e.Bounds);

                using var sep = new Pen(Color.FromArgb(60, 255, 255, 255), 1);
                g.DrawLine(sep, e.Bounds.Right - 1, e.Bounds.Top + 4,
                                e.Bounds.Right - 1, e.Bounds.Bottom - 4);

                using var headerFont = new Font("Segoe UI", 12F, FontStyle.Bold);
                using var textBrush  = new SolidBrush(AppColors.TextWhite);
                using var sf         = new StringFormat
                {
                    Alignment     = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    Trimming      = StringTrimming.EllipsisCharacter
                };
                var textRect = new Rectangle(e.Bounds.X + 4, e.Bounds.Y,
                                             e.Bounds.Width - 8, e.Bounds.Height);
                g.DrawString(e.Header.Text, headerFont, textBrush, textRect, sf);
            };
            listViewCart.DrawItem    += (s, e) => e.DrawDefault = true;
            listViewCart.DrawSubItem += (s, e) => e.DrawDefault = true;

            AdjustListViewColumns();
            listViewCart.Resize += (s, e) => AdjustListViewColumns();
        }

        private void AdjustListViewColumns()
        {
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

            decimal subTotal = 0.0m;
            decimal taxTotal = 0.0m;
            decimal total = 0.0m;

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

                subTotal += item.Subtotal;
                taxTotal += item.TaxAmount;
                total += item.Total;
            }

            labelSubTotal.Text = $"{subTotal.ToString("n2")}";
            labelTotalTax.Text = $"{taxTotal.ToString("n2")}";

            totalGlobal = total;

            UpdateTotals();
        }

        private void UpdateTotals()
        {
            labelTotalValue.Text = totalGlobal.ToString("N2");
        }

        // ═══════════════════════════════════════════════════════════════
        // TEMA VISUAL — Triband Layout
        // Izq: blanco (carrito)  |  Centro: gris suave (productos)
        // Der: navy oscuro (pago) — aspecto de terminal de pago
        // ═══════════════════════════════════════════════════════════════
        private void AplicarEstiloVisual()
        {
            EstilizarBarraSuperior();
            EstilizarColumnaCarrito();
            EstilizarColumnaProductos();
            EstilizarColumnaPago();
            EstilizarSeccionBalanza();
            EstilizarSeparadoresColumnas();

            // Forzar repintado de todos los controles con los nuevos colores
            this.Invalidate(true);
            this.Refresh();
        }

        // ── Barra superior (header del sistema) ──────────────────────────
        private void EstilizarBarraSuperior()
        {
            MaintableLayout.BackColor = AppColors.NavyDark;
            MaintableLayout.Padding   = new Padding(0);
            MaintableLayout.Margin    = new Padding(0);

            // Padding vertical en el header — espacio respirable sin perder altura
            tableLayoutPanel1.BackColor = AppColors.NavyDark;
            tableLayoutPanel1.Margin    = new Padding(0);
            tableLayoutPanel1.Padding   = new Padding(8, 4, 8, 4);

            labelProductName.Font      = new Font("Montserrat", 15F, FontStyle.Bold);
            labelProductName.ForeColor = AppColors.TextWhite;
            labelProductName.BackColor = Color.Transparent;
            labelProductName.Text      = "OMADA POS";

            // CONFIG button visible en header — abre flyout del usuario
            ButtonSettings.Visible = false; // consolidado en el flyout del cajero

            EstilizarScanZone();
            CrearMenuUsuario();
        }

        // ── Scan Zone — contenedor visual profesional para el UPC ────────
        private void EstilizarScanZone()
        {
            var pos = tableLayoutPanel1.GetPositionFromControl(textBoxUPC);

            // ── Panel contenedor ─────────────────────────────────────────
            var scanPanel = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = AppColors.NavyBase,
                Margin    = new Padding(4, 5, 4, 5),
                Cursor    = Cursors.IBeam,
            };

            // Borde redondeado + indicador de scan
            scanPanel.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                var rect = new Rectangle(1, 1, scanPanel.Width - 2, scanPanel.Height - 2);
                using var path   = ElegantButtonStyles.RoundedPath(rect, 8);
                using var border = new Pen(AppColors.AccentGreen, 1.5f);
                g.DrawPath(border, path);
            };

            // ── Ícono de barcode al inicio ───────────────────────────────
            var iconLabel = new Label
            {
                Text      = "⬛",
                Font      = new Font("Segoe UI", 13F),
                ForeColor = AppColors.AccentGreen,
                BackColor = Color.Transparent,
                Dock      = DockStyle.Left,
                Width     = 34,
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor    = Cursors.IBeam,
            };
            iconLabel.Click += (s, e) => textBoxUPC.Focus();

            // ── Botón ✕ Clear integrado al final ─────────────────────────
            var btnClear = new Button
            {
                Text      = "✕",
                Font      = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = AppColors.TextMuted,
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                Dock      = DockStyle.Right,
                Width     = 30,
                Cursor    = Cursors.Hand,
                TabStop   = false,
            };
            btnClear.FlatAppearance.BorderSize         = 0;
            btnClear.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btnClear.Click += (s, e) => { textBoxUPC.Text = ""; textBoxUPC.Focus(); };

            // ── TextBox embebido ─────────────────────────────────────────
            textBoxUPC.Dock            = DockStyle.Fill;
            textBoxUPC.BackColor       = AppColors.NavyBase;
            textBoxUPC.ForeColor       = AppColors.AccentGreen;
            textBoxUPC.Font            = new Font("Consolas", 15F, FontStyle.Bold);
            textBoxUPC.BorderStyle     = BorderStyle.None;
            textBoxUPC.TextAlign       = HorizontalAlignment.Center;
            textBoxUPC.PlaceholderText = "Scan or type UPC...";
            textBoxUPC.Margin          = new Padding(0);

            // ── Ensamblar: orden de controles con Dock ───────────────────
            // (Dock.Left se apila de izquierda a derecha; Dock.Right al revés)
            tableLayoutPanel1.Controls.Remove(textBoxUPC);
            scanPanel.Controls.Add(textBoxUPC);  // Fill — va al centro
            scanPanel.Controls.Add(btnClear);    // Right — va a la derecha
            scanPanel.Controls.Add(iconLabel);   // Left  — va a la izquierda

            tableLayoutPanel1.Controls.Add(scanPanel, pos.Column, pos.Row);
            scanPanel.Click += (s, e) => textBoxUPC.Focus();
        }

        // ── Menú flotante del usuario (flyout dropdown) ───────────────────
        private void CrearMenuUsuario()
        {
            // El flyout se posiciona en tiempo de ejecución al abrir
            _panelUserMenu = new Panel
            {
                Size      = new Size(260, 190),
                BackColor = AppColors.NavyBase,
                Visible   = false,
                // BringToFront() + Parent asignado al form para flotar sobre todo
            };
            _panelUserMenu.Paint += UserMenuPanel_Paint;

            // ── Encabezado — nombre del cajero ─────────────────────────
            var lblNombre = new Label
            {
                Name      = "lblFlyoutName",
                Text      = $"👤  {SessionManager.Name}",
                Font      = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = AppColors.TextWhite,
                BackColor = Color.Transparent,
                Dock      = DockStyle.Top,
                Height    = 48,
                TextAlign = ContentAlignment.MiddleCenter,
                Padding   = new Padding(0, 6, 0, 0),
            };

            // ── Separador ──────────────────────────────────────────────
            var sep = new Panel
            {
                Height    = 1,
                Dock      = DockStyle.Top,
                BackColor = Color.FromArgb(60, 255, 255, 255),
                Margin    = new Padding(0),
            };

            // ── Botones del menú ───────────────────────────────────────
            var btnSettings = CrearBotonFlyout("⚙  Settings", AppColors.NavyBase);
            btnSettings.Click += (s, e) => { OcultarMenuUsuario(); buttonSetting_Click(s, e); };

            var btnDailyClose = CrearBotonFlyout("📋  Daily Close", AppColors.NavyBase);
            btnDailyClose.Click += (s, e) => { OcultarMenuUsuario(); labelCashier_ClickInternal(); };

            var btnLogout = CrearBotonFlyout("⏻  Logout", Color.FromArgb(180, 30, 30));
            btnLogout.Click += async (s, e) => { OcultarMenuUsuario(); await EjecutarLogout(); };

            // ── Ensamblar: orden invertido por Dock.Top ────────────────
            // Los controles con Dock.Top se apilan de abajo hacia arriba en orden de Add
            _panelUserMenu.Controls.Add(btnLogout);
            _panelUserMenu.Controls.Add(btnDailyClose);
            _panelUserMenu.Controls.Add(btnSettings);
            _panelUserMenu.Controls.Add(sep);
            _panelUserMenu.Controls.Add(lblNombre);

            // Agregar al form directamente — flota sobre todos los demás controles
            this.Controls.Add(_panelUserMenu);
            _panelUserMenu.BringToFront();

            // Cerrar el flyout si el usuario hace click en cualquier otro lugar del form
            this.MouseClick += (s, e) => OcultarMenuUsuario();
            MaintableLayout.MouseClick += (s, e) => OcultarMenuUsuario();
        }

        private Button CrearBotonFlyout(string texto, Color bgColor)
        {
            var btn = new Button
            {
                Text      = texto,
                Font      = new Font("Segoe UI", 12F, FontStyle.Regular),
                ForeColor = AppColors.TextWhite,
                BackColor = bgColor,
                FlatStyle = FlatStyle.Flat,
                Dock      = DockStyle.Top,
                Height    = 44,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(18, 0, 0, 0),
                Cursor    = Cursors.Hand,
                TabStop   = false,
            };
            btn.FlatAppearance.BorderSize         = 0;
            btn.FlatAppearance.MouseOverBackColor = AppColors.NavyLight;
            return btn;
        }

        private void UserMenuPanel_Paint(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel panel) return;
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            var bounds = new Rectangle(0, 0, panel.Width - 1, panel.Height - 1);
            using var path   = ElegantButtonStyles.RoundedPath(bounds, 10);
            using var bg     = new SolidBrush(AppColors.NavyBase);
            g.FillPath(bg, path);

            // Línea emerald en el tope — igual que el header
            using var accent = new SolidBrush(AppColors.AccentGreen);
            g.FillRectangle(accent, bounds.X + 12, bounds.Y, bounds.Width - 24, 3);

            // Sombra suave alrededor
            using var border = new Pen(Color.FromArgb(80, 255, 255, 255), 1);
            g.DrawPath(border, path);
        }

        private void MostrarMenuUsuario()
        {
            if (_panelUserMenu == null) return;

            // Actualizar el nombre del cajero (puede haber cambiado desde el login)
            var lbl = _panelUserMenu.Controls.OfType<Label>().FirstOrDefault(l => l.Name == "lblFlyoutName");
            if (lbl != null) lbl.Text = $"👤  {SessionManager.Name}";

            // Posicionar debajo del botón del cajero
            var btnPos   = labelCashier.PointToScreen(Point.Empty);
            var formPos  = this.PointToClient(btnPos);
            _panelUserMenu.Location = new Point(
                formPos.X + labelCashier.Width - _panelUserMenu.Width,
                formPos.Y + labelCashier.Height + 2);

            _panelUserMenu.BringToFront();
            _panelUserMenu.Visible  = true;
            _userMenuVisible        = true;
        }

        private void OcultarMenuUsuario()
        {
            if (_panelUserMenu == null) return;
            _panelUserMenu.Visible = false;
            _userMenuVisible       = false;
        }

        private void labelCashier_ClickInternal()
        {
            var form = new frmCierreDiario();
            form.ShowDialog(this);
        }

        private async Task EjecutarLogout()
        {
            await _userService.Logout(new LogDTO
            {
                AdminId = SessionManager.AdminId ?? 0,
                Phone   = SessionManager.Phone
            });
            new frmSignIn().Show();
            this.Close();
        }

        // ── Columna izquierda — Carrito ───────────────────────────────────
        private void EstilizarColumnaCarrito()
        {
            tableLayoutPanelCart.BackColor       = Color.White;
            tableLayoutPanelButtonCart.BackColor = Color.White;
            tableLayoutPanelCart.Padding         = new Padding(6, 6, 6, 4);

            listViewCart.BackColor = Color.White;
            listViewCart.ForeColor = AppColors.TextPrimary;
            listViewCart.Font      = new Font("Segoe UI", 13F, FontStyle.Regular);

            // RoundedPanel usa BackgroundStart/BackgroundEnd — no BackColor
            roundedPanel1.BackgroundStart = AppColors.NavyDark;
            roundedPanel1.BackgroundEnd   = AppColors.NavyBase;
            roundedPanel1.BorderColor     = AppColors.AccentGreen;
            roundedPanel1.CornerRadius    = 12;
            roundedPanel1.Padding         = new Padding(20, 8, 20, 8);
            roundedPanel1.Margin          = new Padding(6, 4, 6, 4);

            label1.Font      = new Font("Segoe UI", 12F, FontStyle.Regular);
            label1.ForeColor = Color.FromArgb(160, 200, 220);
            label1.BackColor = Color.Transparent;

            label2.Font      = new Font("Segoe UI", 12F, FontStyle.Regular);
            label2.ForeColor = Color.FromArgb(160, 200, 220);
            label2.BackColor = Color.Transparent;

            label3.Font      = new Font("Segoe UI", 14F, FontStyle.Bold);
            label3.ForeColor = Color.White;
            label3.BackColor = Color.Transparent;

            labelSubTotal.Font      = new Font("Consolas", 13F, FontStyle.Regular);
            labelSubTotal.ForeColor = Color.FromArgb(200, 230, 240);
            labelSubTotal.BackColor = Color.Transparent;

            labelTotalTax.Font      = new Font("Consolas", 13F, FontStyle.Regular);
            labelTotalTax.ForeColor = Color.FromArgb(160, 200, 220);
            labelTotalTax.BackColor = Color.Transparent;

            labelTotalValue.Font      = new Font("Montserrat", 24F, FontStyle.Bold);
            labelTotalValue.ForeColor = AppColors.AccentGreen;
            labelTotalValue.BackColor = Color.Transparent;

            buttonCancelOrder.Text    = "✕  CANCEL";
            buttonChangeQuantity.Text = "✎  QTY";
            buttonDeleteItem.Text     = "⌫  REMOVE";
            buttonHold.Text           = "⏸  HOLD";
        }

        // ── Columna central — Productos ───────────────────────────────────
        private void EstilizarColumnaProductos()
        {
            // Gris muy suave — casi blanco, diferencia la zona sin imponer color fuerte
            var productsBg = Color.FromArgb(248, 249, 252);
            tableLayoutPanelCategoria.BackColor = productsBg;

            foreach (TabPage tab in tabControlMenuCategories.TabPages)
            {
                tab.BackColor = productsBg;
                if (tab.Controls.Count > 0 && tab.Controls[0] is FlowLayoutPanel flp)
                    flp.BackColor = productsBg;
            }
        }

        // ── Columna derecha — Pagos — Terminal oscuro ────────────────────
        private void EstilizarColumnaPago()
        {
            tableLayoutPanelPayment.BackColor = AppColors.NavyDark;
            tableLayoutPanelPayment.Padding   = new Padding(6, 6, 6, 4);

            tableLayoutPanel2.BackColor   = AppColors.NavyDark;
          

            // KeyPaymentControl ya aplica NavyDark en su ApplyStyles()
            // pero forzamos para asegurar
            keyPaymentControl1.BackColor  = AppColors.NavyDark;

            // RoundedPanel2 — usa BackgroundStart/End, no BackColor
            roundedPanel2.BackgroundStart = AppColors.NavyBase;
            roundedPanel2.BackgroundEnd   = Color.FromArgb(20, 35, 58);
            roundedPanel2.BorderColor     = Color.FromArgb(60, 130, 180);
            roundedPanel2.CornerRadius    = 12;
            roundedPanel2.Padding         = new Padding(10, 6, 10, 6);
            roundedPanel2.Margin          = new Padding(6, 4, 6, 4);

            tableLayoutPanel4.BackColor   = Color.Transparent;

            label4.Font      = new Font("Segoe UI", 13F, FontStyle.Bold);
            label4.ForeColor = Color.FromArgb(140, 180, 210);
            label4.BackColor = Color.Transparent;

            label5.Font      = new Font("Segoe UI", 13F, FontStyle.Bold);
            label5.ForeColor = Color.FromArgb(140, 180, 210);
            label5.BackColor = Color.Transparent;

            labelInputValue.Font      = new Font("Montserrat", 22F, FontStyle.Bold);
            labelInputValue.ForeColor = Color.White;
            labelInputValue.BackColor = Color.Transparent;

            labelChangeValue.Font      = new Font("Montserrat", 22F, FontStyle.Bold);
            labelChangeValue.ForeColor = AppColors.AccentGreen;
            labelChangeValue.BackColor = Color.Transparent;

            tableLayoutPanel3.BackColor = AppColors.NavyDark;
            tableLayoutPanel3.Padding   = new Padding(4, 2, 4, 6);
        }

        // ── Separadores entre columnas via fondo del contenedor padre ────
        private void EstilizarSeparadoresColumnas()
        {
            // Fondo NavyDark con gap de 2px entre columnas actúa como separador visual
            tableLayoutPanelMain.BackColor = AppColors.NavyDark;
            tableLayoutPanelMain.Padding   = new Padding(0);
            tableLayoutPanelMain.Margin    = new Padding(0);

            // Padding externo del form — espacio respirable sin exagerar
            MaintableLayout.Padding = new Padding(4, 0, 4, 4);
        }

        // ── Sección balanza / scale ──────────────────────────────────────
        private void EstilizarSeccionBalanza()
        {
            tableLayoutPanelPesado.BackColor = AppColors.NavyDark;
            tableLayoutPanelPesado.Padding   = new Padding(6);

            labelWeight.Font      = new Font("Montserrat", 20F, FontStyle.Bold);
            labelWeight.ForeColor = AppColors.Warning;
            labelWeight.BackColor = Color.Transparent;

            labelPesaProduct.Font      = new Font("Segoe UI", 12F, FontStyle.Regular);
            labelPesaProduct.ForeColor = AppColors.TextWhite;
            labelPesaProduct.BackColor = Color.Transparent;

            lblScalStatusDesc.Font      = new Font("Segoe UI", 9F, FontStyle.Italic);
            lblScalStatusDesc.ForeColor = AppColors.TextMuted;
            lblScalStatusDesc.BackColor = Color.Transparent;

            pictureBoxPesado.BackColor = AppColors.NavyDark;
        }

        private async void frmHome_Load(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                AplicarEstiloVisual();

                await LoadMenuCategoriesAsync();
                await LoadCategoriesAsync();

                orderId = await LoadLastInvoiceAsync();

                // CRÍTICO: await explícito para evitar race condition con UpdateProductsDisplay
                await LoadTabInfoAsync();

                await _shoppingCart.LoadCartAsync();

                buttonInvoice.Text = $"⬡  {orderId}";
                labelCashier.Text  = $"👤  {SessionManager.Name}";

                await ActualizarEstadoBotonHold();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al inicializar la aplicación: {ex.Message}", "Error de inicio",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void frmHome_FormClosing(object sender, FormClosingEventArgs e)
        {
            _zebraScannerService?.Close();
        }

        // CRÍTICO: async Task para poder ser awaited — evita race condition con UpdateProductsDisplay
        private async Task LoadTabInfoAsync()
        {
            tabControlMenuCategories.TabPages.Clear();

            var productsBg = Color.FromArgb(248, 249, 252);

            foreach (var menuCategory in MenuCategories)
            {
                var tabName = menuCategory.Name;

                TabPage tabMenu = new TabPage(tabName)
                {
                    Name                    = $"tab{tabName}",
                    UseVisualStyleBackColor = false,
                    BackColor               = productsBg,
                    Padding                 = new Padding(8, 8, 8, 4),
                };

                int[] listCategoriesId = Categories.Where(c => c.Tipo == tabName).Select(c => c.Id).ToArray();
                if (listCategoriesId.Length > 0)
                    tabMenu.Tag = string.Join(",", listCategoriesId);

                FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel
                {
                    BackColor    = productsBg,
                    Name         = $"flowLayoutPanel{tabName}",
                    Dock         = DockStyle.Fill,
                    WrapContents = true,
                    AutoScroll   = true,
                    Padding      = new Padding(4),
                    Font         = new Font("Segoe UI", 11F, FontStyle.Regular),
                };

                // CRÍTICO: NO agregar al Form (this.Controls) — solo al TabPage
                tabMenu.Controls.Add(flowLayoutPanel);
                tabControlMenuCategories.Controls.Add(tabMenu);
            }

            if (tabControlMenuCategories.TabPages.Count > 0)
            {
                _selectedTab = tabControlMenuCategories.TabPages[0];

                if (_selectedTab?.Tag != null)
                {
                    var categoryIds = Array.ConvertAll(_selectedTab.Tag.ToString()!.Split(','), int.Parse);
                    await LoadProductsAsync(categoryIds);
                    UpdateProductsDisplay();
                }
            }
        }

        private async void tabControlMenuCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedTab = tabControlMenuCategories.SelectedTab;

            if (_selectedTab?.Tag != null)
            {
                var categoryIds = Array.ConvertAll(_selectedTab.Tag.ToString()!.Split(','), int.Parse);

                await LoadProductsAsync(categoryIds);

                UpdateProductsDisplay();
            }
        }

        private async void AbecedarioControl1_LetraClicked(object sender, string letra)
        {
            try
            {
                if (_selectedTab?.Tag != null)
                {
                    var categoryIds = Array.ConvertAll(_selectedTab.Tag.ToString()!.Split(','), int.Parse);
                    await LoadProductsAsync(categoryIds, letra);
                    UpdateProductsDisplay();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading products. Please try again.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateProductsDisplay()
        {
            if (_selectedTab?.Controls[0] is FlowLayoutPanel flowLayoutPanel)
            {
                flowLayoutPanel.Controls.Clear();

                foreach (var product in Products)
                {
                    var productControl = new ProductImageControl(product);
                    productControl.ProductClicked += (sender, e) =>
                    {
                        try
                        {
                            var category = Categories.Where(c => c.Id == product.CategoryId).SingleOrDefault();

                            if (category != null && category.Pesado)
                            {
                                labelPesaProduct.Text = $"{product.Name} (${product.Price.Value.ToString("n2")})";
                                labelPesaProduct.AccessibleName = product.Id.ToString();
                                pictureBoxPesado.ImageLocation = product.Image.ConvertUrlString();
                            }
                            else
                            {
                                _shoppingCart.AddItem(new CartItem()
                                {
                                    ProductId = product.Id ?? 0,
                                    ProductName = product.Name,
                                    UnitPrice = product.Price ?? 0.0m,
                                    Quantity = 1
                                });

                                UpdateCartDisplay();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error adding product to cart. Please try again.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    };
                    flowLayoutPanel.Controls.Add(productControl);
                }
            }
        }

        private void KeyPaymentControl1_KeyPaymentClicked(object sender, string tag)
        {
            switch (tag)
            {
                case "0":
                case "00":
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                    if (int.TryParse(inputValue.ToString() + tag, out int parsedDigit))
                        inputValue = parsedDigit;

                    DisplayTotales();
                    break;

                case "1000":
                case "2000":
                case "5000":
                case "10000":
                    if (int.TryParse(tag, out int parsedBill))
                        inputValue = parsedBill;

                    DisplayTotales();
                    break;

                default: // C
                    ClearTotales();
                    break;
            }
        }

        private void DisplayTotales()
        {
            labelInputValue.Text = (inputValue / 100.0).ToString("N2");

            changeValue = (inputValue / 100m) - totalGlobal;

            if (changeValue > 0)
            {
                labelChangeValue.Text = changeValue.ToString("N2");
            }
            else
            {
                labelChangeValue.Text = (0.0).ToString("N2");
            }

            dueValue = totalGlobal - (inputValue / 100m);
        }

        private void ClearTotales(bool all = false)
        {
            inputValue = 0;
            changeValue = 0;
            dueValue = 0;

            if (all)
            {
                labelTotalValue.Text = "0.00";
            }

            labelSubTotal.Text = "0.00";
            labelTotalTax.Text = "0.00";
            labelChangeValue.Text = "0.00";
            labelInputValue.Text = "0.00";
        }

        private void _zebraScannerService_OnBarcodeDataReceived(string barcodeData)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => textBoxUPC.Text = barcodeData));
            }
            else
            {
                textBoxUPC.Text = barcodeData;
            }
        }

        private void _zebraScannerService_OnWeightUpdated(string weightStatus, double w)
        {
            weight = w;
            SharedData.WeightUnit = weightStatus;

            if (labelWeight.InvokeRequired)
            {
                labelWeight.Invoke(new Action(() => labelWeight.Text = weightStatus));
            }
            else
            {
                labelWeight.Text = weightStatus;
            }
            SharedData.WeightUnit = weightStatus;
        }

        private void textBoxUPC_TextChanged(object sender, EventArgs e)
        {
            SearchProduct(textBoxUPC.Text);
            textBoxUPC.Focus();
        }

        private void buttonCancelOrder_Click(object sender, EventArgs e)
        {
            _shoppingCart.Clear();
            _paymentSplitService.Clear();

            UpdateCartDisplay();
            ClearTotales(true);
        }

        private void buttonChangeQuantity_Click(object sender, EventArgs e)
        {
            if (listViewCart.SelectedItems.Count > 0)
            {
                var selectedItem = listViewCart.SelectedItems[0];
                var productId = int.Parse(selectedItem.Tag.ToString());
                var form = new frmPopupQuantity(0, productId);
                form.ShowDialog(this);
            }
        }

        private void buttonDeleteItem_Click(object sender, EventArgs e)
        {
            if (listViewCart.SelectedItems.Count > 0)
            {
                var selectedItem = listViewCart.SelectedItems[0];
                var productId = int.Parse(selectedItem.Tag.ToString());
                _shoppingCart.RemoveItem(productId);
            }
        }

        private void buttonHold_Click(object sender, EventArgs e)
        {
            var frm = new frmHold();
            // Actualizar estado del botón cuando el formulario de hold se cierre
            frm.FormClosed += async (s, args) => await ActualizarEstadoBotonHold();
            frm.Show(this);
        }

        /// <summary>
        /// Consulta holds activos y actualiza el botón visualmente:
        /// sin holds → ámbar normal | con holds → acento verde + contador
        /// </summary>
        private async Task ActualizarEstadoBotonHold()
        {
            try
            {
                var sessionId  = OmadaPOS.Libreria.Utils.WindowsIdProvider.GetMachineGuid();
                var heldCarts  = await _holdService.GetHeldCartsBySessionAsync(sessionId);
                int count      = heldCarts?.Count ?? 0;

                if (count > 0)
                {
                    // Hay pedidos en hold — color de alerta verde con contador
                    ElegantButtonStyles.Style(buttonHold, AppColors.AccentGreen,
                        AppColors.TextWhite, fontSize: 14f);
                    buttonHold.Text = $"⏸  HOLD  [{count}]";
                }
                else
                {
                    // Sin holds — color ámbar estándar
                    ElegantButtonStyles.Style(buttonHold, ElegantButtonStyles.WarningOrange,
                        AppColors.TextWhite, fontSize: 16f);
                    buttonHold.Text = "⏸  HOLD";
                }
            }
            catch
            {
                // Si falla la consulta, dejar el botón en estado neutro sin romper la UI
                buttonHold.Text = "⏸  HOLD";
            }
        }

        private void buttonSetting_Click(object sender, EventArgs e)
        {
            var frm = new frmSetting();
            frm.ShowDialog(this);
        }

        private void buttonDualScreen_Click(object sender, EventArgs e)
        {
            var formulario = new frmCustomerScreen();

            formulario.StartPosition = FormStartPosition.Manual;

            if (Screen.AllScreens.Length > 1)
            {
                Screen pantallaSecundaria = Screen.AllScreens[1];
                formulario.Bounds = pantallaSecundaria.Bounds;
            }

            formulario.Show();
        }

        private async void buttonLogout_Click(object sender, EventArgs e) =>
            await EjecutarLogout();

        private void buttonProductNoTax_Click(object sender, EventArgs e)
        {
            var form = new frmProductNew(false);
            form.Show(this);
        }

        private void buttonLookup_Click(object sender, EventArgs e)
        {
            var form = new frmKeyLookup();
            form.Show(this);
        }

        private async void buttonOpenDrawer_Click(object sender, EventArgs e)
        {
            var config = await _adminSettingService.LoadSettingById(WindowsIdProvider.GetMachineGuid());

            if (config == null || string.IsNullOrEmpty(config.PrinterName))
            {
                MessageBox.Show("Printer not configured", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string printerName = config.PrinterName;

            byte[] openDrawerCommand = [0x1B, 0x70, 0x00, 0x3C, 0xFF];

            await _userService.Log(new LogDTO()
            {
                AdminId = SessionManager.AdminId ?? 0,
                Phone = SessionManager.Phone,
                Info = "Open Drawer"
            });

            RawPrinterHelper.SendBytesToPrinter(printerName, openDrawerCommand);
        }

        private void buttonSplit_Click(object sender, EventArgs e)
        {
            if (_shoppingCart.ItemCount > 0 && totalGlobal > 0)
            {
                var form = new frmSplit();
                form.Show(this);
            }
        }

        private void buttonEBTBalance_Click(object sender, EventArgs e)
        {
            PaymentOrder(PaymentType.EBTBalance);
        }

        public void ChangeQuantity(int qty, int productId)
        {
            if (qty > 0)
            {
                _shoppingCart.UpdateQuantity(productId, qty);
            }
        }

        /// <summary>
        /// Creates an ad-hoc product on the server and adds it to the cart.
        /// Called from frmProductNew. Business logic delegated to ProductApplicationService.
        /// </summary>
        public async void addCustomProduct(bool bTax, decimal price)
        {
            var cartItem = await _productApplicationService.CreateCustomProductAsync(bTax, price);

            if (cartItem != null)
            {
                _shoppingCart.AddItem(cartItem);
                UpdateCartDisplay();
            }
        }

        public async void GiftCardPay()
        {
            if (_shoppingCart.ItemCount > 0)
            {
                var config = await _adminSettingService.LoadSettingById(WindowsIdProvider.GetMachineGuid());

                string terminal = config?.Terminal ?? string.Empty;

                PlaceOrderModel? placeOrderModel = _orderApplicationService.BuildOrderModel(
                    _shoppingCart.Items,
                    changeValue,
                    terminal,
                    "GiftCard",
                    0,
                    bDesc
                );

                if (placeOrderModel != null)
                {
                    var orderResponse = await _orderService.PlaceOrder(placeOrderModel);

                    if (orderResponse != null)
                    {
                        await PaymentSummary(orderResponse.Order_Id, orderResponse.Consecutivo);
                    }
                }
            }
        }

        private async void buttonPayCash_Click(object sender, EventArgs e)
        {
            if (_shoppingCart.ItemCount > 0 && (inputValue / 100.0m) >= totalGlobal && inputValue > 0.0)
            {
                var config = await _adminSettingService.LoadSettingById(WindowsIdProvider.GetMachineGuid());

                string terminal = config?.Terminal ?? string.Empty;

                PlaceOrderModel? placeOrderModel = _orderApplicationService.BuildOrderModel(
                    _shoppingCart.Items,
                    changeValue,
                    terminal,
                    "Cash",
                    0,
                    bDesc
                );

                if (placeOrderModel != null)
                {
                    var orderResponse = await _orderService.PlaceOrder(placeOrderModel);

                    if (orderResponse != null)
                    {
                        await PaymentSummary(orderResponse.Order_Id, orderResponse.Consecutivo);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid amount.", "Invalid Amount", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        async Task PaymentSummary(int oId, int consecutivo)
        {
            _shoppingCart.Clear();
            _paymentSplitService.Clear();

            UpdateCartDisplay();
            ClearTotales(true);

            // CRÍTICO: asignar resultado para que el botón muestre el nuevo número de orden
            orderId = await LoadLastInvoiceAsync();
            buttonInvoice.Text = $"⬡  {orderId}";

            var devuelta = changeValue;

            var formInfoPayment = new frmPopupCashPayment(oId, consecutivo, devuelta);
            formInfoPayment.Show(this);
        }

        private void buttonGiftCard_Click(object sender, EventArgs e)
        {
            var form = new frmGiftCard(totalGlobal, 1);
            form.Show(this);
        }

        private void buttonPayCreditCard_Click(object sender, EventArgs e)
        {
            PaymentOrder(PaymentType.Credit);
        }

        private void buttonPayDebitCard_Click(object sender, EventArgs e)
        {
            PaymentOrder(PaymentType.Debit);
        }

        private void buttonEBTFood_Click(object sender, EventArgs e)
        {
            PaymentOrder(PaymentType.EBT);
        }

        private async void PaymentOrder(PaymentType paymentType)
        {
            var config = await _adminSettingService.LoadSettingById(WindowsIdProvider.GetMachineGuid());

            string terminal = config?.Terminal ?? string.Empty;
            int port = config?.Port ?? 0;
            string ip = config?.IP ?? string.Empty;

            var consecutivo = await _orderService.LoadLastConsecutivoPayment();

            if (paymentType == PaymentType.EBTBalance)
            {
                var paymentResponse = await _paymentService.GetEBTBalanceAsync(new PaymentRequest()
                {
                    Ip = ip,
                    Port = port,
                    Terminal = terminal,
                    Amount = 0,
                    EcrRefNumber = consecutivo.ToString(),
                });

                var formPaymentStatus = new frmPaymentStatus(paymentResponse.MsgInfo + " Balance:" + paymentResponse.Balance.ToString());
                formPaymentStatus.ShowDialog(this);
            }
            else if (_shoppingCart.ItemCount > 0)
            {
                var placeOrderModel = _orderApplicationService.BuildOrderModel(
                    _shoppingCart.Items,
                    0,
                    terminal,
                    paymentType.ToString(),
                    0,
                    bDesc
                );

                if (placeOrderModel != null)
                {
                    var totalPayment = totalGlobal * 100;

                    // Delegate surcharge calculation to domain policy
                    totalPayment = SurchargePolicy.Apply(totalPayment, paymentType, SessionManager.BranchId);

                    var paymentResponse = await _paymentService.ProcessPaymentAsync(paymentType, new PaymentRequest()
                    {
                        Ip = ip,
                        Port = port,
                        Terminal = terminal,
                        Amount = totalPayment,
                        EcrRefNumber = consecutivo.ToString(),
                    });

                    if (paymentResponse != null && paymentResponse.Success)
                    {
                        var formPaymentStatus = new frmPaymentStatus(paymentResponse.MsgInfo + " Balance:" + paymentResponse.Balance.ToString());
                        formPaymentStatus.ShowDialog(this);

                        placeOrderModel.Balance = paymentResponse.Balance;

                        var orderResponse = await _orderService.PlaceOrder(placeOrderModel);

                        if (orderResponse != null)
                        {
                            await PaymentSummary(orderResponse.Order_Id, orderResponse.Consecutivo);
                        }
                    }
                    else
                    {
                        var formPaymentStatus = new frmPaymentStatus(paymentResponse.MsgInfo);
                        formPaymentStatus.ShowDialog(this);
                    }
                }
            }
        }

        private void buttonInvoice_Click(object sender, EventArgs e)
        {
            var printInvoiceForm = new frmPrintInvoice();
            printInvoiceForm.ShowDialog();
        }

        /// <summary>
        /// Resolves a scanned UPC and adds the product to the cart.
        /// Barcode parsing and API lookup delegated to ProductApplicationService.
        /// </summary>
        public async void SearchProduct(string upc)
        {
            if (string.IsNullOrEmpty(upc))
                return;

            var result = await _productApplicationService.SearchByBarcodeAsync(upc);

            if (result.IsFound && result.CartItem != null)
            {
                labelProductName.Text = result.ProductName;
                _shoppingCart.AddItem(result.CartItem);
                UpdateCartDisplay();
                textBoxUPC.Text = "";
            }
            else if (result.ProductNotFoundOnServer)
            {
                var frm = new frmProductNoExist(textBoxUPC.Text);
                frm.ShowDialog(this);
            }
            else
            {
                MessageBox.Show("Product not found for the given UPC.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonSettings_Click(object sender, EventArgs e) =>
            buttonSetting_Click(sender, e);

        public async Task<int> LoadLastInvoiceAsync() =>
            await _orderService.LoadLastInvoiceAdmin();

        public async Task LoadCategoriesAsync()
        {
            var categories = await _categoryService.LoadCategories();
            Categories.Clear();
            foreach (var category in categories)
                Categories.Add(category);
        }

        public async Task LoadMenuCategoriesAsync()
        {
            var menuCategories = await _categoryService.LoadMenuCategories();
            MenuCategories.Clear();
            foreach (var menuCategory in menuCategories)
                MenuCategories.Add(menuCategory);
        }

        public async Task LoadProductsAsync(int[] categoryIds, string? searchLetter = null)
        {
            var products = string.IsNullOrEmpty(searchLetter)
                ? await _categoryService.LoadProductIdCategories(new IdCategoryDTO { Ids = categoryIds })
                : await _categoryService.LoadProductsByCategoryLetra(new IdCategoryDTO { Ids = categoryIds }, searchLetter);

            Products.Clear();
            foreach (var product in products)
                Products.Add(product);
        }

        public async Task<PaymentResponseModel> ProcessPaymentAsync(string paymentType, decimal amount)
        {
            var config = await _adminSettingService.LoadSettingById(WindowsIdProvider.GetMachineGuid());
            if (config == null)
                throw new InvalidOperationException("Payment terminal configuration not found");

            var request = new PaymentRequest
            {
                Ip           = config.IP ?? string.Empty,
                Port         = config.Port ?? 0,
                Amount       = amount * 100,
                EcrRefNumber = (await _orderService.LoadLastConsecutivoPayment()).ToString()
            };

            return paymentType switch
            {
                "CREDIT_CARD" => await _paymentService.ProcessPaymentAsync(PaymentType.Credit, request),
                "DEBIT_CARD"  => await _paymentService.ProcessPaymentAsync(PaymentType.Debit, request),
                "EBT"         => await _paymentService.ProcessPaymentAsync(PaymentType.EBT, request),
                "EBT_BALANCE" => await _paymentService.GetEBTBalanceAsync(request),
                _             => throw new ArgumentException($"Unsupported payment type: {paymentType}")
            };
        }

        public async void ProcessPaymentMultiple()
        {
            var config = await _adminSettingService.LoadSettingById(WindowsIdProvider.GetMachineGuid());

            string terminal = config?.Terminal ?? string.Empty;

            List<PlaceOrderPayment> payments = [];

            var pays = await _paymentSplitService.GetSessionPaymentsAsync();

            foreach (var pay in pays)
            {
                if (pay.Total > 0)
                {
                    payments.Add(new PlaceOrderPayment
                    {
                        Tipo = pay.PaymentType,
                        Total = pay.Total
                    });
                }
            }

            var placeOrderModel = _orderApplicationService.BuildMultipleOrderModel(
                _shoppingCart.Items,
                changeValue,
                terminal,
                payments,
                0,
                bDesc
            );

            var json = JsonConvert.SerializeObject(placeOrderModel);

            var orderResponse = await _orderService.PlaceOrderMultiple(placeOrderModel);

            if (orderResponse != null)
            {
                await PaymentSummary(orderResponse.Order_Id, orderResponse.Consecutivo);
            }
        }

        public async Task<OrderResponse?> PlaceOrderAsync(string paymentMethod, decimal changeAmount)
        {
            var config   = await _adminSettingService.LoadSettingById(WindowsIdProvider.GetMachineGuid());
            var terminal = config?.Terminal ?? string.Empty;

            var orderModel = _orderApplicationService.BuildOrderModel(
                _shoppingCart.Items, changeAmount, terminal, paymentMethod, 0, bDesc);

            if (orderModel == null)
                throw new InvalidOperationException("Failed to create order model");

            return await _orderService.PlaceOrder(orderModel);
        }

        private async void pictureBoxPesado_Click(object sender, EventArgs e)
        {
            if (labelPesaProduct.Text != "" && weight > 0)
            {

                if (!string.IsNullOrEmpty(labelPesaProduct.AccessibleName))
                {
                    int productId = int.Parse(labelPesaProduct.AccessibleName);

                    var product = await _categoryService.LoadProductById(productId);

                    labelProductName.Text = product.Name;

                    _shoppingCart.AddItem(new CartItem()
                    {
                        ProductId = product.Id ?? 0,
                        ProductName = product.Name,
                        UnitPrice = product.Price ?? 0.0m,
                        Quantity = 1,
                        Peso = weight / 1000
                    });

                    UpdateCartDisplay();

                    labelPesaProduct.Text = "";
                    labelPesaProduct.AccessibleName = "";
                    labelWeight.Text = "";
                    weight = 0.0;

                    pictureBoxPesado.Image = null;
                }
            }
        }

        private void buttonCheckPrice_Click(object sender, EventArgs e)
        {
            var form = new frmCheckPrice();
            form.ShowDialog(this);
        }

        private void labelCashier_Click(object sender, EventArgs e)
        {
            if (_userMenuVisible)
                OcultarMenuUsuario();
            else
                MostrarMenuUsuario();
        }
    }
}
