namespace OmadaPOS.Libreria.Models;

public class PaymentResponseModel
{
    public bool Success { get; set; }
    public string? PaymentNumber { get; set; }
    public string? PaymentCardHolder { get; set; }
    public string? PaymentCardType { get; set; }
    public string? PaymentReferenceNumber { get; set; }
    public string? MsgInfo { get; set; }
    public decimal Balance { get; set; }
}
