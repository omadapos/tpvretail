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
    }

    public async Task<BranchModel?> LoadBranch(int branchId)
    {
        string url = Constants.BaseUrl + $"/api/branch/{branchId}";

        // Set token per-request so it always uses the current session token,
        // not a stale one from a previous login session.
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", SessionManager.Token);

        HttpResponseMessage response = await _client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            return null;

        string content = await response.Content.ReadAsStringAsync();

#if DEBUG
        System.Diagnostics.Debug.WriteLine($"[Branch API] GET {url} → {content}");
#endif

        return JsonSerializer.Deserialize<BranchModel>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        });
    }
}