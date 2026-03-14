using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Utils;

namespace OmadaPOS.Domain;

/// <summary>
/// Applies the Cash Discount Program service fee to credit card payments.
///
/// When a branch has CashDiscountEnabled = true, credit card transactions
/// carry a 3.8% service fee. Cash, Debit, EBT and Gift Card are exempt.
///
/// Amounts in cents are used for PAX terminal calls (Apply);
/// amounts in dollars are used for order model and receipt (GetFeeAmount).
/// </summary>
public static class SurchargePolicy
{
    /// <summary>Fixed service-fee rate for the Cash Discount Program.</summary>
    public const decimal Rate = 0.038m;

    /// <summary>
    /// Returns the final amount in CENTS after applying the service fee
    /// when appropriate. Used when building the PAX terminal payment request.
    /// </summary>
    public static decimal Apply(decimal amountInCents, PaymentType paymentType)
    {
        if (SessionManager.CashDiscountEnabled && paymentType == PaymentType.Credit)
            return amountInCents + (amountInCents * Rate);

        return amountInCents;
    }

    /// <summary>
    /// Returns the service fee in DOLLARS for the given order total.
    /// Returns 0 when the payment method is exempt or the program is disabled.
    /// </summary>
    public static decimal GetFeeAmount(decimal orderTotal, PaymentType paymentType)
    {
        if (SessionManager.CashDiscountEnabled && paymentType == PaymentType.Credit)
            return Math.Round(orderTotal * Rate, 2);

        return 0m;
    }
}
