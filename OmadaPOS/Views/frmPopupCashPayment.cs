using OmadaPOS.Impresora;
using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Services;

namespace OmadaPOS.Views;

/// <summary>
/// Shown (modeless) immediately after any payment is processed.
/// - Cash:         displays change due in green.
/// - Credit/Debit: displays approval + masked card number + reference.
/// - EBT:          displays approval + remaining EBT balance.
/// - Split:        displays each payment method with its amount.
/// Always offers one-click receipt print.
/// </summary>
public sealed class frmPopupCashPayment : POSDialog
{
    private readonly IOrderService         _orderService;
    private readonly IBranchService        _branchService;
    private readonly PaymentResponseModel? _paymentResponse;
    private readonly List<PaymentModel>?   _splitPayments;

    private readonly int     _orderId;
    private readonly int     _consecutivo;
    private readonly decimal _devuelta;

    private Button _btnPrint = null!;

    // ── Payment type helpers ──────────────────────────────────────────────────
    private bool IsSplit => _splitPayments is { Count: > 0 };
    private bool IsCash  => !IsSplit && _devuelta > 0 && _paymentResponse == null;
    private bool IsEbt   => !IsSplit && _paymentResponse?.Balance > 0 &&
                            string.IsNullOrWhiteSpace(_paymentResponse?.PaymentCardType);
    private bool IsCard  => !IsSplit && !IsCash && !IsEbt && _paymentResponse != null;

    public frmPopupCashPayment(
        IOrderService          orderService,
        IBranchService         branchService,
        int orderId, int consecutivo, decimal devuelta,
        PaymentResponseModel?  paymentResponse = null,
        List<PaymentModel>?    splitPayments   = null)
    {
        _orderService    = orderService;
        _branchService   = branchService;
        _orderId         = orderId;
        _consecutivo     = consecutivo;
        _devuelta        = devuelta;
        _paymentResponse = paymentResponse;
        _splitPayments   = splitPayments;
    }

    protected override Color  AccentColor => IsEbt   ? AppColors.AccentGreen
                                           : IsCard  ? AppColors.SlateBlue
                                           : IsSplit ? AppColors.SlateBlue
                                           :           AppColors.AccentGreen;
    protected override string Icon        => IsEbt   ? "🏦"
                                           : IsCard  ? "💳"
                                           : IsSplit ? "🔀"
                                           :           "💵";
    protected override string Title       => IsEbt   ? "EBT Payment"
                                           : IsCard  ? "Card Payment"
                                           : IsSplit ? "Split Payment"
                                           :           "Cash Payment";
    protected override string Subtitle    => IsEbt   ? "Approved — EBT balance printed on receipt"
                                           : IsCard  ? "Approved — transaction details printed on receipt"
                                           : IsSplit ? "Payment complete — all methods printed on receipt"
                                           :           "Transaction complete — give change to customer";
    protected override DialogSize Size        => DialogSize.Medium;
    protected override string?    ConfirmText => "🖨  PRINT RECEIPT";
    protected override string     CancelText  => "✕  CLOSE";

    protected override Control BuildContent()
    {
        var outer = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 1,
            RowCount    = 2,
            BackColor   = AppColors.BackgroundPrimary,
            Padding     = new Padding(24, 20, 24, 12),
        };
        outer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        outer.RowStyles.Add(new RowStyle(SizeType.Percent, 60));
        outer.RowStyles.Add(new RowStyle(SizeType.Percent, 40));

