using OmadaPOS.Libreria.Models;
using OmadaPOS.Models;

namespace OmadaPOS.Domain.Services;

/// <summary>
/// Result of pricing a single cart line.
/// </summary>
public sealed record LineResult(decimal Subtotal, decimal Tax, decimal Total);

/// <summary>
/// Result of pricing an entire cart, ready to be sent to the API.
/// </summary>
public sealed record CartPricingResult(
    decimal SubTotal,
    decimal TotalTax,
    decimal TotalDiscount,
    decimal Total,
    List<CartModel> CartModels);

/// <summary>
/// Single source of truth for all price, promotion, weight and tax calculations in the POS.
/// Replaces the duplicated logic that previously existed in:
///   - CartItem.Subtotal (promotion math, no Peso support)
///   - PromotionCalculator.CalculateLineTotal (promotion math + Peso support)
///   - CartCalculator.Calculate (total aggregation)
/// </summary>
public interface IPricingEngine
{
    /// <summary>
    /// Computes subtotal, tax and total for a single cart line and stores the
    /// results back into the item's Subtotal / TaxAmount / Total properties.
    /// </summary>
    void ApplyPricing(CartItem item);

    /// <summary>
    /// Aggregates per-line pricing for every item in the cart and applies an
    /// optional 30% discount, returning the complete pricing result together
    /// with the CartModel list required by the order API.
    /// Discount rate comes from <see cref="OmadaPOS.Domain.DiscountPolicy"/>.
    /// </summary>
    CartPricingResult ComputeCartTotals(IReadOnlyList<CartItem> items, bool applyDiscount);
}
