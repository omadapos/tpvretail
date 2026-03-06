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
    
    public decimal Total => _items.Sum(item => item.Subtotal);

    public string MachineGuid => _machineGuid;

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
            lock (_lockObject)
            {
                var existingItem = _items.FirstOrDefault(i => i.ProductId == item.ProductId);
                if (existingItem != null)
                {
                    existingItem.Quantity += item.Quantity;
                    _sqliteManager.UpdateCartItemQuantityAsync(existingItem.ProductId, existingItem.Quantity, _machineGuid)
                        .GetAwaiter().GetResult();
                }
                else
                {
                    var newItem = item.Clone();
                    _items.Add(newItem);
                    _sqliteManager.SaveCartItemAsync(newItem, _machineGuid)
                        .GetAwaiter().GetResult();
                }

                OnCartChanged();
                return true;
            }
        }
        catch (Exception)
        {
            // Log error or handle appropriately
            return false;
        }
    }

    public bool UpdateQuantity(int productId, int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Quantity cannot be negative", nameof(quantity));

        try
        {
            lock (_lockObject)
            {
                var item = _items.FirstOrDefault(i => i.ProductId == productId);
                if (item == null)
                    return false;

                if (quantity == 0)
                {
                    _items.Remove(item);
                    _sqliteManager.RemoveCartItemAsync(item.ProductId, _machineGuid)
                        .GetAwaiter().GetResult();
                }
                else
                {
                    item.Quantity = quantity;
                    _sqliteManager.UpdateCartItemQuantityAsync(item.ProductId, quantity, _machineGuid)
                        .GetAwaiter().GetResult();
                }

                OnCartChanged();
                return true;
            }
        }
        catch (Exception)
        {
            // Log error or handle appropriately
            return false;
        }
    }

    public bool RemoveItem(int productId)
    {
        try
        {
            lock (_lockObject)
            {
                var item = _items.FirstOrDefault(i => i.ProductId == productId);
                if (item == null)
                    return false;

                _items.Remove(item);
                _sqliteManager.RemoveCartItemAsync(item.ProductId, _machineGuid)
                    .GetAwaiter().GetResult();

                OnCartChanged();
                return true;
            }
        }
        catch (Exception)
        {
            // Log error or handle appropriately
            return false;
        }
    }

    public void Clear()
    {
        try
        {
            lock (_lockObject)
            {
                _items.Clear();
                _sqliteManager.ClearCartAsync(_machineGuid)
                    .GetAwaiter().GetResult();

                OnCartChanged();
            }
        }
        catch (Exception)
        {
            // Log error or handle appropriately
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

