using Microsoft.Extensions.Logging;
using OmadaPOS.Componentes;
using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Services;
using OmadaPOS.Libreria.Utils;
using OmadaPOS.Services;
using OmadaPOS.Services.Navigation;
using System.Drawing.Text;
using System.Text.RegularExpressions;

namespace OmadaPOS.Views;

/// <summary>
/// Full-screen split-payment screen. Migrated from Designer to code-only.
///
/// Layout (3 columns):
///   Left  30% — Order items + payments applied
///   Center 45% — Totals summary card + NumericPadControl (MoneyWithBills)
///   Right  25% — Payment method buttons + EBT utilities
/// Footer: COMPLETE PAYMENT (enabled when remaining == 0)
/// </summary>
public sealed class frmSplit : Form
{
    // ── Services ──────────────────────────────────────────────────────────────
    private readonly IShoppingCart           _shoppingCart;
    private readonly IPaymentSplitService    _paymentSplitService;
    private readonly IPaymentService         _paymentService;
    private readonly IOrderService           _orderService;
    private readonly IAdminSettingService    _adminSettingService;
    private readonly IHomeInteractionService _homeInteractionService;
    private readonly IGiftCardService        _giftCardService;
    private readonly ILogger<frmSplit>       _logger;

    private EventHandler? _cartChangedHandler;

    // ── State ─────────────────────────────────────────────────────────────────
    private decimal _totalGlobal     = 0;
    private decimal _remainingAmount = 0;

    // ── Controls referenced after construction ────────────────────────────────
    private NumericPadControl _numPad         = null!;
    private ListView          _lvCart         = null!;
    private ListView          _lvPayments     = null!;
    private Label             _lblTotal       = null!;
    private Label             _lblEntered     = null!;
    private Label             _lblRemaining   = null!;
    private Button            _btnComplete    = null!;
    private readonly List<Button> _paymentButtons = new();

    // ── Constructor ───────────────────────────────────────────────────────────
    public frmSplit(
        IShoppingCart           shoppingCart,
        IPaymentSplitService    paymentSplitService,
        IPaymentService         paymentService,
        IOrderService           orderService,
        IAdminSettingService    adminSettingService,
        IHomeInteractionService homeInteractionService,
        IGiftCardService        giftCardService,
        ILogger<frmSplit>       logger)
    {
        _shoppingCart           = shoppingCart;
        _paymentSplitService    = paymentSplitService;
        _paymentService         = paymentService;
        _orderService           = orderService;
        _adminSettingService    = adminSettingService;
        _homeInteractionService = homeInteractionService;
        _giftCardService        = giftCardService;
        _logger                 = logger;

        InitForm();

        // Subscribe synchronously so unsubscription is always guaranteed,
        // even if the form is closed before async InitializeAsync completes.
        _cartChangedHandler = (_, _) => LoadCartItems();
        _shoppingCart.CartChanged += _cartChangedHandler;

        Load       += async (_, _) => await InitializeAsync();
        FormClosed += (_, _) => _shoppingCart.CartChanged -= _cartChangedHandler;
    }

