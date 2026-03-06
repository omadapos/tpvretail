namespace OmadaPOS.Libreria.Models;

public class PlaceOrderMultipleModel
{
    public double SubTotal { get; set; }
    public double Order_Amount { get; set; }
    public double Total_Tax_Amount { get; set; }
    public double Total_Desc_Amount { get; set; }
    public string? Payment_Method { get; set; }
    public string? Order_Note { get; set; }
    public string? Order_Status { get; set; }
    public int Branch_Id { get; set; }
    public int Table_Id { get; set; }
    public int Number_Of_People { get; set; }
    public string? Payment_Status { get; set; }
    public string? Order_Type { get; set; }
    public int Customer_Id { get; set; }
    public List<CartModel>? Cart { get; set; }
    public double Devuelta { get; set; }
    public string? Terminal { get; set; }
    public string? UserName { get; set; }
    public List<PlaceOrderPayment>? Payments { get; set; }
    public decimal Balance { get; set; } = 0;
}
