namespace OmadaPOS.Libreria.Models;

public class ProductModel
{
    public int? Id { get; set; }
    public string? UPC { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public string? Base64dataImage { get; set; }
    public int? BranchId { get; set; }
    public string? Image { get; set; }
    public int? Status { get; set; }
    public double? Tax { get; set; }
    public int CategoryId { get; set; }
    public bool? DisplayAddons { get; set; }
    public bool? DisplaySides { get; set; }
    public string? ProductType { get; set; }
    public string PromotionName { get; set; } = string.Empty;
    public double PromotionValue { get; set; } = 0.0;
    public decimal PromotionLimit { get; set; } = 0m;
    public bool Ebt { get; set; } = false;
    /// <summary>
    /// True when the product requires age verification (≥21) at point of sale.
    /// Defaults to false until the backend natively returns this flag;
    /// the local <see cref="IAgeRestrictionConfigService"/> whitelist is used in the interim.
    /// </summary>
    public bool RequiresAgeVerification { get; set; } = false;
}
