using OmadaPOS.Componentes;
using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Services;
using OmadaPOS.Libreria.Utils;
using OmadaPOS.Models;
using OmadaPOS.Presentation.Controls;
using OmadaPOS.Services;
using OmadaPOS.Services.Navigation;
using OmadaPOS.Services.POS;

namespace OmadaPOS.Views
{
    public partial class frmHome : Form
    {
        private readonly ZebraScannerService? _zebraScannerService;

        private POSHeaderControl? _posHeaderControl;
        private CartListViewControl? _cartListViewControl;
        private CartTotalsControl? _cartTotalsControl;
        private PaymentPanelControl? _paymentPanelControl;
        private UserSessionControl? _userSessionControl;

        // Initialized in code so the WinForms Designer cannot remove it on regeneration.
        private NumericPadControl keyPaymentControl1 = null!;

        int orderId = 0;

        decimal totalGlobal = 0;
        decimal changeValue = 0;
        double weight = 0.0;

        private TabPage? _selectedTab;
        public List<MenuCategoryModel> MenuCategories { get; } = new();
        public List<CategoryModel> Categories { get; } = new();
        public List<ProductModel> Products { get; } = new();

        private readonly IShoppingCart _shoppingCart;
        private readonly IUserService _userService;
        private readonly IPaymentSplitService _paymentSplitService;
        private readonly IHomeInitializationService _homeInitializationService;
        private readonly IBarcodeSaleService _barcodeSaleService;
        private readonly IPaymentCoordinatorService _paymentCoordinatorService;
        private readonly IHomeHoldCartService _homeHoldCartService;
        private readonly ICashDrawerService _cashDrawerService;
        private readonly IWindowService _windowService;
        private readonly IHomeInteractionService _homeInteractionService;

        public frmHome(
            ZebraScannerService zebraScannerService,
            IShoppingCart shoppingCart,
            IUserService userService,
            IPaymentSplitService paymentSplitService,
            IHomeInitializationService homeInitializationService,
            IBarcodeSaleService barcodeSaleService,
            IPaymentCoordinatorService paymentCoordinatorService,
            IHomeHoldCartService homeHoldCartService,
            ICashDrawerService cashDrawerService,
            IWindowService windowService,
            IHomeInteractionService homeInteractionService)
        {
            InitializeComponent();

            InicializarControlesUI();

            _userService = userService;
            _paymentSplitService = paymentSplitService;
            _homeInitializationService = homeInitializationService;
            _barcodeSaleService = barcodeSaleService;
            _paymentCoordinatorService = paymentCoordinatorService;
            _homeHoldCartService = homeHoldCartService;
            _cashDrawerService = cashDrawerService;
            _windowService = windowService;
            _homeInteractionService = homeInteractionService;

            _shoppingCart = shoppingCart;
            _shoppingCart.CartChanged += ShoppingCart_CartChanged;
            _homeInteractionService.RegisterHandlers(
                ChangeQuantity,
                AddCustomProductAsync,
                SearchProductAsync,
                ProcessPaymentMultipleAsync,
                UpdateCartDisplay);
            _homeInteractionService.RegisterGiftCardHandler(GiftCardPayAsync);

            ConfigureUI();
            ConfigureListView();

            _zebraScannerService = zebraScannerService;

            _zebraScannerService.OnBarcodeDataReceived += _zebraScannerService_OnBarcodeDataReceived;
            _zebraScannerService.OnWeightUpdated += _zebraScannerService_OnWeightUpdated;

            lblScalStatusDesc.Text = zebraScannerService.Initialize();
        }

