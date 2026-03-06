namespace OmadaPOS.Libreria.Models;

public class GiftCardDTO
{
    public int Id { get; set; }
    public string? CardNo { get; set; }
    public double? Value { get; set; }
    public double? Balance { get; set; }
    public int? CustomerId { get; set; }
    public DateTime? Expiry { get; set; }
    public int? CreatedBy { get; set; }
    public int? BranchId { get; set; }
}
