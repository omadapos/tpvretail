using OmadaPOS.Libreria.Models;
using System.Drawing.Printing;

namespace OmadaPOS.Impresora;

/// <summary>
/// Prints the Daily Close (Cierre Diario) report to a thermal receipt printer
/// using native ESC/POS commands — no dialog boxes, instant print.
///
/// Report layout (80 mm / 48-char):
///
///   ════════════════════════════════════════════════
///            ██  OMADA POS  ██
///            CIERRE DIARIO
///        03/07/2026   Branch Name
///   ════════════════════════════════════════════════
///   Orders:   00001234  →  00001240
///   ------------------------------------------------
///   TERMINAL: POS-001
///   ------------------------------------------------
///   Cash:                          $  340.00
///   Credit Card:                   $  125.50
///   Debit Card:                    $   80.00
///   EBT:                           $   55.00
///   Gift Card:                     $   20.00
///   ────────────────────────────────────────────────
///   TERMINAL TOTAL:                $  620.50
///   ════════════════════════════════════════════════
///   GRAND TOTAL:               $ 1,240.75      (2×)
///   ════════════════════════════════════════════════
///   [PARTIAL CUT]
/// </summary>
public sealed class TicketCierre
{
    private readonly CloseResultModel _cierre;
    private readonly string  _storeName;
    private readonly string? _storeAddress;

    public TicketCierre(CloseResultModel cierre, string storeName, string? storeAddress = null)
    {
        _cierre       = cierre;
        _storeName    = storeName;
        _storeAddress = storeAddress;
    }

    // ── Public entry point ────────────────────────────────────────────────────

    /// <summary>Print to the system default printer immediately — no dialogs.</summary>
    public void Print()
    {
        string printer = new PrinterSettings().PrinterName;
        if (string.IsNullOrWhiteSpace(printer))
            throw new InvalidOperationException("No default printer is configured.");

        byte[] bytes = BuildReport();
        RawPrinterHelper.SendBytesToPrinter(printer, bytes);
    }

    // ── Report assembly ───────────────────────────────────────────────────────

    private byte[] BuildReport()
    {
        var b = new EscPosBuilder();
        b.Init();

        PrintHeader(b);
        PrintOrderRange(b);
        PrintTerminals(b);
        PrintGrandTotal(b);
        PrintFooter(b);

        b.Cut(5);
        return b.Build();
    }

    // ── Sections ──────────────────────────────────────────────────────────────

    private void PrintHeader(EscPosBuilder b)
    {
        b.Center()
         .Bold(true).CharSize(2, 2)
         .Line(Truncate(_storeName.ToUpperInvariant(), 20))
         .NormalStyle()
         .Center()
         .Bold(true).Line("CIERRE DIARIO").NormalStyle();

        if (!string.IsNullOrWhiteSpace(_storeAddress))
            b.Line(_storeAddress);

        b.Line(DateTime.Now.ToString("MM/dd/yyyy   HH:mm"))
         .NewLine()
         .Bold(true).Separator('=').NormalStyle();
    }

    private void PrintOrderRange(EscPosBuilder b)
    {
        b.Left()
         .TwoCol("Orders:", $"{_cierre.OrderInitial:D8}  →  {_cierre.OrderFinal:D8}")
         .Separator('-');
    }

    private void PrintTerminals(EscPosBuilder b)
    {
        double grandCash     = 0, grandCard     = 0, grandDebit = 0;
        double grandGiftCard = 0, grandEbt      = 0;

        foreach (var t in _cierre.Payment ?? [])
        {
            string terminal = t.Terminal ?? "POS";

            b.Bold(true).Line($"TERMINAL: {terminal}").NormalStyle()
             .Separator('-');

            b.TwoCol("Cash:",       t.Cash.ToString("C"));
            b.TwoCol("Credit Card:", t.Card.ToString("C"));
            b.TwoCol("Debit Card:", t.Debit.ToString("C"));
            b.TwoCol("EBT:",        t.EBT.ToString("C"));
            b.TwoCol("Gift Card:",  t.GiftCard.ToString("C"));

            double termTotal = t.Cash + t.Card + t.Debit + t.GiftCard + t.EBT;

            b.Separator('-')
             .Bold(true)
             .TwoCol("TERMINAL TOTAL:", termTotal.ToString("C"))
             .NormalStyle()
             .Separator('=');

            grandCash     += t.Cash;
            grandCard     += t.Card;
            grandDebit    += t.Debit;
            grandGiftCard += t.GiftCard;
            grandEbt      += t.EBT;
        }

        // Summary totals across all terminals (only if > 1)
        if ((_cierre.Payment?.Count ?? 0) > 1)
        {
            b.Bold(true).Line("ALL TERMINALS — SUMMARY").NormalStyle()
             .Separator('-')
             .TwoCol("Cash:",       grandCash.ToString("C"))
             .TwoCol("Credit Card:", grandCard.ToString("C"))
             .TwoCol("Debit Card:", grandDebit.ToString("C"))
             .TwoCol("EBT:",        grandEbt.ToString("C"))
             .TwoCol("Gift Card:",  grandGiftCard.ToString("C"))
             .Bold(true).Separator('=').NormalStyle();
        }
    }

    private void PrintGrandTotal(EscPosBuilder b)
    {
        double grand = (_cierre.Payment ?? [])
            .Sum(p => p.Cash + p.Card + p.Debit + p.GiftCard + p.EBT);

        b.Center()
         .CharSize(2, 2).Bold(true)
         .Line($"TOTAL  {grand:C}")
         .NormalStyle()
         .Bold(true).Separator('=').NormalStyle()
         .Left();
    }

    private static void PrintFooter(EscPosBuilder b)
    {
        b.Center()
         .Line(DateTime.Now.ToString("MM/dd/yyyy  HH:mm:ss"))
         .Line("Powered by Omada POS")
         .Bold(true).Separator('=').NormalStyle()
         .Left();
    }

    private static string Truncate(string s, int max)
        => s.Length > max ? s[..max] : s;
}
