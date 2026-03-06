using OmadaPOS.Libreria.Models;
using OmadaPOS.Models;

namespace OmadaPOS.Domain;

/// <summary>
/// Holds the computed totals and the CartModel list built from a cart session.
/// </summary>
public record CartTotals(
    List<CartModel> CartModels,
    decimal SubTotal,
    decimal TotalTax,
    decimal TotalDesc,
    decimal Total);

/// <summary>
/// Builds the CartModel list and calculates subtotal, tax, discount, and total
/// for a set of cart items.
/// Previously duplicated verbatim inside frmHome.Process() and frmHome.ProcessMultiple().
/// </summary>
public static class CartCalculator
{
    public static CartTotals Calculate(IReadOnlyList<CartItem> items, bool applyDiscount)
    {
        var cartModels = new List<CartModel>();
        decimal subTotal = 0m;
        decimal totalTax = 0m;

        foreach (var cart in items)
        {
            cartModels.Add(new CartModel
            {
                Product_Id = cart.ProductId,
                Quantity = cart.Quantity,
                Price = cart.UnitPrice,
                Discount_Amount = 0,
                Tax_Amount = (decimal)(cart.Tax / 100.0),
                Name = cart.ProductName,
                Peso = cart.Peso ?? 0,
                PromotionName = cart.PromotionName ?? "",
                PromotionValue = cart.PromotionValue,
                PromotionLimit = cart.PromotionLimit
            });

            decimal lineTotal = PromotionCalculator.CalculateLineTotal(cart);
            subTotal += lineTotal;
            totalTax += lineTotal * (decimal)(cart.Tax / 100.0);
        }

        decimal totalDesc = 0m;
        if (applyDiscount)
            totalDesc = (subTotal + totalTax) * 0.30m;

        decimal total = subTotal + totalTax - totalDesc;

        return new CartTotals(cartModels, subTotal, totalTax, totalDesc, total);
    }
}
