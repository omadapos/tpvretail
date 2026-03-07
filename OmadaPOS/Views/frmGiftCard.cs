using OmadaPOS.Libreria.Services;
using OmadaPOS.Services;
using OmadaPOS.Services.Navigation;
using System.Text.RegularExpressions;

namespace OmadaPOS.Views
{
    public partial class frmGiftCard : OmadaPOS.Estilos.EstiloFormularioPOS
    {
        private readonly IGiftCardService _giftCardService;
        private readonly IShoppingCart _shoppingCart;
        private readonly IHomeInteractionService _homeInteractionService;

        private int tipo = 0;
        private decimal totalGlobal = 0.0m;
        private decimal balance = 0.0m;
        private string cardCode = "";

        public frmGiftCard(
            decimal totalGlobal,
            int tipo,
            IGiftCardService giftCardService,
            IShoppingCart shoppingCart,
            IHomeInteractionService homeInteractionService)
        {
            InitializeComponent();

            this.totalGlobal = totalGlobal;
            this.tipo = tipo;

            _giftCardService = giftCardService;
            _shoppingCart = shoppingCart;
            _homeInteractionService = homeInteractionService;

            labelTotal.Text = totalGlobal.ToString("C");
        }

        private void frmGiftCard_Load(object sender, EventArgs e)
        {
            textBoxCode.Focus();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void buttonPay_Click(object sender, EventArgs e)
        {
            if (_shoppingCart.ItemCount <= 0)
            {
                MessageBox.Show("No hay productos en el carrito.", "Carrito vacío",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (balance < totalGlobal)
            {
                MessageBox.Show($"Saldo insuficiente.\nSaldo disponible: {balance:C}\nTotal a pagar: {totalGlobal:C}",
                    "Saldo insuficiente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                buttonPay.Enabled = false;

                var giftCard = await _giftCardService.GetByCode(cardCode);
                if (giftCard == null)
                {
                    MessageBox.Show("No se encontró la Gift Card.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                giftCard.Balance = (double?)(balance - totalGlobal);
                await _giftCardService.PlaceSaldo(giftCard.Id, giftCard);

                await _homeInteractionService.RequestGiftCardPaymentAsync();

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al procesar el pago: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                buttonPay.Enabled = true;
            }
        }

        private async void textBoxCode_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var code = textBoxCode.Text;
                string pattern = @"%(.*?)\?";
                var matches = Regex.Matches(code, pattern);

                foreach (Match match in matches)
                    cardCode = match.Groups[1].Value;

                if (string.IsNullOrWhiteSpace(cardCode)) return;

                var giftCard = await _giftCardService.GetByCode(cardCode);
                if (giftCard != null)
                {
                    balance = (decimal)(giftCard.Balance ?? 0);
                    labelSaldo.Text = "Card Balance: " + balance.ToString("C");
                }

                textBoxCode.Focus();
            }
            catch (Exception ex)
            {
                labelSaldo.Text = "Error al verificar tarjeta";
                System.Diagnostics.Debug.WriteLine($"GiftCard lookup error: {ex.Message}");
            }
        }
    }
}
