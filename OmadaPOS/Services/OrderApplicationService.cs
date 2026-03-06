using OmadaPOS.Domain;
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
/// Constructs the order payload that is sent to the API.
/// Previously this logic lived inside frmHome.Process() and frmHome.ProcessMultiple()
/// as ~190 lines of duplicated calculation code.
/// </summary>
public class OrderApplicationService : IOrderApplicationService
{
    public PlaceOrderModel? BuildOrderModel(
        IReadOnlyList<CartItem> items,
        decimal changeValue,
        string terminal,
        string paymentMethod,
        decimal balance,
        bool applyDiscount)
    {
        var totals = CartCalculator.Calculate(items, applyDiscount);

        if (totals.Total <= 0)
            return null;

        return new PlaceOrderModel
        {
            SubTotal = (double)totals.SubTotal,
            Order_Amount = (double)totals.Total,
            Total_Tax_Amount = (double)totals.TotalTax,
            Total_Desc_Amount = (double)totals.TotalDesc,
            Order_Note = "",
            Order_Status = "confirmed",
            Order_Type = "delivery",
            Payment_Method = paymentMethod,
            Branch_Id = SessionManager.BranchId ?? 0,
            Table_Id = 1,
            Number_Of_People = 1,
            Payment_Status = "paid",
            Customer_Id = Constants.CUSTOMERID,
            Cart = totals.CartModels,
            Devuelta = (double)changeValue,
            Terminal = terminal,
            UserName = SessionManager.UserName,
            Balance = balance
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
        var totals = CartCalculator.Calculate(items, applyDiscount);

        if (totals.Total <= 0)
            return null;

        return new PlaceOrderMultipleModel
        {
            SubTotal = (double)totals.SubTotal,
            Order_Amount = (double)totals.Total,
            Total_Tax_Amount = (double)totals.TotalTax,
            Total_Desc_Amount = (double)totals.TotalDesc,
            Order_Note = "",
            Order_Status = "confirmed",
            Order_Type = "delivery",
            Payment_Method = "Multiple",
            Branch_Id = SessionManager.BranchId ?? 0,
            Table_Id = 1,
            Number_Of_People = 1,
            Payment_Status = "paid",
            Customer_Id = Constants.CUSTOMERID,
            Cart = totals.CartModels,
            Devuelta = (double)changeValue,
            Terminal = terminal,
            Payments = payments,
            UserName = SessionManager.UserName,
            Balance = balance
        };
    }
}
