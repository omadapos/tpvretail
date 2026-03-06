using OmadaPOS.Libreria.Utils;

namespace OmadaPOS.Domain;

/// <summary>
/// Applies branch-specific surcharges to a payment amount (in cents).
/// Previously hardcoded as an inline if-block inside frmHome.PaymentOrder()
/// tied to a specific BranchId. Centralizing here makes the rule visible,
/// testable, and easy to extend for additional branches.
/// </summary>
public static class SurchargePolicy
{
    /// <summary>
    /// Returns the final amount in cents after applying any applicable surcharge.
    /// Branch 45 (Cintron Supermarket) adds a 3.8% surcharge to Credit card payments.
    /// </summary>
    public static decimal Apply(decimal amountInCents, PaymentType paymentType, int? branchId)
    {
        if (branchId == 45 && paymentType == PaymentType.Credit)
            return amountInCents + (amountInCents * 0.038m);

        return amountInCents;
    }
}
