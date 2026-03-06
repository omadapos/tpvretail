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
            ElegantButtonStyles.Style(buttonInvoice, ElegantButtonStyles.HeaderNavy, fontSize: 18f);
            ElegantButtonStyles.Style(ButtonSettings, ElegantButtonStyles.HeaderNavy, fontSize: 18f);
            ElegantButtonStyles.Style(DualScreenButton, ElegantButtonStyles.HeaderNavy, fontSize: 18f);
            ElegantButtonStyles.Style(labelCashier, ElegantButtonStyles.HeaderNavy, fontSize: 18f);
            ElegantButtonStyles.Style(buttonClose, ElegantButtonStyles.AlertRed, fontSize: 18f);
            ElegantButtonStyles.Style(buttonCheckPrice, ElegantButtonStyles.AlertRed, fontSize: 18f);

            ElegantButtonStyles.Style(buttonHold, ElegantButtonStyles.DebitGray, fontSize: 18f);
            ElegantButtonStyles.Style(buttonCancelOrder, ElegantButtonStyles.AlertRed, fontSize: 18f);
            ElegantButtonStyles.Style(buttonChangeQuantity, ElegantButtonStyles.WarningOrange, fontSize: 18f);
            ElegantButtonStyles.Style(buttonDeleteItem, ElegantButtonStyles.AlertRed, fontSize: 18f);

            ElegantButtonStyles.Style(buttonQsale, ElegantButtonStyles.HeaderNavy, fontSize: 18f);
            ElegantButtonStyles.Style(buttonLookup, ElegantButtonStyles.HeaderNavy, fontSize: 18f);

            ElegantButtonStyles.Style(buttonEBTBalance, ElegantButtonStyles.EBTBalanceOrange, fontSize: 18f);
            ElegantButtonStyles.Style(buttonEBTFood, ElegantButtonStyles.EBTOrange, fontSize: 18f);
            ElegantButtonStyles.Style(buttonGiftCard, ElegantButtonStyles.GiftPurple, fontSize: 18f);
            ElegantButtonStyles.Style(buttonOpenDrawer, ElegantButtonStyles.HeaderNavy, fontSize: 18f);
            ElegantButtonStyles.Style(buttonPayCash, ElegantButtonStyles.CashGreen, fontSize: 24f);
            ElegantButtonStyles.Style(buttonPayCreditCard, ElegantButtonStyles.CreditBlue, fontSize: 18f);
            ElegantButtonStyles.Style(buttonPayDebitCard, ElegantButtonStyles.DebitGray, fontSize: 18f);
            ElegantButtonStyles.Style(buttonSplit, ElegantButtonStyles.HeaderNavy, fontSize: 18f);

            ElegantButtonStyles.StyleSplitContainer(splitContainerScale);
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
            listViewCart.Font = new Font("Montserrat", 18F, FontStyle.Regular);

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

        private async void frmHome_Load(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                await LoadMenuCategoriesAsync();

                await LoadCategoriesAsync();

                orderId = await LoadLastInvoiceAsync();

                LoadTabInfo();

                UpdateProductsDisplay();

                await _shoppingCart.LoadCartAsync();

                buttonInvoice.Text = $"Last One : {orderId}";
                labelCashier.Text = SessionManager.Name;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing application. Please try again.", "Error",
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

        private async void LoadTabInfo()
        {
            tabControlMenuCategories.TabPages.Clear();

            string firstName = "";
            foreach (var menuCategory in MenuCategories)
            {
                var tabName = menuCategory.Name;

                if (string.IsNullOrEmpty(firstName))
                {
                    firstName = tabName;
                }

                TabPage tabMenu = new TabPage(tabName)
                {
                    Name = $"tab{tabName}",
                    UseVisualStyleBackColor = true,
                    BackColor = Color.White,
                    Padding = new Padding(10),
                };

                int[] listCategoriesId = Categories.Where(c => c.Tipo == tabName).Select(c => c.Id).ToArray();

                if (listCategoriesId.Length > 0)
                {
                    tabMenu.Tag = string.Join(",", listCategoriesId);
                }

                FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel
                {
                    BackColor = Color.White,
                    Name = $"flowLayoutPanel{tabName}",
                    Dock = DockStyle.Fill,
                    WrapContents = true,
                    AutoScroll = true
                };

                flowLayoutPanel.ControlAdded += (sender, e) =>
                {
                    Control control = e.Control;
                    control.Left = (flowLayoutPanel.ClientSize.Width - control.Width) / 2;
                };

                Font flowLayoutPanelFont = new Font("Arial", 18, FontStyle.Regular);
                flowLayoutPanel.Font = flowLayoutPanelFont;

                Controls.Add(flowLayoutPanel);

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
                    try
                    {
                        string numTemp = inputValue.ToString();
                        numTemp += tag;
                        int valNum = int.Parse(numTemp);
                        inputValue = valNum;
                    }
                    catch { }

                    DisplayTotales();
                    break;

                case "1000":
                case "2000":
                case "5000":
                case "10000":
                    try
                    {
                        int valNum = int.Parse(tag);
                        inputValue = valNum;
                    }
                    catch { }

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

        private void buttonClearCode_Click(object sender, EventArgs e)
        {
            textBoxUPC.Text = "";
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
            frm.Show(this);
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

        private async void buttonLogout_Click(object sender, EventArgs e)
        {
            try
            {
                await _userService.Logout(new LogDTO
                {
                    AdminId = SessionManager.AdminId ?? 0,
                    Phone = SessionManager.Phone
                });

                frmSignIn login = new frmSignIn();
                login.Show();

                this.Close();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

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

            await LoadLastInvoiceAsync();

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

        private void ButtonSettings_Click(object sender, EventArgs e)
        {
            var settingForm = new frmSetting();
            settingForm.ShowDialog();
        }

        public async Task<int> LoadLastInvoiceAsync()
        {
            try
            {
                return await _orderService.LoadLastInvoiceAdmin();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task LoadCategoriesAsync()
        {
            try
            {
                var categories = await _categoryService.LoadCategories();

                Categories.Clear();
                foreach (var category in categories)
                {
                    Categories.Add(category);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task LoadMenuCategoriesAsync()
        {
            try
            {
                var menuCategories = await _categoryService.LoadMenuCategories();

                MenuCategories.Clear();

                foreach (var menuCategory in menuCategories)
                {
                    MenuCategories.Add(menuCategory);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task LoadProductsAsync(int[] categoryIds, string? searchLetter = null)
        {
            try
            {
                var products = string.IsNullOrEmpty(searchLetter)
                    ? await _categoryService.LoadProductIdCategories(new IdCategoryDTO { Ids = categoryIds })
                    : await _categoryService.LoadProductsByCategoryLetra(new IdCategoryDTO { Ids = categoryIds }, searchLetter);

                Products.Clear();
                foreach (var product in products)
                {
                    Products.Add(product);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<PaymentResponseModel> ProcessPaymentAsync(string paymentType, decimal amount)
        {
            try
            {
                var config = await _adminSettingService.LoadSettingById(WindowsIdProvider.GetMachineGuid());
                if (config == null)
                {
                    throw new InvalidOperationException("Payment terminal configuration not found");
                }

                var request = new PaymentRequest
                {
                    Ip = config.IP ?? string.Empty,
                    Port = config.Port ?? 0,
                    Amount = amount * 100,
                    EcrRefNumber = (await _orderService.LoadLastConsecutivoPayment()).ToString()
                };

                return paymentType switch
                {
                    "CREDIT_CARD" => await _paymentService.ProcessPaymentAsync(PaymentType.Credit, request),
                    "DEBIT_CARD" => await _paymentService.ProcessPaymentAsync(PaymentType.Debit, request),
                    "EBT" => await _paymentService.ProcessPaymentAsync(PaymentType.EBT, request),
                    "EBT_BALANCE" => await _paymentService.GetEBTBalanceAsync(request),
                    _ => throw new ArgumentException($"Unsupported payment type: {paymentType}")
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async void ProcessPaymentMultipleAsync()
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
            try
            {
                var config = await _adminSettingService.LoadSettingById(WindowsIdProvider.GetMachineGuid());
                var terminal = config?.Terminal ?? string.Empty;

                var orderModel = _orderApplicationService.BuildOrderModel(
                    _shoppingCart.Items,
                    changeAmount,
                    terminal,
                    paymentMethod,
                    0,
                    bDesc
                );

                if (orderModel == null)
                {
                    throw new InvalidOperationException("Failed to create order model");
                }

                return await _orderService.PlaceOrder(orderModel);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async void pictureBoxPesado_Click(object sender, EventArgs e)
        {
            if (labelPesaProduct.Text != "" && weight > 0)
            {
                weight = 1.2;

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
            var form = new frmCierreDiario();
            form.ShowDialog(this);
        }
    }
}
