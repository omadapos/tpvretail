using OmadaPOS.Libreria.Models;
using System.Drawing.Printing;
using System.Text.RegularExpressions;

namespace OmadaPOS.Impresora;

/// <summary>
/// Prints a customer receipt to a thermal printer using native ESC/POS commands.
///
/// Receipt layout (80 mm / 48-char Font-A):
///
///   ════════════════════════════════════════════════
///              ██  OMADA POS  ██              (2× bold)
///          123 Main St, Anytown PR
///              Tel: (787) 555-0100
///   ════════════════════════════════════════════════
///   Invoice #:  00001234
///   Date:       03/07/2026  14:32
///   Cashier:    María García
///   ------------------------------------------------
///   PRODUCT                      QTY       TOTAL
///   ------------------------------------------------
///   Whole Milk 1Gal               2        $6.98
///     @ $3.49 ea
///   Bananas                      lb        $1.20
///     @ $0.49/lb × 2.45 lb
///   [PROMO 3×$5.00]               3        $5.00
///   ════════════════════════════════════════════════
///   Subtotal:                            $13.18
///   Tax:                                  $0.92
///   Discount:                             $0.00
///   ════════════════════════════════════════════════
///                  TOTAL   $14.10              (2× size)
///   ════════════════════════════════════════════════
///   Payment:  CASH                       $20.00
///   CHANGE:                               $5.90
///   ------------------------------------------------
///   Items: 3   │   EBT: No   │   WIC: No
///
///        [  QR CODE — INV-00001234  ]
///      Scan to verify your receipt
///
///      |||||||  00001234  |||||||    (CODE128)
///   ────────────────────────────────────────────────
///     Thank you for shopping at OMADA POS!
///     Powered by Omada Systems
///   ════════════════════════════════════════════════
///   [PARTIAL CUT]
/// </summary>
public sealed class ReceiptPrinter
{
    // ── Data ──────────────────────────────────────────────────────────────────
    private readonly OrderModel             _order;
    private readonly List<OrderDetailModel> _details;
    private readonly string  _cashier;
    private readonly string  _storeName;
    private readonly string  _storeAddress;
    private readonly string? _storePhone;
    private readonly string? _footerMsg;
    private readonly PaymentResponseModel?  _paymentResponse;
    private readonly List<PaymentModel>?    _splitPayments;

    // ── Constructor ───────────────────────────────────────────────────────────
    public ReceiptPrinter(
        OrderModel             order,
        List<OrderDetailModel> details,
        string  cashier,
        string  storeName,
        string  storeAddress,
        string? storePhone              = null,
        string? footerMsg               = null,
        PaymentResponseModel? paymentResponse = null,
        List<PaymentModel>?   splitPayments   = null)
    {
        _order           = order;
        _details         = details;
        _cashier         = cashier;
        _storeName       = storeName;
        _storeAddress    = storeAddress;
        _storePhone      = storePhone;
        _footerMsg       = footerMsg;
        _paymentResponse = paymentResponse;
        _splitPayments   = splitPayments;
    }

    // ── Public entry point ────────────────────────────────────────────────────

    /// <summary>
    /// Build the ESC/POS byte stream and send it to the default printer.
    /// Throws <see cref="InvalidOperationException"/> when no printer is installed.
    /// </summary>
    public void Print()
    {
        string printer = new PrinterSettings().PrinterName;
        if (string.IsNullOrWhiteSpace(printer))
            throw new InvalidOperationException("No default printer is configured.");

        byte[] bytes = BuildReceipt();
        RawPrinterHelper.SendBytesToPrinter(printer, bytes);
    }

    /// <summary>Send to an explicit printer name.</summary>
    public void PrintTo(string printerName)
    {
        byte[] bytes = BuildReceipt();
        RawPrinterHelper.SendBytesToPrinter(printerName, bytes);
    }

    // ── Receipt assembly ──────────────────────────────────────────────────────

    private byte[] BuildReceipt()
    {
        var b = new EscPosBuilder();
        b.Init();

        PrintHeader(b);
        PrintMeta(b);
        PrintItems(b);
        PrintTotals(b);

        bool isSplit = _splitPayments is { Count: > 0 };
        if (isSplit)
            PrintSplitPayments(b);
        else
            PrintPayment(b);

        // Transaction details (card / EBT) — only for single-method payments
        if (!isSplit)
            PrintTransactionDetails(b);

        PrintQrAndBarcode(b);
        PrintFooter(b);

        b.Cut(6);
        return b.Build();
    }

    // ── Sections ──────────────────────────────────────────────────────────────

