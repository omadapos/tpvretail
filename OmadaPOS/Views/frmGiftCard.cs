using OmadaPOS.Libreria.Services;
using System.Text.RegularExpressions;

namespace OmadaPOS.Views
{
    public partial class frmGiftCard : OmadaPOS.Estilos.EstiloFormularioPOS
    {
        int tipo = 0;
        decimal totalGlobal = 0.0m;
        decimal balance = 0.0m;

        private IGiftCardService? giftCardService;
        string value = "";

        public frmGiftCard(decimal totalGlobal, int tipo)
        {
            InitializeComponent();

            this.totalGlobal = totalGlobal;
            this.tipo = tipo;

            labelTotal.Text = totalGlobal.ToString("C");
        }

        private void frmGiftCard_Load(object sender, EventArgs e)
        {
            textBoxCode.Focus();

            giftCardService = Program.GetService<IGiftCardService>();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void buttonPay_Click(object sender, EventArgs e)
        {
            //if (SharedData.Items.Count > 0)
            //{
            //    if (balance >= totalGlobal)
            //    {
            //        var giftCard = await giftCardService!.GetByCode(value);

            //        giftCard.Balance = (double?)(balance - totalGlobal);

            //        await giftCardService.PlaceSaldo(giftCard.Id, giftCard);

            //        if (tipo == 1)
            //        {
            //            ((frmHome)Owner).GiftCardPay();
            //        }
            //        else
            //        {
            //            //((frmPaymentSplit)Owner).GiftCardPay();
            //        }

            //        this.Close();
            //    }
            //    else
            //    {
            //        MessageBox.Show("No tiene saldo");
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("No hay productos");
            //}
        }

        private async void textBoxCode_TextChanged(object sender, EventArgs e)
        {
            var code = textBoxCode.Text;

            string pattern = @"%(.*?)\?";

            var matches = Regex.Matches(code, pattern);

            foreach (Match match in matches)
            {
                value = match.Groups[1].Value;
            }

            var giftCard = await giftCardService.GetByCode(value);

            if (giftCard != null)
            {
                balance = (decimal)(giftCard.Balance ?? 0);

                labelSaldo.Text = "Card Balance: " + balance.ToString("C");
            }

            textBoxCode.Focus();
        }
    }
}
