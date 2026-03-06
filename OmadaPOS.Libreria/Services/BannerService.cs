using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Utils;
using System.Net.Http.Headers;
using System.Text.Json;

namespace OmadaPOS.Libreria.Services;

public interface IBannerService
{
    Task<List<BannerModel>?> LoadBanners();
}

public class BannerService : IBannerService
{
    private readonly HttpClient _client;

    public BannerService(HttpClient client)
    {
        _client = client;

        _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", SessionManager.Token);
    }

    public async Task<List<BannerModel>?> LoadBanners()
    {
        var mCategories = new List<BannerModel>();

        HttpResponseMessage response = await _client.GetAsync(Constants.BaseUrl + "/api/banner/branch/" + SessionManager.BranchId);

        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();

            mCategories = JsonSerializer.Deserialize<List<BannerModel>>(content, options: new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
        }

        return mCategories;
    }

}