        private void InicializarControlesUI()
        {
            // Create the payment numpad in code so the Designer cannot overwrite it.
            keyPaymentControl1 = new NumericPadControl(NumericPadControl.PadMode.MoneyWithBills)
            {
                Dock = DockStyle.Fill,
                Name = "keyPaymentControl1"
            };
            tableLayoutPanelPayment.Controls.Add(keyPaymentControl1, 0, 1);

            // abecedarioControl1 is declared in the Designer but stripped from
            // InitializeComponent on every regeneration — create it in code.
            if (abecedarioControl1 == null)
            {
                abecedarioControl1 = new Componentes.AbecedarioControl
                {
                    Dock = DockStyle.Fill,
                    Name = "abecedarioControl1",
                };
                abecedarioControl1.LetraClicked += AbecedarioControl1_LetraClicked;
                tableLayoutPanelCategoria.Controls.Add(abecedarioControl1, 0, 1);
            }

            ScanInputControl.Attach(tableLayoutPanel1, textBoxUPC);
            _userSessionControl = UserSessionControl.Attach(this, MaintableLayout, tableLayoutPanel1, labelCashier);
            _userSessionControl.SettingsRequested += (_, _) => buttonSetting_Click(this, EventArgs.Empty);
            _userSessionControl.DailyCloseRequested += (_, _) => labelCashier_ClickInternal();
            _userSessionControl.LogoutRequested += async (_, _) => await EjecutarLogout();

            _posHeaderControl = POSHeaderControl.Attach(
                MaintableLayout,
                tableLayoutPanel1,
                buttonInvoice,
                ButtonSettings,
                buttonClose);

            // roundedPanel1/roundedPanel2 are declared in the Designer but
            // the Designer strips their initialization from InitializeComponent.
            // We create them in code to guarantee they are never null.
            if (roundedPanel1 == null)
            {
                roundedPanel1 = new Componentes.RoundedPanel { Dock = DockStyle.Fill };
                tableLayoutPanelTotal.Dock = DockStyle.Fill;
                roundedPanel1.Controls.Add(tableLayoutPanelTotal);
                tableLayoutPanelCart.Controls.Add(roundedPanel1, 0, 1);
            }

            if (roundedPanel2 == null)
            {
                roundedPanel2 = new Componentes.RoundedPanel { Dock = DockStyle.Fill };
                tableLayoutPanel4.Dock = DockStyle.Fill;
                roundedPanel2.Controls.Add(tableLayoutPanel4);
                tableLayoutPanelPayment.Controls.Add(roundedPanel2, 0, 2);
            }

            _cartListViewControl = CartListViewControl.Attach(tableLayoutPanelCart, listViewCart);
            _cartTotalsControl = CartTotalsControl.Attach(
                tableLayoutPanelCart,
                roundedPanel1,
                label1,
                label2,
                label3,
                labelSubTotal,
                labelTotalTax,
                labelTotalValue);

            _paymentPanelControl = PaymentPanelControl.Attach(
                tableLayoutPanelMain,
                tableLayoutPanelPayment,
                tableLayoutPanel2,
                keyPaymentControl1,
                roundedPanel2,
                tableLayoutPanel4,
                label4,
                label5,
                labelInputValue,
                labelChangeValue,
                tableLayoutPanel3,
                splitContainerScale,
                tableLayoutPanelPesado,
                labelWeight,
                labelPesaProduct,
                lblScalStatusDesc,
                pictureBoxPesado);

            keyPaymentControl1.ValueChanged += (_, _) => DisplayTotales();
        }

        private void ConfigureUI()
        {
            // ═══════════════════════════════════════════════════════════
            // GRUPO 1 — Barra superior (header)
            // ═══════════════════════════════════════════════════════════
            // Botones compactos del header — ícono solo, tooltip para accesibilidad
            ElegantButtonStyles.Style(buttonInvoice,  ElegantButtonStyles.Keypad,   fontSize: 20f);
            ElegantButtonStyles.Style(buttonClose,    ElegantButtonStyles.AlertRed, fontSize: 20f);
            ElegantButtonStyles.Style(ButtonSettings, ElegantButtonStyles.Keypad,   fontSize: 16f);

            // Tooltips accesibles para botones de ícono
            var toolTip = new ToolTip { ShowAlways = true };
            components ??= new System.ComponentModel.Container();
            components.Add(toolTip);
            toolTip.SetToolTip(buttonInvoice,  "Reimprimir último recibo");
            toolTip.SetToolTip(ButtonSettings, "Configuración");
            toolTip.SetToolTip(buttonClose,    "Salir / Cerrar sesión");

            // ═══════════════════════════════════════════════════════════
            // GRUPO 2 — Acciones del carrito
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
            => _cartListViewControl?.Configure();

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
            var result = _cartListViewControl?.UpdateCartItems(_shoppingCart.Items);
            if (result == null) return;

            totalGlobal = result.Total;
            _cartTotalsControl?.UpdateTotals(result.SubTotal, result.TaxTotal, result.Total);
        }

