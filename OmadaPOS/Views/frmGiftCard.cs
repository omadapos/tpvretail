using OmadaPOS.Libreria.Services;
using OmadaPOS.Services;
using OmadaPOS.Services.Navigation;
using System.Text.RegularExpressions;

namespace OmadaPOS.Views;

public sealed class frmGiftCard : POSDialog
{
    private readonly IGiftCardService         _giftCardService;
    private readonly IShoppingCart            _shoppingCart;
    private readonly IHomeInteractionService  _homeInteractionService;

    private readonly int     _tipo;
    private readonly decimal _totalGlobal;
    private decimal          _balance;
    private string           _cardCode = "";

    private TextBox _textCode    = null!;
    private Label   _lblBalance  = null!;
    private Label   _lblTotal    = null!;

    public frmGiftCard(
        decimal totalGlobal,
        int tipo,
        IGiftCardService giftCardService,
        IShoppingCart shoppingCart,
        IHomeInteractionService homeInteractionService)
    {
        _totalGlobal           = totalGlobal;
        _tipo                  = tipo;
        _giftCardService       = giftCardService;
        _shoppingCart          = shoppingCart;
        _homeInteractionService = homeInteractionService;

        Shown += (_, _) => _textCode.Focus();
    }

    protected override Color      AccentColor => AppColors.PaymentGiftCard;
    protected override string     Icon        => "🎁";
    protected override string     Title       => "Gift Card";
    protected override string     Subtitle    => "Scan or enter the gift card code";
    protected override DialogSize Size        => DialogSize.Medium;
    protected override string?    ConfirmText => "🎁  PAY WITH GIFT CARD";
    protected override string     CancelText  => "✕  CANCEL";

    protected override Control BuildContent()
    {
        var outer = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.BackgroundPrimary,
            Padding   = new Padding(20, 16, 20, 8),
        };

        // ── Card code input ───────────────────────────────────────────────────
        FieldPanel("Gift Card Code", out _textCode, placeholder: "Scan or type code…");
        _textCode.TextChanged += TextCode_Changed;
        var fieldPanel = FieldPanel("Gift Card Code", out _textCode, "Scan or type code…");
        _textCode.TextChanged += TextCode_Changed;

        // ── Balance row ───────────────────────────────────────────────────────
        var balanceRow = new Panel { Dock = DockStyle.Top, Height = 48, BackColor = Color.Transparent };

        var lblBalLbl = new Label
        {
            Text      = "Available Balance:",
            Font      = AppTypography.RowLabel,
            ForeColor = AppColors.TextSecondary,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Left,
            Width     = 180,
            TextAlign = ContentAlignment.MiddleLeft,
        };

        _lblBalance = new Label
        {
            Text      = "—",
            Font      = AppTypography.AmountMono,
            ForeColor = AppColors.AccentGreen,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleRight,
        };

        balanceRow.Controls.Add(_lblBalance);
        balanceRow.Controls.Add(lblBalLbl);

        // ── Total due row ─────────────────────────────────────────────────────
        var totalRow = new Panel { Dock = DockStyle.Top, Height = 48, BackColor = Color.Transparent };

        var lblTotLbl = new Label
        {
            Text      = "Total Due:",
            Font      = AppTypography.RowLabel,
            ForeColor = AppColors.TextSecondary,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Left,
            Width     = 180,
            TextAlign = ContentAlignment.MiddleLeft,
        };

        _lblTotal = new Label
        {
            Text      = _totalGlobal.ToString("C"),
            Font      = AppTypography.AmountMono,
            ForeColor = AppColors.TextPrimary,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleRight,
        };

        totalRow.Controls.Add(_lblTotal);
        totalRow.Controls.Add(lblTotLbl);

        // Separator
        var sep = new Panel { Dock = DockStyle.Top, Height = 1, BackColor = AppColors.SurfaceMuted };

        outer.Controls.Add(balanceRow);
        outer.Controls.Add(totalRow);
        outer.Controls.Add(sep);
        outer.Controls.Add(fieldPanel);
        return outer;
    }

    private async void TextCode_Changed(object? sender, EventArgs e)
    {
        try
        {
            var code    = _textCode.Text;
            var matches = Regex.Matches(code, @"%(.*?)\?");
            foreach (Match m in matches)
                _cardCode = m.Groups[1].Value;

            if (string.IsNullOrWhiteSpace(_cardCode)) return;

            var card = await _giftCardService.GetByCode(_cardCode);
            if (card != null)
            {
                _balance = (decimal)(card.Balance ?? 0);
                _lblBalance.Text = _balance.ToString("C");
            }
            _textCode.Focus();
        }
        catch { _lblBalance.Text = "Error reading card"; }
    }

    protected override async Task<bool> OnConfirmAsync()
    {
        if (_shoppingCart.ItemCount <= 0)
        {
            MessageBox.Show("No hay productos en el carrito.", "Empty Cart",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        if (_balance < _totalGlobal)
        {
            MessageBox.Show(
                $"Insufficient balance.\nAvailable: {_balance:C}\nDue: {_totalGlobal:C}",
                "Insufficient Balance", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        var card = await _giftCardService.GetByCode(_cardCode);
        if (card == null)
        {
            MessageBox.Show("Gift card not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        card.Balance = (double?)(_balance - _totalGlobal);
        await _giftCardService.PlaceSaldo(card.Id, card);
        await _homeInteractionService.RequestGiftCardPaymentAsync();
        return true;
    }
}
