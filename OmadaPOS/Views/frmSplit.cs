using Microsoft.Extensions.Logging;
using OmadaPOS.Componentes; // Asegúrate de tener RoundedPanel.cs en esta carpeta
using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Services;
using OmadaPOS.Libreria.Utils;
using OmadaPOS.Services;

namespace OmadaPOS.Views
{
    public partial class frmSplit : Form
    {
        private readonly IShoppingCart _shoppingCart;
        private readonly IPaymentSplitService _paymentSplitService;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IAdminSettingService _adminSettingService;

        private readonly ILogger<frmSplit> _logger;

        int inputValue = 0;
        decimal totalGlobal = 0;
        decimal remainingAmount = 0;
        decimal due = 0;
        int totalItemPayments = 0;

        private RoundedPanel panelSplitPay;

        public frmSplit()
        {
            InitializeComponent();
            ConfigureCartListView();
            ConfigurePaymentListView();
            ConfigureUI();
            // ── Métodos de pago ───────────────────────────────────────
            ElegantButtonStyles.Style(buttonCash,          ElegantButtonStyles.CashGreen,        fontSize: 24f);
            ElegantButtonStyles.Style(buttonCredit,        ElegantButtonStyles.CreditBlue,       fontSize: 18f);
            ElegantButtonStyles.Style(buttonDebit,         ElegantButtonStyles.DebitGray,        fontSize: 18f);
            ElegantButtonStyles.Style(buttonEbt,           ElegantButtonStyles.EBTOrange,        fontSize: 18f);

            // ── Acciones EBT ──────────────────────────────────────────
            ElegantButtonStyles.Style(buttonEbtBalance,    ElegantButtonStyles.EBTBalanceOrange, fontSize: 18f);
            ElegantButtonStyles.Style(buttonCalculateEBT,  ElegantButtonStyles.EBTBalanceOrange, fontSize: 18f);

            // ── Confirmar pago (verde = acción positiva/completar) ────
            ElegantButtonStyles.Style(buttonPrintBill,     ElegantButtonStyles.CashGreen,        fontSize: 18f);

            // ── Cerrar ────────────────────────────────────────────────
            ElegantButtonStyles.Style(buttonClose,         ElegantButtonStyles.AlertRed,         fontSize: 18f);

            // ── Teclado numérico (manejado en ConfigureNumericButtons) ─
            ElegantButtonStyles.Style(buttonClear,         ElegantButtonStyles.Keypad,           fontSize: 18f);

            _shoppingCart = Program.GetService<IShoppingCart>();
            _paymentSplitService = Program.GetService<IPaymentSplitService>();
            _paymentService = Program.GetService<IPaymentService>();
            _orderService = Program.GetService<IOrderService>();
            _adminSettingService = Program.GetService<IAdminSettingService>();

            _logger = Program.GetService<ILogger<frmSplit>>();

            ClearTotales();

            this.Load += async (s, e) => await InitializeAsync();
        }

        private void ConfigureUI()
        {
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;

            // Crear panel estilizado
            panelSplitPay = new RoundedPanel
            {
                Location = new Point(30, 500),
                Size = new Size(400, 180),
                CornerRadius = 16,
                BorderColor = Color.FromArgb(220, 224, 228),
                BackgroundStart = Color.FromArgb(245, 247, 250),
                BackgroundEnd = Color.FromArgb(238, 240, 243),
                ShadowColor = Color.FromArgb(15, 0, 0, 0),
                Name = "panelSplitPay"
            };
            this.Controls.Add(panelSplitPay);

            var numberButtonColor = Color.FromArgb(64, 129, 191);
            var actionButtonColor = Color.FromArgb(20, 83, 153);
            var greenButton = Color.FromArgb(34, 139, 34);
            var orangeButton = Color.FromArgb(255, 140, 0);
            var violetButton = Color.FromArgb(138, 43, 226);
            var redButton = Color.FromArgb(220, 53, 69);

            ConfigureNumericButtons(numberButtonColor);
            //    ConfigureActionButtons(actionButtonColor, greenButton, orangeButton, violetButton, redButton);
        }

        private void ConfigureCartListView()
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
            listViewCart.Font = new Font("Segoe UI", 10F);
        }

        private void ConfigurePaymentListView()
        {
            listViewPayments.View = View.Details;
            listViewPayments.FullRowSelect = true;
            listViewPayments.GridLines = true;
            listViewPayments.MultiSelect = false;

            listViewPayments.Columns.Add("Payment", 200);
            listViewPayments.Columns.Add("Total", 100);

            listViewPayments.BackColor = Color.White;
            listViewPayments.Font = new Font("Segoe UI", 10F);
        }

        private async Task InitializeAsync()
        {
            try
            {
                await _shoppingCart.LoadCartAsync();

                LoadCartItems();

                await LoadPaymentsAsync();

                _shoppingCart.CartChanged += (s, e) => LoadCartItems();
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
            // due = remainingAmount - (inputValue / 100.0m);

            labelDueValue.Text = due.ToString("C");
            labelChargedValue.Text = (inputValue / 100.0m).ToString("C");
            buttonRemain.Text = remainingAmount.ToString("C");
        }

        private async void PaymentButton_Click(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                try
                {
                    string paymentType = button.Tag?.ToString() ?? "Unknown";
                    decimal paymentAmount = inputValue / 100.0m;

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
                inputValue = (int)(remainingAmount * 100);
                DisplayTotales();
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

                ((frmHome)Owner).ProcessPaymentMultipleAsync();
                this.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error printing bill");
                MessageBox.Show("Error printing bill", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonKey_Click(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                var tag = button.Tag?.ToString();
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
        }

        private void ClearTotales()
        {
            inputValue = 0;
            labelChargedValue.Text = "0.00";
            labelDueValue.Text = "0.00";
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

        private void ConfigureNumericButtons(Color numberButtonColor)
        {
            // Dígitos del teclado — Keypad navy
            var numericButtons = new[] { button0, button00, button1, button2, button3,
                                          button4, button5, button6, button7, button8, button9 };
            foreach (var btn in numericButtons)
                ElegantButtonStyles.Style(btn, ElegantButtonStyles.Keypad, fontSize: 30f);

            // Billetes y monto restante — verde efectivo, tamaño consistente con frmHome
            var moneyButtons = new[] { button10usd, button20usd, button50usd, button100usd };
            foreach (var btn in moneyButtons)
                ElegantButtonStyles.Style(btn, ElegantButtonStyles.CashGreen, fontSize: 24f);

            // Monto restante — azul split (semánticamente: "lo que falta por pagar")
            ElegantButtonStyles.Style(buttonRemain, ElegantButtonStyles.SplitBlueLight, fontSize: 22f);
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
            var config = await _adminSettingService.LoadSettingById(WindowsIdProvider.GetMachineGuid());

            string terminal = config?.Terminal ?? string.Empty;
            int port = config?.Port ?? 0;
            string ip = config?.IP ?? string.Empty;

            var consecutivo = await _orderService.LoadLastConsecutivoPayment();

            await _paymentService.GetEBTBalanceAsync(new PaymentRequest()
            {
                Ip = ip,
                Port = port,
                Terminal = terminal,
                Amount = 0,
                EcrRefNumber = consecutivo.ToString(),
            });
        }
    }
}
