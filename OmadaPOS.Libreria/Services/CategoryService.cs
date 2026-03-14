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

    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public CategoryService(HttpClient client)
    {
        _client = client;
    }

    // Builds a request with the current session token — avoids DefaultRequestHeaders race condition.
    private HttpRequestMessage Auth(HttpMethod method, string url, HttpContent? content = null)
    {
        var req = new HttpRequestMessage(method, url);
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", SessionManager.Token);
        if (content != null) req.Content = content;
        return req;
    }

    public async Task<List<MenuCategoryModel>?> LoadMenuCategories()
    {
        var response = await _client.SendAsync(
            Auth(HttpMethod.Get, Constants.BaseUrl + $"/api/menucategory/branch/{SessionManager.BranchId}")).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode) return new List<MenuCategoryModel>();

        string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<List<MenuCategoryModel>>(content, _jsonOptions);
    }

    public async Task<List<CategoryModel>?> LoadCategories()
    {
        var response = await _client.SendAsync(
            Auth(HttpMethod.Get, Constants.BaseUrl + "/api/category/branch/" + SessionManager.BranchId)).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode) return new List<CategoryModel>();

        string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<List<CategoryModel>>(content, _jsonOptions);
    }

    public async Task<List<ProductModel>?> LoadProductsByCategory(int categoryId)
    {
        string url = Constants.BaseUrl + $"/api/product/category/{SessionManager.BranchId}/{categoryId}";

        var response = await _client.SendAsync(Auth(HttpMethod.Get, url)).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode) return new List<ProductModel>();

        string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<List<ProductModel>>(content, _jsonOptions);
    }

    public async Task<ProductModel?> LoadProductById(int productId)
    {
        string url = Constants.BaseUrl + $"/api/product/{productId}";

        var response = await _client.SendAsync(Auth(HttpMethod.Get, url)).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode) return new ProductModel();

        string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<ProductModel>(content, _jsonOptions);
    }

    public async Task<ProductModel?> LoadProductByUPC(string upc)
    {
        string url = Constants.BaseUrl + $"/api/product/upc/{SessionManager.BranchId}/{upc}";

        var response = await _client.SendAsync(Auth(HttpMethod.Get, url)).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode) return null;

        string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<ProductModel>(content, _jsonOptions);
    }

    public async Task<ProductModel?> LoadProductByUPC_Promotion(string upc)
    {
        string url = Constants.BaseUrl + $"/api/product/upc/promotion/{SessionManager.BranchId}/{upc}";

        var response = await _client.SendAsync(Auth(HttpMethod.Get, url)).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode) return null;

        string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<ProductModel>(content, _jsonOptions);
    }

    public async Task<PluModel> LoadPluByCode(string code)
    {
        string url = Constants.BaseUrl + $"/api/plu/{code}";

        var response = await _client.SendAsync(Auth(HttpMethod.Get, url)).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode) return new PluModel();

        string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<PluModel>(content, _jsonOptions);
    }

    public async Task<List<ProductModel>> LoadProductIdCategories(IdCategoryDTO model)
    {
        var json    = JsonSerializer.Serialize(model);
        var payload = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.SendAsync(
            Auth(HttpMethod.Post, Constants.BaseUrl + $"/api/product/list/{SessionManager.BranchId}", payload)).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode) return new List<ProductModel>();

        string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<List<ProductModel>>(content, _jsonOptions) ?? new List<ProductModel>();
    }

    public async Task<List<ProductModel>> LoadProductsByCategoryLetra(IdCategoryDTO model, string letra)
    {
        string url = Constants.BaseUrl + $"/api/product/category/{SessionManager.BranchId}/{letra}";

        var json    = JsonSerializer.Serialize(model);
        var payload = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.SendAsync(Auth(HttpMethod.Post, url, payload)).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode) return new List<ProductModel>();

        string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<List<ProductModel>>(content, _jsonOptions) ?? new List<ProductModel>();
    }

    public async Task<ProductModel> LoadProductInfoByUPC(string upc)
    {
        // Uses the session branch, not a hardcoded value.
        string url = Constants.BaseUrl + $"/api/product/new/branch/{SessionManager.BranchId}/{upc}";

        var response = await _client.SendAsync(Auth(HttpMethod.Get, url)).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode) return new ProductModel();

        string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<ProductModel>(content, _jsonOptions) ?? new ProductModel();
    }

    public async Task<ProductModel?> SaveProduct(ProductCreateModel model)
    {
        var json    = JsonSerializer.Serialize(model);
        var payload = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.SendAsync(
            Auth(HttpMethod.Post, Constants.BaseUrl + "/api/product", payload)).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode) return null;

        string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<ProductModel>(content, _jsonOptions);
    }

    public async Task<List<ProductSearchDTO>?> Autocomplete(string text)
    {
        string url = Constants.BaseUrl + $"/api/product/autocomplete/branch/{SessionManager.BranchId}?name={text}";

        var response = await _client.SendAsync(Auth(HttpMethod.Get, url)).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode) return new List<ProductSearchDTO>();

        string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<List<ProductSearchDTO>>(content, _jsonOptions);
    }
}
