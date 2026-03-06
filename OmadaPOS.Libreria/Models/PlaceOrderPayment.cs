namespace OmadaPOS.Libreria.Models;

public class PlaceOrderPayment
{
    public string? Tipo { get; set; }
    public decimal Total { get; set; }
    public string? PaymentNumber { get; set; }
    public string? PaymentCardHolder { get; set; }
    public string? PaymentCardType { get; set; }
    public string? PaymentReferenceNumber { get; set; }
}
