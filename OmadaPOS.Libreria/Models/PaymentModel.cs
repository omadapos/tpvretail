namespace OmadaPOS.Libreria.Models;

public class PaymentModel
{
    public string? PaymentId { get; set; }
    public decimal Total { get; set; }
    public string PaymentType { get; set; } = string.Empty;
}