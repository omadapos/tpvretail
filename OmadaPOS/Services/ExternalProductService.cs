using OmadaPOS.Presentation.Styling;
using System.Text.Json;

namespace OmadaPOS.Services;

/// <summary>
/// Represents all product data returned by the OmadaPOS global product catalog API.
/// All fields except Name are nullable — the catalog may be partially enriched.
/// </summary>
public sealed record ExternalProductInfo(
    string   Name,
    string?  Brand,
    string?  ImageUrl,       // first "main" image, or first image in list
    string?  CategoryName,   // category name from API (may be null)
    string?  Size,           // e.g. "1.5 oz", "12 fl oz"
    string?  WeightGrams,    // raw weightGrams as display string, e.g. "42.5 g"
    string?  DescriptionEn,
    string?  DescriptionEs,
    bool     EbtEligible,
    bool     WicEligible,
    bool     RequiresAgeVerification
);

public interface IExternalProductService
{
    /// <summary>
    /// Looks up a UPC in the OmadaPOS global product catalog.
    /// Returns null if the product is not found or the request fails.
    /// </summary>
    Task<ExternalProductInfo?> LookupByUpcAsync(string upc, CancellationToken ct = default);
}

/// <summary>
/// Queries the OmadaPOS product catalog API for UPC lookups when the local
/// database does not contain the product.
/// </summary>
public sealed class ExternalProductService : IExternalProductService
{
    private readonly HttpClient _http;

    public ExternalProductService(HttpClient http)
    {
        _http = http;
    }

    public async Task<ExternalProductInfo?> LookupByUpcAsync(string upc, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(upc)) return null;

        try
        {
            using var req = new HttpRequestMessage(
                HttpMethod.Get,
                $"{AppConstants.ExternalProductApiBase}{upc.Trim()}");

            req.Headers.Add("x-api-key", AppConstants.ExternalProductApiKey);
            req.Headers.UserAgent.ParseAdd(AppConstants.UserAgent);

            using var resp = await _http.SendAsync(req, ct).ConfigureAwait(false);

            if (resp.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
            if (!resp.IsSuccessStatusCode)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[ExternalProductService] HTTP {(int)resp.StatusCode} for UPC {upc}");
                return null;
            }

            string json = await resp.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
            return ParseResponse(json);
        }
        catch (OperationCanceledException)
        {
            System.Diagnostics.Debug.WriteLine($"[ExternalProductService] Timeout for UPC {upc}");
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ExternalProductService] Error: {ex.Message}");
            return null;
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // JSON parsing — maps the { success, data: {...}, images: [...] } envelope
    // returned by https://api.omadapos.com/v1/products/upc/{upc}
    // ─────────────────────────────────────────────────────────────────────────
    private static ExternalProductInfo? ParseResponse(string json)
    {
        try
        {
            using var doc  = JsonDocument.Parse(json);
            var       root = doc.RootElement;

            // Top-level success guard
            if (root.TryGetProperty("success", out var ok) && ok.ValueKind == JsonValueKind.False)
                return null;

            // Unwrap envelope: { data: {...} } or { product: {...} } or root itself
            JsonElement p = root;
            if (root.TryGetProperty("data",    out var d) && d.ValueKind == JsonValueKind.Object) p = d;
            else if (root.TryGetProperty("product", out var pr) && pr.ValueKind == JsonValueKind.Object) p = pr;

            // Name is required — abort if missing
            string? name = GetStr(p, "name", "productName", "product_name", "title");
            if (string.IsNullOrWhiteSpace(name)) return null;

            // ── Scalar fields ─────────────────────────────────────────────────
            string? brand        = GetStr(p, "brand", "brandName", "brand_name", "manufacturer");
            string? categoryName = GetStr(p, "category", "categoryName", "category_name");
            string? size         = GetStr(p, "size", "packageSize", "package_size", "servingSize");
            string? descEn       = GetStr(p, "descriptionEn", "description_en", "description", "descriptionEnglish");
            string? descEs       = GetStr(p, "descriptionEs", "description_es", "descriptionSpanish");

            // weightGrams may be a number — read as decimal then format
            string? weightDisplay = null;
            if (p.TryGetProperty("weightGrams", out var wg) && wg.ValueKind == JsonValueKind.Number)
            {
                var grams = wg.GetDecimal();
                weightDisplay = grams > 0 ? $"{grams:0.#} g" : null;
            }

            // ── Boolean flags ─────────────────────────────────────────────────
            bool ebt = GetBool(p, "ebtEligible", "ebt_eligible", "ebt");
            bool wic = GetBool(p, "wicEligible", "wic_eligible", "wic");
            bool age = GetBool(p, "requiresAgeVerification", "age_restricted", "ageRestricted");

            // ── Images array — prefer type == "main", then lowest sortOrder ──
            string? imageUrl = null;
            if (p.TryGetProperty("images", out var imgs) && imgs.ValueKind == JsonValueKind.Array)
                imageUrl = BestImageUrl(imgs);

            // Fallback: scalar image field (older API shape)
            if (imageUrl == null)
                imageUrl = GetStr(p, "imageUrl", "image_url", "image", "photo", "thumbnail");

            return new ExternalProductInfo(
                name, brand, imageUrl, categoryName,
                size, weightDisplay, descEn, descEs,
                ebt, wic, age);
        }
        catch (JsonException ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ExternalProductService] JSON parse error: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Picks the best image URL from the images array.
    /// Priority: type == "main" → lowest sortOrder → first entry.
    /// </summary>
    private static string? BestImageUrl(JsonElement imagesArray)
    {
        string? mainUrl  = null;
        string? firstUrl = null;
        int     minSort  = int.MaxValue;

        foreach (var img in imagesArray.EnumerateArray())
        {
            string? url  = GetStr(img, "url", "imageUrl", "src");
            if (string.IsNullOrWhiteSpace(url)) continue;

            firstUrl ??= url;

            string? type = GetStr(img, "type", "imageType");
            int sort = img.TryGetProperty("sortOrder", out var so) && so.ValueKind == JsonValueKind.Number
                ? so.GetInt32() : int.MaxValue;

            if (string.Equals(type, "main", StringComparison.OrdinalIgnoreCase))
            {
                if (mainUrl == null || sort < minSort)
                {
                    mainUrl = url;
                    minSort = sort;
                }
            }
        }

        return mainUrl ?? firstUrl;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────
    private static string? GetStr(JsonElement el, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (el.TryGetProperty(key, out var v) && v.ValueKind == JsonValueKind.String)
            {
                var s = v.GetString();
                if (!string.IsNullOrWhiteSpace(s)) return s.Trim();
            }
        }
        return null;
    }

    private static bool GetBool(JsonElement el, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (el.TryGetProperty(key, out var v))
            {
                if (v.ValueKind == JsonValueKind.True)  return true;
                if (v.ValueKind == JsonValueKind.False) return false;
            }
        }
        return false;
    }
}
