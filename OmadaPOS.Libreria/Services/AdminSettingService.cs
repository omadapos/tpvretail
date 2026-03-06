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


public class AdminSettingService: IAdminSettingService
{
    private readonly HttpClient _client;

    public AdminSettingService(HttpClient client)
    {
        _client = client;

        _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", SessionManager.Token);
    }

    public async Task<List<AdminSetting>?> LoadSettings(int branchId)
    {
        var result = new List<AdminSetting>();

        HttpResponseMessage response = await _client.GetAsync(Constants.BaseUrl + $"/api/AdminSetting/branch/" + SessionManager.BranchId);

        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();

            result = JsonSerializer.Deserialize<List<AdminSetting>>(content, options: new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
        }

        return result;
    }

    public async Task<AdminSetting?> LoadSettingById(string windowsId)
    {
        AdminSetting? setting = null;

        string url = Constants.BaseUrl + $"/api/AdminSetting/windows/{windowsId}";

        HttpResponseMessage response = await _client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            setting = JsonSerializer.Deserialize<AdminSetting>(content, options: new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
        }

        return setting;
    }

    public async Task<bool> UpdateSetting(AdminSetting setting)
    {
        string url = Constants.BaseUrl + $"/api/AdminSetting/windows/{setting.WindowsId}";

        var json = JsonSerializer.Serialize(setting);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _client.PutAsync(url, content);

        return response.IsSuccessStatusCode;
    }
}
