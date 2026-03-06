using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Utils;
using System.Net.Http.Headers;
using System.Text.Json;

namespace OmadaPOS.Libreria.Services;

public interface IBranchService
{
    Task<BranchModel?> LoadBranch(int branchId);
}

public class BranchService : IBranchService
{

    private readonly HttpClient _client;

    public BranchService(HttpClient client)
    {
        _client = client;

        _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", SessionManager.Token);
    }

    public async Task<BranchModel?> LoadBranch(int branchId)
    {
        var branch = new BranchModel();

        string url = Constants.BaseUrl + $"/api/branch/{branchId}";

        HttpResponseMessage response = await _client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();

            branch = JsonSerializer.Deserialize<BranchModel>(content, options: new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
        }

        return branch;
    }
}