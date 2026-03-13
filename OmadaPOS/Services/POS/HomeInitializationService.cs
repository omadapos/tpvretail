using System.Collections.Concurrent;
using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Services;

namespace OmadaPOS.Services.POS;

public interface IHomeInitializationService
{
    Task<int> LoadLastInvoiceAsync();
    Task LoadCategoriesAsync(ICollection<CategoryModel> categories);
    Task LoadMenuCategoriesAsync(ICollection<MenuCategoryModel> menuCategories);
    Task LoadProductsAsync(ICollection<ProductModel> products, int[] categoryIds, string? searchLetter = null, CancellationToken ct = default);
    bool IsCached(int[] categoryIds);
    void ClearProductCache();
}

public class HomeInitializationService : IHomeInitializationService
{
    private readonly ICategoryService _categoryService;
    private readonly IOrderService _orderService;

    // Caché por sesión: clave = IDs de categoría ordenados, valor = lista de productos
    private readonly ConcurrentDictionary<string, List<ProductModel>> _productCache = new();

    public HomeInitializationService(ICategoryService categoryService, IOrderService orderService)
    {
        _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
    }

    public Task<int> LoadLastInvoiceAsync() => _orderService.LoadLastInvoiceAdmin();

    public async Task LoadCategoriesAsync(ICollection<CategoryModel> categories)
    {
        ArgumentNullException.ThrowIfNull(categories);

        var loadedCategories = await _categoryService.LoadCategories().ConfigureAwait(false);
        categories.Clear();

        foreach (var category in loadedCategories)
            categories.Add(category);
    }

    public async Task LoadMenuCategoriesAsync(ICollection<MenuCategoryModel> menuCategories)
    {
        ArgumentNullException.ThrowIfNull(menuCategories);

        var loadedMenuCategories = await _categoryService.LoadMenuCategories().ConfigureAwait(false);
        menuCategories.Clear();

        foreach (var menuCategory in loadedMenuCategories)
            menuCategories.Add(menuCategory);
    }

    public async Task LoadProductsAsync(ICollection<ProductModel> products, int[] categoryIds, string? searchLetter = null, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(products);
        ArgumentNullException.ThrowIfNull(categoryIds);

        List<ProductModel> loadedProducts;

        if (string.IsNullOrEmpty(searchLetter))
        {
            var cacheKey = string.Join(",", categoryIds.OrderBy(x => x));

            if (!_productCache.TryGetValue(cacheKey, out var cached))
            {
                cached = await _categoryService.LoadProductIdCategories(new IdCategoryDTO { Ids = categoryIds }).ConfigureAwait(false);
                ct.ThrowIfCancellationRequested();
                _productCache.TryAdd(cacheKey, cached);
            }

            loadedProducts = cached;
        }
        else
        {
            loadedProducts = await _categoryService.LoadProductsByCategoryLetra(new IdCategoryDTO { Ids = categoryIds }, searchLetter).ConfigureAwait(false);
        }

        ct.ThrowIfCancellationRequested();

        products.Clear();
        foreach (var product in loadedProducts)
            products.Add(product);
    }

    public bool IsCached(int[] categoryIds)
    {
        var cacheKey = string.Join(",", categoryIds.OrderBy(x => x));
        return _productCache.ContainsKey(cacheKey);
    }

    public void ClearProductCache() => _productCache.Clear();
}
