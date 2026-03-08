using OmadaPOS.Componentes;
using OmadaPOS.Services.Navigation;

namespace OmadaPOS.Views;

public sealed class frmKeyLookup : NumericPadDialog
{
    private readonly IHomeInteractionService _svc;

    public frmKeyLookup(IHomeInteractionService svc)
    {
        _svc = svc;
    }

    protected override NumericPadControl.PadMode PadMode      => NumericPadControl.PadMode.Integer;
    protected override Color                     AccentColor  => AppColors.Info;
    protected override string                    Icon         => "⌕";
    protected override string                    Title        => "Product Lookup";
    protected override string                    Subtitle     => "Scan or enter the UPC / barcode";
    protected override string                    ConfirmText  => "⌕  SEARCH";

    protected override async Task<bool> OnConfirmAsync(NumericPadControl pad)
    {
        var code = pad.TextValue;
        if (string.IsNullOrWhiteSpace(code)) return false;

        await _svc.RequestProductSearchAsync(code);
        return true;
    }
}
