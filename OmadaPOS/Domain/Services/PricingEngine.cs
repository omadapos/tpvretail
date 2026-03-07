using OmadaPOS.Domain;
using OmadaPOS.Libreria.Models;
using OmadaPOS.Models;

namespace OmadaPOS.Domain.Services;

/// <summary>
/// Centralised pricing engine for the POS.
///
/// Calculation rules (applied in order):
///   1. Effective quantity = Peso (weight in kg) when > 0, otherwise Quantity.
///   2. Base line subtotal = effectiveQty × UnitPrice.
///   3. "Price" promotion (N items for $X):
///        promoGroups = ⌊effectiveQty / PromotionValue⌋
///        remainder   = effectiveQty mod PromotionValue
///        subtotal    = promoGroups × PromotionLimit + remainder × UnitPrice
///   4. Tax = subtotal × (Tax% / 100)
///   5. Total = subtotal + tax
///
/// Cart totals:
///   SubTotal = Σ subtotals
///   TotalTax = Σ taxes
///   TotalDiscount = (SubTotal + TotalTax) × 0.30  when applyDiscount = true
///   Total = SubTotal + TotalTax − TotalDiscount
/// </summary>
public sealed class PricingEngine : IPricingEngine
{
    // ─── Line calculation ─────────────────────────────────────────────────────

    public void ApplyPricing(CartItem item)
    {
        var (subtotal, tax, total) = ComputeLine(item);
        item.Subtotal   = subtotal;
        item.TaxAmount  = tax;
        item.Total      = total;
    }

    // ─── Cart aggregation ─────────────────────────────────────────────────────

    public CartPricingResult ComputeCartTotals(IReadOnlyList<CartItem> items, bool applyDiscount)
    {
        var cartModels = new List<CartModel>(items.Count);
        decimal subTotal = 0m;
        decimal totalTax = 0m;

        foreach (var item in items)
        {
            ApplyPricing(item);

            cartModels.Add(BuildCartModel(item));

            subTotal += item.Subtotal;
            totalTax += item.TaxAmount;
        }

        decimal totalDiscount = (subTotal + totalTax) * DiscountPolicy.GetRate(applyDiscount);
        decimal total         = subTotal + totalTax - totalDiscount;

        return new CartPricingResult(subTotal, totalTax, totalDiscount, total, cartModels);
    }

    // ─── Private helpers ──────────────────────────────────────────────────────

    private static (decimal subtotal, decimal tax, decimal total) ComputeLine(CartItem item)
    {
        // Rule 1 — Effective quantity respects weight-based items
        double effectiveQty = (item.Peso.HasValue && item.Peso > 0)
            ? item.Peso.Value
            : item.Quantity;

        // Rule 2 — Base subtotal
        decimal subtotal = (decimal)effectiveQty * item.UnitPrice;

        // Rule 3 — "Price" promotion: N for $X (e.g. 2 for $5)
        if (!string.IsNullOrEmpty(item.PromotionName) &&
            item.PromotionName.Equals("Price", StringComparison.OrdinalIgnoreCase) &&
            item.PromotionValue > 0)
        {
            int promoGroups = (int)(effectiveQty / item.PromotionValue);
            int remainder   = (int)(effectiveQty % item.PromotionValue);

            subtotal = promoGroups * item.PromotionLimit
                     + remainder   * item.UnitPrice;
        }

        // Rules 4 & 5
        decimal tax   = subtotal * (decimal)(item.Tax / 100.0);
        decimal total = subtotal + tax;

        return (subtotal, tax, total);
    }

    private static CartModel BuildCartModel(CartItem item) => new()
    {
        Product_Id      = item.ProductId,
        Quantity        = item.Quantity,
        Price           = item.UnitPrice,
        Discount_Amount = 0,
        Tax_Amount      = (decimal)(item.Tax / 100.0),
        Name            = item.ProductName,
        Peso            = item.Peso ?? 0,
        PromotionName   = item.PromotionName ?? string.Empty,
        PromotionValue  = item.PromotionValue,
        PromotionLimit  = item.PromotionLimit
    };
}
