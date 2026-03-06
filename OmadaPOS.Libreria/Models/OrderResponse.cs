namespace OmadaPOS.Libreria.Models;

public class OrderResponse
{
    public int Order_Id { get; set; }
    public string? Branch_Table_Token { get; set; }
    public string? Message { get; set; }
    public int Consecutivo { get; set; } = 0;
    
}
