using Microsoft.Extensions.Logging;
using OmadaPOS.Componentes; // Asegúrate de tener RoundedPanel.cs en esta carpeta
using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Services;
using OmadaPOS.Libreria.Utils;
using OmadaPOS.Services;
using OmadaPOS.Services.Navigation;

namespace OmadaPOS.Views
{
    public partial class frmSplit : Form
    {
        private readonly IShoppingCart _shoppingCart;
        private readonly IPaymentSplitService _paymentSplitService;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IAdminSettingService _adminSettingService;
        private readonly IHomeInteractionService _homeInteractionService;

        private readonly ILogger<frmSplit> _logger;

        // Campo para almacenar el handler de CartChanged y poder desuscribirlo.
        // Una lambda anónima no puede desuscribirse si no se guarda la referencia.
        private EventHandler? _cartChangedHandler;

        decimal totalGlobal = 0;
        decimal remainingAmount = 0;
        decimal due = 0;
        int totalItemPayments = 0;

        private RoundedPanel panelSplitPay;
        private NumericPadControl _numPad = null!;

        public frmSplit(
            IShoppingCart shoppingCart,
            IPaymentSplitService paymentSplitService,
            IPaymentService paymentService,
            IOrderService orderService,
            IAdminSettingService adminSettingService,
            IHomeInteractionService homeInteractionService,
            ILogger<frmSplit> logger)
        {
            InitializeComponent();
            ConfigureCartListView();
            ConfigurePaymentListView();
            ConfigureUI();

            // ── Métodos de pago ───────────────────────────────────────
            ButtonVariants.PaymentCash(buttonCash,        fontSize: 24f);
            ButtonVariants.PaymentCredit(buttonCredit,    fontSize: 18f);
            ButtonVariants.PaymentDebit(buttonDebit,      fontSize: 18f);
            ButtonVariants.PaymentEBT(buttonEbt,          fontSize: 18f);

            // ── Acciones EBT ──────────────────────────────────────────
            ButtonVariants.EBTBalance(buttonEbtBalance,   fontSize: 18f);
            ButtonVariants.EBTBalance(buttonCalculateEBT, fontSize: 18f);

            // ── Confirmar pago ────────────────────────────────────────
            ButtonVariants.Primary(buttonPrintBill,       fontSize: 18f);

            // ── Cerrar ────────────────────────────────────────────────
            ButtonVariants.Danger(buttonClose,            fontSize: 18f);

            // ── Clear del teclado inline (ya no se usa, ocultamos) ───
            ButtonVariants.Keypad(buttonClear,            fontSize: 18f);

            _shoppingCart           = shoppingCart;
            _paymentSplitService    = paymentSplitService;
            _paymentService         = paymentService;
            _orderService           = orderService;
            _adminSettingService    = adminSettingService;
            _homeInteractionService = homeInteractionService;
            _logger                 = logger;

            ClearTotales();

            this.Load   += async (s, e) => await InitializeAsync();
            this.FormClosed += FrmSplit_FormClosed;
        }

        private void FrmSplit_FormClosed(object? sender, FormClosedEventArgs e)
        {
            // Desuscribir de IShoppingCart (Singleton) para que el GC pueda
            // recolectar esta instancia. Sin esto, cada apertura de frmSplit
            // acumula una instancia que nunca se libera.
            if (_cartChangedHandler != null)
            {
                _shoppingCart.CartChanged -= _cartChangedHandler;
                _cartChangedHandler = null;
            }
        }

