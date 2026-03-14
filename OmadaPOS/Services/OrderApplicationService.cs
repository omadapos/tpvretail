using OmadaPOS.Domain.Services;
using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Utils;
using OmadaPOS.Models;

namespace OmadaPOS.Services;

public interface IOrderApplicationService
{
    /// <summary>
    /// Builds a PlaceOrderModel from the current cart items.
    /// Returns null when the computed total is zero or negative.
    /// </summary>
    PlaceOrderModel? BuildOrderModel(
        IReadOnlyList<CartItem> items,
        decimal changeValue,
        string terminal,
        string paymentMethod,
        decimal balance,
        bool applyDiscount);

    /// <summary>
    /// Builds a PlaceOrderMultipleModel for split-payment orders.
    /// Returns null when the computed total is zero or negative.
    /// </summary>
    PlaceOrderMultipleModel? BuildMultipleOrderModel(
        IReadOnlyList<CartItem> items,
        decimal changeValue,
        string terminal,
        List<PlaceOrderPayment> payments,
        decimal balance,
        bool applyDiscount);
}

/// <summary>
/// Constructs the order payload sent to the API.
/// All pricing is delegated to <see cref="IPricingEngine"/> — this class
/// never performs calculations directly.
/// </summary>
public class OrderApplicationService : IOrderApplicationService
{
    private readonly IPricingEngine _pricing;

    public OrderApplicationService(IPricingEngine pricingEngine)
    {
        _pricing = pricingEngine ?? throw new ArgumentNullException(nameof(pricingEngine));
    }

    public PlaceOrderModel? BuildOrderModel(
        IReadOnlyList<CartItem> items,
        decimal changeValue,
        string terminal,
        string paymentMethod,
        decimal balance,
        bool applyDiscount)
    {
        var totals = _pricing.ComputeCartTotals(items, applyDiscount);

        if (totals.Total <= 0)
            return null;

        return new PlaceOrderModel
        {
            SubTotal             = (double)totals.SubTotal,
            Order_Amount         = (double)totals.Total,
            Total_Tax_Amount     = (double)totals.TotalTax,
            Total_Desc_Amount    = (double)totals.TotalDiscount,
            Order_Note           = string.Empty,
            Order_Status         = "confirmed",
            Order_Type           = "pos",
            Payment_Method       = paymentMethod,
            Branch_Id            = SessionManager.BranchId ?? 0,
            Table_Id             = 1,
            Number_Of_People     = 1,
            Payment_Status       = "paid",
            Customer_Id          = Constants.CUSTOMERID,
            Cart                 = totals.CartModels,
            Devuelta             = (double)changeValue,
            Terminal             = terminal,
            UserName             = SessionManager.UserName,
            Balance              = balance
        };
    }

    public PlaceOrderMultipleModel? BuildMultipleOrderModel(
        IReadOnlyList<CartItem> items,
        decimal changeValue,
        string terminal,
        List<PlaceOrderPayment> payments,
        decimal balance,
        bool applyDiscount)
    {
        var totals = _pricing.ComputeCartTotals(items, applyDiscount);

        if (totals.Total <= 0)
            return null;

        return new PlaceOrderMultipleModel
        {
            SubTotal             = (double)totals.SubTotal,
            Order_Amount         = (double)totals.Total,
            Total_Tax_Amount     = (double)totals.TotalTax,
            Total_Desc_Amount    = (double)totals.TotalDiscount,
            Order_Note           = string.Empty,
            Order_Status         = "confirmed",
            Order_Type           = "pos",
            Payment_Method       = "Multiple",
            Branch_Id            = SessionManager.BranchId ?? 0,
            Table_Id             = 1,
            Number_Of_People     = 1,
            Payment_Status       = "paid",
            Customer_Id          = Constants.CUSTOMERID,
            Cart                 = totals.CartModels,
            Devuelta             = (double)changeValue,
            Terminal             = terminal,
            Payments             = payments,
            UserName             = SessionManager.UserName,
            Balance              = balance
        };
    }
}
