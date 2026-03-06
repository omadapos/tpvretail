using OmadaPOS.Libreria.Models;
using System.Drawing.Printing;

namespace OmadaPOS.Impresora;

public class Ticket
{
    int ticketNo;
    string drawnBy;
    int consecutivo = 0;
    OrderModel order;
    List<OrderDetailModel> details;
    Graphics graphics;

    Bitmap imagePrint;

    int totalNumber = 0;
    int itemPerPage = 0;
    int itemTotalPage = 20;
    bool bFirst = false;
    int margenIzquierda = 10;

    string address = string.Empty;
    string footerMsg = string.Empty;

    float currentY = 0;
    Font font = new Font("Roboto", 10, FontStyle.Bold);
    Brush brush = new SolidBrush(Color.Black);
    int width = 400;
    string underLine = "*****************************************************";

    public Ticket(
        int ticketNo, int consecutivo, 
        OrderModel order, List<OrderDetailModel> details, string drawnBy,
        Bitmap imagePrint, string address, string footerMsg)
    {
        this.ticketNo = ticketNo;
        this.drawnBy = drawnBy;
        this.order = order;
        this.details = details;
        this.imagePrint = imagePrint;
        this.consecutivo = consecutivo;
        this.address = address;
        this.footerMsg = footerMsg;
    }

    public void Print()
    {
        PrintDialog pd = new PrintDialog();

        PrintDocument pdoc = new PrintDocument();

        pd.Document = pdoc;

        pdoc.PrintPage += new PrintPageEventHandler(pdoc_PrintPage);

        PrintPreviewDialog pp = new PrintPreviewDialog
        {
            Document = pdoc
        };

        pdoc.Print();
    }

    void pdoc_PrintPage(object sender, PrintPageEventArgs e)
    {
        currentY = 10;

        graphics = e.Graphics;

        if (!bFirst)
        {
            //graphics.DrawImage(imagePrint, new Rectangle(50, 10, 200, 100));

            //currentY += 120;

            WriteThreeColumnLine("", address, "");

            NewLine();

            WriteLine("Invoice # " + consecutivo.ToString());
            WriteLine("Invoice Date: " + order.Created_At.ToString());

            WriteLine(underLine);

            // Products
            WriteFourColumnLine("Product", "", "Qty", "Price");

            bFirst = true;
        }

        while (totalNumber < details.Count)
        {
            var d = details[totalNumber];

            var price = d.Price;
            var quantity = d.Quantity;

            var promotionName = d.PromotionName;
            double quantityPromotion = d.PromotionValue;
            double precioPromocion = d.PromotionLimit; // Price

            if (d.Product_Name != null)
            {
                if (d.Product_Name.Length > 25)
                {
                    WriteLine(d.Product_Name.Substring(0, 25).ToLower());
                }
                else
                {
                    WriteLine(d.Product_Name.ToLower());
                }
            }

            if (d.Peso > 0)
            {
                WriteTwoColumnLine(price.ToString("C") + "@" + d.Peso + "lb", (d.Peso * price).ToString("C"));
            }
            else
            {
                double sTotal = quantity * price;

                string promocion = "";

                // Verificar si es promoción
                if (!string.IsNullOrEmpty(promotionName))
                {
                    if (promotionName.Equals("Price"))
                    {
                        int vecesPromocion = (int)(quantity / quantityPromotion);

                        int productosRestantes = (int)(quantity % quantityPromotion);

                        sTotal = vecesPromocion * precioPromocion + productosRestantes * price;

                        promocion = $"{quantityPromotion}X{precioPromocion.ToString("C")}";
                    }
                }

                WriteFourColumnLine(promocion.Length > 0 ? "Prom" : "", 
                    promocion, quantity.ToString(), sTotal.ToString("C"));

            }

            totalNumber += 1;

            if (itemPerPage < itemTotalPage)
            {
                itemPerPage += 1;

                e.HasMorePages = false;
            }
            else
            {
                itemPerPage = 0;

                e.HasMorePages = true;

                return;

            }

        }

        if (details.Count == totalNumber)
        {
            WriteLine(underLine);

            WriteTwoColumnLine("Subtotal : ", order.SubTotal.ToString("C"));

            WriteTwoColumnLine("Tax: ", order.Total_Tax_Amount.ToString("C"));

            WriteTwoColumnLine("Desc: ", order.Total_Desc_Amount.ToString("C"));

            WriteTwoColumnLine("Total: ", order.Order_Amount.ToString("C"));

            WriteTwoColumnLine("Change: ", order.Devuelta.ToString("C"));

            WriteTwoColumnLine("Payment Method: ", order.Payment_Method);

            if(order.Balance > 0)
            {
                WriteTwoColumnLine("Balance Payment: ", order.Balance.ToString("C"));
            }

            WriteLine(underLine);

            WriteLine("Cashier - " + drawnBy);

            WriteLine(underLine);
            WriteLine(underLine);

            WriteLine(footerMsg);
        
            WriteLine(underLine);
            WriteLine(underLine);
        }
    }
    public void WriteLine(string text)
    {
        graphics.DrawString(text, font, Brushes.Black, margenIzquierda, currentY);

        NewLine();
    }

    public void NewLine()
    {
        currentY += 20;
    }

    public void WriteAtColumn(int x, string text)
    {
        graphics.DrawString(text, font, brush, x, currentY);
    }

    public void WriteTwoColumnLine(string leftText, string rightText)
    {
        WriteAtColumn(margenIzquierda, leftText);
        WriteAtColumn(margenIzquierda + 3 * (width / 6), rightText);
        NewLine();
    }

    public void WriteThreeColumnLine(string text1, string text2, string text3, bool bold = false, bool italic = true, int fontSize = 5)
    {
        WriteAtColumn(margenIzquierda, text1);
        WriteAtColumn(margenIzquierda + width / 4, text2);
        WriteAtColumn(margenIzquierda + 2 * (width / 4), text3);
        NewLine();
    }

    public void WriteFourColumnLine(string text1, string text2, string text3, string text4)
    {
        WriteAtColumn(margenIzquierda, text1);
        WriteAtColumn(margenIzquierda + width / 5, text2);
        WriteAtColumn(margenIzquierda + 2 * (width / 6), text3);
        WriteAtColumn(margenIzquierda + 3 * (width / 6), text4);
        NewLine();
    }

    public void WriteCenter(string text)
    {
        WriteAtColumn(margenIzquierda + width / 2 - text.Length / 2, text);
        NewLine();
    }

}
