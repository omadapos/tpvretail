using OmadaPOS.Models;

namespace OmadaPOS.Domain;

/// <summary>
/// Single source of truth for N-for-$X "Price" promotion line-total calculation.
/// Previously duplicated in: frmHome.Process(), frmHome.ProcessMultiple(),
/// CartItem.Subtotal getter, and frmCustomerScreen.CalculateItemTotal().
/// </summary>
public static class PromotionCalculator
{
    /// <summary>
    /// Returns the effective line total for a cart item, applying promotion rules.
    /// Uses Peso (weight in kg) as the effective quantity when the item is sold by weight.
    /// </summary>
    public static decimal CalculateLineTotal(CartItem cart)
    {
        double quantity = cart.Quantity;
        if (cart.Peso > 0)
            quantity = cart.Peso ?? 0;

        decimal price = cart.UnitPrice;
        decimal total = (decimal)quantity * price;

        if (!string.IsNullOrEmpty(cart.PromotionName) && cart.PromotionName.Equals("Price"))
        {
            double quantityPromotion = cart.PromotionValue;
            decimal precioPromocion = cart.PromotionLimit;

            int vecesPromocion = (int)(quantity / quantityPromotion);
            int productosRestantes = (int)(quantity % quantityPromotion);

            total = vecesPromocion * precioPromocion + productosRestantes * price;
        }

        return total;
    }
}
