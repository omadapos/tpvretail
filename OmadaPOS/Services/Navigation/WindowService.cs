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
        var form = _provider.GetRequiredService<frmHome>();
        ShowModeless(form);
    }

    public void OpenSignIn()
    {
        var form = _provider.GetRequiredService<frmSignIn>();
        ShowModeless(form);
    }

    public void OpenSplitPayment(IWin32Window? owner = null)
        => ShowModeless(CreateForm<frmSplit>(), owner);

    public void OpenHold(IWin32Window? owner = null, Action? onClosed = null)
    {
        var form = CreateForm<frmHold>();
        if (onClosed != null)
            form.FormClosed += (_, _) => onClosed();

        ShowModeless(form, owner);
    }

    public void OpenCheckPrice(IWin32Window? owner = null)
        => ShowDialog(CreateForm<frmCheckPrice>(), owner);

    public void OpenProductNew(bool applyTax, IWin32Window? owner = null)
        => ShowModeless(CreateForm<frmProductNew>(applyTax), owner);

    public void OpenSettings(IWin32Window? owner = null)
        => ShowDialog(CreateForm<frmSetting>(), owner);

    public void OpenDailyClose(IWin32Window? owner = null)
        => ShowDialog(CreateForm<frmCierreDiario>(), owner);

    public void OpenPopupQuantity(int number, int productId, IWin32Window? owner = null)
        => ShowDialog(CreateForm<frmPopupQuantity>(number, productId), owner);

    public void OpenKeyLookup(IWin32Window? owner = null)
        => ShowModeless(CreateForm<frmKeyLookup>(), owner);

    public void OpenPopupCashPayment(int orderId, int consecutivo, decimal devuelta, IWin32Window? owner = null, PaymentResponseModel? paymentResponse = null, List<PaymentModel>? splitPayments = null)
    {
        // ActivatorUtilities cannot resolve typed null params — build arg list explicitly
        var args = new List<object> { (object)orderId, consecutivo, devuelta };
        if (paymentResponse is not null) args.Add(paymentResponse);
        if (splitPayments   is not null) args.Add(splitPayments);

        var form = CreateForm<frmPopupCashPayment>([.. args]);
        ShowModeless(form, owner);
    }

    public void OpenGiftCard(decimal totalGlobal, int tipo, IWin32Window? owner = null)
        => ShowModeless(CreateForm<frmGiftCard>(totalGlobal, tipo), owner);

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
        using (form)   // dispose after ShowDialog returns — WinForms does NOT auto-dispose modal forms
        {
            if (owner != null)
                form.ShowDialog(owner);
            else
                form.ShowDialog();
        }
    }

    private void ShowModeless(Form form, IWin32Window? owner = null)
    {
        _modelessForms.Add(form);
        form.FormClosed += (_, _) =>
        {
            _modelessForms.Remove(form);
            form.Dispose();
        };
        // Passing the owner makes Windows:
        //   1. always keep the child on top of the owner
        //   2. automatically close the child when the owner closes (cascade)
        if (owner != null)
            form.Show(owner);
        else
            form.Show();
    }
}
