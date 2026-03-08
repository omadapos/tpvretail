using Microsoft.Extensions.DependencyInjection;
using OmadaPOS.Libreria.Models;
using OmadaPOS.Views;

namespace OmadaPOS.Services.Navigation;

public class WindowService : IWindowService
{
    private readonly IServiceProvider _provider;
    private readonly List<Form>       _modelessForms = [];

    public WindowService(IServiceProvider provider)
    {
        _provider = provider;
    }

    public void CloseAllModeless()
    {
        foreach (var form in _modelessForms.ToList())
        {
            if (!form.IsDisposed)
                form.Close();
        }
        _modelessForms.Clear();
    }

    public void OpenHome()
    {
        _provider.GetRequiredService<frmHome>().Show();
    }

    public void OpenSignIn()
    {
        _provider.GetRequiredService<frmSignIn>().Show();
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

    public void OpenSettings(IWin32Window? owner = null)
        => ShowDialog(CreateForm<frmSetting>(), owner);

    public void OpenDailyClose(IWin32Window? owner = null)
        => ShowDialog(CreateForm<frmCierreDiario>(), owner);

    public void OpenPopupQuantity(int number, int productId, IWin32Window? owner = null)
        => ShowDialog(CreateForm<frmPopupQuantity>(number, productId), owner);

    public void OpenKeyLookup(IWin32Window? owner = null)
        => ShowModeless(CreateForm<frmKeyLookup>());

    public void OpenPopupCashPayment(int orderId, int consecutivo, decimal devuelta, IWin32Window? owner = null, PaymentResponseModel? paymentResponse = null, List<PaymentModel>? splitPayments = null)
    {
        // ActivatorUtilities cannot resolve typed null params — build arg list explicitly
        var args = new List<object> { (object)orderId, consecutivo, devuelta };
        if (paymentResponse is not null) args.Add(paymentResponse);
        if (splitPayments   is not null) args.Add(splitPayments);

        var form = CreateForm<frmPopupCashPayment>([.. args]);
        ShowModeless(form);
    }

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

    private void ShowModeless(Form form)
    {
        _modelessForms.Add(form);
        form.FormClosed += (_, _) => _modelessForms.Remove(form);
        form.Show();
    }
}
