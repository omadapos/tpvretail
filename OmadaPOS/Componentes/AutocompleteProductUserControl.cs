using OmadaPOS.Libreria.Services;
using OmadaPOS.Models;

namespace OmadaPOS.Componentes
{
    public partial class AutocompleteProductUserControl : UserControl
    {
        private readonly ICategoryService _categoryService;
        private List<ProductSearchDTO>? _currentSuggestions = [];

        // Evento para notificar la selección de un producto
        public event Action<ProductSearchDTO>? ProductSelected;

        public AutocompleteProductUserControl()
        {
            InitializeComponent();

            _categoryService = Program.GetService<ICategoryService>();
        }

        private async void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var text = textBoxSearch.Text.Trim();

                if (text.Length >= 3)
                {
                    _currentSuggestions = await _categoryService.Autocomplete(text) ?? [];

                    var autoCompleteCollection = new AutoCompleteStringCollection();
                    autoCompleteCollection.AddRange(_currentSuggestions
                        .Where(p => !string.IsNullOrEmpty(p.Name))
                        .Select(p => p.Name)
                        .ToArray()!);

                    textBoxSearch.AutoCompleteCustomSource = autoCompleteCollection;
                    textBoxSearch.AutoCompleteMode         = AutoCompleteMode.SuggestAppend;
                    textBoxSearch.AutoCompleteSource       = AutoCompleteSource.CustomSource;
                }
                else
                {
                    _currentSuggestions?.Clear();
                    textBoxSearch.AutoCompleteCustomSource = new AutoCompleteStringCollection();
                }
            }
            catch (Exception ex)
            {
                _currentSuggestions = [];
                System.Diagnostics.Debug.WriteLine($"Autocomplete error: {ex.Message}");
            }
        }

        // Detectar selección por Enter
        private void TextBoxSearch_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TrySelectProduct();
            }
        }

        // Detectar selección al perder el foco
        private void TextBoxSearch_Leave(object? sender, EventArgs e)
        {
            TrySelectProduct();
        }

        private void TrySelectProduct()
        {
            var selectedName = textBoxSearch.Text.Trim();
            var selectedProduct = _currentSuggestions.FirstOrDefault(p => p.Name?.Equals(selectedName, StringComparison.OrdinalIgnoreCase) == true);

            if (selectedProduct != null)
            {
                ProductSelected?.Invoke(selectedProduct);
            }
        }

    }
}
