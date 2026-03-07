using Microsoft.Extensions.DependencyInjection;
using OmadaPOS.Views;

namespace OmadaPOS.Services.Navigation;

public class WindowService : IWindowService
{
    private readonly IServiceProvider _provider;

    public WindowService(IServiceProvider provider)
    {
        _provider = provider;
    }

    public frmHome OpenHome()
    {
        var form = _provider.GetRequiredService<frmHome>();
        form.Show();
        return form;
    }

    public frmSignIn OpenSignIn()
    {
        var form = _provider.GetRequiredService<frmSignIn>();
        form.Show();
        return form;
    }

    public void OpenSplitPayment(IWin32Window? owner = null)
        => ShowModeless(CreateForm<frmSplit>());

    public void OpenHold(IWin32Window? owner = null, Action? onClosed = null)
    {
        var form = CreateForm<frmHold>();
        if (onClosed != null)
            form.FormClosed += (_, _) => onClosed();

        ShowModeless(form);
    }

    public void OpenCheckPrice(IWin32Window? owner = null)
        => ShowDialog(CreateForm<frmCheckPrice>(), owner);

    public void OpenProductNew(bool applyTax, IWin32Window? owner = null)
        => ShowModeless(CreateForm<frmProductNew>(applyTax));

    public void OpenCustomerScreen()
    {
        var form = CreateForm<frmCustomerScreen>();
        form.StartPosition = FormStartPosition.Manual;

        if (Screen.AllScreens.Length > 1)
        {
            var secondary = Screen.AllScreens[1];
            form.Bounds = secondary.Bounds;
        }

        ShowModeless(form);
    }

    public void OpenSettings(IWin32Window? owner = null)
        => ShowDialog(CreateForm<frmSetting>(), owner);

    public void OpenDailyClose(IWin32Window? owner = null)
        => ShowDialog(CreateForm<frmCierreDiario>(), owner);

    public void OpenPopupQuantity(int number, int productId, IWin32Window? owner = null)
        => ShowDialog(CreateForm<frmPopupQuantity>(number, productId), owner);

    public void OpenKeyLookup(IWin32Window? owner = null)
        => ShowModeless(CreateForm<frmKeyLookup>());

    public void OpenPopupCashPayment(int orderId, int consecutivo, decimal devuelta, IWin32Window? owner = null)
        => ShowModeless(CreateForm<frmPopupCashPayment>(orderId, consecutivo, devuelta));

    public void OpenGiftCard(decimal totalGlobal, int tipo, IWin32Window? owner = null)
        => ShowModeless(CreateForm<frmGiftCard>(totalGlobal, tipo));

    public void OpenPaymentStatus(string message, IWin32Window? owner = null)
        => ShowDialog(CreateForm<frmPaymentStatus>(message), owner);

    public void OpenPrintInvoice(IWin32Window? owner = null)
        => ShowDialog(CreateForm<frmPrintInvoice>(), owner);

    public void OpenProductNoExist(string upc, IWin32Window? owner = null)
        => ShowDialog(CreateForm<frmProductNoExist>(upc), owner);

    public void OpenError(string message, IWin32Window? owner = null)
        => ShowDialog(CreateForm<frmError>(message), owner);

    private T CreateForm<T>(params object[] args) where T : Form
        => ActivatorUtilities.CreateInstance<T>(_provider, args);

    private static void ShowDialog(Form form, IWin32Window? owner)
    {
        if (owner != null)
            form.ShowDialog(owner);
        else
            form.ShowDialog();
    }

    private static void ShowModeless(Form form) => form.Show();
}
