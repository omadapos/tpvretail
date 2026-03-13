using OmadaPOS.Componentes;
using OmadaPOS.Domain;
using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Services;
using OmadaPOS.Libreria.Utils;
using OmadaPOS.Models;
using OmadaPOS.Presentation.Controls;
using OmadaPOS.Presentation.Styling;
using OmadaPOS.Services;
using OmadaPOS.Services.Navigation;
using OmadaPOS.Services.POS;

namespace OmadaPOS.Views
{
    public partial class frmHome : Form
    {
        private readonly ZebraScannerService? _zebraScannerService;

        private POSHeaderControl?    _posHeaderControl;
        private CartListViewControl? _cartListViewControl;
        private CartTotalsControl?   _cartTotalsControl;
        private PaymentPanelControl? _paymentPanelControl;

        decimal totalGlobal = 0;
        decimal changeValue = 0;
        double weight = 0.0;

        private TabPage? _selectedTab;
        public List<MenuCategoryModel> MenuCategories { get; } = new();
        public List<CategoryModel> Categories { get; } = new();
        public List<ProductModel> Products { get; } = new();

        // Cancela cargas anteriores cuando el usuario cambia de pestaña rápido
        private CancellationTokenSource? _productLoadCts;

        private const int ProductPageSize = 30;

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
        private readonly frmCustomerScreen _customerScreen;

        // ── Age verification ───────────────────────────────────────────────────
        private readonly IAgeVerificationService      _ageVerificationService;
        private readonly IAgeRestrictionConfigService _ageRestrictionConfig = null!;
        private AgeVerificationResult?               _currentAgeVerification;
        private Label                                _labelAgeVerificationStatus = null!;


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
            IHomeInteractionService homeInteractionService,
            frmCustomerScreen customerScreen)
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
            _customerScreen = customerScreen;

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

            // Age verification — resolved via service locator (matches existing pattern)
            _ageVerificationService = Program.GetService<IAgeVerificationService>();
            _ageRestrictionConfig   = Program.GetService<IAgeRestrictionConfigService>();
            SetupAgeVerificationBadge();

            _zebraScannerService = zebraScannerService;

