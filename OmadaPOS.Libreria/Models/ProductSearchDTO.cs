namespace OmadaPOS.Models;

public class ProductSearchDTO
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public double? Price { get; set; }
    public string? Upc { get; set; }
    public string? Image { get; set; }
}
