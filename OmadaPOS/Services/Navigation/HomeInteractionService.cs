namespace OmadaPOS.Services.Navigation;

public class HomeInteractionService : IHomeInteractionService
{
    private Action<int, int>? _changeQuantityHandler;
    private Func<bool, decimal, Task>? _addCustomProductHandler;
    private Func<string, Task>? _searchProductHandler;
    private Func<Task>? _splitPaymentCompletedHandler;
    private Action? _refreshCartDisplayHandler;
    private Func<Task>? _giftCardPaymentHandler;

    public void RegisterHandlers(
        Action<int, int> changeQuantityHandler,
        Func<bool, decimal, Task> addCustomProductHandler,
        Func<string, Task> searchProductHandler,
        Func<Task> splitPaymentCompletedHandler,
        Action refreshCartDisplayHandler)
    {
        _changeQuantityHandler = changeQuantityHandler;
        _addCustomProductHandler = addCustomProductHandler;
        _searchProductHandler = searchProductHandler;
        _splitPaymentCompletedHandler = splitPaymentCompletedHandler;
        _refreshCartDisplayHandler = refreshCartDisplayHandler;
    }

    public void RegisterGiftCardHandler(Func<Task> handler)
        => _giftCardPaymentHandler = handler;

    public void ClearHandlers()
    {
        _changeQuantityHandler = null;
        _addCustomProductHandler = null;
        _searchProductHandler = null;
        _splitPaymentCompletedHandler = null;
        _refreshCartDisplayHandler = null;
        _giftCardPaymentHandler = null;
    }

    public void RequestQuantityChange(int quantity, int productId)
        => _changeQuantityHandler?.Invoke(quantity, productId);

    public Task RequestCustomProductAsync(bool applyTax, decimal price)
        => _addCustomProductHandler?.Invoke(applyTax, price) ?? Task.CompletedTask;

    public Task RequestProductSearchAsync(string code)
        => _searchProductHandler?.Invoke(code) ?? Task.CompletedTask;

    public Task RequestSplitPaymentCompletionAsync()
        => _splitPaymentCompletedHandler?.Invoke() ?? Task.CompletedTask;

    public void RequestCartRefresh()
        => _refreshCartDisplayHandler?.Invoke();

    public Task RequestGiftCardPaymentAsync()
        => _giftCardPaymentHandler?.Invoke() ?? Task.CompletedTask;
}
