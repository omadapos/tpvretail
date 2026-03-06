namespace OmadaPOS.Libreria.Models;

public class PaymentRequest
{
    public string Ip { get; set; } = string.Empty;
    public int Port { get; set; }
    public string EcrRefNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Terminal { get; set; } = string.Empty;
}