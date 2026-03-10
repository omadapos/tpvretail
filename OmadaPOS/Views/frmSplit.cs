using Microsoft.Extensions.Logging;
using OmadaPOS.Componentes;
using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Services;
using OmadaPOS.Libreria.Utils;
using OmadaPOS.Services;
using OmadaPOS.Services.Navigation;
using System.Drawing.Text;

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
    private readonly ILogger<frmSplit>       _logger;

    private EventHandler? _cartChangedHandler;

    // ── State ─────────────────────────────────────────────────────────────────
    private decimal _totalGlobal     = 0;
    private decimal _remainingAmount = 0;

    // ── Controls referenced after construction ────────────────────────────────
    private NumericPadControl _numPad       = null!;
    private ListView          _lvCart       = null!;
    private ListView          _lvPayments   = null!;
    private Label             _lblTotal     = null!;
    private Label             _lblEntered   = null!;
    private Label             _lblRemaining = null!;
    private Button            _btnComplete  = null!;

    // ── Constructor ───────────────────────────────────────────────────────────
    public frmSplit(
        IShoppingCart           shoppingCart,
        IPaymentSplitService    paymentSplitService,
        IPaymentService         paymentService,
        IOrderService           orderService,
        IAdminSettingService    adminSettingService,
        IHomeInteractionService homeInteractionService,
        ILogger<frmSplit>       logger)
    {
        _shoppingCart           = shoppingCart;
        _paymentSplitService    = paymentSplitService;
        _paymentService         = paymentService;
        _orderService           = orderService;
        _adminSettingService    = adminSettingService;
        _homeInteractionService = homeInteractionService;
        _logger                 = logger;

        InitForm();

        Load       += async (_, _) => await InitializeAsync();
        FormClosed += (_, _) =>
        {
            if (_cartChangedHandler != null)
            {
                _shoppingCart.CartChanged -= _cartChangedHandler;
                _cartChangedHandler = null;
            }
        };
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
        AttachOwnerDraw(_lvCart);
        _lvCart.Resize += (_, _) => FillColumn(_lvCart, fillIdx: 1);
        col.Controls.Add(_lvCart, 0, 1);

        col.Controls.Add(SectionLabel("💳  Payments Applied"), 0, 2);

        _lvPayments = MakeListView();
        _lvPayments.Columns.Add("Method", 200, HorizontalAlignment.Left);
        _lvPayments.Columns.Add("Amount", 120, HorizontalAlignment.Right);
        AttachOwnerDraw(_lvPayments);
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
                Font      = new Font("Consolas", 14F, FontStyle.Bold),
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
            RowCount    = 9,
            BackColor   = Color.Transparent,
            Padding     = new Padding(10, 0, 0, 0),
            Margin      = new Padding(0),
        };
        col.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        col.RowStyles.Add(new RowStyle(SizeType.Percent, 18)); // CASH (larger)
        col.RowStyles.Add(new RowStyle(SizeType.Percent, 12)); // CREDIT
        col.RowStyles.Add(new RowStyle(SizeType.Percent, 12)); // DEBIT
        col.RowStyles.Add(new RowStyle(SizeType.Percent, 12)); // EBT
        col.RowStyles.Add(new RowStyle(SizeType.Percent,  5)); // spacer
        col.RowStyles.Add(new RowStyle(SizeType.Percent, 11)); // LOAD REMAINING
        col.RowStyles.Add(new RowStyle(SizeType.Percent, 10)); // EBT ELIGIBLE
        col.RowStyles.Add(new RowStyle(SizeType.Percent, 10)); // EBT BALANCE
        col.RowStyles.Add(new RowStyle(SizeType.Percent, 10)); // CLOSE

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
            col.Controls.Add(b, 0, row);
        }

        AddPayBtn("CASH",        "CASH",   AppColors.AccentGreen, 0);
        AddPayBtn("CREDIT CARD", "CREDIT", AppColors.SlateBlue,   1);
        AddPayBtn("DEBIT CARD",  "DEBIT",  AppColors.SlateBlue,   2);
        AddPayBtn("EBT",         "EBT",    AppColors.SlateBlue,   3);

        // Spacer
        col.Controls.Add(new Label { Dock = DockStyle.Fill, BackColor = Color.Transparent }, 0, 4);

        // Load Remaining
        var btnRemain = new Button { Dock = DockStyle.Fill, Text = "LOAD REMAINING", Margin = new Padding(0, 0, 0, 6) };
        ElegantButtonStyles.Style(btnRemain, AppColors.Warning, AppColors.TextWhite, fontSize: 13f);
        btnRemain.Click += (_, _) => _numPad.ValueCents = (int)(_remainingAmount * 100);
        col.Controls.Add(btnRemain, 0, 5);

        // EBT Eligible
        var btnCalcEbt = new Button { Dock = DockStyle.Fill, Text = "EBT ELIGIBLE", Margin = new Padding(0, 0, 0, 6) };
        ElegantButtonStyles.Style(btnCalcEbt, AppColors.NavyBase, AppColors.TextWhite, fontSize: 13f);
        btnCalcEbt.Click += (_, _) =>
        {
            decimal ebt = _shoppingCart.Items.Where(i => i.IsEBT).Sum(i => i.Total);
            _numPad.ValueCents = (int)(ebt * 100);
        };
        col.Controls.Add(btnCalcEbt, 0, 6);

        // EBT Balance
        var btnEbtBal = new Button { Dock = DockStyle.Fill, Text = "EBT BALANCE", Margin = new Padding(0, 0, 0, 6) };
        ElegantButtonStyles.Style(btnEbtBal, AppColors.NavyBase, AppColors.TextWhite, fontSize: 13f);
        btnEbtBal.Click += async (_, _) => await QueryEbtBalanceAsync(btnEbtBal);
        col.Controls.Add(btnEbtBal, 0, 7);

        // Close
        var btnClose = new Button { Dock = DockStyle.Fill, Text = "✕  CLOSE", Margin = new Padding(0, 8, 0, 0) };
        ElegantButtonStyles.Style(btnClose, AppColors.Danger, AppColors.TextWhite, fontSize: 15f);
        btnClose.Click += (_, _) => Close();
        col.Controls.Add(btnClose, 0, 8);

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

            _cartChangedHandler = (_, _) => LoadCartItems();
            _shoppingCart.CartChanged += _cartChangedHandler;
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
            lvi.SubItems.Add(item.Subtotal.ToString("C"));
            lvi.Tag = item.ProductId;
            _lvCart.Items.Add(lvi);
            total += item.Subtotal;
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

        if (paymentAmount > _remainingAmount)
        {
            MessageBox.Show(
                $"Amount exceeds remaining balance of {_remainingAmount:C}.",
                "Invalid Amount", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        btn.Enabled = false;
        try
        {
            bool ok = paymentType == "CASH"
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
            btn.Enabled = true;
        }
    }

    private async Task<bool> ProcessCashAsync(decimal amount)
    {
        await _paymentSplitService.CreatePaymentAsync("CASH", amount);
        return true;
    }

    private async Task<bool> ProcessCardAsync(string paymentType, decimal amount)
    {
        var pType = paymentType switch
        {
            "DEBIT" => PaymentType.Debit,
            "EBT"   => PaymentType.EBT,
            _       => PaymentType.Credit,
        };

        var config      = await _adminSettingService.LoadSettingById(WindowsIdProvider.GetMachineGuid());
        var consecutivo = await _orderService.LoadLastConsecutivoPayment();

        var response = await _paymentService.ProcessPaymentAsync(pType, new PaymentRequest
        {
            Amount       = amount * 100,
            EcrRefNumber = consecutivo.ToString(),
            Ip           = config?.IP       ?? string.Empty,
            Port         = config?.Port     ?? 0,
            Terminal     = config?.Terminal ?? string.Empty,
        });

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

    private async void BtnComplete_Click(object? sender, EventArgs e)
    {
        _btnComplete.Enabled = false;
        try
        {
            if (_remainingAmount > 0 || _shoppingCart.ItemCount == 0)
            {
                MessageBox.Show("Cannot complete payment while there is a remaining balance.",
                    "Incomplete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            await _homeInteractionService.RequestSplitPaymentCompletionAsync();
            Close();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing split payment");
            MessageBox.Show($"Error completing payment: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            await _paymentService.GetEBTBalanceAsync(new PaymentRequest
            {
                Ip           = config?.IP       ?? string.Empty,
                Port         = config?.Port     ?? 0,
                Terminal     = config?.Terminal ?? string.Empty,
                Amount       = 0,
                EcrRefNumber = consecutivo.ToString(),
            });
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

    // ── ListView helpers ──────────────────────────────────────────────────────
    private static ListView MakeListView() => new()
    {
        Dock              = DockStyle.Fill,
        View              = View.Details,
        FullRowSelect     = true,
        GridLines         = false,
        MultiSelect       = false,
        BackColor         = AppColors.SurfaceCard,
        ForeColor         = AppColors.TextPrimary,
        Font              = AppTypography.Body,
        BorderStyle       = BorderStyle.FixedSingle,
        HeaderStyle       = ColumnHeaderStyle.Nonclickable,
        OwnerDraw         = true,
        UseCompatibleStateImageBehavior = false,
    };

    private static void AttachOwnerDraw(ListView lv)
    {
        lv.DrawColumnHeader += (_, e) =>
        {
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            using var bg = new SolidBrush(AppColors.NavyDark);
            e.Graphics.FillRectangle(bg, e.Bounds);
            using var tb = new SolidBrush(AppColors.TextWhite);
            using var sf = new StringFormat
            {
                Alignment     = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
            };
            e.Graphics.DrawString(e.Header.Text, AppTypography.RowLabel, tb, e.Bounds, sf);
        };

        lv.DrawItem += (_, e) =>
        {
            bool isAlt = e.ItemIndex % 2 == 1;
            bool isSel = (e.State & ListViewItemStates.Selected) == ListViewItemStates.Selected;
            var  bg    = isSel ? AppColors.NavyBase
                       : isAlt ? AppColors.SurfaceMuted
                       : AppColors.SurfaceCard;
            using var br = new SolidBrush(bg);
            e.Graphics.FillRectangle(br, e.Bounds);
        };

        lv.DrawSubItem += (_, e) =>
        {
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            bool isSel = (e.ItemState & ListViewItemStates.Selected) == ListViewItemStates.Selected;

            // Respect column alignment (Right for numbers)
            var colAlign  = e.ColumnIndex < lv.Columns.Count
                            ? lv.Columns[e.ColumnIndex].TextAlign
                            : HorizontalAlignment.Left;
            var strAlign  = colAlign == HorizontalAlignment.Right  ? StringAlignment.Far
                          : colAlign == HorizontalAlignment.Center ? StringAlignment.Center
                          : StringAlignment.Near;

            using var tb = new SolidBrush(isSel ? AppColors.TextWhite : AppColors.TextPrimary);
            using var sf = new StringFormat
            {
                Alignment     = strAlign,
                LineAlignment = StringAlignment.Center,
                Trimming      = StringTrimming.EllipsisCharacter,
            };
            var rect = new Rectangle(e.Bounds.X + 6, e.Bounds.Y, e.Bounds.Width - 10, e.Bounds.Height);
            e.Graphics.DrawString(e.SubItem.Text, lv.Font, tb, rect, sf);
        };
    }

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