        private void ConfigureUI()
        {
            this.WindowState     = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;

            panelSplitPay = new RoundedPanel
            {
                Location        = new Point(30, 500),
                Size            = new Size(400, 180),
                CornerRadius    = AppRadii.Dialog,
                BorderColor     = AppBorders.PanelLight,
                BackgroundStart = AppColors.BackgroundPrimary,
                BackgroundEnd   = AppColors.SurfaceMuted,
                ShadowColor     = AppShadows.Medium,
                Name            = "panelSplitPay"
            };
            this.Controls.Add(panelSplitPay);

            // ── Reemplazar tableLayoutPanel4 con NumericPadControl unificado ──
            _numPad = new NumericPadControl(NumericPadControl.PadMode.MoneyWithBills)
            {
                Dock = DockStyle.Fill
            };
            _numPad.ValueChanged += (_, _) => DisplayTotales();

            tableLayoutPanel3.Controls.Remove(tableLayoutPanel4);
            tableLayoutPanel3.Controls.Add(_numPad, 0, 1);

            // Ocultar fila de billetes inline (ahora incluidos en NumericPadControl)
            tableLayoutPanel5.Visible = false;

            // Estilizar botón "Remaining"
            ElegantButtonStyles.Style(buttonRemain, ElegantButtonStyles.SplitBlueLight, fontSize: 22f);
        }

        private void ConfigureCartListView()
        {
            listViewCart.View         = View.Details;
            listViewCart.FullRowSelect = true;
            listViewCart.GridLines    = true;
            listViewCart.MultiSelect  = false;

            listViewCart.Columns.Add("#",        80);
            listViewCart.Columns.Add("Product",  200);
            listViewCart.Columns.Add("Quantity", 80);
            listViewCart.Columns.Add("Price",    100);
            listViewCart.Columns.Add("Subtotal", 100);

            listViewCart.BackColor = AppColors.BackgroundSecondary;
            listViewCart.Font      = AppTypography.Body;
        }

        private void ConfigurePaymentListView()
        {
            listViewPayments.View         = View.Details;
            listViewPayments.FullRowSelect = true;
            listViewPayments.GridLines    = true;
            listViewPayments.MultiSelect  = false;

            listViewPayments.Columns.Add("Payment", 200);
            listViewPayments.Columns.Add("Total",   100);

            listViewPayments.BackColor = AppColors.BackgroundSecondary;
            listViewPayments.Font      = AppTypography.Body;
        }

