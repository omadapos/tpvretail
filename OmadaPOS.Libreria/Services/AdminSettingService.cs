using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Utils;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace OmadaPOS.Libreria.Services;

public interface IAdminSettingService
{
    Task<List<AdminSetting>?> LoadSettings(int branchId);
    Task<AdminSetting?> LoadSettingById(string windowsId);
    Task<bool> UpdateSetting(AdminSetting setting);
}


public class AdminSettingService : IAdminSettingService
{
    private readonly HttpClient _client;

    private static readonly JsonSerializerOptions _jsonOpts =
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

    public AdminSettingService(HttpClient client)
    {
        _client = client;
        // Do NOT set DefaultRequestHeaders here — singleton HttpClient race condition.
        // Use per-request headers in each method instead.
    }

    private static HttpRequestMessage BuildRequest(HttpMethod method, string url)
    {
        var req = new HttpRequestMessage(method, url);
        req.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", SessionManager.Token);
        return req;
    }

    public async Task<List<AdminSetting>?> LoadSettings(int branchId)
    {
        // Use the supplied branchId parameter (not SessionManager.BranchId, which
        // was previously used here by mistake, ignoring the caller's argument).
        using var req = BuildRequest(HttpMethod.Get,
            Constants.BaseUrl + $"/api/AdminSetting/branch/{branchId}");

        var response = await _client.SendAsync(req).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            return null;

        string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<List<AdminSetting>>(content, _jsonOpts);
    }

    public async Task<AdminSetting?> LoadSettingById(string windowsId)
    {
        using var req = BuildRequest(HttpMethod.Get,
            Constants.BaseUrl + $"/api/AdminSetting/windows/{windowsId}");

        var response = await _client.SendAsync(req).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            return null;

        string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<AdminSetting>(content, _jsonOpts);
    }

    public async Task<bool> UpdateSetting(AdminSetting setting)
    {
        string url = Constants.BaseUrl + $"/api/AdminSetting/windows/{setting.WindowsId}";

        var json = JsonSerializer.Serialize(setting);
        using var req = BuildRequest(HttpMethod.Put, url);
        req.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.SendAsync(req).ConfigureAwait(false);
        return response.IsSuccessStatusCode;
    }
}
