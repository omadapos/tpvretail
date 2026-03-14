using OmadaPOS.Libreria.Extensions;
using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Services;
using OmadaPOS.Models;

namespace OmadaPOS.Services.POS;

public sealed class BarcodeSaleProcessResult
{
    public bool AddedToCart { get; init; }
    public bool ProductNotFoundOnServer { get; init; }
    public string? ProductName { get; init; }
}

public sealed class ProductSelectionResult
{
    public bool IsWeighted { get; init; }
    public bool AddedToCart { get; init; }
    public string? ProductName { get; init; }
    public string? WeightDisplayText { get; init; }
    public string? ProductIdText { get; init; }
    public string? ImageLocation { get; init; }
}

public sealed class WeightedProductAddResult
{
    public bool AddedToCart { get; init; }
    public string? ProductName { get; init; }
}

public interface IBarcodeSaleService
{
    Task<BarcodeSaleProcessResult> ProcessBarcodeAsync(string upc);
    Task<bool> AddCustomProductAsync(bool bTax, decimal price);
    ProductSelectionResult HandleProductSelection(ProductModel product, IReadOnlyList<CategoryModel> categories);
    Task<WeightedProductAddResult> AddWeightedProductAsync(int productId, double weight);
}

public class BarcodeSaleService : IBarcodeSaleService
{
    private readonly IProductApplicationService  _productApplicationService;
    private readonly IShoppingCart               _shoppingCart;
    private readonly ICategoryService            _categoryService;
    private readonly IAgeRestrictionConfigService _ageConfig;

    public BarcodeSaleService(
        IProductApplicationService   productApplicationService,
        IShoppingCart                shoppingCart,
        ICategoryService             categoryService,
        IAgeRestrictionConfigService ageConfig)
    {
        _productApplicationService = productApplicationService ?? throw new ArgumentNullException(nameof(productApplicationService));
        _shoppingCart              = shoppingCart              ?? throw new ArgumentNullException(nameof(shoppingCart));
        _categoryService           = categoryService           ?? throw new ArgumentNullException(nameof(categoryService));
        _ageConfig                 = ageConfig                 ?? throw new ArgumentNullException(nameof(ageConfig));
    }

    public async Task<BarcodeSaleProcessResult> ProcessBarcodeAsync(string upc)
    {
        if (string.IsNullOrEmpty(upc))
            return new BarcodeSaleProcessResult();

        var result = await _productApplicationService.SearchByBarcodeAsync(upc).ConfigureAwait(false);

        if (result.IsFound && result.CartItem != null)
        {
            bool added = _shoppingCart.AddItem(result.CartItem);
            return new BarcodeSaleProcessResult
            {
                AddedToCart = added,
                ProductName = result.ProductName
            };
        }

        return new BarcodeSaleProcessResult
        {
            AddedToCart = false,
            ProductNotFoundOnServer = result.ProductNotFoundOnServer
        };
    }

    public async Task<bool> AddCustomProductAsync(bool bTax, decimal price)
    {
        var cartItem = await _productApplicationService.CreateCustomProductAsync(bTax, price).ConfigureAwait(false);
        if (cartItem == null)
            return false;

        return _shoppingCart.AddItem(cartItem);
    }

    public ProductSelectionResult HandleProductSelection(ProductModel product, IReadOnlyList<CategoryModel> categories)
    {
        ArgumentNullException.ThrowIfNull(product);
        ArgumentNullException.ThrowIfNull(categories);

        var category = categories.SingleOrDefault(c => c.Id == product.CategoryId);

        if (category != null && category.Pesado)
        {
            return new ProductSelectionResult
            {
                IsWeighted = true,
                ProductName = product.Name,
                WeightDisplayText = $"{product.Name} (${product.Price.Value:n2})",
                ProductIdText = product.Id?.ToString(),
                ImageLocation = product.Image.ConvertUrlString()
            };
        }

        bool added = _shoppingCart.AddItem(new CartItem
        {
            ProductId               = product.Id ?? 0,
            ProductName             = product.Name ?? string.Empty,
            UnitPrice               = product.Price ?? 0.0m,
            Quantity                = 1,
            Tax                     = product.Tax ?? 0.0,
            IsEBT                   = product.Ebt,
            UPC                     = product.UPC,
            Image                   = product.Image.ConvertUrlString(),
            PromotionName           = product.PromotionName,
            PromotionValue          = product.PromotionValue,
            PromotionLimit          = product.PromotionLimit,
            RequiresAgeVerification = product.RequiresAgeVerification
                                      || _ageConfig.IsRestricted(product.UPC, product.CategoryId),
        });

        return new ProductSelectionResult
        {
            AddedToCart = added,
            ProductName = product.Name
        };
    }

    public async Task<WeightedProductAddResult> AddWeightedProductAsync(int productId, double weight)
    {
        var product = await _categoryService.LoadProductById(productId).ConfigureAwait(false);
        if (product == null)
            return new WeightedProductAddResult();

        bool added = _shoppingCart.AddItem(new CartItem
        {
            ProductId               = product.Id ?? 0,
            ProductName             = product.Name ?? string.Empty,
            UnitPrice               = product.Price ?? 0.0m,
            Quantity                = 1,
            Peso                    = weight / 1000,   // weight comes in grams; Peso is kg
            Tax                     = product.Tax ?? 0.0,
            IsEBT                   = product.Ebt,
            UPC                     = product.UPC,
            Image                   = product.Image.ConvertUrlString(),
            PromotionName           = product.PromotionName,
            PromotionValue          = product.PromotionValue,
            PromotionLimit          = product.PromotionLimit,
            RequiresAgeVerification = product.RequiresAgeVerification
                                      || _ageConfig.IsRestricted(product.UPC, product.CategoryId),
        });

        return new WeightedProductAddResult
        {
            AddedToCart = added,
            ProductName = product.Name
        };
    }
}