        outer.Controls.Add(BuildMainCard(), 0, 0);
        outer.Controls.Add(BuildInvoiceRow(), 0, 1);
        return outer;
    }

    // ── Main card — adapts to payment type ───────────────────────────────────
    private Control BuildMainCard()
    {
        if (IsSplit) return BuildSplitCard();
        if (IsCash)  return BuildCashCard();
        if (IsEbt)   return BuildEbtCard();
        return BuildCardApprovalCard();
    }

    private Control BuildSplitCard()
    {
        var card = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 1,
            RowCount    = 0,
            BackColor   = Color.FromArgb(239, 246, 255),
            Margin      = new Padding(0, 0, 0, 12),
            AutoSize    = false,
        };
        card.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        // Header caption
        card.RowCount++;
        card.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        card.Controls.Add(MakeCaption("PAYMENT BREAKDOWN"), 0, card.RowCount - 1);

        // One row per payment line
        foreach (var p in _splitPayments!)
        {
            if (p.Total <= 0) continue;
            string method = FormatMethod(p.PaymentType);

            var row = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1,
                BackColor = Color.Transparent, Margin = new Padding(8, 2, 8, 2),
            };
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            row.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            row.Controls.Add(new Label
            {
                AutoSize = false, Dock = DockStyle.Fill,
                Text = method, Font = AppTypography.Body,
                ForeColor = AppColors.TextPrimary, BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft,
            }, 0, 0);
            row.Controls.Add(new Label
            {
                AutoSize = false, Dock = DockStyle.Fill,
                Text = p.Total.ToString("C"), Font = AppTypography.Body,
                ForeColor = AppColors.TextPrimary, BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleRight,
            }, 1, 0);

            card.RowCount++;
            card.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            card.Controls.Add(row, 0, card.RowCount - 1);
        }

        // Change due line (if any)
        if (_devuelta > 0)
        {
            card.RowCount++;
            card.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));

            var changeRow = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1,
                BackColor = Color.FromArgb(220, 252, 231), Margin = new Padding(0, 4, 0, 0),
            };
            changeRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
            changeRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            changeRow.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            changeRow.Controls.Add(new Label
            {
                AutoSize = false, Dock = DockStyle.Fill,
                Text = "CHANGE DUE", Font = AppTypography.Body,
                ForeColor = AppColors.AccentGreen, BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft,
            }, 0, 0);
            changeRow.Controls.Add(new Label
            {
                AutoSize = false, Dock = DockStyle.Fill,
                Text = _devuelta.ToString("C"), Font = new Font("Montserrat", 14F, FontStyle.Bold),
                ForeColor = AppColors.AccentGreen, BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleRight,
            }, 1, 0);
            card.Controls.Add(changeRow, 0, card.RowCount - 1);
        }

        return card;
    }

    private static string FormatMethod(string? method) =>
        method?.ToUpperInvariant() switch
        {
            "CASH"        => "💵  Cash",
            "CREDIT"      => "💳  Credit Card",
            "CREDIT_CARD" => "💳  Credit Card",
            "DEBIT"       => "💳  Debit Card",
            "DEBIT_CARD"  => "💳  Debit Card",
            "EBT"         => "🏦  EBT",
            "GIFTCARD"    => "🎁  Gift Card",
            null          => "—",
            var m         => m,
        };

    private Control BuildCashCard()
    {
        var card = MakeInfoCard(Color.FromArgb(236, 253, 245));

        card.Controls.Add(MakeCaption("CHANGE DUE"),  0, 0);
        card.Controls.Add(MakeBigAmount(_devuelta.ToString("C"), AppColors.AccentGreen), 0, 1);
        return card;
    }

    private Control BuildEbtCard()
    {
        var card = MakeInfoCard(Color.FromArgb(236, 253, 245));
        card.Controls.Add(MakeCaption("EBT APPROVED  ✓"), 0, 0);

        string balanceText = _paymentResponse?.Balance > 0
            ? $"Balance: {_paymentResponse.Balance:C}"
            : "Approved";
        card.Controls.Add(MakeBigAmount(balanceText, AppColors.AccentGreen), 0, 1);
        return card;
    }

    private Control BuildCardApprovalCard()
    {
        var card = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 1,
            RowCount    = 4,
            BackColor   = Color.FromArgb(239, 246, 255),
            Margin      = new Padding(0, 0, 0, 12),
        };
        card.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        card.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));   // caption
        card.RowStyles.Add(new RowStyle(SizeType.Percent,  40));   // "APPROVED ✓"
        card.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));   // card / holder
        card.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));   // reference

        string cardType = _paymentResponse?.PaymentCardType?.ToUpperInvariant() ?? "CARD";
        string cardNum  = MaskCard(_paymentResponse?.PaymentNumber);
        string holder   = _paymentResponse?.PaymentCardHolder?.ToUpperInvariant() ?? "";
        string refNum   = _paymentResponse?.PaymentReferenceNumber ?? "";

        card.Controls.Add(MakeCaption($"{cardType} APPROVED  ✓"), 0, 0);
        card.Controls.Add(MakeBigAmount("✓ APPROVED", AppColors.SlateBlue), 0, 1);
        card.Controls.Add(MakeInfoLabel($"{cardNum}   {holder}"), 0, 2);
        card.Controls.Add(MakeInfoLabel($"Ref: {refNum}"), 0, 3);
        return card;
    }

    // ── Invoice info row ──────────────────────────────────────────────────────
    private Control BuildInvoiceRow()
    {
        var row = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 2,
            RowCount    = 1,
            BackColor   = Color.Transparent,
            Margin      = new Padding(0),
        };
        row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 48));
        row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        row.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        row.Controls.Add(new Label
        {
            AutoSize  = false, Dock = DockStyle.Fill,
            Text      = "🧾", Font = new Font("Segoe UI Emoji", 22F),
            BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleCenter,
        }, 0, 0);

        row.Controls.Add(new Label
        {
            AutoSize  = false, Dock = DockStyle.Fill,
            Text      = $"Invoice  #  {_consecutivo}",
            Font      = AppTypography.SectionTitle,
            ForeColor = AppColors.TextPrimary,
            BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft,
        }, 1, 0);

        return row;
    }

    // ── UI helpers ────────────────────────────────────────────────────────────
    private static TableLayoutPanel MakeInfoCard(Color bg)
    {
        var c = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2,
            BackColor = bg, Margin = new Padding(0, 0, 0, 12),
        };
        c.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        c.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        c.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        return c;
    }

    private static Label MakeCaption(string text) => new()
    {
        AutoSize = false, Dock = DockStyle.Fill, Text = text,
        Font = AppTypography.RowLabel, ForeColor = AppColors.TextSecondary,
        BackColor = Color.Transparent, TextAlign = ContentAlignment.BottomCenter,
    };

    private static Label MakeBigAmount(string text, Color color) => new()
    {
        AutoSize = false, Dock = DockStyle.Fill, Text = text,
        Font = new Font("Montserrat", 36F, FontStyle.Bold),
        ForeColor = color, BackColor = Color.Transparent,
        TextAlign = ContentAlignment.MiddleCenter,
    };

    private static Label MakeInfoLabel(string text) => new()
    {
        AutoSize = false, Dock = DockStyle.Fill, Text = text,
        Font = AppTypography.Body, ForeColor = AppColors.TextSecondary,
        BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleCenter,
    };

    private static string MaskCard(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return "•••• •••• •••• ••••";
        string digits = System.Text.RegularExpressions.Regex.Replace(raw, @"\D", "");
        return digits.Length >= 4 ? $"•••• •••• •••• {digits[^4..]}" : raw;
    }

    protected override async Task<bool> OnConfirmAsync()
    {
        try
        {
            var order        = await _orderService.GetOrderById(_orderId);
            var orderDetails = await _orderService.GetOrderDetailsByOrderId(_orderId);

            if (order == null || orderDetails == null)
            {
                MessageBox.Show("Could not retrieve order information.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            var branch = await _branchService.LoadBranch(SessionManager.BranchId ?? 0);
            if (branch != null)
            {
                new ReceiptPrinter(
                    order,
                    orderDetails,
                    cashier:         SessionManager.Name ?? "",
                    storeName:       branch.Name    ?? "OMADA POS",
                    storeAddress:    branch.Address ?? "",
                    storePhone:      branch.Contact,
                    footerMsg:       branch.FooterMsg,
                    paymentResponse: _paymentResponse,
                    splitPayments:   _splitPayments).Print();
            }

            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Print error:\n{ex.Message}", "Print Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }
}
