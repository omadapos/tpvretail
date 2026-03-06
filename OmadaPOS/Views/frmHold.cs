using OmadaPOS.Libreria.Utils;
using OmadaPOS.Models;
using OmadaPOS.Services;
using System.ComponentModel;

namespace OmadaPOS.Views
{
    public partial class frmHold : Form
    {
        private readonly IHoldService _holdService;
        private readonly IShoppingCart _shoppingCart;
        private BindingList<HoldCartModel> _heldCarts;

        string[] colors = {
            "Red",
            "Blue",
            "Green",
            "Yellow",
            "Orange",
            "Purple",
            "Pink",
            "Brown",
            "Gray",
            "Black"
        };

        public frmHold()
        {
            InitializeComponent();

            _holdService = Program.GetService<IHoldService>();
            _shoppingCart = Program.GetService<IShoppingCart>();

            _heldCarts = new BindingList<HoldCartModel>();

            listBoxHold.DataSource = _heldCarts;
            listBoxHold.ValueMember = "HoldId";

            ElegantButtonStyles.Style(buttonCancel, AppColors.Danger,       AppColors.TextWhite, fontSize: 18f);
            ElegantButtonStyles.Style(buttonHold,   AppColors.AccentGreen,  AppColors.TextWhite, fontSize: 18f);
        }

        private async void frmHold_Load(object sender, EventArgs e)
        {
            try
            {
                await _shoppingCart.LoadCartAsync();
                await LoadHeldCartsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los carritos en hold: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadHeldCartsAsync()
        {
            try
            {
                var carts = await _holdService.GetHeldCartsBySessionAsync(WindowsIdProvider.GetMachineGuid());
                _heldCarts.Clear();
                foreach (var cart in carts)
                {
                    _heldCarts.Add(cart);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los carritos: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void listBoxHold_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxHold.SelectedIndex == -1) return;

            try
            {
                var selectedCart = (HoldCartModel)listBoxHold.SelectedItem;
                if (selectedCart == null) return;

                // Obtener los items del carrito en hold
                var cartItems = await _holdService.RetrieveHeldCartAsync(selectedCart.HoldId);

                // Limpiar el carrito actual
                _shoppingCart.Clear();

                // Agregar los items al carrito actual
                foreach (var item in cartItems)
                {
                    _shoppingCart.AddItem(item);
                }

                // Eliminar el carrito en hold
                await _holdService.DeleteHeldCartAsync(selectedCart.HoldId);

                // Actualizar la visualización del carrito
                ((frmHome)Owner).UpdateCartDisplay();

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al recuperar el carrito: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void buttonAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (_shoppingCart.ItemCount > 0)
                {

                    string _color = "";

                    foreach (var color in colors)
                    {
                        var holdCartModel = await _holdService.GetHeldCartsByIdAsync(color);
                        if(holdCartModel == null)
                        {
                            _color = color;
                            break;
                        }
                    }

                    var bResult = await _holdService.HoldCartAsync(WindowsIdProvider.GetMachineGuid(), _color);
                    if(!bResult)
                    {
                        MessageBox.Show("No se pudo guardar el carrito en hold. Intente nuevamente.", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    
                    this.Close();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el carrito: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
