using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Utils;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace OmadaPOS.Libreria.Services;

public interface IOrderService
{
    Task<OrderResponse> PlaceOrder(PlaceOrderModel order);
    Task<OrderResponse> PlaceOrderMultiple(PlaceOrderMultipleModel order);
    Task<int> LoadLastInvoiceGeneral();
    Task<int> LoadLastInvoiceAdmin();
    Task<long> LoadLastConsecutivoPayment();
    Task<OrderModel> GetOrderById(int orderId);
    Task<List<OrderDetailModel>> GetOrderDetailsByOrderId(int orderId);
    Task<CloseResultModel> CierreDiario(string fecha, string username);
    Task<List<OrderModel>> GetOrderTop();
    Task<List<OrderModel>> GetOrderTop(string fecha1, string fecha2);
}

public class OrderService : IOrderService
{
    private readonly HttpClient _client;

    public OrderService(HttpClient client)
    {
        _client = client;

        _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", SessionManager.Token);
    }

    public async Task<OrderResponse> PlaceOrder(PlaceOrderModel order)
    {
        var orderResponse = new OrderResponse();

        var json = JsonSerializer.Serialize(order);
        var data = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _client.PostAsync(Constants.BaseUrl + "/api/order/place", data);

        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();

            orderResponse = JsonSerializer.Deserialize<OrderResponse>(content, options: new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
        }

        return orderResponse;
    }

    public async Task<int> LoadLastInvoiceGeneral()
    {
        int orderId = 0;
        try
        {
            string url = Constants.BaseUrl + $"/api/order/last/invoice/{SessionManager.BranchId}";

            HttpResponseMessage response = await _client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();

                var orderLast = JsonSerializer.Deserialize<OrderLast>(content, options: new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });
                if (orderLast != null)
                {
                    orderId = orderLast.OrderId;
                }
            }
        }
        catch (Exception)
        {
            orderId = 0;
        }

        return orderId;
    }

    public async Task<long> LoadLastConsecutivoPayment()
    {
        long consecutivo = 0;
        try
        {
            string url = Constants.BaseUrl + $"/api/ConsecutivoPayment/next/{SessionManager.BranchId}";

            HttpResponseMessage response = await _client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();

                var consecutivoLast = JsonSerializer.Deserialize<long>(content, options: new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });

                consecutivo = consecutivoLast;
            }
        }
        catch (Exception)
        {
        }

        return consecutivo;
    }

    public async Task<int> LoadLastInvoiceAdmin()
    {
        int orderId = 0;
        try
        {
            string url = Constants.BaseUrl + $"/api/order/last/invoice/{SessionManager.BranchId}/admin";

            HttpResponseMessage response = await _client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();

                var orderLast = JsonSerializer.Deserialize<OrderLast>(content, options: new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });
                if (orderLast != null)
                {
                    orderId = orderLast.OrderId;
                }
            }
        }
        catch (Exception)
        {
            orderId = 0;
        }

        return orderId;
    }

    public async Task<OrderModel> GetOrderById(int orderId)
    {
        OrderModel order = null;

        try
        {
            HttpResponseMessage response = await _client.GetAsync(Constants.BaseUrl + $"/api/order/{orderId}");

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();

                order = JsonSerializer.Deserialize<OrderModel>(content, options: new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });

            }
        }
        catch (Exception)
        {

        }

        return order;
    }

    public async Task<List<OrderDetailModel>> GetOrderDetailsByOrderId(int orderId)
    {
        var details = new List<OrderDetailModel>();

        HttpResponseMessage response = await _client.GetAsync(Constants.BaseUrl + $"/api/orderdetail/{orderId}");

        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();

            details = JsonSerializer.Deserialize<List<OrderDetailModel>>(content, options: new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
        }

        return details;
    }

    public async Task<CloseResultModel> CierreDiario(string fecha, string username)
    {
        CloseResultModel closeResponse = null;

        string url = Constants.BaseUrl + $"/api/cierrediario/usuario/branch/{SessionManager.BranchId}/{fecha}/{username}";

        HttpResponseMessage response = await _client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();

            closeResponse = JsonSerializer.Deserialize<CloseResultModel>(content, options: new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
        }

        return closeResponse;
    }

    public async Task<OrderResponse> PlaceOrderMultiple(PlaceOrderMultipleModel order)
    {
        var orderResponse = new OrderResponse();

        var json = JsonSerializer.Serialize(order);
        var data = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _client.PostAsync(Constants.BaseUrl + "/api/order/place/multiple", data);

        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();

            orderResponse = JsonSerializer.Deserialize<OrderResponse>(content, options: new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
        }

        return orderResponse;
    }

    public async Task<List<OrderModel>> GetOrderTop()
    {
        var orders = new List<OrderModel>();

        HttpResponseMessage response = await _client.GetAsync(
            Constants.BaseUrl + $"/api/Order/top/branch/{SessionManager.BranchId}/50");

        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();

            orders = JsonSerializer.Deserialize<List<OrderModel>>(content, options: new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
        }

        return orders;
    }

    public async Task<List<OrderModel>> GetOrderTop(string fecha1, string fecha2)
    {
        var orders = new List<OrderModel>();

        HttpResponseMessage response = await _client.GetAsync(
            Constants.BaseUrl + $"/api/Order/top/branch/{SessionManager.BranchId}/{fecha1}/{fecha2}");

        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();

            orders = JsonSerializer.Deserialize<List<OrderModel>>(content, options: new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
        }

        return orders;
    }
}