    private void PrintHeader(EscPosBuilder b)
    {
        b.Center()
         .Bold(true).CharSize(2, 2)
         .Line(Truncate(_storeName.ToUpperInvariant(), 20))
         .NormalStyle()
         .Center();

        if (!string.IsNullOrWhiteSpace(_storeAddress))
            b.Line(_storeAddress);

        if (!string.IsNullOrWhiteSpace(_storePhone))
            b.Line($"Tel: {_storePhone}");

        b.NewLine()
         .Bold(true).Separator('=').NormalStyle();
    }

    private void PrintMeta(EscPosBuilder b)
    {
        b.Left()
         .TwoCol("Invoice #:", _order.Consecutivo.ToString("D8"))
         .TwoCol("Date:",      _order.Created_At.ToString("MM/dd/yyyy  HH:mm"))
         .TwoCol("Cashier:",   Truncate(_cashier, 26))
         .Separator('-');

        // Column headers
        b.Bold(true)
         .Line(FormatItemHeader())
         .NormalStyle()
         .Separator('-');
    }

    private void PrintItems(EscPosBuilder b)
    {
        int ebtItems = 0;
        int wicItems = 0;

        foreach (var d in _details)
        {
            string name = Truncate((d.Product_Name ?? "Unknown").ToUpperInvariant(), 28);

            if (d.Peso > 0)
            {
                // Weight item
                double total = d.Peso * d.Price;
                b.Line(FormatItemLine(name, "lb", total));
                b.Line($"  @ {d.Price:C}/lb x {d.Peso:F3} lb");
            }
            else
            {
                double qty   = d.Quantity;
                double total = qty * d.Price;

                // Promotion recalculation
                string promoTag = "";
                if (!string.IsNullOrEmpty(d.PromotionName) && d.PromotionName == "Price")
                {
                    int    times = (int)(qty / d.PromotionValue);
                    int    rest  = (int)(qty % d.PromotionValue);
                    total    = times * d.PromotionLimit + rest * d.Price;
                    promoTag = $"  [PROMO {d.PromotionValue}x{d.PromotionLimit:C}]";
                }

                b.Line(FormatItemLine(name, qty % 1 == 0 ? ((int)qty).ToString() : qty.ToString("F2"), total));

                if (!string.IsNullOrEmpty(promoTag))
                    b.Line(promoTag);
                else if (qty > 1)
                    b.Line($"  @ {d.Price:C} ea");
            }
        }

        b.Separator('-');
        b.TwoCol($"Items: {_details.Count}", "");
    }

    private void PrintTotals(EscPosBuilder b)
    {
        b.Separator('=')
         .TwoCol("Subtotal:",  _order.SubTotal.ToString("C"))
         .TwoCol("Tax:",       _order.Total_Tax_Amount.ToString("C"));

        if (_order.Total_Desc_Amount > 0)
            b.TwoCol("Discount:", $"-{_order.Total_Desc_Amount:C}");

        b.Bold(true).Separator('=').NormalStyle();

        // Grand total — double-size, centered
        b.Center()
         .CharSize(2, 2).Bold(true)
         .Line($"TOTAL  {_order.Order_Amount:C}")
         .NormalStyle()
         .Bold(true).Separator('=').NormalStyle()
         .Left();
    }

    private void PrintPayment(EscPosBuilder b)
    {
        string method = FormatPaymentMethod(_order.Payment_Method);
        b.TwoCol($"Payment ({method}):", _order.Order_Amount.ToString("C"));

        if (_order.Devuelta > 0)
            b.Bold(true)
             .TwoCol("CHANGE:", _order.Devuelta.ToString("C"))
             .NormalStyle();

        if (_order.Balance > 0)
            b.TwoCol("Balance Due:", _order.Balance.ToString("C"));

        b.Separator('-').NewLine();
    }

    /// <summary>
    /// Prints each split-payment line (method + amount), a subtotal, and
    /// the change due when applicable. Called in place of PrintPayment
    /// when _splitPayments is populated.
    /// </summary>
    private void PrintSplitPayments(EscPosBuilder b)
    {
        b.Separator('=')
         .Center().Bold(true).Line("SPLIT PAYMENT").NormalStyle().Left()
         .Separator('-');

        decimal paidTotal = 0;
        foreach (var p in _splitPayments!)
        {
            if (p.Total <= 0) continue;
            string label = $"  {FormatPaymentMethod(p.PaymentType)}:";
            b.TwoCol(label, p.Total.ToString("C"));
            paidTotal += p.Total;
        }

        b.Separator('-');
        b.Bold(true).TwoCol("TOTAL PAID:", paidTotal.ToString("C")).NormalStyle();

        if (_order.Devuelta > 0)
            b.Bold(true)
             .TwoCol("CHANGE:", _order.Devuelta.ToString("C"))
             .NormalStyle();

        b.Separator('-').NewLine();
    }

