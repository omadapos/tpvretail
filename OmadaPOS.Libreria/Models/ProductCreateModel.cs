namespace OmadaPOS.Libreria.Models;

public class ProductCreateModel
{
    public string? Name { get; set; }
    public string? Upc { get; set; }
    public string? Ean { get; set; }
    public string? Short_Name { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
    public double Price { get; set; } = 0.0;
    public double Tax { get; set; } = 0.0;
    public string? Available_Time_Starts { get; set; }
    public string? Available_Time_Ends { get; set; }
    public int? Status { get; set; }
    public string[]? Attributes { get; set; }
    public int Category_Ids { get; set; }
    public double? Discount { get; set; }
    public string? Discount_Type { get; set; }
    public string? Tax_Type { get; set; }
    public int? Set_Menu { get; set; }
    public int? Popularity_Count { get; set; }
    public string? Product_Type { get; set; }
    public int? BranchId { get; set; }
    public string? Base64dataImage { get; set; }
    public int CategoryId { get; set; }
    public bool Display_Addons { get; set; }
    public bool Display_Sides { get; set; }
    public bool Ebt { get; set; }
    public bool Wic { get; set; }
    public double Cost { get; set; } = 0.0;
    public double? Stock { get; set; }
}
