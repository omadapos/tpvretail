using OmadaPOS.Componentes;
using OmadaPOS.Services.Navigation;

namespace OmadaPOS.Views;

public sealed class frmProductNew : NumericPadDialog
{
    private readonly bool                    _bTax;
    private readonly IHomeInteractionService _svc;

    public frmProductNew(bool bTax, IHomeInteractionService svc)
    {
        _bTax = bTax;
        _svc  = svc;
    }

    protected override NumericPadControl.PadMode PadMode     => NumericPadControl.PadMode.Money;
    protected override Color                     AccentColor => _bTax ? AppColors.Warning : AppColors.AccentGreen;
    protected override string                    Icon        => _bTax ? "$+" : "$";
    protected override string                    Title       => _bTax ? "Quick Sale  +Tax" : "Quick Sale";
    protected override string                    Subtitle    => _bTax ? "Enter price — tax will be applied"
                                                                       : "Enter the custom product price";
    protected override string                    ConfirmText => _bTax ? "$+  ADD +TAX" : "$  ADD ITEM";

    protected override async Task<bool> OnConfirmAsync(NumericPadControl pad)
    {
        decimal valor = pad.ValueDecimal;
        if (valor > 0)
        {
            await _svc.RequestCustomProductAsync(_bTax, valor);
            return true;
        }

        MessageBox.Show("Please enter a valid amount greater than zero.",
            "Invalid Amount", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return false;
    }
}
