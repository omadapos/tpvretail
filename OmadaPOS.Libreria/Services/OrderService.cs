using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Utils;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace OmadaPOS.Libreria.Services;

public interface IOrderService
{
    Task<OrderResponse?> PlaceOrder(PlaceOrderModel order);
    Task<OrderResponse?> PlaceOrderMultiple(PlaceOrderMultipleModel order);
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

    // Single shared options instance — creating per-call adds unnecessary allocation.
    private static readonly JsonSerializerOptions _jsonOpts =
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

    public OrderService(HttpClient client)
    {
        _client = client;
        // Do NOT set DefaultRequestHeaders here — the HttpClient is a singleton
        // shared across all transient service instances. Setting headers in the
        // constructor causes a race condition when tokens rotate. Use per-request
        // headers instead (see BuildRequest helper below).
    }

    // ── Auth helper ───────────────────────────────────────────────────────────

    private static HttpRequestMessage BuildRequest(HttpMethod method, string url)
    {
        var req = new HttpRequestMessage(method, url);
        req.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", SessionManager.Token);
        return req;
    }

    // ── Branch guard ──────────────────────────────────────────────────────────

    private static int RequireBranchId() =>
        SessionManager.BranchId
        ?? throw new InvalidOperationException("BranchId is not set. Ensure the user is logged in.");

    // ── Order placement ───────────────────────────────────────────────────────

    public async Task<OrderResponse?> PlaceOrder(PlaceOrderModel order)
    {
        var json = JsonSerializer.Serialize(order);
        using var req = BuildRequest(HttpMethod.Post, Constants.BaseUrl + "/api/order/place");
        req.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.SendAsync(req).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            return null;

        string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<OrderResponse>(content, _jsonOpts);
    }

    public async Task<OrderResponse?> PlaceOrderMultiple(PlaceOrderMultipleModel order)
    {
        var json = JsonSerializer.Serialize(order);
        using var req = BuildRequest(HttpMethod.Post, Constants.BaseUrl + "/api/order/place/multiple");
        req.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.SendAsync(req).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            return null;

        string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<OrderResponse>(content, _jsonOpts);
    }

    // ── Invoice / consecutivo ─────────────────────────────────────────────────

    public async Task<int> LoadLastInvoiceGeneral()
    {
        try
        {
            int branchId = RequireBranchId();
            using var req = BuildRequest(HttpMethod.Get,
                Constants.BaseUrl + $"/api/order/last/invoice/{branchId}");
            var response = await _client.SendAsync(req).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode) return 0;
            string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var orderLast = JsonSerializer.Deserialize<OrderLast>(content, _jsonOpts);
            return orderLast?.OrderId ?? 0;
        }
        catch (Exception) { return 0; }
    }

    public async Task<int> LoadLastInvoiceAdmin()
    {
        try
        {
            int branchId = RequireBranchId();
            using var req = BuildRequest(HttpMethod.Get,
                Constants.BaseUrl + $"/api/order/last/invoice/{branchId}/admin");
            var response = await _client.SendAsync(req).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode) return 0;
            string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var orderLast = JsonSerializer.Deserialize<OrderLast>(content, _jsonOpts);
            return orderLast?.OrderId ?? 0;
        }
        catch (Exception) { return 0; }
    }

    public async Task<long> LoadLastConsecutivoPayment()
    {
        try
        {
            int branchId = RequireBranchId();
            using var req = BuildRequest(HttpMethod.Get,
                Constants.BaseUrl + $"/api/ConsecutivoPayment/next/{branchId}");
            var response = await _client.SendAsync(req).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode) return 0;
            string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<long>(content, _jsonOpts);
        }
        catch (Exception) { return 0; }
    }

    // ── Order queries ─────────────────────────────────────────────────────────

    public async Task<OrderModel> GetOrderById(int orderId)
    {
        try
        {
            using var req = BuildRequest(HttpMethod.Get,
                Constants.BaseUrl + $"/api/order/{orderId}");
            var response = await _client.SendAsync(req).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode) return null;
            string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<OrderModel>(content, _jsonOpts);
        }
        catch (Exception) { return null; }
    }

    public async Task<List<OrderDetailModel>> GetOrderDetailsByOrderId(int orderId)
    {
        try
        {
            using var req = BuildRequest(HttpMethod.Get,
                Constants.BaseUrl + $"/api/orderdetail/{orderId}");
            var response = await _client.SendAsync(req).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode) return new List<OrderDetailModel>();
            string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<List<OrderDetailModel>>(content, _jsonOpts)
                   ?? new List<OrderDetailModel>();
        }
        catch (Exception) { return new List<OrderDetailModel>(); }
    }

    public async Task<CloseResultModel> CierreDiario(string fecha, string username)
    {
        try
        {
            int branchId = RequireBranchId();
            string url = Constants.BaseUrl +
                $"/api/cierrediario/usuario/branch/{branchId}/{Uri.EscapeDataString(fecha)}/{Uri.EscapeDataString(username)}";
            using var req = BuildRequest(HttpMethod.Get, url);
            var response = await _client.SendAsync(req).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode) return null;
            string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<CloseResultModel>(content, _jsonOpts);
        }
        catch (Exception) { return null; }
    }

    public async Task<List<OrderModel>> GetOrderTop()
    {
        try
        {
            int branchId = RequireBranchId();
            using var req = BuildRequest(HttpMethod.Get,
                Constants.BaseUrl + $"/api/Order/top/branch/{branchId}/50");
            var response = await _client.SendAsync(req).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode) return new List<OrderModel>();
            string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<List<OrderModel>>(content, _jsonOpts)
                   ?? new List<OrderModel>();
        }
        catch (Exception) { return new List<OrderModel>(); }
    }

    public async Task<List<OrderModel>> GetOrderTop(string fecha1, string fecha2)
    {
        try
        {
            int branchId = RequireBranchId();
            string url = Constants.BaseUrl +
                $"/api/Order/top/branch/{branchId}/{Uri.EscapeDataString(fecha1)}/{Uri.EscapeDataString(fecha2)}";
            using var req = BuildRequest(HttpMethod.Get, url);
            var response = await _client.SendAsync(req).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode) return new List<OrderModel>();
            string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<List<OrderModel>>(content, _jsonOpts)
                   ?? new List<OrderModel>();
        }
        catch (Exception) { return new List<OrderModel>(); }
    }
}