        // ═══════════════════════════════════════════════════════════════
        // TEMA VISUAL — Triband Layout
        // ═══════════════════════════════════════════════════════════════
        private void AplicarEstiloVisual()
        {
            MaintableLayout.BackColor = AppColors.NavyDark;
            MaintableLayout.Padding = new Padding(0);
            MaintableLayout.Margin = new Padding(0);

            _posHeaderControl?.ApplyTheme();
            _cartTotalsControl?.ApplyTheme();
            EstilizarColumnaProductos();
            _paymentPanelControl?.ApplyTheme();
            EstilizarSeparadoresColumnas();

            this.Invalidate(true);
            this.Refresh();
        }

        private void labelCashier_ClickInternal()
            => _windowService.OpenDailyClose(this);

        private async Task EjecutarLogout()
        {
            await _userService.Logout(new LogDTO
            {
                AdminId = SessionManager.AdminId ?? 0,
                Phone   = SessionManager.Phone
            });
            _supervisorApproved = true;   // Tell FormClosing this close is authorised.
            _windowService.OpenSignIn();
            this.Close();
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

        // ── Separadores entre columnas via fondo del contenedor padre ────
        private void EstilizarSeparadoresColumnas()
        {
            // Borde sutil entre columnas — gris claro en lugar de navy oscuro
            tableLayoutPanelMain.BackColor = Color.FromArgb(210, 215, 225);
            tableLayoutPanelMain.Padding   = new Padding(0);
            tableLayoutPanelMain.Margin    = new Padding(0);

            MaintableLayout.Padding = new Padding(4, 0, 4, 4);
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

                _posHeaderControl?.SetInvoiceDisplay(orderId);
                _userSessionControl?.InvalidateSession();

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

        // Set to true by EjecutarLogout() so FormClosing knows it is an authorised close.
        private bool _supervisorApproved = false;

        private void frmHome_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Block any user-initiated close (Alt+F4, task-bar close, etc.)
            // unless the supervisor PIN was already verified via EjecutarLogout().
            if (!_supervisorApproved && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                if (!VerificarPinSupervisor()) return;

                // PIN accepted — run logout and let it close cleanly.
                _ = EjecutarLogout();
                return;
            }

            // Authorised close — clean up singleton event subscriptions.
            _shoppingCart.CartChanged -= ShoppingCart_CartChanged;

            if (_zebraScannerService != null)
            {
                _zebraScannerService.OnBarcodeDataReceived -= _zebraScannerService_OnBarcodeDataReceived;
                _zebraScannerService.OnWeightUpdated       -= _zebraScannerService_OnWeightUpdated;
                _zebraScannerService.Close();
            }

            _homeInteractionService.ClearHandlers();
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
            try
            {
                _selectedTab = tabControlMenuCategories.SelectedTab;

                if (_selectedTab?.Tag != null)
                {
                    var categoryIds = Array.ConvertAll(_selectedTab.Tag.ToString()!.Split(','), int.Parse);
                    await LoadProductsAsync(categoryIds);
                    UpdateProductsDisplay();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            catch (Exception)
            {
                MessageBox.Show("Error loading products. Please try again.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateProductsDisplay()
        {
            if (_selectedTab?.Controls[0] is FlowLayoutPanel flowLayoutPanel)
            {
                // Dispose explícito de cada control antes de removerlo.
                // ControlCollection.Clear() NO llama Dispose — los ProductImageControl
                // descartados retendrían HWNDs, Regions y Fonts activos.
                foreach (Control c in flowLayoutPanel.Controls)
                    c.Dispose();

                flowLayoutPanel.Controls.Clear();

                foreach (var product in Products)
                {
                    var productControl = new ProductImageControl(product);
                    productControl.ProductClicked += (sender, e) =>
                    {
                        try
                        {
                            var selection = _barcodeSaleService.HandleProductSelection(product, Categories);

                            if (selection.IsWeighted)
                            {
                                labelPesaProduct.Text = selection.WeightDisplayText ?? string.Empty;
                                labelPesaProduct.AccessibleName = selection.ProductIdText ?? string.Empty;
                                pictureBoxPesado.ImageLocation = selection.ImageLocation;
                            }
                            else if (selection.AddedToCart)
                            {
                                labelProductName.Text = selection.ProductName ?? string.Empty;
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

        private void DisplayTotales()
        {
            var tendered  = keyPaymentControl1.ValueDecimal;
            changeValue   = tendered - totalGlobal;
            var safeChange = changeValue > 0 ? changeValue : 0.0m;

            _paymentPanelControl?.UpdatePaymentValues(tendered, safeChange);
        }

        private void ClearTotales(bool all = false)
        {
            keyPaymentControl1.Reset();
            changeValue = 0;

            _cartTotalsControl?.ResetTotals(includeGrandTotal: all);
            _paymentPanelControl?.ResetPaymentValues();
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
                _windowService.OpenPopupQuantity(0, productId, this);
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
            _windowService.OpenHold(this, () => _ = ActualizarEstadoBotonHold());
        }

        /// <summary>
        /// Consulta holds activos y actualiza el botón visualmente:
        /// sin holds → ámbar normal | con holds → acento verde + contador
        /// </summary>
        private async Task ActualizarEstadoBotonHold()
        {
            try
            {
                int count = (await _homeHoldCartService.GetCurrentSessionStateAsync()).Count;

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
            => _windowService.OpenSettings(this);


        private async void buttonLogout_Click(object sender, EventArgs e)
        {
            if (!VerificarPinSupervisor()) return;
            try { await EjecutarLogout(); }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during logout: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Shows the supervisor PIN dialog. Returns true when the correct PIN is entered.
        /// </summary>
        private bool VerificarPinSupervisor()
        {
            using var dlg = new frmSupervisorPin();
            return dlg.ShowDialog(this) == DialogResult.OK;
        }

        private void buttonProductNoTax_Click(object sender, EventArgs e)
            => _windowService.OpenProductNew(false, this);

        private void buttonLookup_Click(object sender, EventArgs e)
            => _windowService.OpenKeyLookup(this);

        private async void buttonOpenDrawer_Click(object sender, EventArgs e)
        {
            try
            {
                var result = await _cashDrawerService.OpenDrawerAsync();

                if (!result.Success)
                {
                    MessageBox.Show(result.ErrorMessage ?? "Printer not configured", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening cash drawer: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonSplit_Click(object sender, EventArgs e)
        {
            if (_shoppingCart.ItemCount > 0 && totalGlobal > 0)
            {
                _windowService.OpenSplitPayment(this);
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

        private async Task AddCustomProductAsync(bool bTax, decimal price)
        {
            if (await _barcodeSaleService.AddCustomProductAsync(bTax, price))
            {
                UpdateCartDisplay();
            }
        }

        private async Task GiftCardPayAsync()
        {
            var orderResponse = await _paymentCoordinatorService.ProcessGiftCardAsync(changeValue, false);

            if (orderResponse != null)
            {
                await PaymentSummary(orderResponse.Order_Id, orderResponse.Consecutivo);
            }
        }

        private async void buttonPayCash_Click(object sender, EventArgs e)
        {
            try
            {
                var result = await _paymentCoordinatorService.ProcessCashSaleAsync(totalGlobal, keyPaymentControl1.ValueCents, changeValue, false);

                if (!result.IsValidAmount)
                {
                    MessageBox.Show(result.ErrorMessage ?? "Please enter a valid amount.",
                        "Invalid Amount", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (result.OrderResponse != null)
                {
                    await PaymentSummary(result.OrderResponse.Order_Id, result.OrderResponse.Consecutivo);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cash payment error: {ex.Message}", "Payment Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        async Task PaymentSummary(int oId, int consecutivo)
        {
            // Capturar el cambio ANTES de ClearTotales() que lo resetea a 0.
            var devuelta = changeValue > 0 ? changeValue : 0m;

            _shoppingCart.Clear();
            _paymentSplitService.Clear();

            UpdateCartDisplay();
            ClearTotales(true);

            // CRÍTICO: asignar resultado para que el botón muestre el nuevo número de orden
            orderId = await LoadLastInvoiceAsync();
            _posHeaderControl?.SetInvoiceDisplay(orderId);

            _windowService.OpenPopupCashPayment(oId, consecutivo, devuelta, this);
        }

        private void buttonGiftCard_Click(object sender, EventArgs e)
            => _windowService.OpenGiftCard(totalGlobal, 1, this);

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
            try
            {
                var result = await _paymentCoordinatorService.ProcessTerminalPaymentAsync(paymentType, totalGlobal, false);

                if (result.PaymentResponse != null)
                {
                    string message = result.PaymentResponse.MsgInfo;
                    if (result.IsBalanceInquiry || result.PaymentResponse.Success)
                        message += " Balance:" + result.PaymentResponse.Balance.ToString();

                    _windowService.OpenPaymentStatus(message, this);
                }

                if (result.OrderResponse != null)
                {
                    await PaymentSummary(result.OrderResponse.Order_Id, result.OrderResponse.Consecutivo);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Payment error: {ex.Message}", "Payment Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonInvoice_Click(object sender, EventArgs e)
            => _windowService.OpenPrintInvoice(this);

        /// <summary>
        /// Resolves a scanned UPC and adds the product to the cart.
        /// Barcode parsing and API lookup delegated to ProductApplicationService.
        /// </summary>
        public async void SearchProduct(string upc)
        {
            try { await SearchProductAsync(upc); }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching product: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task SearchProductAsync(string upc)
        {
            if (string.IsNullOrEmpty(upc))
                return;

            var result = await _barcodeSaleService.ProcessBarcodeAsync(upc);

            if (result.AddedToCart)
            {
                labelProductName.Text = result.ProductName ?? string.Empty;
                UpdateCartDisplay();
                textBoxUPC.Text = "";
            }
            else if (result.ProductNotFoundOnServer)
            {
                _windowService.OpenProductNoExist(textBoxUPC.Text, this);
            }
            else
            {
                MessageBox.Show("Product not found for the given UPC.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonSettings_Click(object sender, EventArgs e) =>
            buttonSetting_Click(sender, e);

        public async Task<int> LoadLastInvoiceAsync() =>
            await _homeInitializationService.LoadLastInvoiceAsync();

        public async Task LoadCategoriesAsync()
            => await _homeInitializationService.LoadCategoriesAsync(Categories);

        public async Task LoadMenuCategoriesAsync()
            => await _homeInitializationService.LoadMenuCategoriesAsync(MenuCategories);

        public async Task LoadProductsAsync(int[] categoryIds, string? searchLetter = null)
            => await _homeInitializationService.LoadProductsAsync(Products, categoryIds, searchLetter);

        private async Task ProcessPaymentMultipleAsync()
        {
            var orderResponse = await _paymentCoordinatorService.ProcessMultiplePaymentsAsync(changeValue, false);

            if (orderResponse != null)
            {
                await PaymentSummary(orderResponse.Order_Id, orderResponse.Consecutivo);
            }
        }

        private async void pictureBoxPesado_Click(object sender, EventArgs e)
        {
            try
            {
                if (labelPesaProduct.Text != "" && weight > 0)
                {
                    if (!string.IsNullOrEmpty(labelPesaProduct.AccessibleName))
                    {
                        int productId = int.Parse(labelPesaProduct.AccessibleName);
                        var result = await _barcodeSaleService.AddWeightedProductAsync(productId, weight);

                        if (result.AddedToCart)
                        {
                            labelProductName.Text = result.ProductName ?? string.Empty;
                            UpdateCartDisplay();

                            labelPesaProduct.Text = "";
                            labelPesaProduct.AccessibleName = "";
                            labelWeight.Text = "";
                            weight = 0.0;

                            pictureBoxPesado.Image = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding weighted product: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void labelCashier_Click(object sender, EventArgs e)
            => _userSessionControl?.ToggleMenu();
    }
}
