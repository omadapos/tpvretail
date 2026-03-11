using OmadaPOS.Domain;
using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Services;
using OmadaPOS.Models;

namespace OmadaPOS.Services;

/// <summary>
/// Returned by SearchByBarcodeAsync so frmHome only decides what to show,
/// not how to resolve or build the product.
/// </summary>
public class BarcodeSearchResult
{
    public bool IsFound { get; init; }

    /// <summary>True when the UPC was not found in the API — triggers frmProductNoExist.</summary>
    public bool ProductNotFoundOnServer { get; init; }

    /// <summary>CartItem ready to pass to IShoppingCart.AddItem().</summary>
    public CartItem? CartItem { get; init; }

    /// <summary>Product name for displaying in labelProductName.</summary>
    public string? ProductName { get; init; }
}

public interface IProductApplicationService
{
    /// <summary>
    /// Resolves a scanned UPC (weight-embedded or standard) and returns a ready-to-add
    /// CartItem. Handles weight barcode parsing, PLU lookup, and custom product creation
    /// when required by the weight barcode flow.
    /// </summary>
    Task<BarcodeSearchResult> SearchByBarcodeAsync(string upc);

    /// <summary>
    /// Creates an ad-hoc "Grocery" product on the server and returns a CartItem.
    /// Previously the body of frmHome.addCustomProduct().
    /// </summary>
    Task<CartItem?> CreateCustomProductAsync(bool bTax, decimal price);
}

/// <summary>
/// Handles product lookup and custom product creation.
/// Extracted from frmHome.SearchProduct() (~110 lines) and frmHome.addCustomProduct() (~40 lines).
/// </summary>
public class ProductApplicationService : IProductApplicationService
{
    private readonly ICategoryService              _categoryService;
    private readonly IAgeRestrictionConfigService  _ageConfig;

    public ProductApplicationService(
        ICategoryService             categoryService,
        IAgeRestrictionConfigService ageConfig)
    {
        _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        _ageConfig       = ageConfig       ?? throw new ArgumentNullException(nameof(ageConfig));
    }

    public async Task<BarcodeSearchResult> SearchByBarcodeAsync(string upc)
    {
        if (string.IsNullOrEmpty(upc))
            return new BarcodeSearchResult { IsFound = false };

        if (WeightBarcodeParser.IsWeightBarcode(upc))
            return await HandleWeightBarcodeAsync(upc).ConfigureAwait(false);

        return await HandleStandardBarcodeAsync(upc).ConfigureAwait(false);
    }

    private async Task<BarcodeSearchResult> HandleWeightBarcodeAsync(string upc)
    {
        if (!WeightBarcodeParser.TryParse(upc, out string code, out decimal price))
            return new BarcodeSearchResult { IsFound = false };

        var plu = await _categoryService.LoadProductByUPC_Promotion(code).ConfigureAwait(false);
        if (plu == null)
            return new BarcodeSearchResult { IsFound = false };

        var prod = new ProductCreateModel
        {
            Name = plu.Name,
            Short_Name = "...",
            Description = "...",
            Price = (double)price,
            Status = 1,
            BranchId = SessionManager.BranchId,
            Tax = plu.Tax ?? 0,
            CategoryId = 761,
            Display_Addons = false,
            Display_Sides = false,
            Image = "product-default.jpg",
            Upc = "33324324344",
            Ebt = false,
            Wic = false,
            Stock = 0,
            Cost = 0
        };

        var pResult = await _categoryService.SaveProduct(prod).ConfigureAwait(false);
        if (pResult == null)
            return new BarcodeSearchResult { IsFound = false };

        return new BarcodeSearchResult
        {
            IsFound = true,
            ProductName = plu.Name,
            CartItem = new CartItem
            {
                ProductId               = pResult.Id ?? 0,
                ProductName             = pResult.Name,
                UnitPrice               = pResult.Price ?? 0,
                Quantity                = 1,
                UPC                     = upc,
                Image                   = "product-default.jpg",
                Tax                     = pResult.Tax ?? 0,
                PromotionName           = "",
                PromotionValue          = 0.0,
                PromotionLimit          = 0m,
                IsEBT                   = plu.Ebt,
                RequiresAgeVerification = plu.RequiresAgeVerification
                                          || _ageConfig.IsRestricted(plu.UPC, plu.CategoryId),
            }
        };
    }

    private async Task<BarcodeSearchResult> HandleStandardBarcodeAsync(string upc)
    {
        var product = await _categoryService.LoadProductByUPC_Promotion(upc).ConfigureAwait(false);
        if (product == null)
            return new BarcodeSearchResult { IsFound = false, ProductNotFoundOnServer = true };

        return new BarcodeSearchResult
        {
            IsFound = true,
            ProductName = product.Name,
            CartItem = new CartItem
            {
                ProductId               = product.Id ?? 0,
                ProductName             = product.Name,
                UnitPrice               = product.Price ?? 0.0m,
                Quantity                = 1,
                UPC                     = product.UPC,
                Image                   = product.Image,
                Tax                     = product.Tax ?? 0,
                PromotionName           = product.PromotionName,
                PromotionValue          = product.PromotionValue,
                PromotionLimit          = product.PromotionLimit,
                IsEBT                   = product.Ebt,
                RequiresAgeVerification = product.RequiresAgeVerification
                                          || _ageConfig.IsRestricted(product.UPC, product.CategoryId),
            }
        };
    }

    public async Task<CartItem?> CreateCustomProductAsync(bool bTax, decimal price)
    {
        var prod = new ProductCreateModel
        {
            Name = "Grocery",
            Short_Name = "...",
            Description = "...",
            Price = (double)price,
            Status = 1,
            BranchId = SessionManager.BranchId,
            Tax = bTax ? 7 : 0,
            CategoryId = 761,
            Display_Addons = false,
            Display_Sides = false,
            Image = "product-default.jpg",
            Upc = "343434343434",
            Ebt = false,
            Wic = false,
            Stock = 0,
            Cost = 0
        };

        var pResult = await _categoryService.SaveProduct(prod).ConfigureAwait(false);
        if (pResult == null)
            return null;

        return new CartItem
        {
            ProductId = pResult.Id ?? 0,
            ProductName = pResult.Name,
            UnitPrice = pResult.Price ?? 0,
            Image = pResult.Image,
            Tax = bTax ? 7 : 0,
            Quantity = 1,
            Peso = 0.0
        };
    }
}
