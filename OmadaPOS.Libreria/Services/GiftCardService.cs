using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Utils;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace OmadaPOS.Libreria.Services;

public interface IGiftCardService
{
    Task<GiftCardDTO> GetByCode(string code);

    Task<bool> PlaceSaldo(int id, GiftCardDTO giftCard);

    Task<bool> PlaceRecarga(string code, int value);
}

public class GiftCardService : IGiftCardService
{
    private readonly HttpClient _client;

    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public GiftCardService(HttpClient client)
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

    public async Task<GiftCardDTO> GetByCode(string code)
    {
        GiftCardDTO gift = null;

        try
        {
            var response = await _client.SendAsync(
                Auth(HttpMethod.Get, Constants.BaseUrl + $"/api/giftcard/code/{code}")).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                gift = JsonSerializer.Deserialize<GiftCardDTO>(content, _jsonOptions);
            }
        }
        catch (Exception)
        {
        }

        return gift;
    }

    public async Task<bool> PlaceSaldo(int id, GiftCardDTO giftCard)
    {
        var json    = JsonSerializer.Serialize(giftCard);
        var payload = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.SendAsync(
            Auth(HttpMethod.Put, Constants.BaseUrl + $"/api/giftcard/{id}", payload)).ConfigureAwait(false);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> PlaceRecarga(string code, int value)
    {
        var json    = JsonSerializer.Serialize(new GiftCardRecargarDTO { Code = code, Value = value });
        var payload = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.SendAsync(
            Auth(HttpMethod.Post, Constants.BaseUrl + "/api/giftcard/recargar", payload)).ConfigureAwait(false);

        return response.IsSuccessStatusCode;
    }
}