        private async Task InitializeAsync()
        {
            try
            {
                await _shoppingCart.LoadCartAsync();

                LoadCartItems();

                await LoadPaymentsAsync();

                _cartChangedHandler = (s, e) => LoadCartItems();
                _shoppingCart.CartChanged += _cartChangedHandler;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing split payment form");
                MessageBox.Show($"Error al inicializar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadPaymentsAsync()
        {
            try
            {
                listViewPayments.Items.Clear();
                var payments = await _paymentSplitService.GetSessionPaymentsAsync();
                decimal totalPaid = 0;
                totalItemPayments = payments.Count;

                foreach (var payment in payments)
                {
                    var item = new ListViewItem(payment.PaymentType);
                    item.SubItems.Add(payment.Total.ToString("C"));
                    item.Tag = payment;
                    listViewPayments.Items.Add(item);
                    totalPaid += payment.Total;
                }

                remainingAmount = totalGlobal - totalPaid;

                DisplayTotales();

                buttonPrintBill.Enabled = remainingAmount <= 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading payments");
                MessageBox.Show("Error loading payments", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayTotales()
        {
            labelDueValue.Text     = due.ToString("C");
            labelChargedValue.Text = _numPad.ValueDecimal.ToString("C");
            buttonRemain.Text      = remainingAmount.ToString("C");
        }

        private async void PaymentButton_Click(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                try
                {
                    string paymentType = button.Tag?.ToString() ?? "Unknown";
                    decimal paymentAmount = _numPad.ValueDecimal;

                    if (paymentAmount <= 0 || totalGlobal == 0)
                    {
                        MessageBox.Show("No remaining amount to pay", "Information",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    if (paymentAmount > remainingAmount)
                    {
                        MessageBox.Show($"Payment amount exceeds remaining amount of {remainingAmount:C}",
                            "Invalid Amount", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (paymentType == "CASH")
                    {
                        await _paymentSplitService.CreatePaymentAsync(paymentType, paymentAmount);

                        await LoadPaymentsAsync();

                        ClearTotales();

                    }
                    else if (paymentType == "CREDIT" || paymentType == "DEBIT" || paymentType == "EBT")
                    {
                        var pType = PaymentType.Credit;

                        if (paymentType == "DEBIT") pType = PaymentType.Debit;

                        if (paymentType == "EBT") pType = PaymentType.EBT;

                        var config = await _adminSettingService.LoadSettingById(WindowsIdProvider.GetMachineGuid());

                        string terminal = config?.Terminal ?? string.Empty;
                        int port = config?.Port ?? 0;
                        string ip = config?.IP ?? string.Empty;

                        var consecutivo = await _orderService.LoadLastConsecutivoPayment();

                        var paymentResponse = await _paymentService.ProcessPaymentAsync(pType, new PaymentRequest()
                        {
                            Amount = paymentAmount * 100,
                            EcrRefNumber = consecutivo.ToString(),
                            Ip = ip,
                            Port = port,
                            Terminal = terminal
                        });

                        if (paymentResponse != null && paymentResponse.Success)
                        {
                            await _paymentSplitService.CreatePaymentAsync(paymentType, paymentAmount);

                            await LoadPaymentsAsync();

                            ClearTotales();

                        }
                        else if (paymentResponse != null)
                        {
                            MessageBox.Show($"Error: {paymentResponse.MsgInfo}");
                        }
                    }
                    else if (paymentType == "GIFTCARD")
                    {
                        MessageBox.Show("Error: Payment not implemented");
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing payment");
                    MessageBox.Show("Error processing payment", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonRemain_Click(object sender, EventArgs e)
        {
            try
            {
                _numPad.ValueCents = (int)(remainingAmount * 100);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting remaining amount");
                MessageBox.Show("Error setting remaining amount", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void buttonPrintBill_Click(object sender, EventArgs e)
        {
            try
            {
                if (remainingAmount > 0 || _shoppingCart.ItemCount == 0)
                {
                    MessageBox.Show("Cannot print bill while there is remaining amount to pay",
                        "Incomplete Payment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                await _homeInteractionService.RequestSplitPaymentCompletionAsync();
                this.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error printing bill");
                MessageBox.Show("Error printing bill", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void ClearTotales()
        {
            _numPad.Reset();
            labelChargedValue.Text = "0.00";
            labelDueValue.Text     = "0.00";
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LoadCartItems()
        {
            try
            {
                listViewCart.Items.Clear();
                decimal total = 0;

                foreach (var item in _shoppingCart.Items)
                {
                    var listItem = new ListViewItem(item.Number.ToString());
                    listItem.SubItems.Add(item.ProductName);
                    listItem.SubItems.Add(item.Quantity.ToString());
                    listItem.SubItems.Add(item.UnitPrice.ToString("C"));
                    listItem.SubItems.Add(item.Subtotal.ToString("C"));
                    listItem.Tag = item.ProductId;

                    listViewCart.Items.Add(listItem);
                    total += item.Subtotal;
                }

                labelTotalValue.Text = total.ToString("C");
                totalGlobal = total;

                foreach (ColumnHeader column in listViewCart.Columns)
                {
                    column.Width = -2;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los items del carrito: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        

        private void buttonCalculateEBT_Click(object sender, EventArgs e)
        {
            decimal total = 0;

            foreach (var cartItem in _shoppingCart.Items)
            {
                if (cartItem.IsEBT)
                {
                    total += cartItem.Total;
                }
            }

            buttonRemain.Text = total.ToString("C");
        }

        private async void buttonEbtBalance_Click(object sender, EventArgs e)
        {
            try
            {
                buttonEbtBalance.Enabled = false;
                var config = await _adminSettingService.LoadSettingById(WindowsIdProvider.GetMachineGuid());

                string terminal   = config?.Terminal ?? string.Empty;
                int    port       = config?.Port     ?? 0;
                string ip         = config?.IP       ?? string.Empty;
                var    consecutivo = await _orderService.LoadLastConsecutivoPayment();

                await _paymentService.GetEBTBalanceAsync(new PaymentRequest
                {
                    Ip           = ip,
                    Port         = port,
                    Terminal     = terminal,
                    Amount       = 0,
                    EcrRefNumber = consecutivo.ToString(),
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al consultar balance EBT:\n{ex.Message}", "Error EBT",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                buttonEbtBalance.Enabled = true;
            }
        }
    }
}