    private void PrintTransactionDetails(EscPosBuilder b)
    {
        if (_paymentResponse == null) return;

        string method = (_order.Payment_Method ?? "").ToUpperInvariant();
        bool isCard = method is "CREDIT" or "DEBIT";
        bool isEbt  = method is "EBT";

        if (!isCard && !isEbt) return;

        b.Separator('=');

        if (isCard)
        {
            string cardLabel = method == "CREDIT" ? "CREDIT CARD TRANSACTION" : "DEBIT CARD TRANSACTION";
            b.Center().Bold(true).Line(cardLabel).NormalStyle().Left();
            b.Separator('-');

            // Card number — show only last 4 digits if present
            if (!string.IsNullOrWhiteSpace(_paymentResponse.PaymentNumber))
            {
                string raw    = Regex.Replace(_paymentResponse.PaymentNumber, @"\D", "");
                string masked = raw.Length >= 4
                    ? $"**** **** **** {raw[^4..]}"
                    : _paymentResponse.PaymentNumber;
                b.TwoCol("Card:", masked);
            }

            if (!string.IsNullOrWhiteSpace(_paymentResponse.PaymentCardHolder))
                b.TwoCol("Holder:", Truncate(_paymentResponse.PaymentCardHolder.ToUpperInvariant(), 26));

            if (!string.IsNullOrWhiteSpace(_paymentResponse.PaymentCardType))
                b.TwoCol("Card Type:", _paymentResponse.PaymentCardType.ToUpperInvariant());

            if (!string.IsNullOrWhiteSpace(_paymentResponse.PaymentReferenceNumber))
                b.TwoCol("Reference:", _paymentResponse.PaymentReferenceNumber);
        }
        else // EBT
        {
            b.Center().Bold(true).Line("EBT TRANSACTION").NormalStyle().Left();
            b.Separator('-');

            if (!string.IsNullOrWhiteSpace(_paymentResponse.PaymentReferenceNumber))
                b.TwoCol("Reference:", _paymentResponse.PaymentReferenceNumber);

            // EBT remaining balance — highlighted
            if (_paymentResponse.Balance > 0)
            {
                b.Separator('-')
                 .Bold(true)
                 .TwoCol("EBT BALANCE REMAINING:", _paymentResponse.Balance.ToString("C"))
                 .NormalStyle();
            }
        }

        b.Separator('=');
    }

    private void PrintQrAndBarcode(EscPosBuilder b)
    {
        string qrData = $"INV-{_order.Consecutivo:D8}";

        b.Center()
         .QrCode(qrData, moduleSize: 5)
         .NormalStyle()
         .Line("Scan to verify your receipt")
         .NewLine();

        // CODE128 barcode of consecutivo
        string barData = _order.Consecutivo.ToString("D8");
        b.Center()
         .Barcode128(barData, height: 56, width: 2)
         .NormalStyle();

        b.Separator('-');
    }

    private void PrintFooter(EscPosBuilder b)
    {
        string footer = !string.IsNullOrWhiteSpace(_footerMsg)
            ? _footerMsg
            : "Thank you for your purchase!";

        b.Center()
         .Bold(true).Line(footer).NormalStyle()
         .Line("Powered by Omada POS")
         .Bold(true).Separator('=').NormalStyle()
         .Left();
    }

    // ── Formatting helpers ────────────────────────────────────────────────────

    // 48 chars: name(28) + qty(6) + price(12) + 2 padding
    private static string FormatItemHeader()
    {
        return "PRODUCT".PadRight(28) + "QTY".PadLeft(6) + "TOTAL".PadLeft(12) + "  ";
    }

    private static string FormatItemLine(string name, string qty, double total)
    {
        name = Truncate(name, 28).PadRight(28);
        qty  = Truncate(qty,  6).PadLeft(6);
        string price = total.ToString("C").PadLeft(12);
        return name + qty + price;
    }

    private static string FormatPaymentMethod(string? method) =>
        method?.ToUpperInvariant() switch
        {
            "CASH"   => "Cash",
            "CREDIT" => "Credit Card",
            "DEBIT"  => "Debit Card",
            "EBT"    => "EBT",
            "SPLIT"  => "Split",
            null     => "—",
            _        => method,
        };

    private static string Truncate(string s, int max)
        => s.Length > max ? s[..max] : s;
}
