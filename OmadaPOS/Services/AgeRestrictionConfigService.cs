using Microsoft.Extensions.Logging;

namespace OmadaPOS.Services;

// ── DTO ────────────────────────────────────────────────────────────────────────

/// <summary>Represents a single age-restriction rule stored in SQLite.</summary>
public record AgeRestrictedEntry(
    int       Id,
    string?   Upc,
    int?      CategoryId,
    string?   Note,
    DateTime  CreatedAt);

// ── Interface ──────────────────────────────────────────────────────────────────

public interface IAgeRestrictionConfigService
{
    /// <summary>
    /// Returns true when the product identified by <paramref name="upc"/> or
    /// <paramref name="categoryId"/> requires age verification (≥21).
    /// This is the hot path — called for every product lookup — so it uses only
    /// the in-memory cache; no DB access.
    /// </summary>
    bool IsRestricted(string? upc, int categoryId);

    Task AddUpcAsync(string upc, string? note);
    Task AddCategoryAsync(int categoryId, string? note);
    Task RemoveUpcAsync(string upc);
    Task RemoveCategoryAsync(int categoryId);
    Task<List<AgeRestrictedEntry>> GetAllEntriesAsync();

    /// <summary>
    /// Reloads the in-memory cache from SQLite.
    /// Called once at startup and after every write operation.
    /// </summary>
    Task ReloadAsync();
}

// ── Implementation ─────────────────────────────────────────────────────────────

public sealed class AgeRestrictionConfigService : IAgeRestrictionConfigService
{
    private readonly ISqliteManager             _db;
    private readonly ILogger<AgeRestrictionConfigService>? _logger;

    // In-memory caches — rebuilt by ReloadAsync()
    private volatile HashSet<string> _restrictedUpcs       = new(StringComparer.OrdinalIgnoreCase);
    private volatile HashSet<int>    _restrictedCategories = [];

    // ── Hardcoded dev UPCs ─────────────────────────────────────────────────────
    // Remove (or move to SQLite) once the backend natively returns
    // RequiresAgeVerification = true for alcohol products.
    // ── DEV / QA test UPCs ────────────────────────────────────────────────────
    // These UPCs are always treated as age-restricted regardless of the DB or
    // backend flag.  Remove or empty this set before going to production.
    private static readonly HashSet<string> _devUPCs = new(StringComparer.OrdinalIgnoreCase)
    {
        "071537001303",   // DEV TEST — remove before production
    };

    public AgeRestrictionConfigService(ISqliteManager db, ILogger<AgeRestrictionConfigService>? logger = null)
    {
        _db     = db     ?? throw new ArgumentNullException(nameof(db));
        _logger = logger;
    }

    // ── Hot path — no allocations ──────────────────────────────────────────────

    public bool IsRestricted(string? upc, int categoryId)
    {
        // Check hardcoded dev list first (fastest)
        if (upc != null && _devUPCs.Contains(upc))
            return true;

        // Check in-memory cache (loaded from SQLite at startup)
        if (upc != null && _restrictedUpcs.Contains(upc))
            return true;

        if (_restrictedCategories.Contains(categoryId))
            return true;

        return false;
    }

    // ── Write operations ───────────────────────────────────────────────────────

    public async Task AddUpcAsync(string upc, string? note)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(upc);
        await _db.AddAgeRestrictedUpcAsync(upc, note).ConfigureAwait(false);
        await ReloadAsync().ConfigureAwait(false);
        _logger?.LogInformation("Age restriction added for UPC {UPC}", upc);
    }

    public async Task AddCategoryAsync(int categoryId, string? note)
    {
        await _db.AddAgeRestrictedCategoryAsync(categoryId, note).ConfigureAwait(false);
        await ReloadAsync().ConfigureAwait(false);
        _logger?.LogInformation("Age restriction added for CategoryId {CategoryId}", categoryId);
    }

    public async Task RemoveUpcAsync(string upc)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(upc);
        await _db.RemoveAgeRestrictedUpcAsync(upc).ConfigureAwait(false);
        await ReloadAsync().ConfigureAwait(false);
        _logger?.LogInformation("Age restriction removed for UPC {UPC}", upc);
    }

    public async Task RemoveCategoryAsync(int categoryId)
    {
        await _db.RemoveAgeRestrictedCategoryAsync(categoryId).ConfigureAwait(false);
        await ReloadAsync().ConfigureAwait(false);
        _logger?.LogInformation("Age restriction removed for CategoryId {CategoryId}", categoryId);
    }

    public Task<List<AgeRestrictedEntry>> GetAllEntriesAsync()
        => _db.GetAllAgeRestrictedEntriesAsync();

    // ── Cache reload ───────────────────────────────────────────────────────────

    public async Task ReloadAsync()
    {
        try
        {
            var upcs = await _db.GetAgeRestrictedUpcsAsync().ConfigureAwait(false);
            var cats = await _db.GetAgeRestrictedCategoryIdsAsync().ConfigureAwait(false);

            // Swap atomically — volatile write ensures visibility on all threads
            _restrictedUpcs       = new HashSet<string>(upcs, StringComparer.OrdinalIgnoreCase);
            _restrictedCategories = [.. cats];

            _logger?.LogDebug(
                "Age restriction cache reloaded: {UpcCount} UPCs, {CatCount} categories",
                upcs.Count, cats.Count);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to reload age restriction cache");
        }
    }
}
