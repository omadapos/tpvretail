using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Utils;
using OmadaPOS.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace OmadaPOS.Libreria.Services;

public interface ICategoryService
{
    Task<List<MenuCategoryModel>?> LoadMenuCategories();
    Task<List<CategoryModel>?> LoadCategories();
    Task<List<ProductModel>?> LoadProductsByCategory(int categoryId);
    Task<List<ProductModel>> LoadProductsByCategoryLetra(IdCategoryDTO ids, string letra);
    Task<ProductModel?> LoadProductById(int productId);
    Task<ProductModel?> LoadProductByUPC(string upc);
    Task<ProductModel?> LoadProductByUPC_Promotion(string upc);
    Task<ProductModel> LoadProductInfoByUPC(string upc);
    Task<PluModel> LoadPluByCode(string code);
    Task<List<ProductModel>> LoadProductIdCategories(IdCategoryDTO model);
    Task<ProductModel?> SaveProduct(ProductCreateModel model);
    Task<List<ProductSearchDTO>?> Autocomplete(string text);
}

public class CategoryService : ICategoryService
{
    private readonly HttpClient _client;

    public CategoryService(HttpClient client)
    {
        _client = client;

        _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", SessionManager.Token);
    }

    public async Task<List<MenuCategoryModel>?> LoadMenuCategories()
    {
        var mCategories = new List<MenuCategoryModel>();

        HttpResponseMessage response = await _client.GetAsync(Constants.BaseUrl + $"/api/menucategory/branch/" + SessionManager.BranchId);

        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();

            mCategories = JsonSerializer.Deserialize<List<MenuCategoryModel>>(content, options: new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
        }

        return mCategories;
    }

    public async Task<List<CategoryModel>?> LoadCategories()
    {
        var categories = new List<CategoryModel>();

        HttpResponseMessage response = await _client.GetAsync(Constants.BaseUrl + "/api/category/branch/" + SessionManager.BranchId);

        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();

            categories = JsonSerializer.Deserialize<List<CategoryModel>>(content, options: new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
        }

        return categories;
    }

    public async Task<List<ProductModel>?> LoadProductsByCategory(int categoryId)
    {
        var products = new List<ProductModel>();

        string url = Constants.BaseUrl + $"/api/product/category/{SessionManager.BranchId}/{categoryId}";

        HttpResponseMessage response = await _client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();

            products = JsonSerializer.Deserialize<List<ProductModel>>(content, options: new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
        }

        return products;
    }

    public async Task<ProductModel?> LoadProductById(int productId)
    {
        var product = new ProductModel();

        string url = Constants.BaseUrl + $"/api/product/{productId}";

        HttpResponseMessage response = await _client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();

            product = JsonSerializer.Deserialize<ProductModel>(content, options: new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
        }

        return product;
    }

    public async Task<ProductModel?> LoadProductByUPC(string upc)
    {
        ProductModel? product = null;

        string url = Constants.BaseUrl + $"/api/product/upc/{SessionManager.BranchId}/{upc}";

        HttpResponseMessage response = await _client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();

            product = JsonSerializer.Deserialize<ProductModel>(content, options: new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
        }

        return product;
    }

    public async Task<ProductModel?> LoadProductByUPC_Promotion(string upc)
    {
        ProductModel? product = null;

        string url = Constants.BaseUrl + $"/api/product/upc/promotion/{SessionManager.BranchId}/{upc}";

        HttpResponseMessage response = await _client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();

            product = JsonSerializer.Deserialize<ProductModel>(content, options: new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
        }

        return product;
    }

    public async Task<PluModel> LoadPluByCode(string code)
    {
        var plu = new PluModel();

        string url = Constants.BaseUrl + $"/api/plu/{code}";

        HttpResponseMessage response = await _client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();

            plu = JsonSerializer.Deserialize<PluModel>(content, options: new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
        }

        return plu;
    }

    public async Task<List<ProductModel>> LoadProductIdCategories(IdCategoryDTO model)
    {
        var result = new List<ProductModel>();

        var json = JsonSerializer.Serialize(model);
        var data = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _client.PostAsync(Constants.BaseUrl + $"/api/product/list/{SessionManager.BranchId}", data);

        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();

            result = JsonSerializer.Deserialize<List<ProductModel>>(content, options: new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
        }

        return result;
    }

    public async Task<List<ProductModel>> LoadProductsByCategoryLetra(IdCategoryDTO model, string letra)
    {
        var products = new List<ProductModel>();

        string url = Constants.BaseUrl + $"/api/product/category/{SessionManager.BranchId}/{letra}";

        var json = JsonSerializer.Serialize(model);
        var data = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _client.PostAsync(url, data);

        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();

            products = JsonSerializer.Deserialize<List<ProductModel>>(content, options: new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
        }

        return products;
    }

    public async Task<ProductModel> LoadProductInfoByUPC(string upc)
    {
        var product = new ProductModel();

        string url = Constants.BaseUrl + $"/api/product/new/branch/31/{upc}";

        HttpResponseMessage response = await _client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();

            product = JsonSerializer.Deserialize<ProductModel>(content, options: new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
        }

        return product;
    }

    public async Task<ProductModel?> SaveProduct(ProductCreateModel model)
    {
        var json = JsonSerializer.Serialize(model);
        var data = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _client.PostAsync(Constants.BaseUrl + "/api/product", data);

        if (response.IsSuccessStatusCode)
        {

            string content = await response.Content.ReadAsStringAsync();
            var product = JsonSerializer.Deserialize<ProductModel>(content, options: new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
            return product;
        }

        return null;
    }

    public async Task<List<ProductSearchDTO>?> Autocomplete(string text)
    {
        var products = new List<ProductSearchDTO>();

        string url = Constants.BaseUrl + $"/api/product/autocomplete/branch/{SessionManager.BranchId}?name={text}";

        HttpResponseMessage response = await _client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();

            products = JsonSerializer.Deserialize<List<ProductSearchDTO>>(content, options: new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
        }

        return products;
    }

}