    // ── Layout construction ───────────────────────────────────────────────────
    private void InitForm()
    {
        DoubleBuffered  = true;
        WindowState     = FormWindowState.Maximized;
        FormBorderStyle = FormBorderStyle.None;
        BackColor       = AppColors.BackgroundSecondary;
        Text            = "Split Payment";

        SuspendLayout();

        var root = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 1,
            RowCount    = 3,
            BackColor   = Color.Transparent,
            Padding     = new Padding(0),
            Margin      = new Padding(0),
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));

        root.SuspendLayout();
        root.Controls.Add(BuildHeader(), 0, 0);
        root.Controls.Add(BuildBody(),   0, 1);
        root.Controls.Add(BuildFooter(), 0, 2);
        root.ResumeLayout(false);

        Controls.Add(root);
        ResumeLayout(false);
    }

    // ── Header ────────────────────────────────────────────────────────────────
    private Panel BuildHeader()
    {
        var header = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.NavyDark,
            Padding   = new Padding(0),
        };
        header.Paint += (_, e) =>
        {
            using var accent = new SolidBrush(AppColors.AccentGreen);
            e.Graphics.FillRectangle(accent, 0, header.Height - 3, header.Width, 3);
        };

        var lblIcon = new Label
        {
            Text      = "⚡",
            Font      = new Font("Segoe UI Emoji", 20F),
            ForeColor = AppColors.AccentGreen,
            BackColor = Color.Transparent,
            Location  = new Point(24, 0),
            Size      = new Size(52, 72),
            TextAlign = ContentAlignment.MiddleCenter,
        };

        var lblTitle = new Label
        {
            Text      = "Split Payment",
            Font      = AppTypography.SectionTitle,
            ForeColor = AppColors.TextWhite,
            BackColor = Color.Transparent,
            Location  = new Point(82, 0),
            Size      = new Size(500, 72),
            TextAlign = ContentAlignment.MiddleLeft,
        };

        var lblSub = new Label
        {
            Text      = "Apply multiple payment methods to a single order",
            Font      = AppTypography.Caption,
            ForeColor = AppColors.TextMuted,
            BackColor = Color.Transparent,
            Location  = new Point(84, 40),
            Size      = new Size(500, 28),
            TextAlign = ContentAlignment.TopLeft,
        };

        // Position close button flush-right; repositioned on Resize
        var btnX = new Button { Text = "✕", Size = new Size(52, 52) };
        ElegantButtonStyles.Style(btnX, AppColors.Danger, AppColors.TextWhite, fontSize: 16f);
        btnX.Click += (_, _) => Close();
        header.Resize += (_, _) => btnX.Location = new Point(header.Width - 70, 10);

        header.Controls.Add(lblSub);
        header.Controls.Add(lblTitle);
        header.Controls.Add(lblIcon);
        header.Controls.Add(btnX);

        return header;
    }

    // ── Body: 3-column ────────────────────────────────────────────────────────
    private Control BuildBody()
    {
        var body = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 3,
            RowCount    = 1,
            BackColor   = Color.Transparent,
            Padding     = new Padding(16, 12, 16, 8),
            Margin      = new Padding(0),
        };
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30)); // cart
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45)); // numpad
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25)); // methods
        body.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        body.Controls.Add(BuildCartColumn(),    0, 0);
        body.Controls.Add(BuildNumpadColumn(),  1, 0);
        body.Controls.Add(BuildMethodsColumn(), 2, 0);

        return body;
    }

    // ── Column 1: Order items + payments list ─────────────────────────────────
    private Control BuildCartColumn()
    {
        var col = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 1,
            RowCount    = 4,
            BackColor   = Color.Transparent,
            Padding     = new Padding(0, 0, 10, 0),
            Margin      = new Padding(0),
        };
        col.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        col.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));   // label
        col.RowStyles.Add(new RowStyle(SizeType.Percent,  65));   // cart list
        col.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));   // label
        col.RowStyles.Add(new RowStyle(SizeType.Percent,  35));   // payments list

        col.Controls.Add(SectionLabel("🛒  Order Items"),       0, 0);

        _lvCart = MakeListView();
        _lvCart.Columns.Add("#",       55,  HorizontalAlignment.Center);
        _lvCart.Columns.Add("Product", 200, HorizontalAlignment.Left);
        _lvCart.Columns.Add("Qty",     55,  HorizontalAlignment.Center);
        _lvCart.Columns.Add("Price",   90,  HorizontalAlignment.Right);
        _lvCart.Columns.Add("Total",   90,  HorizontalAlignment.Right);
        ListViewTheme.Apply(_lvCart);
        _lvCart.Resize += (_, _) => FillColumn(_lvCart, fillIdx: 1);
        col.Controls.Add(_lvCart, 0, 1);

        col.Controls.Add(SectionLabel("💳  Payments Applied"), 0, 2);

        _lvPayments = MakeListView();
        _lvPayments.Columns.Add("Method", 200, HorizontalAlignment.Left);
        _lvPayments.Columns.Add("Amount", 120, HorizontalAlignment.Right);
        ListViewTheme.Apply(_lvPayments, numericColumns: [1]);
        _lvPayments.Resize += (_, _) => FillColumn(_lvPayments, fillIdx: 0);
        col.Controls.Add(_lvPayments, 0, 3);

        return col;
    }

    // ── Column 2: Totals card + numpad ────────────────────────────────────────
    private Control BuildNumpadColumn()
    {
        var col = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 1,
            RowCount    = 2,
            BackColor   = Color.Transparent,
            Padding     = new Padding(8, 0, 8, 0),
            Margin      = new Padding(0),
        };
        col.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        col.RowStyles.Add(new RowStyle(SizeType.Absolute, 148)); // totals card
        col.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // numpad

        col.Controls.Add(BuildTotalsCard(), 0, 0);

        _numPad = new NumericPadControl(NumericPadControl.PadMode.MoneyWithBills)
        {
            Dock = DockStyle.Fill,
        };
        _numPad.ValueChanged += (_, _) => RefreshDisplay();
        col.Controls.Add(_numPad, 0, 1);

        return col;
    }

    private Panel BuildTotalsCard()
    {
        var card = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.NavyDark,
            Margin    = new Padding(0, 0, 0, 10),
        };
        card.Paint += (_, e) =>
        {
            using var border = new Pen(Color.FromArgb(40, AppColors.AccentGreen), 1f);
            e.Graphics.DrawRectangle(border, 0, 0, card.Width - 1, card.Height - 1);
            using var accent = new SolidBrush(AppColors.AccentGreen);
            e.Graphics.FillRectangle(accent, 0, card.Height - 3, card.Width, 3);
        };

        var grid = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 2,
            RowCount    = 3,
            BackColor   = Color.Transparent,
            Padding     = new Padding(20, 10, 20, 10),
            Margin      = new Padding(0),
        };
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
        grid.RowStyles.Add(new RowStyle(SizeType.Percent, 33));
        grid.RowStyles.Add(new RowStyle(SizeType.Percent, 33));
        grid.RowStyles.Add(new RowStyle(SizeType.Percent, 34));

        // Helper: plain label on the left
        static Label Lbl(string t) => new()
        {
            AutoSize  = false,
            Dock      = DockStyle.Fill,
            Text      = t,
            Font      = AppTypography.Body,
            ForeColor = AppColors.TextMuted,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleLeft,
        };

        // Helper: value label on the right; captures out param for later update
        Label Val(string t, Color fg, out Label capture)
        {
            var l = new Label
            {
                AutoSize  = false,
                Dock      = DockStyle.Fill,
                Text      = t,
                Font      = AppTypography.AmountMono,
                ForeColor = fg,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleRight,
            };
            capture = l;
            return l;
        }

        grid.Controls.Add(Lbl("Order Total"),                                   0, 0);
        grid.Controls.Add(Val("$0.00", AppColors.TextWhite,         out _lblTotal),     1, 0);
        grid.Controls.Add(Lbl("Entered"),                                        0, 1);
        grid.Controls.Add(Val("$0.00", AppColors.AccentGreenLight,  out _lblEntered),   1, 1);
        grid.Controls.Add(Lbl("Remaining"),                                      0, 2);
        grid.Controls.Add(Val("$0.00", AppColors.Warning,           out _lblRemaining), 1, 2);

        card.Controls.Add(grid);
        return card;
    }

    // ── Column 3: Payment method buttons ─────────────────────────────────────
    private Control BuildMethodsColumn()
    {
        var col = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 1,
            RowCount    = 11,
            BackColor   = Color.Transparent,
            Padding     = new Padding(10, 0, 0, 0),
            Margin      = new Padding(0),
        };
        col.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        col.RowStyles.Add(new RowStyle(SizeType.Percent, 15)); // CASH
        col.RowStyles.Add(new RowStyle(SizeType.Percent, 10)); // CREDIT
        col.RowStyles.Add(new RowStyle(SizeType.Percent, 10)); // DEBIT
        col.RowStyles.Add(new RowStyle(SizeType.Percent, 10)); // EBT
        col.RowStyles.Add(new RowStyle(SizeType.Percent, 10)); // GIFT CARD
        col.RowStyles.Add(new RowStyle(SizeType.Percent,  3)); // spacer
        col.RowStyles.Add(new RowStyle(SizeType.Percent,  9)); // LOAD REMAINING
        col.RowStyles.Add(new RowStyle(SizeType.Percent,  8)); // EBT ELIGIBLE
        col.RowStyles.Add(new RowStyle(SizeType.Percent,  8)); // EBT BALANCE
        col.RowStyles.Add(new RowStyle(SizeType.Percent,  9)); // UNDO LAST
        col.RowStyles.Add(new RowStyle(SizeType.Percent,  8)); // CLOSE

        void AddPayBtn(string label, string tag, Color color, int row)
        {
            var b = new Button
            {
                AutoSize = false,
                Dock     = DockStyle.Fill,
                Text     = label,
                Margin   = new Padding(0, 0, 0, 8),
                Tag      = tag,
            };
            ElegantButtonStyles.Style(b, color, AppColors.TextWhite, fontSize: 15f);
            b.Click += PaymentButton_Click;
            _paymentButtons.Add(b);
            col.Controls.Add(b, 0, row);
        }

        AddPayBtn("CASH",        PaymentMethod.Cash,   AppColors.AccentGreen, 0);
        AddPayBtn("CREDIT CARD", PaymentMethod.Credit, AppColors.SlateBlue,   1);
        AddPayBtn("DEBIT CARD",  PaymentMethod.Debit,  AppColors.SlateBlue,   2);
        AddPayBtn("EBT",         PaymentMethod.Ebt,    AppColors.SlateBlue,   3);

        // Gift Card
        var btnGC = new Button { Dock = DockStyle.Fill, Text = "🎁  GIFT CARD", Margin = new Padding(0, 0, 0, 6) };
        ElegantButtonStyles.Style(btnGC, AppColors.SlateBlue, AppColors.TextWhite, fontSize: 13f);
        btnGC.Click += async (_, _) => await ProcessGiftCardInSplitAsync(btnGC);
        _paymentButtons.Add(btnGC);
        col.Controls.Add(btnGC, 0, 4);

        // Spacer
        col.Controls.Add(new Label { Dock = DockStyle.Fill, BackColor = Color.Transparent }, 0, 5);

        // Load Remaining
        var btnRemain = new Button { Dock = DockStyle.Fill, Text = "LOAD REMAINING", Margin = new Padding(0, 0, 0, 6) };
        ElegantButtonStyles.Style(btnRemain, AppColors.Warning, AppColors.TextWhite, fontSize: 13f);
        btnRemain.Click += (_, _) => _numPad.ValueCents = (int)(_remainingAmount * 100);
        col.Controls.Add(btnRemain, 0, 6);

        // EBT Eligible
        var btnCalcEbt = new Button { Dock = DockStyle.Fill, Text = "EBT ELIGIBLE", Margin = new Padding(0, 0, 0, 6) };
        ElegantButtonStyles.Style(btnCalcEbt, AppColors.NavyBase, AppColors.TextWhite, fontSize: 13f);
        btnCalcEbt.Click += (_, _) =>
        {
            decimal ebt = _shoppingCart.Items.Where(i => i.IsEBT).Sum(i => i.Total);
            _numPad.ValueCents = (int)(ebt * 100);
        };
        col.Controls.Add(btnCalcEbt, 0, 7);

        // EBT Balance
        var btnEbtBal = new Button { Dock = DockStyle.Fill, Text = "EBT BALANCE", Margin = new Padding(0, 0, 0, 6) };
        ElegantButtonStyles.Style(btnEbtBal, AppColors.NavyBase, AppColors.TextWhite, fontSize: 13f);
        btnEbtBal.Click += async (_, _) => await QueryEbtBalanceAsync(btnEbtBal);
        col.Controls.Add(btnEbtBal, 0, 8);

        // Undo Last Payment
        var btnUndo = new Button { Dock = DockStyle.Fill, Text = "↩  UNDO LAST", Margin = new Padding(0, 0, 0, 6) };
        ElegantButtonStyles.Style(btnUndo, AppColors.Warning, AppColors.TextWhite, fontSize: 13f);
        btnUndo.Click += async (_, _) => await UndoLastPaymentAsync(btnUndo);
        col.Controls.Add(btnUndo, 0, 9);

        // Close
        var btnClose = new Button { Dock = DockStyle.Fill, Text = "✕  CLOSE", Margin = new Padding(0, 8, 0, 0) };
        ElegantButtonStyles.Style(btnClose, AppColors.Danger, AppColors.TextWhite, fontSize: 15f);
        btnClose.Click += (_, _) => Close();
        col.Controls.Add(btnClose, 0, 10);

        return col;
    }

    // ── Footer: Complete payment ──────────────────────────────────────────────
    private Control BuildFooter()
    {
        var footer = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.SurfaceMuted,
            Padding   = new Padding(80, 14, 80, 14),
        };
        footer.Paint += (_, e) =>
        {
            using var top = new Pen(Color.FromArgb(30, 0, 0, 0), 1f);
            e.Graphics.DrawLine(top, 0, 0, footer.Width, 0);
        };

        _btnComplete = new Button { Dock = DockStyle.Fill, Text = "✔  COMPLETE PAYMENT", Enabled = false };
        ElegantButtonStyles.Style(_btnComplete, AppColors.AccentGreen, AppColors.TextWhite, fontSize: 18f);
        _btnComplete.Click += BtnComplete_Click;
        footer.Controls.Add(_btnComplete);

        return footer;
    }

    // ── Data loading ──────────────────────────────────────────────────────────
    private async Task InitializeAsync()
    {
        try
        {
            await _shoppingCart.LoadCartAsync();
            LoadCartItems();
            await LoadPaymentsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing split payment form");
            MessageBox.Show($"Error initializing: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LoadCartItems()
    {
        _lvCart.Items.Clear();
        decimal total = 0;

        foreach (var item in _shoppingCart.Items)
        {
            var lvi = new ListViewItem(item.Number.ToString());
            lvi.SubItems.Add(item.ProductName);
            lvi.SubItems.Add(item.Quantity.ToString());
            lvi.SubItems.Add(item.UnitPrice.ToString("C"));
            lvi.SubItems.Add(item.Total.ToString("C"));
            lvi.Tag = item.ProductId;
            _lvCart.Items.Add(lvi);
            total += item.Total;
        }

        _totalGlobal = total;
        RefreshDisplay();
    }

    private async Task LoadPaymentsAsync()
    {
        try
        {
            _lvPayments.Items.Clear();
            var payments   = await _paymentSplitService.GetSessionPaymentsAsync();
            decimal totalPaid = 0;

            foreach (var p in payments)
            {
                var lvi = new ListViewItem(p.PaymentType);
                lvi.SubItems.Add(p.Total.ToString("C"));
                lvi.Tag = p;
                _lvPayments.Items.Add(lvi);
                totalPaid += p.Total;
            }

            _remainingAmount = _totalGlobal - totalPaid;
            RefreshDisplay();

            _btnComplete.Enabled = _remainingAmount <= 0 && _shoppingCart.ItemCount > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading payments");
            MessageBox.Show("Error loading payments.", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void RefreshDisplay()
    {
        if (_lblTotal     != null) _lblTotal.Text     = _totalGlobal.ToString("C");
        if (_lblEntered   != null) _lblEntered.Text   = _numPad.ValueDecimal.ToString("C");
        if (_lblRemaining != null) _lblRemaining.Text = _remainingAmount.ToString("C");
    }

    // ── Payment processing ────────────────────────────────────────────────────
    private async void PaymentButton_Click(object? sender, EventArgs e)
    {
        if (sender is not Button btn) return;

        string  paymentType   = btn.Tag?.ToString() ?? string.Empty;
        decimal paymentAmount = _numPad.ValueDecimal;

        if (paymentAmount <= 0 || _totalGlobal == 0)
        {
            MessageBox.Show("Enter an amount to charge.", "Information",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        // Cash may exceed the remaining balance (cashier returns change); cards cannot.
        if (paymentType != PaymentMethod.Cash && paymentAmount > _remainingAmount)
        {
            MessageBox.Show(
                $"Amount exceeds remaining balance of {_remainingAmount:C}.",
                "Invalid Amount", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Disable all payment buttons to prevent double-submission while awaiting.
        foreach (var b in _paymentButtons) b.Enabled = false;
        try
        {
            bool ok = paymentType == PaymentMethod.Cash
                ? await ProcessCashAsync(paymentAmount)
                : await ProcessCardAsync(paymentType, paymentAmount);

            if (ok)
            {
                _numPad.Reset();
                await LoadPaymentsAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing {Type} payment", paymentType);
            MessageBox.Show($"Payment error: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            foreach (var b in _paymentButtons) b.Enabled = true;
        }
    }

    private async Task<bool> ProcessCashAsync(decimal amount)
    {
        await _paymentSplitService.CreatePaymentAsync(PaymentMethod.Cash, amount);
        return true;
    }

    private async Task<bool> ProcessCardAsync(string paymentType, decimal amount)
    {
        var pType = paymentType switch
        {
            PaymentMethod.Debit => PaymentType.Debit,
            PaymentMethod.Ebt   => PaymentType.EBT,
            _       => PaymentType.Credit,
        };

        // Fast pre-flight calls (config + consecutivo) — no overlay needed yet
        var config      = await _adminSettingService.LoadSettingById(WindowsIdProvider.GetMachineGuid());
        var consecutivo = await _orderService.LoadLastConsecutivoPayment();

        // Show overlay for the PAX terminal call (can take up to 180 s)
        var waiting = new frmPaymentWaiting(amount, pType);
        waiting.TimeoutElapsed += (_, _) =>
            MessageBox.Show("The terminal did not respond within 90 seconds. Please check the device.",
                "Terminal Timeout", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        waiting.Show(this);
        try
        {
            var response = await _paymentService.ProcessPaymentAsync(pType, new PaymentRequest
            {
                Amount       = amount * 100,
                EcrRefNumber = consecutivo.ToString(),
                Ip           = config?.IP       ?? string.Empty,
                Port         = config?.Port     ?? 0,
                Terminal     = config?.Terminal ?? string.Empty,
            });

            if (!waiting.IsDisposed) waiting.Close();

            if (response == null || !response.Success)
            {
                MessageBox.Show(
                    $"Payment declined: {response?.MsgInfo ?? "No response from terminal."}",
                    "Declined", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            await _paymentSplitService.CreatePaymentAsync(paymentType, amount);
            return true;
        }
        catch
        {
            if (!waiting.IsDisposed) waiting.Close();
            throw;   // bubble up to PaymentButton_Click catch block
        }
    }

    private async Task ProcessGiftCardInSplitAsync(Button btnGC)
    {
        decimal amount = _numPad.ValueDecimal;
        if (amount <= 0)
        {
            MessageBox.Show("Enter the amount to charge to the gift card.", "Gift Card",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (amount > _remainingAmount)
        {
            MessageBox.Show($"Amount exceeds remaining balance of {_remainingAmount:C}.",
                "Invalid Amount", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Inline code-entry dialog
        string? cardCode = ShowCardCodeDialog();
        if (string.IsNullOrWhiteSpace(cardCode)) return;

        foreach (var b in _paymentButtons) b.Enabled = false;
        try
        {
            var card = await _giftCardService.GetByCode(cardCode);
            if (card == null)
            {
                MessageBox.Show("Gift card not found. Verify the card number and try again.",
                    "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            decimal balance = (decimal)(card.Balance ?? 0);
            if (balance < amount)
            {
                MessageBox.Show(
                    $"Insufficient balance.\n\nCard balance: {balance:C}\nCharge amount: {amount:C}",
                    "Insufficient Balance", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            card.Balance = (double?)(balance - amount);
            bool deducted = await _giftCardService.PlaceSaldo(card.Id, card);
            if (!deducted)
            {
                MessageBox.Show("Gift card deduction failed. Please try again.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            await _paymentSplitService.CreatePaymentAsync(PaymentMethod.GiftCard, amount);
            _numPad.Reset();
            await LoadPaymentsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing gift card in split");
            MessageBox.Show($"Gift card error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            foreach (var b in _paymentButtons) b.Enabled = true;
        }
    }

    /// <summary>
    /// Shows a minimal dialog to scan or type a gift card code.
    /// Returns the parsed card code, or null if cancelled.
    /// </summary>
    private string? ShowCardCodeDialog()
    {
        using var dlg  = new Form();
        dlg.Text       = "Gift Card";
        dlg.Size       = new Size(420, 180);
        dlg.StartPosition = FormStartPosition.CenterParent;
        dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
        dlg.MaximizeBox = false; dlg.MinimizeBox = false;
        dlg.BackColor  = AppColors.BackgroundPrimary;

        var lbl = new Label { Text = "Scan or type the gift card number:", Dock = DockStyle.Top,
            Height = 36, Font = AppTypography.Body, ForeColor = AppColors.TextPrimary,
            BackColor = Color.Transparent, TextAlign = ContentAlignment.BottomLeft,
            Padding = new Padding(12, 0, 0, 0) };

        var tb = new TextBox { Dock = DockStyle.Top, Height = 36, Font = AppTypography.Body,
            BackColor = AppColors.SurfaceMuted, ForeColor = AppColors.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle, Margin = new Padding(12, 4, 12, 0) };

        var pBtn = new Panel { Dock = DockStyle.Bottom, Height = 48,
            BackColor = AppColors.SurfaceMuted, Padding = new Padding(12, 8, 12, 8) };

        var btnOk = new Button { Text = "OK", Dock = DockStyle.Right, Width = 100,
            DialogResult = DialogResult.OK };
        ElegantButtonStyles.Style(btnOk, AppColors.AccentGreen, AppColors.TextWhite, fontSize: 11f);

        var btnCancel = new Button { Text = "Cancel", Dock = DockStyle.Right, Width = 100,
            DialogResult = DialogResult.Cancel };
        ElegantButtonStyles.Style(btnCancel, AppColors.Danger, AppColors.TextWhite, fontSize: 11f);

        pBtn.Controls.Add(btnOk);
        pBtn.Controls.Add(btnCancel);
        dlg.Controls.Add(pBtn);
        dlg.Controls.Add(tb);
        dlg.Controls.Add(lbl);
        dlg.AcceptButton = btnOk;
        dlg.CancelButton = btnCancel;

        dlg.Shown += (_, _) => tb.Focus();

        if (dlg.ShowDialog(this) != DialogResult.OK) return null;

        string raw = tb.Text.Trim();
        // Parse MSR track data (%...?) if present; otherwise use raw input
        var m = Regex.Match(raw, @"%(.*?)\?");
        return m.Success ? m.Groups[1].Value : raw;
    }

    private async Task UndoLastPaymentAsync(Button btnUndo)
    {
        var payments = await _paymentSplitService.GetSessionPaymentsAsync();
        if (payments.Count == 0)
        {
            MessageBox.Show("No payments to undo.", "Undo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var last = payments[^1];
        var confirm = MessageBox.Show(
            $"Remove last payment?\n\n  {last.PaymentType}   {last.Total:C}",
            "Undo Last Payment", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes) return;

        btnUndo.Enabled = false;
        try
        {
            await _paymentSplitService.RemoveLastPaymentAsync();
            await LoadPaymentsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error undoing last payment");
            MessageBox.Show($"Could not undo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnUndo.Enabled = true;
        }
    }

    private async void BtnComplete_Click(object? sender, EventArgs e)
    {
        _btnComplete.Enabled = false;
        try
        {
            if (_remainingAmount > 0 || _shoppingCart.ItemCount == 0)
            {
                MessageBox.Show("Cannot complete payment while there is a remaining balance.",
                    "Incomplete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;   // re-enable happens in finally
            }

            await _homeInteractionService.RequestSplitPaymentCompletionAsync();
            Close();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing split payment");
            MessageBox.Show($"Error completing payment: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            // Always re-enable unless the form is already closing/closed.
            if (!IsDisposed && !Disposing)
                _btnComplete.Enabled = true;
        }
    }

    private async Task QueryEbtBalanceAsync(Button btn)
    {
        btn.Enabled = false;
        try
        {
            var config      = await _adminSettingService.LoadSettingById(WindowsIdProvider.GetMachineGuid());
            var consecutivo = await _orderService.LoadLastConsecutivoPayment();

            var balanceResponse = await _paymentService.GetEBTBalanceAsync(new PaymentRequest
            {
                Ip           = config?.IP       ?? string.Empty,
                Port         = config?.Port     ?? 0,
                Terminal     = config?.Terminal ?? string.Empty,
                Amount       = 0,
                EcrRefNumber = consecutivo.ToString(),
            });

            if (balanceResponse != null && balanceResponse.Success)
            {
                MessageBox.Show(
                    $"EBT Available Balance: {balanceResponse.Balance:C}",
                    "EBT Balance", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(
                    balanceResponse?.MsgInfo ?? "Unable to retrieve EBT balance.",
                    "EBT Balance", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"EBT Balance error:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btn.Enabled = true;
        }
    }

    // ── ListView factory ──────────────────────────────────────────────────────
    private static ListView MakeListView() => new()
    {
        Dock              = DockStyle.Fill,
        View              = View.Details,
        MultiSelect       = false,
        Font              = AppTypography.ListItem,
        BorderStyle       = BorderStyle.None,
        HeaderStyle       = ColumnHeaderStyle.Nonclickable,
        UseCompatibleStateImageBehavior = false,
    };

    // Keep the "fill" column as wide as possible after fixed-width columns
    private static void FillColumn(ListView lv, int fillIdx)
    {
        if (lv.Columns.Count == 0) return;
        int total   = lv.ClientSize.Width;
        int fixedW  = 0;
        for (int i = 0; i < lv.Columns.Count; i++)
            if (i != fillIdx) fixedW += lv.Columns[i].Width;
        lv.Columns[fillIdx].Width = Math.Max(total - fixedW, 60);
    }

    // ── Section label ─────────────────────────────────────────────────────────
    private static Label SectionLabel(string text) => new()
    {
        AutoSize  = false,
        Dock      = DockStyle.Fill,
        Text      = text,
        Font      = AppTypography.RowLabel,
        ForeColor = AppColors.TextSecondary,
        BackColor = Color.Transparent,
        TextAlign = ContentAlignment.BottomLeft,
        Padding   = new Padding(2, 0, 0, 4),
    };
}
