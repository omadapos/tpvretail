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

    public GiftCardService(HttpClient client)
    {
        _client = client;

        _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", SessionManager.Token);
    }
    
    public async Task<GiftCardDTO> GetByCode(string code)
    {
        GiftCardDTO gift = null;

        try
        {
            HttpResponseMessage response = await _client.GetAsync(Constants.BaseUrl + $"/api/giftcard/code/{code}");

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();

                gift = JsonSerializer.Deserialize<GiftCardDTO>(content, options: new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });

            }
        }
        catch (Exception)
        {

        }

        return gift;
    }

    public async Task<bool> PlaceSaldo(int id, GiftCardDTO giftCard)
    {
        var json = JsonSerializer.Serialize(giftCard);
        var data = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _client.PutAsync(Constants.BaseUrl + $"/api/giftcard/{id}", data);

        if (response.IsSuccessStatusCode)
        {

        }

        return true;
    }

    public async Task<bool> PlaceRecarga(string code, int value)
    {
        var json = JsonSerializer.Serialize(new GiftCardRecargarDTO()
        {
            Code = code,
            Value = value
        });
        var data = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _client.PostAsync(Constants.BaseUrl + "/api/giftcard/recargar", data);

        if (response.IsSuccessStatusCode)
        {

        }

        return true;
    }
}
