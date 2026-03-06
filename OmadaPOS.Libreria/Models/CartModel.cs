namespace OmadaPOS.Libreria.Models;

public class CartModel
{
    public int Product_Id { get; set; }
    public decimal Price { get; set; }
    public decimal Discount_Amount { get; set; }
    public double Quantity { get; set; }
    public decimal Tax_Amount { get; set; }
    public string? Name { get; set; }
    public double Peso { get; set; }
    public string? PromotionName { get; set; }
    public double PromotionValue { get; set; }
    public decimal PromotionLimit { get; set; }
}
