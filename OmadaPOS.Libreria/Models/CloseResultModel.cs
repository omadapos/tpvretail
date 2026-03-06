namespace OmadaPOS.Libreria.Models;

public class CloseResultModel
{
    public CierreDiarioStore? Store { get; set; }
    public int OrderInitial { get; set; }
    public int OrderFinal { get; set; }
    public List<CierreDiarioPayment>? Payment { get; set; }
}

public class CierreDiarioStore
{
    public string? Name { get; set; }
    public string? Address { get; set; }
}

public class CierreDiarioPayment
{
    public string? Terminal { get; set; }
    public double Cash { get; set; }
    public double Card { get; set; }
    public double Debit { get; set; }
    public double EBT { get; set; }
    public double GiftCard { get; set; }
}