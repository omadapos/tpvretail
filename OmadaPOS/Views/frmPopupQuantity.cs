using OmadaPOS.Componentes;
using OmadaPOS.Services.Navigation;

namespace OmadaPOS.Views;

public sealed class frmPopupQuantity : NumericPadDialog
{
    private readonly int                     _productId;
    private readonly int                     _currentQty;
    private readonly IHomeInteractionService _svc;

    public frmPopupQuantity(int number, int productId, IHomeInteractionService svc)
    {
        _productId  = productId;
        _currentQty = number;
        _svc        = svc;

        // Pre-fill the pad with the current quantity so the cashier sees and edits from it.
        if (_currentQty > 0)
            Shown += (_, _) => Pad.TextValue = _currentQty.ToString();
    }

    protected override NumericPadControl.PadMode PadMode      => NumericPadControl.PadMode.Integer;
    protected override Color                     AccentColor  => AppColors.NavyBase;
    protected override string                    Icon         => "#";
    protected override string                    Title        => "Change Quantity";
    protected override string                    Subtitle     => "Enter the new quantity for this item";
    protected override string                    ConfirmText  => "✔  CONFIRM";

    protected override Task<bool> OnConfirmAsync(NumericPadControl pad)
    {
        if (int.TryParse(pad.TextValue, out int qty) && qty > 0)
        {
            _svc.RequestQuantityChange(qty, _productId);
            return Task.FromResult(true);
        }

        MessageBox.Show("Please enter a valid quantity greater than 0.",
            "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return Task.FromResult(false);
    }
}
