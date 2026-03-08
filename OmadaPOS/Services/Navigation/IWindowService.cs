using OmadaPOS.Views;

namespace OmadaPOS.Services.Navigation;

public interface IWindowService
{
    void OpenHome();
    void OpenSignIn();
    void OpenSplitPayment(IWin32Window? owner = null);
    void OpenHold(IWin32Window? owner = null, Action? onClosed = null);
    void OpenCheckPrice(IWin32Window? owner = null);
    void OpenProductNew(bool applyTax, IWin32Window? owner = null);
    void OpenSettings(IWin32Window? owner = null);
    void OpenDailyClose(IWin32Window? owner = null);
    void OpenPopupQuantity(int number, int productId, IWin32Window? owner = null);
    void OpenKeyLookup(IWin32Window? owner = null);
    void OpenPopupCashPayment(int orderId, int consecutivo, decimal devuelta, IWin32Window? owner = null);
    void OpenGiftCard(decimal totalGlobal, int tipo, IWin32Window? owner = null);
    void OpenPaymentStatus(string message, IWin32Window? owner = null);
    void OpenPrintInvoice(IWin32Window? owner = null);
    void OpenProductNoExist(string upc, IWin32Window? owner = null);
    void OpenError(string message, IWin32Window? owner = null);
}
