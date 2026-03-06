namespace OmadaPOS.Models;

public class CartItem
{
    public int Number { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public double Quantity { get; set; }
    public double Weight { get; set; }
    public decimal UnitPrice { get; set; }
    public double Tax { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public string? PromotionName { get; set; }
    public double PromotionValue { get; set; }
    public decimal PromotionLimit { get; set; }
    public bool IsEBT { get; set; }
    public string? UPC { get; set; }
    public string? Image { get; set; }
    public double? Peso { get; set; }

    public decimal Subtotal
    {
        get
        {
            decimal total = UnitPrice * (decimal)Quantity;
            
            if (!string.IsNullOrEmpty(PromotionName))
            {
                if (PromotionName.Equals("Price", StringComparison.OrdinalIgnoreCase))
                {
                    int promotionTimes = (int)(Quantity / PromotionValue);
                    int remainingItems = (int)(Quantity % PromotionValue);
                    
                    total = (promotionTimes * PromotionLimit) + (remainingItems * UnitPrice);
                }
            }
            
            return total;
        }
    }

    public decimal TaxAmount => Subtotal * (decimal)(Tax / 100.0);
    public decimal Total => Subtotal + TaxAmount;

    public CartItem Clone()
    {
        return new CartItem
        {
            Number = this.Number,
            ProductId = this.ProductId,
            ProductName = this.ProductName,
            Quantity = this.Quantity,
            Weight = this.Weight,
            UnitPrice = this.UnitPrice,
            Tax = this.Tax,
            Date = this.Date,
            PromotionName = this.PromotionName,
            PromotionValue = this.PromotionValue,
            PromotionLimit = this.PromotionLimit,
            IsEBT = this.IsEBT,
            UPC = this.UPC,
            Image = this.Image,
            Peso = this.Peso
        };
    }
}
