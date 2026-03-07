using OmadaPOS.Libreria.Utils;
using OmadaPOS.Models;

namespace OmadaPOS.Services;

public interface IShoppingCart
{
    IReadOnlyList<CartItem> Items { get; }

    int ItemCount { get; }

    decimal Total { get; }

    string MachineGuid { get; }

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
    private readonly List<CartItem> _items = new();
    private readonly object _lockObject = new();
    private readonly ISqliteManager _sqliteManager;
    private readonly string? _machineGuid;

    public IReadOnlyList<CartItem> Items => _items.AsReadOnly();
    
    public int ItemCount => _items.Count;

    // CRÍTICO: Total incluye impuesto (Subtotal + TaxAmount)
    public decimal Total => _items.Sum(item => item.Total);

    public string MachineGuid => _machineGuid ?? string.Empty;

    public event EventHandler? CartChanged;

    public ShoppingCart(ISqliteManager sqliteManager)
    {
        _sqliteManager = sqliteManager ?? throw new ArgumentNullException(nameof(sqliteManager));
        _machineGuid = WindowsIdProvider.GetMachineGuid();
    }

    public async Task LoadCartAsync()
    {
        try
        {
            var items = await _sqliteManager.GetCartItemsAsync(_machineGuid);
            lock (_lockObject)
            {
                _items.Clear();
                _items.AddRange(items);
            }
            OnCartChanged();
        }
        catch (Exception)
        {
            // Log error or handle appropriately
            throw;
        }
    }

    public bool AddItem(CartItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));
        if (item.Quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(item));

        try
        {
            CartItem? toUpdate = null;
            CartItem? toAdd    = null;

            // Mutación en memoria dentro del lock — rápido, sin I/O
            lock (_lockObject)
            {
                var existing = _items.FirstOrDefault(i => i.ProductId == item.ProductId);
                if (existing != null)
                {
                    existing.Quantity += item.Quantity;
                    toUpdate = existing;
                }
                else
                {
                    toAdd = item.Clone();
                    _items.Add(toAdd);
                }
            }

            // Persistencia SQLite fuera del lock — fire-and-forget seguro
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
        catch (Exception)
        {
            return false;
        }
    }

    public bool UpdateQuantity(int productId, int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Quantity cannot be negative", nameof(quantity));

        try
        {
            CartItem? removed = null;
            CartItem? updated = null;

            lock (_lockObject)
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
        catch (Exception)
        {
            return false;
        }
    }

    public bool RemoveItem(int productId)
    {
        try
        {
            CartItem? removed = null;

            lock (_lockObject)
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
        catch (Exception)
        {
            return false;
        }
    }

    public void Clear()
    {
        try
        {
            lock (_lockObject)
                _items.Clear();

            _ = Task.Run(async () => await _sqliteManager.ClearCartAsync(_machineGuid));

            OnCartChanged();
        }
        catch (Exception)
        {
            // Log error
        }
    }

    public CartItem? GetItem(int productId)
    {
        lock (_lockObject)
        {
            return _items.FirstOrDefault(i => i.ProductId == productId)?.Clone();
        }
    }

    protected virtual void OnCartChanged()
    {
        CartChanged?.Invoke(this, EventArgs.Empty);
    }
}

