using Microsoft.Extensions.Logging;
using OmadaPOS.Domain.Services;
using OmadaPOS.Libreria.Utils;
using OmadaPOS.Models;

namespace OmadaPOS.Services;

public interface IShoppingCart
{
    IReadOnlyList<CartItem> Items { get; }

    int     ItemCount  { get; }
    decimal Subtotal   { get; }
    decimal TotalTax   { get; }
    decimal Total      { get; }
    string  MachineGuid { get; }

    bool AddItem(CartItem item);
    bool UpdateQuantity(int productId, int quantity);
    bool RemoveItem(int productId);
    void Clear();
    CartItem? GetItem(int productId);

    event EventHandler? CartChanged;

    Task LoadCartAsync();
}

public class ShoppingCart : IShoppingCart
{
    private readonly List<CartItem>     _items      = new();
    private readonly object             _lock       = new();
    private readonly ISqliteManager     _sqliteManager;
    private readonly IPricingEngine     _pricing;
    private readonly ILogger<ShoppingCart> _logger;
    private readonly string?            _machineGuid;

    public IReadOnlyList<CartItem> Items => _items.AsReadOnly();

    public int     ItemCount   => _items.Count;
    public decimal Subtotal    => _items.Sum(i => i.Subtotal);
    public decimal TotalTax    => _items.Sum(i => i.TaxAmount);
    /// <summary>Cart grand total (subtotal + tax). Always consistent with what PricingEngine computed.</summary>
    public decimal Total       => _items.Sum(i => i.Total);
    public string  MachineGuid => _machineGuid ?? string.Empty;

    public event EventHandler? CartChanged;

    public ShoppingCart(ISqliteManager sqliteManager, IPricingEngine pricingEngine, ILogger<ShoppingCart> logger)
    {
        _sqliteManager = sqliteManager ?? throw new ArgumentNullException(nameof(sqliteManager));
        _pricing       = pricingEngine ?? throw new ArgumentNullException(nameof(pricingEngine));
        _logger        = logger        ?? throw new ArgumentNullException(nameof(logger));
        _machineGuid   = WindowsIdProvider.GetMachineGuid();
    }

    // ─── Load ─────────────────────────────────────────────────────────────────

    public async Task LoadCartAsync()
    {
        var items = await _sqliteManager.GetCartItemsAsync(_machineGuid);
        lock (_lock)
        {
            _items.Clear();
            foreach (var item in items)
            {
                _pricing.ApplyPricing(item);
                _items.Add(item);
            }
        }
        OnCartChanged();
    }

    // ─── AddItem ──────────────────────────────────────────────────────────────

    public bool AddItem(CartItem item)
    {
        if (item == null)         throw new ArgumentNullException(nameof(item));
        if (item.Quantity <= 0)   throw new ArgumentException("Quantity must be greater than zero.", nameof(item));

        try
        {
            CartItem? toUpdate = null;
            CartItem? toAdd    = null;

            lock (_lock)
            {
                var existing = _items.FirstOrDefault(i => i.ProductId == item.ProductId);
                if (existing != null)
                {
                    existing.Quantity += item.Quantity;
                    _pricing.ApplyPricing(existing);
                    toUpdate = existing;
                }
                else
                {
                    _pricing.ApplyPricing(item);
                    toAdd = item.Clone();
                    _items.Add(toAdd);
                }
            }

            _ = Task.Run(async () =>
            {
                if (toUpdate != null)
                    await _sqliteManager.UpdateCartItemQuantityAsync(toUpdate.ProductId, toUpdate.Quantity, _machineGuid);
                else if (toAdd != null)
                    await _sqliteManager.SaveCartItemAsync(toAdd, _machineGuid);
            });

            OnCartChanged();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ShoppingCart.AddItem failed for ProductId={ProductId}", item.ProductId);
            return false;
        }
    }

    // ─── UpdateQuantity ───────────────────────────────────────────────────────

    public bool UpdateQuantity(int productId, int quantity)
    {
        if (quantity < 0) throw new ArgumentException("Quantity cannot be negative.", nameof(quantity));

        try
        {
            CartItem? removed = null;
            CartItem? updated = null;

            lock (_lock)
            {
                var item = _items.FirstOrDefault(i => i.ProductId == productId);
                if (item == null) return false;

                if (quantity == 0)
                {
                    _items.Remove(item);
                    removed = item;
                }
                else
                {
                    item.Quantity = quantity;
                    _pricing.ApplyPricing(item);
                    updated = item;
                }
            }

            _ = Task.Run(async () =>
            {
                if (removed != null)
                    await _sqliteManager.RemoveCartItemAsync(removed.ProductId, _machineGuid);
                else if (updated != null)
                    await _sqliteManager.UpdateCartItemQuantityAsync(updated.ProductId, updated.Quantity, _machineGuid);
            });

            OnCartChanged();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ShoppingCart.UpdateQuantity failed for ProductId={ProductId}", productId);
            return false;
        }
    }

    // ─── RemoveItem ───────────────────────────────────────────────────────────

    public bool RemoveItem(int productId)
    {
        try
        {
            CartItem? removed = null;

            lock (_lock)
            {
                var item = _items.FirstOrDefault(i => i.ProductId == productId);
                if (item == null) return false;

                _items.Remove(item);
                removed = item;
            }

            _ = Task.Run(async () =>
            {
                if (removed != null)
                    await _sqliteManager.RemoveCartItemAsync(removed.ProductId, _machineGuid);
            });

            OnCartChanged();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ShoppingCart.RemoveItem failed for ProductId={ProductId}", productId);
            return false;
        }
    }

    // ─── Clear ────────────────────────────────────────────────────────────────

    public void Clear()
    {
        try
        {
            lock (_lock)
                _items.Clear();

            _ = Task.Run(async () => await _sqliteManager.ClearCartAsync(_machineGuid));

            OnCartChanged();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ShoppingCart.Clear failed");
        }
    }

    // ─── GetItem ──────────────────────────────────────────────────────────────

    public CartItem? GetItem(int productId)
    {
        lock (_lock)
            return _items.FirstOrDefault(i => i.ProductId == productId)?.Clone();
    }

    // ─── Events ───────────────────────────────────────────────────────────────

    protected virtual void OnCartChanged() => CartChanged?.Invoke(this, EventArgs.Empty);
}
