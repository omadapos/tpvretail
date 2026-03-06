namespace OmadaPOS.Libreria.Models;

public class BannerModel
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Image { get; set; }
    public bool? Status { get; set; }
    public int? Product_Id { get; set; }
    public int? Category_Id { get; set; }
}
