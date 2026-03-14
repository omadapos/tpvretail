namespace OmadaPOS.Libreria.Models;

public class OrderModel
{
    public int Id { get; set; }
    public int Consecutivo { get; set; }
    public double Order_Amount { get; set; }
    public double SubTotal { get; set; }
    public string? Payment_Status { get; set; }
    public string? Order_Status { get; set; }
    public double Total_Tax_Amount { get; set; }
    public double Total_Desc_Amount { get; set; }
    public string? Payment_Method { get; set; }
    public DateTime Created_At { get; set; }
    public int Branch_Id { get; set; }
    public double Devuelta { get; set; }
    public double Balance { get; set; }

    /// <summary>Cash Discount Program service fee recorded at time of sale.</summary>
    public double Service_Fee { get; set; }
}

