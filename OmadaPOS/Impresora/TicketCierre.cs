using OmadaPOS.Libreria.Models;
using System.Drawing.Printing;

namespace OmadaPOS.Impresora;

public class TicketCierre
{
    PrintDocument pdoc = null;
    private CloseResultModel cierre;

    string address = string.Empty;
    string title = string.Empty;

    public TicketCierre(CloseResultModel cierre, string address, string title)
    {
        this.cierre = cierre;
        this.address = address;
        this.title = title;
    }

    public void print()
    {
        PrintDialog pd = new PrintDialog();
        pdoc = new PrintDocument();
        PrinterSettings ps = new PrinterSettings();
        Font font = new Font("Courier New", 14);


        PaperSize psize = new PaperSize("Custom", 80, 1000);
        ps.DefaultPageSettings.PaperSize = psize;

        pd.Document = pdoc;
        pd.Document.DefaultPageSettings.PaperSize = psize;
        //pdoc.DefaultPageSettings.PaperSize.Height =320;
        pdoc.DefaultPageSettings.PaperSize.Height = 1600;

        pdoc.DefaultPageSettings.PaperSize.Width = 500;

        pdoc.PrintPage += new PrintPageEventHandler(pdoc_PrintPage);

        DialogResult result = pd.ShowDialog();
        if (result == DialogResult.OK)
        {
            PrintPreviewDialog pp = new PrintPreviewDialog();
            pp.Document = pdoc;
            result = pp.ShowDialog();
            if (result == DialogResult.OK)
            {
                pdoc.Print();
            }
        }
    }

    void pdoc_PrintPage(object sender, PrintPageEventArgs e)
    {
        Graphics graphics = e.Graphics;
        Font font = new Font("Arial", 14,FontStyle.Bold);
        float fontHeight = font.GetHeight();
        int startX = 10;
        int startY = 15;
        int Offset = 15;

        graphics.DrawString(title, new Font("Courier New", 18),
                            new SolidBrush(Color.Black), startX, startY + Offset);

        Offset = Offset + 15;

        graphics.DrawString("Cierre Diario", new Font("Courier New", 14),
                            new SolidBrush(Color.Black), startX, startY + Offset);

        Offset = Offset + 15;

        graphics.DrawString("Date " + DateTime.Now.ToString("dd/MM/yyyy"),
                 new Font("Courier New", 12),
                 new SolidBrush(Color.Black), startX, startY + Offset);

        Offset = Offset + 20;

        string underLine = "------------------------------------------";

        graphics.DrawString(underLine, new Font("Courier New", 10),
                 new SolidBrush(Color.Black), startX, startY + Offset);

        Offset = Offset + 20;

        graphics.DrawString("Range Orders",
                 new Font("Courier New", 12),
                 new SolidBrush(Color.Black), startX, startY + Offset);

        Offset = Offset + 20;

        graphics.DrawString("Initial " + cierre.OrderInitial,
                 new Font("Courier New", 12),
                 new SolidBrush(Color.Black), startX, startY + Offset);

        Offset = Offset + 20;

        graphics.DrawString("Final " + cierre.OrderFinal,
                 new Font("Courier New", 12),
                 new SolidBrush(Color.Black), startX, startY + Offset);

        Offset = Offset + 40;

        graphics.DrawString("Payment",
                 new Font("Courier New", 12),
                 new SolidBrush(Color.Black), startX, startY + Offset);
        Offset = Offset + 20;

        double cash = 0;
        double card = 0;
        double debit = 0;
        double giftCard = 0;
        double EBT = 0;

        foreach (var data in cierre.Payment)
        {
            graphics.DrawString(underLine, font, Brushes.Black, startX, startY + Offset);

            Offset = Offset + 20;

            graphics.DrawString("Terminal " + data.Terminal,
                 new Font("Courier New", 12),
                 new SolidBrush(Color.Black), startX, startY + Offset);

            Offset = Offset + 20;

            graphics.DrawString("Cash " + data.Cash.ToString("N2"),
                 new Font("Courier New", 12),
                 new SolidBrush(Color.Black), startX, startY + Offset);

            Offset = Offset + 20;

            graphics.DrawString("Card " + data.Card.ToString("N2"),
                 new Font("Courier New", 12),
                 new SolidBrush(Color.Black), startX, startY + Offset);

            Offset = Offset + 20;

            graphics.DrawString("Debit " + data.Debit.ToString("N2"),
                 new Font("Courier New", 12),
                 new SolidBrush(Color.Black), startX, startY + Offset);

            Offset = Offset + 20;

            graphics.DrawString("Gift Card " + data.GiftCard.ToString("N2"),
                 new Font("Courier New", 12),
                 new SolidBrush(Color.Black), startX, startY + Offset);

            Offset = Offset + 20;

            graphics.DrawString("EBT " + data.EBT.ToString("N2"),
                 new Font("Courier New", 12),
                 new SolidBrush(Color.Black), startX, startY + Offset);

            Offset = Offset + 20;

            graphics.DrawString(underLine, font, Brushes.Black, startX, startY + Offset);

            Offset = Offset + 20;

            cash += data.Cash;
            card += data.Card;
            debit += data.Debit;
            giftCard += data.GiftCard;
            EBT += data.EBT;
        }

        graphics.DrawString("Totales",
                 new Font("Courier New", 12),
                 new SolidBrush(Color.Black), startX, startY + Offset);

        Offset = Offset + 20;

        graphics.DrawString("Cash " + cash.ToString("N2"),
                new Font("Courier New", 12),
                new SolidBrush(Color.Black), startX, startY + Offset);

        Offset = Offset + 20;

        graphics.DrawString("Card " + card.ToString("N2"),
             new Font("Courier New", 12),
             new SolidBrush(Color.Black), startX, startY + Offset);

        Offset = Offset + 20;

        graphics.DrawString("Debit " + debit.ToString("N2"),
             new Font("Courier New", 12),
             new SolidBrush(Color.Black), startX, startY + Offset);

        Offset = Offset + 20;

        graphics.DrawString("Gift Card " + giftCard.ToString("N2"),
             new Font("Courier New", 12),
             new SolidBrush(Color.Black), startX, startY + Offset);

        Offset = Offset + 20;

        graphics.DrawString("EBT " + EBT.ToString("N2"),
             new Font("Courier New", 12),
             new SolidBrush(Color.Black), startX, startY + Offset);

        Offset = Offset + 20;

        graphics.DrawString(underLine, font, Brushes.Black, startX, startY + Offset);

        Offset = Offset + 20;

        graphics.DrawString("Toal " + (cash + card + debit + giftCard+ EBT).ToString("N2"),
             new Font("Courier New", 12),
             new SolidBrush(Color.Black), startX, startY + Offset);
    }


}
