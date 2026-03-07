namespace OmadaPOS.Services.Navigation;

public interface IHomeInteractionService
{
    void RegisterHandlers(
        Action<int, int> changeQuantityHandler,
        Func<bool, decimal, Task> addCustomProductHandler,
        Func<string, Task> searchProductHandler,
        Func<Task> splitPaymentCompletedHandler,
        Action refreshCartDisplayHandler);

    /// <summary>Registers the handler that frmHome uses to place a Gift Card order.</summary>
    void RegisterGiftCardHandler(Func<Task> handler);

    void ClearHandlers();
    void RequestQuantityChange(int quantity, int productId);
    Task RequestCustomProductAsync(bool applyTax, decimal price);
    Task RequestProductSearchAsync(string code);
    Task RequestSplitPaymentCompletionAsync();
    void RequestCartRefresh();

    /// <summary>Called by frmGiftCard once the card balance is verified and deducted.</summary>
    Task RequestGiftCardPaymentAsync();
}
