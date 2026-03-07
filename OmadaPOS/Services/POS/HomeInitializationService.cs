using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Services;

namespace OmadaPOS.Services.POS;

public interface IHomeInitializationService
{
    Task<int> LoadLastInvoiceAsync();
    Task LoadCategoriesAsync(ICollection<CategoryModel> categories);
    Task LoadMenuCategoriesAsync(ICollection<MenuCategoryModel> menuCategories);
    Task LoadProductsAsync(ICollection<ProductModel> products, int[] categoryIds, string? searchLetter = null);
}

public class HomeInitializationService : IHomeInitializationService
{
    private readonly ICategoryService _categoryService;
    private readonly IOrderService _orderService;

    public HomeInitializationService(ICategoryService categoryService, IOrderService orderService)
    {
        _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
    }

    public Task<int> LoadLastInvoiceAsync() => _orderService.LoadLastInvoiceAdmin();

    public async Task LoadCategoriesAsync(ICollection<CategoryModel> categories)
    {
        ArgumentNullException.ThrowIfNull(categories);

        var loadedCategories = await _categoryService.LoadCategories();
        categories.Clear();

        foreach (var category in loadedCategories)
            categories.Add(category);
    }

    public async Task LoadMenuCategoriesAsync(ICollection<MenuCategoryModel> menuCategories)
    {
        ArgumentNullException.ThrowIfNull(menuCategories);

        var loadedMenuCategories = await _categoryService.LoadMenuCategories();
        menuCategories.Clear();

        foreach (var menuCategory in loadedMenuCategories)
            menuCategories.Add(menuCategory);
    }

    public async Task LoadProductsAsync(ICollection<ProductModel> products, int[] categoryIds, string? searchLetter = null)
    {
        ArgumentNullException.ThrowIfNull(products);
        ArgumentNullException.ThrowIfNull(categoryIds);

        var loadedProducts = string.IsNullOrEmpty(searchLetter)
            ? await _categoryService.LoadProductIdCategories(new IdCategoryDTO { Ids = categoryIds })
            : await _categoryService.LoadProductsByCategoryLetra(new IdCategoryDTO { Ids = categoryIds }, searchLetter);

        products.Clear();

        foreach (var product in loadedProducts)
            products.Add(product);
    }
}
