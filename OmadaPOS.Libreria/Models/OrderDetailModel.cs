namespace OmadaPOS.Libreria.Models;

public class OrderDetailModel
{
    public int Product_Id { get; set; }
    public double Price { get; set; }
    public double Quantity { get; set; }
    public string? Product_Name { get; set; }
    public double Tax_Amount { get; set; }
    public double Peso { get; set; }
    public Product_Details? Product_Details { get; set; }
    public string PromotionName { get; set; } = "";
    public double PromotionValue { get; set; } = 0.0;
    public double PromotionLimit { get; set; } = 0.0;
}

public class Product_Details
{
    public string?CategoryName { get; set; }
}