            _zebraScannerService.OnBarcodeDataReceived += _zebraScannerService_OnBarcodeDataReceived;
            _zebraScannerService.OnWeightUpdated       += _zebraScannerService_OnWeightUpdated;
        }

        // ── Scanner disconnected banner ────────────────────────────────────────
        private Label?  _scannerBanner;
        private Button? _scannerBannerBtn;

        private void ShowScannerBanner()
        {
            if (_scannerBanner != null) return; // already shown

            _scannerBanner = new Label
            {
                Text      = "⚠  Scanner no conectado — verifique el cable USB",
                AutoSize  = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Font      = AppTypography.BodySmall,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(180, 83, 9),   // amber-700
                Dock      = DockStyle.Top,
                Height    = 32,
                Padding   = new Padding(12, 0, 0, 0),
            };

            _scannerBannerBtn = new Button
            {
                Text      = "Reconectar",
                FlatStyle = FlatStyle.Flat,
                Font      = AppTypography.BodySmall,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(154, 52, 18),  // amber-800
                Size      = new Size(90, 22),
                Anchor    = AnchorStyles.Right | AnchorStyles.Top,
            };
            _scannerBannerBtn.FlatAppearance.BorderSize = 0;
            _scannerBannerBtn.Click += ScannerBannerBtn_Click;

            _scannerBanner.Controls.Add(_scannerBannerBtn);

            Controls.Add(_scannerBanner);
            _scannerBanner.BringToFront();
            PerformLayout();
        }

        private void ScannerBannerBtn_Click(object? sender, EventArgs e)
        {
            if (_zebraScannerService == null) return;
            if (_scannerBannerBtn != null)
            {
                _scannerBannerBtn.Enabled = false;
                _scannerBannerBtn.Text    = "Conectando…";
            }

            // UI thread (STA) — same apartment as the COM objects. Blocks max 1 s (ClaimDevice).
            bool ok = _zebraScannerService.TryReconnectScanner();

            if (ok)
            {
                Controls.Remove(_scannerBanner);
                _scannerBanner?.Dispose();
                _scannerBanner    = null;
                _scannerBannerBtn = null;
                _paymentPanelControl?.SetScaleStatus("");
                ShowToast("Scanner conectado");
            }
            else
            {
                if (_scannerBannerBtn != null)
                {
                    _scannerBannerBtn.Enabled = true;
                    _scannerBannerBtn.Text    = "Reintentar";
                }
            }
        }

        // ── Scale disconnected banner ──────────────────────────────────────────
        private Label?  _scaleBanner;
        private Button? _scaleBannerBtn;

        private void ShowScaleBanner()
        {
            if (_scaleBanner != null) return;

            _scaleBanner = new Label
            {
                Text      = "⚖  Báscula no conectada — verifique el cable USB",
                AutoSize  = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Font      = AppTypography.BodySmall,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(30, 64, 175),   // blue-800
                Dock      = DockStyle.Top,
                Height    = 32,
                Padding   = new Padding(12, 0, 0, 0),
            };

            _scaleBannerBtn = new Button
            {
                Text      = "Reconectar",
                FlatStyle = FlatStyle.Flat,
                Font      = AppTypography.BodySmall,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(29, 78, 216),   // blue-700
                Size      = new Size(90, 22),
                Anchor    = AnchorStyles.Right | AnchorStyles.Top,
                Cursor    = Cursors.Default,
            };
            _scaleBannerBtn.FlatAppearance.BorderSize         = 0;
            _scaleBannerBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(29, 78, 216);
            _scaleBannerBtn.FlatAppearance.MouseDownBackColor = Color.FromArgb(29, 78, 216);
            _scaleBannerBtn.Click += ScaleBannerBtn_Click;

            _scaleBanner.Controls.Add(_scaleBannerBtn);
            Controls.Add(_scaleBanner);
            _scaleBanner.BringToFront();
            PerformLayout();
        }

        private void ScaleBannerBtn_Click(object? sender, EventArgs e)
        {
            if (_zebraScannerService == null) return;
            if (_scaleBannerBtn != null)
            {
                _scaleBannerBtn.Enabled = false;
                _scaleBannerBtn.Text    = "Conectando…";
            }

            // UI thread (STA) — same apartment as the COM objects. Blocks max 2 s (ClaimDevice).
            bool ok = _zebraScannerService.TryReconnectScale();

            if (ok)
            {
                Controls.Remove(_scaleBanner);
                _scaleBanner?.Dispose();
                _scaleBanner    = null;
                _scaleBannerBtn = null;
                _paymentPanelControl?.SetScaleStatus("");
                ShowToast("Báscula conectada");
            }
            else
            {
                if (_scaleBannerBtn != null)
                {
                    _scaleBannerBtn.Enabled = true;
                    _scaleBannerBtn.Text    = "Reintentar";
                }
                ShowToast("No se pudo conectar la báscula", success: false);
            }
        }

        private void ShowToast(string message, bool success = true)
        {
            var toast = new Label
            {
                Text      = message,
                AutoSize  = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Font      = AppTypography.Body,
                ForeColor = Color.White,
                BackColor = success ? Color.FromArgb(22, 163, 74) : Color.FromArgb(220, 38, 38),
                Size      = new Size(260, 40),
                Location  = new Point((ClientSize.Width - 260) / 2, 12),
                Padding   = new Padding(0),
            };

            Controls.Add(toast);
            toast.BringToFront();

            var fadeTimer = new System.Windows.Forms.Timer { Interval = 2_500 };
            fadeTimer.Tick += (_, _) =>
            {
                fadeTimer.Stop();
                fadeTimer.Dispose();
                Controls.Remove(toast);
                toast.Dispose();
            };
            fadeTimer.Start();
        }

        private void InicializarControlesUI()
        {
            // ── Abecedario — always created in code; Designer never initializes it ─
            abecedarioControl1 = new Componentes.AbecedarioControl
            {
                Dock = DockStyle.Fill,
                Name = "abecedarioControl1",
            };
            abecedarioControl1.LetraClicked += AbecedarioControl1_LetraClicked;
            tableLayoutPanelCategoria.Controls.Add(abecedarioControl1, 0, 1);

            // ── Header: brand, cashier, scan input, product name, config, exit ──
            _posHeaderControl = POSHeaderControl.Attach(MaintableLayout, tableLayoutPanel1, textBoxUPC);
            _posHeaderControl.SettingsRequested   += (_, _) => buttonSetting_Click(this, EventArgs.Empty);
            _posHeaderControl.DailyCloseRequested += (_, _) => labelCashier_ClickInternal();
            _posHeaderControl.InvoiceRequested    += (_, _) => _windowService.OpenPrintInvoice(this);
            _posHeaderControl.LogoutRequested     += async (_, _) => await EjecutarLogout();
            _posHeaderControl.ExitRequested       += (_, _) => Close(); // triggers frmHome_FormClosing → PIN check → Application.Exit()

            // ── Cart list ─────────────────────────────────────────────────────
            _cartListViewControl = CartListViewControl.Attach(tableLayoutPanelCart, listViewCart);

            // ── Cart totals card — fully self-contained; inserts itself at row 1 ──
            _cartTotalsControl = CartTotalsControl.Attach(tableLayoutPanelCart, column: 0, row: 1);

            // ── Payment panel — self-contained code-only replacement ──────────
            // PaymentPanelControl.Attach() finds tableLayoutPanelPayment in
            // tableLayoutPanelMain, removes and disposes it, then inserts itself
            // at the same grid position. No Designer controls are needed.
            _paymentPanelControl = PaymentPanelControl.Attach(tableLayoutPanelMain, tableLayoutPanelPayment);

            _paymentPanelControl.CashPayClicked     += buttonPayCash_Click;
            _paymentPanelControl.CreditPayClicked   += buttonPayCreditCard_Click;
            _paymentPanelControl.DebitPayClicked    += buttonPayDebitCard_Click;
            _paymentPanelControl.EBTFoodClicked     += buttonEBTFood_Click;
            _paymentPanelControl.EBTBalanceClicked  += buttonEBTBalance_Click;
            _paymentPanelControl.SplitPayClicked    += buttonSplit_Click;
            _paymentPanelControl.GiftCardClicked    += buttonGiftCard_Click;
            _paymentPanelControl.OpenDrawerClicked  += buttonOpenDrawer_Click;
            _paymentPanelControl.QuickSaleClicked   += buttonProductNoTax_Click;
            _paymentPanelControl.UPCLookupClicked   += buttonLookup_Click;
            _paymentPanelControl.ScalePictureClicked+= pictureBoxPesado_Click;
            _paymentPanelControl.NumpadValueChanged += (_, _) => DisplayTotales();
        }

        private void ConfigureUI()
        {
            // ═══════════════════════════════════════════════════════════
            // GRUPO 2 — Acciones del carrito
            // ═══════════════════════════════════════════════════════════
            ElegantButtonStyles.Style(buttonCancelOrder,    ElegantButtonStyles.AlertRed,     fontSize: 15f);
            ElegantButtonStyles.Style(buttonChangeQuantity, ElegantButtonStyles.WarningOrange, fontSize: 15f);
            ElegantButtonStyles.Style(buttonDeleteItem,     ElegantButtonStyles.AlertRed,     fontSize: 15f);
            ElegantButtonStyles.Style(buttonHold,           ElegantButtonStyles.WarningOrange, fontSize: 15f);

            // Initial state — all action buttons disabled until cart has items
            RefreshCartButtons(hasItems: false, hasSelection: false);

            // Update button states on every selection change in the cart list
            listViewCart.SelectedIndexChanged += (_, _) =>
                RefreshCartButtons(
                    hasItems:     _shoppingCart.ItemCount > 0,
                    hasSelection: listViewCart.SelectedItems.Count > 0);

            // Payment buttons, Quick Sale, Lookup UPC and Scale are now
            // owned and styled entirely by PaymentPanelControl.

            ConfigureProductColumn();
        }

        /// <summary>
        /// Enables or disables the cart action buttons based on current cart state.
        /// Called on cart change and on list selection change.
        /// </summary>
        private void RefreshCartButtons(bool hasItems, bool hasSelection)
        {
            buttonCancelOrder.Enabled    = hasItems;
            buttonChangeQuantity.Enabled = hasSelection;
            buttonDeleteItem.Enabled     = hasSelection;
            // Hold is always available — can hold even an empty cart to resume later
        }

        // ── Columna central — fondo claro neutro para el área de productos
        private static readonly Color _productsBg = AppColors.BackgroundSecondary;

        private void ConfigureProductColumn()
        {
            var tab = tabControlMenuCategories;

            tab.Font     = new Font("Segoe UI", 13F, FontStyle.Bold);
            tab.DrawMode = TabDrawMode.OwnerDrawFixed;
            tab.SizeMode = TabSizeMode.FillToRight;
            tab.ItemSize = new Size(0, 54);
            tab.Padding  = new Point(18, 14);
            tab.DrawItem += TabControl_DrawItem;

            ApplyProductColumnColors();
        }

        private void ApplyProductColumnColors()
        {
            tableLayoutPanelCategoria.BackColor = _productsBg;
            tableLayoutPanelCategoria.Padding   = new Padding(0);
            tabControlMenuCategories.BackColor  = _productsBg;

            foreach (TabPage tab in tabControlMenuCategories.TabPages)
            {
                tab.BackColor = _productsBg;
                if (tab.Controls.Count > 0 && tab.Controls[0] is FlowLayoutPanel flp)
                    flp.BackColor = _productsBg;
            }
        }

        // Fonts cacheados para el TabControl y FlowLayoutPanels — se crean una sola vez
        private static readonly Font _tabFontSelected = new Font("Segoe UI", 12F, FontStyle.Bold);
        private static readonly Font _tabFontNormal   = new Font("Segoe UI", 12F, FontStyle.Regular);
        private static readonly Font _tabFlowFont     = new Font("Segoe UI", 11F, FontStyle.Regular);

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
            Color back = isSelected ? AppColors.NavyLight : AppColors.SurfaceMuted;
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
                using var sep = new Pen(AppBorders.SeparatorOnDark, 1);
                g.DrawLine(sep, bounds.Right - 1, bounds.Y + 6, bounds.Right - 1, bounds.Bottom - 6);
            }

            // ── Texto — reutiliza fonts cacheados ─────────────────────────
            Color fore     = isSelected ? AppColors.TextWhite : AppColors.SlateBlue;
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
                Invoke(new Action(() => { UpdateCartDisplay(); CheckAgeVerificationValidity(); }));
                return;
            }
            UpdateCartDisplay();
            CheckAgeVerificationValidity();
        }

        public void UpdateCartDisplay()
        {
            var result = _cartListViewControl?.UpdateCartItems(_shoppingCart.Items);
            if (result == null) return;

            totalGlobal = result.Total;
            _cartTotalsControl?.UpdateTotals(result.SubTotal, result.TaxTotal, result.Total);

            // Sync cart action button states
            RefreshCartButtons(
                hasItems:     _shoppingCart.ItemCount > 0,
                hasSelection: listViewCart.SelectedItems.Count > 0);
        }

        // ═══════════════════════════════════════════════════════════════
        // TEMA VISUAL — Triband Layout
        // ═══════════════════════════════════════════════════════════════
        private void AplicarEstiloVisual()
        {
            MaintableLayout.BackColor = AppColors.BackgroundPrimary;
            MaintableLayout.Padding = new Padding(0);
            MaintableLayout.Margin = new Padding(0);

            ApplyProductColumnColors();
            _paymentPanelControl?.ApplyTheme();
            EstilizarSeparadoresColumnas();

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
            _isLoggingOut      = true;    // Tell FormClosing NOT to exit the app.
            _windowService.OpenSignIn();
            this.Close();
        }

        // EstilizarColumnaProductos merged into ApplyProductColumnColors() above.

        // ── Separadores entre columnas via fondo del contenedor padre ────
        private void EstilizarSeparadoresColumnas()
        {
            // Separador sutil entre columnas — gris claro neutro
            tableLayoutPanelMain.BackColor = AppColors.SurfaceMuted;
            tableLayoutPanelMain.Padding   = new Padding(0);
            tableLayoutPanelMain.Margin    = new Padding(0);

            MaintableLayout.Padding = new Padding(4, 0, 4, 4);
        }

        private async void frmHome_Load(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // Show the customer display on the secondary screen (LED 1024×768).
                if (!_customerScreen.Visible)
                {
                    _customerScreen.PositionOnSecondaryScreen();
                    _customerScreen.Show();
                }

                AplicarEstiloVisual();

                // Fire the 3 independent data calls in parallel — cuts sequential latency by ~2/3.
                // LoadTabInfoAsync must wait because it builds tabs from MenuCategories + Categories.
                await Task.WhenAll(
                    LoadMenuCategoriesAsync(),
                    LoadCategoriesAsync(),
                    LoadLastInvoiceAsync()
                );

                // CRÍTICO: await explícito para evitar race condition con UpdateProductsDisplay
                await LoadTabInfoAsync();

                await _shoppingCart.LoadCartAsync();

                _posHeaderControl?.UpdateCashier(SessionManager.Name ?? "");

                await ActualizarEstadoBotonHold();

                // Defer OPOS init until AFTER the form is fully painted.
                // BeginInvoke posts to the message queue — the form renders first,
                // then InitializeDevices() runs on the UI thread (STA), which is the same
                // apartment where the COM objects live. No cross-apartment marshaling.
                BeginInvoke(InitializeDevices);
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

        /// <summary>
        /// Called via BeginInvoke from frmHome_Load — runs on the UI thread (STA) after
        /// the form has been fully painted. OPOS COM objects were created on the UI thread
        /// in the ZebraScannerService constructor, so Initialize() runs in the correct apartment.
        /// DataEvent callbacks fire on Zebra's OPOS thread; InvokeRequired dispatches them to UI.
        /// </summary>
        private void InitializeDevices()
        {
            if (_zebraScannerService == null) return;

            _zebraScannerService.Initialize();

            if (!_zebraScannerService.IsConnected)
                ShowScannerBanner();

            if (!_zebraScannerService.IsScaleConnected)
                ShowScaleBanner();
        }

        // _supervisorApproved: set by EjecutarLogout() so FormClosing skips the PIN prompt.
        // _isLoggingOut: set by EjecutarLogout() so FormClosing knows it's a session
        //                switch (not a full application exit).
        private bool _supervisorApproved = false;
        private bool _isLoggingOut       = false;

        private void frmHome_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Block any user-initiated close (Alt+F4, task-bar close, EXIT button, etc.)
            // unless the supervisor PIN was already verified.
            if (!_supervisorApproved && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                if (!VerificarPinSupervisor()) return;

                // PIN accepted for EXIT — set flag and re-trigger close so the
                // cleanup block below runs, then Application.Exit() is called.
                _supervisorApproved = true;
                Close();
                return;
            }

            // ── Customer screen ──────────────────────────────────────────────
            // On logout: hide the customer screen so it's ready for the next
            // session (the Singleton must NOT be disposed or Close()'d here).
            // On full exit: close it so it disappears before the process ends.
            if (!_customerScreen.IsDisposed)
            {
                if (_isLoggingOut)
                    _customerScreen.Hide();
                else
                    _customerScreen.Close();
            }

            // ── Close any open modeless child forms (Hold, Split, KeyLookup…) ─
            _windowService.CloseAllModeless();

            // ── Unsubscribe per-session event handlers ───────────────────────
            _shoppingCart.CartChanged -= ShoppingCart_CartChanged;

            if (_zebraScannerService != null)
            {
                _zebraScannerService.OnBarcodeDataReceived -= _zebraScannerService_OnBarcodeDataReceived;
                _zebraScannerService.OnWeightUpdated       -= _zebraScannerService_OnWeightUpdated;
                _zebraScannerService.Close();
            }

            _homeInteractionService.ClearHandlers();

            // Cancelar cargas en vuelo y limpiar caché de productos al cerrar sesión
            _productLoadCts?.Cancel();
            _productLoadCts?.Dispose();
            _homeInitializationService.ClearProductCache();

            _scaleBanner?.Dispose();
            _scaleBanner    = null;
            _scaleBannerBtn = null;

            // On full exit (not a logout) tear down the app cleanly.
            if (!_isLoggingOut)
                Application.Exit();
        }

        // CRÍTICO: async Task para poder ser awaited — evita race condition con UpdateProductsDisplay
        private async Task LoadTabInfoAsync()
        {
            tabControlMenuCategories.TabPages.Clear();

            var productsBg = AppColors.BackgroundSecondary;

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
                    Font         = _tabFlowFont,
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
            // Cancelar cualquier carga anterior en vuelo si el usuario cambia de pestaña rápido
            _productLoadCts?.Cancel();
            _productLoadCts?.Dispose();
            _productLoadCts = new CancellationTokenSource();
            var ct = _productLoadCts.Token;

            try
            {
                _selectedTab = tabControlMenuCategories.SelectedTab;

                if (_selectedTab?.Tag == null) return;

                var categoryIds = Array.ConvertAll(_selectedTab.Tag.ToString()!.Split(','), int.Parse);

                // Solo mostrar "Cargando..." si la categoría NO está en caché (primera visita)
                bool needsNetwork = !_homeInitializationService.IsCached(categoryIds);
                if (needsNetwork)
                    ShowLoadingInCurrentTab();

                await LoadProductsAsync(categoryIds, ct: ct);

                if (!ct.IsCancellationRequested)
                    UpdateProductsDisplay();
            }
            catch (OperationCanceledException)
            {
                // Normal — el usuario cambió de pestaña antes de que terminara la carga
            }
            catch (Exception ex)
            {
                if (!ct.IsCancellationRequested)
                    MessageBox.Show($"Error loading products: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ShowLoadingInCurrentTab()
        {
            if (_selectedTab?.Controls[0] is not FlowLayoutPanel flow) return;

            foreach (Control c in flow.Controls) c.Dispose();
            flow.Controls.Clear();

            flow.Controls.Add(new Label
            {
                Text      = "Cargando productos...",
                Font      = new Font("Segoe UI", 13f, FontStyle.Regular),
                ForeColor = AppColors.TextMuted,
                AutoSize  = false,
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
            });
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
            if (_selectedTab?.Controls[0] is not FlowLayoutPanel flowLayoutPanel) return;

            // Dispose explícito de cada control antes de removerlo.
            // ControlCollection.Clear() NO llama Dispose — los ProductImageControl
            // descartados retendrían HWNDs, Regions y Fonts activos.
            foreach (Control c in flowLayoutPanel.Controls)
                c.Dispose();

            flowLayoutPanel.Controls.Clear();

            // Renderizar solo la primera página para que la UI responda rápido.
            // Si hay más productos, se muestra un botón "Ver más".
            var page = Products.Take(ProductPageSize).ToList();

            flowLayoutPanel.SuspendLayout();

            foreach (var product in page)
                flowLayoutPanel.Controls.Add(BuildProductControl(product));

            if (Products.Count > ProductPageSize)
                flowLayoutPanel.Controls.Add(BuildVerMasButton(flowLayoutPanel, ProductPageSize));

            flowLayoutPanel.ResumeLayout();
        }

        private ProductImageControl BuildProductControl(ProductModel product)
        {
            var ctrl = new ProductImageControl(product);
            ctrl.ProductClicked += (_, _) =>
            {
                try
                {
                    var selection = _barcodeSaleService.HandleProductSelection(product, Categories);

                    if (selection.IsWeighted)
                    {
                        _paymentPanelControl?.SetScaleProduct(
                            selection.WeightDisplayText ?? string.Empty,
                            selection.ProductIdText     ?? string.Empty,
                            selection.ImageLocation     ?? string.Empty);
                    }
                    else if (selection.AddedToCart)
                    {
                        _posHeaderControl?.UpdateProductName(selection.ProductName ?? string.Empty);
                        UpdateCartDisplay();
                    }
                }
                catch
                {
                    MessageBox.Show("Error adding product to cart. Please try again.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            return ctrl;
        }

        private Button BuildVerMasButton(FlowLayoutPanel flow, int alreadyShown)
        {
            var remaining = Products.Count - alreadyShown;
            var btn = new Button
            {
                Text      = $"Ver {remaining} productos más ▼",
                Height    = 48,
                Width     = flow.ClientSize.Width - 24,
                FlatStyle = FlatStyle.Flat,
                BackColor = AppColors.SurfaceCard,
                ForeColor = AppColors.TextSecondary,
                Font      = new Font("Segoe UI", 11f, FontStyle.Regular),
                Margin    = new Padding(4),
                Cursor    = Cursors.Default,
            };
            btn.FlatAppearance.BorderColor        = AppColors.SurfaceMuted;
            btn.FlatAppearance.MouseOverBackColor  = Color.Transparent;
            btn.FlatAppearance.MouseDownBackColor  = Color.Transparent;

            btn.Click += (_, _) =>
            {
                btn.Dispose();
                flow.SuspendLayout();
                foreach (var product in Products.Skip(alreadyShown))
                    flow.Controls.Add(BuildProductControl(product));
                flow.ResumeLayout();
            };
            return btn;
        }

        private void DisplayTotales()
        {
            if (_paymentPanelControl == null) return;
            var tendered   = _paymentPanelControl.ValueDecimal;
            changeValue    = tendered - totalGlobal;
            var safeChange = changeValue > 0 ? changeValue : 0.0m;
            _paymentPanelControl.UpdatePaymentValues(tendered, safeChange);
        }

        private void ClearTotales(bool all = false)
        {
            _paymentPanelControl?.Reset();
            changeValue = 0;
            _cartTotalsControl?.ResetTotals(includeGrandTotal: all);
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
            // OPOS callbacks arrive on a background thread — marshal all UI work to the UI thread.
            if (InvokeRequired)
            {
                Invoke(() => _zebraScannerService_OnWeightUpdated(weightStatus, w));
                return;
            }
            weight                = w;
            SharedData.WeightUnit = weightStatus;
            _paymentPanelControl?.SetScaleWeight(weightStatus);
        }

        private async void textBoxUPC_TextChanged(object sender, EventArgs e)
        {
            await SearchProduct(textBoxUPC.Text);
            textBoxUPC.Focus();
        }

        private void buttonCancelOrder_Click(object sender, EventArgs e)
        {
            if (_shoppingCart.ItemCount == 0) return;

            var confirm = MessageBox.Show(
                "Are you sure you want to cancel this order?\nAll items will be removed.",
                "Cancel Order",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (confirm != DialogResult.Yes) return;

            _currentAgeVerification = null;
            UpdateAgeVerificationBadge();

            _shoppingCart.Clear();
            _paymentSplitService.Clear();

            UpdateCartDisplay();
            ClearTotales(true);
        }

        private void buttonChangeQuantity_Click(object sender, EventArgs e)
        {
            if (listViewCart.SelectedItems.Count == 0) return;

            var selectedItem = listViewCart.SelectedItems[0];
            if (!int.TryParse(selectedItem.Tag?.ToString(), out int productId))
            {
                MessageBox.Show("Invalid item selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _windowService.OpenPopupQuantity(0, productId, this);
        }

        private void buttonDeleteItem_Click(object sender, EventArgs e)
        {
            if (listViewCart.SelectedItems.Count == 0) return;

            var selectedItem = listViewCart.SelectedItems[0];
            if (!int.TryParse(selectedItem.Tag?.ToString(), out int productId))
            {
                MessageBox.Show("Invalid item selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _shoppingCart.RemoveItem(productId);
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

        private async void buttonSplit_Click(object sender, EventArgs e)
        {
            if (_shoppingCart.ItemCount > 0 && totalGlobal > 0)
            {
                if (!await EnsureAgeVerificationAsync()) return;
                _windowService.OpenSplitPayment(this);
            }
        }

        private async void buttonEBTBalance_Click(object sender, EventArgs e) => await PaymentOrder(PaymentType.EBTBalance);

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
            if (!await EnsureAgeVerificationAsync()) return;

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
                if (!await EnsureAgeVerificationAsync()) return;

                var result = await _paymentCoordinatorService.ProcessCashSaleAsync(totalGlobal, (int)(_paymentPanelControl?.ValueCents ?? 0L), changeValue, false);

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

        async Task PaymentSummary(int oId, int consecutivo,
            OmadaPOS.Libreria.Models.PaymentResponseModel? paymentResponse = null,
            List<OmadaPOS.Libreria.Models.PaymentModel>?   splitPayments   = null)
        {
            // Capturar el cambio ANTES de ClearTotales() que lo resetea a 0.
            var devuelta = changeValue > 0 ? changeValue : 0m;

            _currentAgeVerification = null;
            UpdateAgeVerificationBadge();

            _shoppingCart.Clear();
            _paymentSplitService.Clear();

            UpdateCartDisplay();
            ClearTotales(true);

            await LoadLastInvoiceAsync();

            _windowService.OpenPopupCashPayment(oId, consecutivo, devuelta, this, paymentResponse, splitPayments);
        }

        private void buttonGiftCard_Click(object sender, EventArgs e)
            => _windowService.OpenGiftCard(totalGlobal, 1, this);

        private async void buttonPayCreditCard_Click(object sender, EventArgs e) => await PaymentOrder(PaymentType.Credit);

        private async void buttonPayDebitCard_Click(object sender, EventArgs e) => await PaymentOrder(PaymentType.Debit);

        private async void buttonEBTFood_Click(object sender, EventArgs e) => await PaymentOrder(PaymentType.EBT);

        private async Task PaymentOrder(PaymentType paymentType)
        {
            // EBT Balance is a read-only balance inquiry — no product sale, no age gate
            if (paymentType != PaymentType.EBTBalance && !await EnsureAgeVerificationAsync())
                return;

            // Show full-screen overlay while the PAX terminal processes the transaction.
            // The try/finally guarantees the overlay always closes — even on exception.
            var waiting = new frmPaymentWaiting(totalGlobal, paymentType);
            waiting.Show(this);   // non-blocking; owner = this (covers frmHome)

            try
            {
                var result = await _paymentCoordinatorService.ProcessTerminalPaymentAsync(paymentType, totalGlobal, false);

                // Close overlay before showing result popups so they render on top cleanly.
                waiting.Dispose();

                if (result.PaymentResponse != null && !result.PaymentResponse.Success)
                {
                    _windowService.OpenPaymentStatus(result.PaymentResponse.MsgInfo ?? "Payment declined", this);
                }

                if (result.OrderResponse != null)
                {
                    await PaymentSummary(
                        result.OrderResponse.Order_Id,
                        result.OrderResponse.Consecutivo,
                        result.PaymentResponse);
                }
            }
            catch (Exception ex)
            {
                waiting.Dispose();
                MessageBox.Show($"Payment error: {ex.Message}", "Payment Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Resolves a scanned UPC and adds the product to the cart.
        /// Barcode parsing and API lookup delegated to ProductApplicationService.
        /// </summary>
        public async Task SearchProduct(string upc)
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
                _posHeaderControl?.UpdateProductName(result.ProductName ?? string.Empty);
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


        public async Task<int> LoadLastInvoiceAsync() =>
            await _homeInitializationService.LoadLastInvoiceAsync();

        public async Task LoadCategoriesAsync()
            => await _homeInitializationService.LoadCategoriesAsync(Categories);

        public async Task LoadMenuCategoriesAsync()
            => await _homeInitializationService.LoadMenuCategoriesAsync(MenuCategories);

        public async Task LoadProductsAsync(int[] categoryIds, string? searchLetter = null, CancellationToken ct = default)
            => await _homeInitializationService.LoadProductsAsync(Products, categoryIds, searchLetter, ct);

        private async Task ProcessPaymentMultipleAsync()
        {
            // Capture split payments BEFORE ProcessMultiplePaymentsAsync so we
            // have them available for the receipt even after the session is cleared.
            var splitPayments = await _paymentSplitService.GetSessionPaymentsAsync();

            var orderResponse = await _paymentCoordinatorService.ProcessMultiplePaymentsAsync(changeValue, false);

            if (orderResponse != null)
            {
                await PaymentSummary(orderResponse.Order_Id, orderResponse.Consecutivo,
                    splitPayments: splitPayments.Count > 0 ? splitPayments : null);
            }
        }

        private async void pictureBoxPesado_Click(object sender, EventArgs e)
        {
            try
            {
                if (_paymentPanelControl == null) return;
                if (_paymentPanelControl.ScaleProductDisplayText == "" || weight <= 0) return;

                if (!string.IsNullOrEmpty(_paymentPanelControl.ScaleProductId) &&
                    int.TryParse(_paymentPanelControl.ScaleProductId, out int productId))
                {
                    var result    = await _barcodeSaleService.AddWeightedProductAsync(productId, weight);

                    if (result.AddedToCart)
                    {
                        _posHeaderControl?.UpdateProductName(result.ProductName ?? string.Empty);
                        UpdateCartDisplay();
                        weight = 0.0;
                        _paymentPanelControl.ClearScaleProduct();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding weighted product: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Age verification integration ───────────────────────────────────────

        /// <summary>
        /// Creates the age-verification status badge and attaches it to the bottom
        /// of the cart totals control so it appears directly beneath the subtotal/tax/total card.
        /// </summary>
        private void SetupAgeVerificationBadge()
        {
            _labelAgeVerificationStatus = new Label
            {
                Dock      = DockStyle.Bottom,
                Height    = 36,
                Font      = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(55, 65, 81),
                TextAlign = ContentAlignment.MiddleCenter,
                Visible   = false,
                Padding   = new Padding(4, 0, 4, 0),
            };

            _cartTotalsControl?.Controls.Add(_labelAgeVerificationStatus);
        }

        /// <summary>
        /// Returns true if any cart item requires age verification — checks both the
        /// stored flag and a real-time lookup in the restriction config (safety net for
        /// items whose flag was incorrectly set to false at add-time due to UPC mismatch).
        /// </summary>
        private bool CartHasAgeRestrictedItem()
            => _ageVerificationService.RequiresVerification(_shoppingCart.Items)
               || _shoppingCart.Items.Any(i => _ageRestrictionConfig.IsRestricted(i.UPC, 0));

        /// <summary>
        /// Updates the badge text and color based on the current verification state.
        /// Shows a pending warning as soon as a restricted item is in the cart.
        /// </summary>
        private void UpdateAgeVerificationBadge()
        {
            if (!CartHasAgeRestrictedItem())
            {
                _labelAgeVerificationStatus.Visible = false;
                return;
            }

            _labelAgeVerificationStatus.Visible = true;

            if (_currentAgeVerification == null)
            {
                _labelAgeVerificationStatus.BackColor = Color.FromArgb(180, 83, 9);
                _labelAgeVerificationStatus.Text      = "⚠️  Age Verification Required";
                return;
            }

            switch (_currentAgeVerification.Status)
            {
                case AgeVerificationStatus.Approved:
                    _labelAgeVerificationStatus.BackColor = Color.FromArgb(16, 120, 70);
                    _labelAgeVerificationStatus.Text      = "✅  Age Verified";
                    break;
                case AgeVerificationStatus.Denied:
                    _labelAgeVerificationStatus.BackColor = Color.FromArgb(185, 28, 28);
                    _labelAgeVerificationStatus.Text      = "❌  Sale Denied";
                    break;
                default:
                    _labelAgeVerificationStatus.BackColor = Color.FromArgb(180, 83, 9);
                    _labelAgeVerificationStatus.Text      = "⚠️  Age Verification Required";
                    break;
            }
        }

        /// <summary>
        /// Called on every cart change. Clears verification if no restricted items remain.
        /// </summary>
        private void CheckAgeVerificationValidity()
        {
            if (_currentAgeVerification != null && !CartHasAgeRestrictedItem())
                _currentAgeVerification = null;

            UpdateAgeVerificationBadge();
        }

        /// <summary>
        /// Returns true when the cart may proceed to payment.
        /// Shows <see cref="frmAgeVerification"/> when verification is needed.
        /// Temporarily removes the home form's scanner subscription so the ID barcode
        /// is routed to the verification form rather than to product lookup.
        /// </summary>
        private async Task<bool> EnsureAgeVerificationAsync()
        {
            if (!CartHasAgeRestrictedItem())
                return true;

            if (_currentAgeVerification?.Status == AgeVerificationStatus.Approved)
                return true;

            // Hand the scanner to frmAgeVerification for the duration of the dialog
            if (_zebraScannerService != null)
                _zebraScannerService.OnBarcodeDataReceived -= _zebraScannerService_OnBarcodeDataReceived;

            try
            {
                using var dlg = new frmAgeVerification(_ageVerificationService, _zebraScannerService);
                var dr = dlg.ShowDialog(this);

                if (dr != DialogResult.OK)
                    return false;

                _currentAgeVerification = dlg.VerificationResult;

                if (_currentAgeVerification?.Status == AgeVerificationStatus.Denied)
                {
                    MessageBox.Show(
                        $"Sale denied. {_currentAgeVerification.DenialReason}",
                        "Age Verification — Sale Denied",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    UpdateAgeVerificationBadge();
                    return false;
                }

                // Persist audit record (fire-and-log, never throws)
                if (_currentAgeVerification != null)
                {
                    await _ageVerificationService.SaveAuditAsync(new AgeVerificationAuditRecord
                    {
                        SessionId          = _shoppingCart.MachineGuid,
                        CashierName        = SessionManager.UserName ?? string.Empty,
                        VerifiedAt         = DateTime.Now,
                        VerificationMethod = _currentAgeVerification.Method.ToString(),
                        VerificationResult = _currentAgeVerification.Status.ToString(),
                        CustomerIs21OrOver = _currentAgeVerification.Status == AgeVerificationStatus.Approved,
                        IdType             = _currentAgeVerification.IdType,
                        IdLast4OrToken     = _currentAgeVerification.IdLast4OrToken,
                        DenialReason       = _currentAgeVerification.DenialReason,
                    });
                }

                UpdateAgeVerificationBadge();
                return _currentAgeVerification?.Status == AgeVerificationStatus.Approved;
            }
            finally
            {
                if (_zebraScannerService != null)
                    _zebraScannerService.OnBarcodeDataReceived += _zebraScannerService_OnBarcodeDataReceived;
            }
        }


    }
}
