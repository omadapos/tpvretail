using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Utils;
using System.Text;
using System.Text.Json;

namespace OmadaPOS.Libreria.Services;

public interface IUserService
{
    Task<LoginResponse> Login(LoginRequest model);
    Task<bool> Logout(LogDTO model);
    Task<bool> Log(LogDTO model);
}

public class UserService : IUserService
{
    private readonly HttpClient _client;

    public UserService(HttpClient client)
    {
        _client = client;
    }

    
    public async Task<LoginResponse> Login(LoginRequest model)
    {
        var loginResponse = new LoginResponse();

        var json = JsonSerializer.Serialize(model);
        var data = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _client.PostAsync(Constants.BaseUrl + "/token/authenticate", data);

        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();

            loginResponse = JsonSerializer.Deserialize<LoginResponse>(content, options: new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
        }

        return loginResponse;
    }

    public async Task<bool> Logout(LogDTO model)
    {
        var json = JsonSerializer.Serialize(model);
        var data = new StringContent(json, Encoding.UTF8, "application/json");

        await _client.PostAsync(Constants.BaseUrl + "/token/logout", data);

        return true;
    }

    public async Task<bool> Log(LogDTO model)
    {
        var json = JsonSerializer.Serialize(model);
        var data = new StringContent(json, Encoding.UTF8, "application/json");

        await _client.PostAsync(Constants.BaseUrl + "/token/log", data);

        return true;
    }

}
