namespace OmadaPOS.Domain;

/// <summary>
/// Centralises the employee / special discount rules applied at the cart level.
///
/// The rate is extracted here instead of being hardcoded inside PricingEngine so
/// that it can be changed (or loaded from AdminSetting in the future) in a single place.
/// </summary>
public static class DiscountPolicy
{
    /// <summary>
    /// Standard employee / promotional discount applied when the cashier enables
    /// the discount toggle. Currently 30%.
    /// </summary>
    public const decimal StandardRate = 0.30m;

    /// <summary>
    /// Returns the discount rate to apply to the cart total.
    /// </summary>
    /// <param name="applyDiscount">True when the cashier has activated the discount.</param>
    public static decimal GetRate(bool applyDiscount) => applyDiscount ? StandardRate : 0m;
}
