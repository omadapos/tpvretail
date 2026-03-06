using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using OmadaPOS.Libreria.Models;
using OmadaPOS.Models;

namespace OmadaPOS.Services;

public interface ISqliteManager : IDisposable
{
    Task InitializeDatabaseAsync();
    Task DropTablesAsync();

    // Cart methods
    Task SaveCartItemAsync(CartItem item, string sessionId);
    Task<List<CartItem>> GetCartItemsAsync(string sessionId);
    Task RemoveCartItemAsync(int productId, string sessionId);
    Task UpdateCartItemQuantityAsync(int productId, double quantity, string sessionId);
    Task ClearCartAsync(string sessionId);
    Task<int> GetNextCartNumberAsync(string sessionId);

    // Hold methods
    Task HoldCartAsync(string sessionId, string holdId);
    Task<List<HoldCartModel>> GetHeldCartsBySessionAsync(string sessionId);
    Task<HoldCartModel?> GetHeldCartsByIdAsync(string holdId);
    Task<List<CartItem>> RetrieveHeldCartAsync(string holdId);
    Task DeleteHeldCartAsync(string holdId);

    // Payment methods
    Task SavePaymentAsync(string sessionId, decimal total, string paymentType);
    Task<List<PaymentModel>> GetPaymentsAsync(string sessionId);
    Task ClearPaymentAsync(string sessionId);
}

public sealed class SqliteManager : ISqliteManager, IDisposable
{
    private readonly string _dbPath;
    private readonly ILogger<SqliteManager>? _logger;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private bool _disposed;

    // Tables
    private const string CREATE_CART_TABLE_SQL = @"
        CREATE TABLE IF NOT EXISTS CartItems (
            SessionId TEXT NOT NULL,
            Number INTEGER NOT NULL,
            ProductId INTEGER NOT NULL,
            ProductName TEXT NOT NULL,
            UPC TEXT,
            Image TEXT,
            UnitPrice DECIMAL(18,2) NOT NULL,
            Quantity FLOAT NOT NULL,
            Weight FLOAT NOT NULL DEFAULT 0,
            Tax FLOAT NOT NULL DEFAULT 0,
            Date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
            PromotionName TEXT,
            PromotionValue FLOAT,
            PromotionLimit DECIMAL(18,2),
            IsEBT BOOLEAN NOT NULL DEFAULT 0,
            LastModified DATETIME DEFAULT CURRENT_TIMESTAMP,
            PRIMARY KEY (SessionId, ProductId)
        )";

    private const string CREATE_PAYMENTS_TABLE_SQL = @"
        CREATE TABLE IF NOT EXISTS Payments (
            SessionId TEXT NOT NULL,
            PaymentId TEXT NOT NULL,
            Total DECIMAL(18,2) NOT NULL,
            PaymentType TEXT NOT NULL,
            LastModified DATETIME DEFAULT CURRENT_TIMESTAMP,
            PRIMARY KEY (SessionId, PaymentId)
        )";

    private const string CREATE_HOLD_TABLE_SQL = @"
        CREATE TABLE IF NOT EXISTS HoldCarts (
            HoldId TEXT NOT NULL,
            SessionId TEXT NOT NULL,
            LastModified DATETIME DEFAULT CURRENT_TIMESTAMP,
            PRIMARY KEY (HoldId, SessionId)
        )";

    private const string CREATE_HOLD_ITEMS_TABLE_SQL = @"
        CREATE TABLE IF NOT EXISTS HoldCartItems (
            HoldId TEXT NOT NULL,
            Number INTEGER NOT NULL,
            ProductId INTEGER NOT NULL,
            ProductName TEXT NOT NULL,
            UPC TEXT,
            Image TEXT,
            UnitPrice DECIMAL(18,2) NOT NULL,
            Quantity FLOAT NOT NULL,
            Weight FLOAT NOT NULL DEFAULT 0,
            Tax FLOAT NOT NULL DEFAULT 0,
            PromotionName TEXT,
            PromotionValue FLOAT,
            PromotionLimit DECIMAL(18,2),
            IsEBT BOOLEAN NOT NULL DEFAULT 0,
            LastModified DATETIME DEFAULT CURRENT_TIMESTAMP,
            PRIMARY KEY (HoldId, ProductId)
        )";

    private const string INSERT_CART_ITEM_SQL = @"
        INSERT OR REPLACE INTO CartItems 
        (SessionId, Number, ProductId, ProductName, UPC, Image, UnitPrice, Quantity, 
         Weight, Tax, Date, PromotionName, PromotionValue, PromotionLimit, IsEBT, LastModified)
        VALUES 
        (@SessionId, @Number, @ProductId, @ProductName, @UPC, @Image, @UnitPrice, @Quantity,
         @Weight, @Tax, @Date, @PromotionName, @PromotionValue, @PromotionLimit, @IsEBT, CURRENT_TIMESTAMP)";

    private const string SELECT_CART_ITEMS_SQL = @"
        SELECT Number, ProductId, ProductName, UPC, Image, UnitPrice, Quantity, 
               Weight, Tax, Date, PromotionName, PromotionValue, PromotionLimit, IsEBT
        FROM CartItems
        WHERE SessionId = @SessionId
        ORDER BY Number";

    private const string DELETE_CART_ITEM_SQL = @"
        DELETE FROM CartItems
        WHERE SessionId = @SessionId AND ProductId = @ProductId";

    private const string DELETE_CART_SESSION_SQL = @"
        DELETE FROM CartItems
        WHERE SessionId = @SessionId";

    private const string GET_NEXT_NUMBER_SQL = @"
        SELECT COALESCE(MAX(Number), 0) + 1
        FROM CartItems
        WHERE SessionId = @SessionId";

    private const string INSERT_PAYMENT_SQL = @"
        INSERT INTO Payments 
        (SessionId, PaymentId, Total, PaymentType, LastModified)
        VALUES 
        (@SessionId, @PaymentId, @Total, @PaymentType, CURRENT_TIMESTAMP)";

    private const string SELECT_PAYMENTS_SQL = @"
        SELECT PaymentId, Total, PaymentType
        FROM Payments
        WHERE SessionId = @SessionId";

    private const string DELETE_PAYMENT_SESSION_SQL = @"
        DELETE FROM Payments
        WHERE SessionId = @SessionId";

    private const string INSERT_HOLD_CART_SQL = @"
        INSERT INTO HoldCarts (HoldId, SessionId)
        VALUES (@HoldId, @SessionId)";

    private const string INSERT_HOLD_ITEM_SQL = @"
        INSERT INTO HoldCartItems 
        (HoldId, Number, ProductId, ProductName, UPC, Image, UnitPrice, Quantity, 
         Weight, Tax, PromotionName, PromotionValue, PromotionLimit, IsEBT)
        SELECT 
            @HoldId, Number, ProductId, ProductName, UPC, Image, UnitPrice, Quantity,
            Weight, Tax, PromotionName, PromotionValue, PromotionLimit, IsEBT
        FROM CartItems
        WHERE SessionId = @SessionId";

    private const string SELECT_HELD_ITEMS_SQL = @"
        SELECT Number, ProductId, ProductName, UPC, Image, UnitPrice, Quantity, 
               Weight, Tax, PromotionName, PromotionValue, PromotionLimit, IsEBT
        FROM HoldCartItems
        WHERE HoldId = @HoldId
        ORDER BY Number";

    private const string DELETE_HOLD_CART_SQL = @"
        DELETE FROM HoldCarts WHERE HoldId = @HoldId";

    private const string DELETE_HOLD_ITEMS_SQL = @"
        DELETE FROM HoldCartItems WHERE HoldId = @HoldId";

    private const string SELECT_HELD_CARTS_BY_SESSION_SQL = @"
        SELECT HoldId, SessionId
        FROM HoldCarts
        WHERE SessionId = @SessionId";

    private const string SELECT_HELD_CARTS_BY_ID_SQL = @"
        SELECT HoldId, SessionId
        FROM HoldCarts
        WHERE HoldId = @HoldId";

    public SqliteManager(string dbFileName = "sqliteomada.db", ILogger<SqliteManager>? logger = null)
    {
        if (string.IsNullOrWhiteSpace(dbFileName))
            throw new ArgumentNullException(nameof(dbFileName));

        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        _dbPath = Path.Combine(localAppData, dbFileName);
        _logger = logger;
    }

    private string ConnectionString => $"Filename={_dbPath};Mode=ReadWrite;Cache=Shared";

    public async Task InitializeDatabaseAsync()
    {
        try
        {
            await _semaphore.WaitAsync();
            try
            {
                var directory = Path.GetDirectoryName(_dbPath);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                if (!File.Exists(_dbPath))
                    await File.Create(_dbPath).DisposeAsync();

                await using var db = new SqliteConnection(ConnectionString);
                await db.OpenAsync();

                await using var createCartTable = new SqliteCommand(CREATE_CART_TABLE_SQL, db);
                await createCartTable.ExecuteNonQueryAsync();

                await using var createPaymentsTable = new SqliteCommand(CREATE_PAYMENTS_TABLE_SQL, db);
                await createPaymentsTable.ExecuteNonQueryAsync();

                await using var createHoldTable = new SqliteCommand(CREATE_HOLD_TABLE_SQL, db);
                await createHoldTable.ExecuteNonQueryAsync();

                await using var createHoldItemsTable = new SqliteCommand(CREATE_HOLD_ITEMS_TABLE_SQL, db);
                await createHoldItemsTable.ExecuteNonQueryAsync();

                _logger?.LogInformation("Database initialized successfully at {DbPath}", _dbPath);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error initializing database at {DbPath}", _dbPath);
            throw;
        }
    }

    // Cart
    public async Task SaveCartItemAsync(CartItem item, string sessionId)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
            throw new ArgumentNullException(nameof(sessionId));

        try
        {
            await using var db = new SqliteConnection(ConnectionString);
            await db.OpenAsync();

            item.Number = await GetNextCartNumberAsync(sessionId);
            
            await using var command = new SqliteCommand(INSERT_CART_ITEM_SQL, db);
            command.Parameters.AddWithValue("@SessionId", sessionId);
            command.Parameters.AddWithValue("@Number", item.Number);
            command.Parameters.AddWithValue("@ProductId", item.ProductId);
            command.Parameters.AddWithValue("@ProductName", item.ProductName);
            command.Parameters.AddWithValue("@UPC", (object?)item.UPC ?? DBNull.Value);
            command.Parameters.AddWithValue("@Image", (object?)item.Image ?? DBNull.Value);
            command.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
            command.Parameters.AddWithValue("@Quantity", item.Quantity);
            command.Parameters.AddWithValue("@Weight", item.Weight);
            command.Parameters.AddWithValue("@Tax", item.Tax);
            command.Parameters.AddWithValue("@Date", item.Date);
            command.Parameters.AddWithValue("@PromotionName", (object?)item.PromotionName ?? DBNull.Value);
            command.Parameters.AddWithValue("@PromotionValue", item.PromotionValue);
            command.Parameters.AddWithValue("@PromotionLimit", item.PromotionLimit);
            command.Parameters.AddWithValue("@IsEBT", item.IsEBT);

            await command.ExecuteNonQueryAsync();

            _logger?.LogDebug("Cart item saved: Number={Number}, ProductId={ProductId}, SessionId={SessionId}", 
                item.Number, item.ProductId, sessionId);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error saving cart item: ProductId={ProductId}, SessionId={SessionId}", item.ProductId, sessionId);
            throw;
        }
    }

    public async Task<List<CartItem>> GetCartItemsAsync(string sessionId)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
            throw new ArgumentNullException(nameof(sessionId));

        var items = new List<CartItem>();
        try
        {
            await using var db = new SqliteConnection(ConnectionString);
            await db.OpenAsync();

            await using var command = new SqliteCommand(SELECT_CART_ITEMS_SQL, db);
            command.Parameters.AddWithValue("@SessionId", sessionId);

            await using var reader = await command.ExecuteReaderAsync();
            
            // Cache column ordinals
            var numberOrdinal = reader.GetOrdinal("Number");
            var productIdOrdinal = reader.GetOrdinal("ProductId");
            var productNameOrdinal = reader.GetOrdinal("ProductName");
            var upcOrdinal = reader.GetOrdinal("UPC");
            var imageOrdinal = reader.GetOrdinal("Image");
            var unitPriceOrdinal = reader.GetOrdinal("UnitPrice");
            var quantityOrdinal = reader.GetOrdinal("Quantity");
            var weightOrdinal = reader.GetOrdinal("Weight");
            var taxOrdinal = reader.GetOrdinal("Tax");
            var dateOrdinal = reader.GetOrdinal("Date");
            var promotionNameOrdinal = reader.GetOrdinal("PromotionName");
            var promotionValueOrdinal = reader.GetOrdinal("PromotionValue");
            var promotionLimitOrdinal = reader.GetOrdinal("PromotionLimit");
            var isEBTOrdinal = reader.GetOrdinal("IsEBT");

            while (await reader.ReadAsync())
            {
                var item = new CartItem
                {
                    Number = reader.GetInt32(numberOrdinal),
                    ProductId = reader.GetInt32(productIdOrdinal),
                    ProductName = reader.GetString(productNameOrdinal),
                    UPC = reader.IsDBNull(upcOrdinal) ? null : reader.GetString(upcOrdinal),
                    Image = reader.IsDBNull(imageOrdinal) ? null : reader.GetString(imageOrdinal),
                    UnitPrice = reader.GetDecimal(unitPriceOrdinal),
                    Quantity = reader.GetDouble(quantityOrdinal),
                    Weight = reader.GetDouble(weightOrdinal),
                    Tax = reader.GetDouble(taxOrdinal),
                    Date = reader.GetDateTime(dateOrdinal),
                    PromotionName = reader.IsDBNull(promotionNameOrdinal) ? null : reader.GetString(promotionNameOrdinal),
                    PromotionValue = reader.GetDouble(promotionValueOrdinal),
                    PromotionLimit = reader.GetDecimal(promotionLimitOrdinal),
                    IsEBT = reader.GetBoolean(isEBTOrdinal)
                };
                items.Add(item);
            }

            _logger?.LogDebug("Retrieved {Count} cart items for session {SessionId}", items.Count, sessionId);
            return items;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error retrieving cart items for session {SessionId}", sessionId);
            throw;
        }
    }

    public async Task RemoveCartItemAsync(int productId, string sessionId)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
            throw new ArgumentNullException(nameof(sessionId));

        try
        {
            await using var db = new SqliteConnection(ConnectionString);
            await db.OpenAsync();

            await using var deleteCmd = new SqliteCommand(DELETE_CART_ITEM_SQL, db);
            deleteCmd.Parameters.AddWithValue("@SessionId", sessionId);
            deleteCmd.Parameters.AddWithValue("@ProductId", productId);
            await deleteCmd.ExecuteNonQueryAsync();

            _logger?.LogDebug("Cart item removed and reordered: ProductId={productId}, SessionId={sessionId}",
                productId, sessionId);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error removing cart item: ProductId={productId}, SessionId={sessionId}",
                productId, sessionId);
            throw;
        }
    }

    public async Task UpdateCartItemQuantityAsync(int productId, double quantity, string sessionId)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
            throw new ArgumentNullException(nameof(sessionId));

        if (quantity < 0)
            throw new ArgumentException("Quantity cannot be negative", nameof(quantity));

        try
        {
            await using var db = new SqliteConnection(ConnectionString);
            await db.OpenAsync();

            if (quantity == 0)
            {
                await RemoveCartItemAsync(productId, sessionId);
            }
            else
            {
                await using var command = new SqliteCommand(
                    "UPDATE CartItems SET Quantity = @Quantity, LastModified = CURRENT_TIMESTAMP " +
                    "WHERE SessionId = @SessionId AND ProductId = @ProductId", db);
                command.Parameters.AddWithValue("@SessionId", sessionId);
                command.Parameters.AddWithValue("@ProductId", productId);
                command.Parameters.AddWithValue("@Quantity", quantity);

                await command.ExecuteNonQueryAsync();
                _logger?.LogDebug("Cart item quantity updated: ProductId={productId}, Quantity={quantity}, SessionId={sessionId}", 
                    productId, quantity, sessionId);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error updating cart item quantity: ProductId={productId}, SessionId={sessionId}", productId, sessionId);
            throw;
        }
    }

    public async Task ClearCartAsync(string sessionId)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
            throw new ArgumentNullException(nameof(sessionId));

        try
        {
            await using var db = new SqliteConnection(ConnectionString);
            await db.OpenAsync();

            await using var command = new SqliteCommand(DELETE_CART_SESSION_SQL, db);
            command.Parameters.AddWithValue("@SessionId", sessionId);

            await command.ExecuteNonQueryAsync();
            _logger?.LogDebug("Cart cleared for session {sessionId}", sessionId);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error clearing cart for session {SessionId}", sessionId);
            throw;
        }
    }

    public async Task<int> GetNextCartNumberAsync(string sessionId)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
            throw new ArgumentNullException(nameof(sessionId));

        try
        {
            await using var db = new SqliteConnection(ConnectionString);
            await db.OpenAsync();

            await using var command = new SqliteCommand(GET_NEXT_NUMBER_SQL, db);
            command.Parameters.AddWithValue("@SessionId", sessionId);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error getting next cart number for session {SessionId}", sessionId);
            throw;
        }
    }

    // Payments
    public async Task SavePaymentAsync(string sessionId, decimal total, string paymentType)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
            throw new ArgumentNullException(nameof(sessionId));

        try
        {
            await using var db = new SqliteConnection(ConnectionString);
            await db.OpenAsync();

            var paymentId = Guid.NewGuid().ToString();
            
            await using var command = new SqliteCommand(INSERT_PAYMENT_SQL, db);
            command.Parameters.AddWithValue("@SessionId", sessionId);
            command.Parameters.AddWithValue("@PaymentId", paymentId);
            command.Parameters.AddWithValue("@Total", total);
            command.Parameters.AddWithValue("@PaymentType", paymentType);

            await command.ExecuteNonQueryAsync();

            _logger?.LogDebug("Payment saved: PaymentId={PaymentId}, SessionId={SessionId}, Total={Total}", 
                paymentId, sessionId, total);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error saving payment: SessionId={SessionId}, Total={Total}", sessionId, total);
            throw;
        }
    }

    public async Task<List<PaymentModel>> GetPaymentsAsync(string sessionId)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
            throw new ArgumentNullException(nameof(sessionId));

        var payments = new List<PaymentModel>();
        try
        {
            await using var db = new SqliteConnection(ConnectionString);
            await db.OpenAsync();

            await using var command = new SqliteCommand(SELECT_PAYMENTS_SQL, db);
            command.Parameters.AddWithValue("@SessionId", sessionId);

            await using var reader = await command.ExecuteReaderAsync();
            
            var paymentIdOrdinal = reader.GetOrdinal("PaymentId");
            var totalOrdinal = reader.GetOrdinal("Total");
            var paymentTypeOrdinal = reader.GetOrdinal("PaymentType");

            while (await reader.ReadAsync())
            {
                var payment = new PaymentModel
                {
                    PaymentId = reader.GetString(paymentIdOrdinal),
                    Total = reader.GetDecimal(totalOrdinal),
                    PaymentType = reader.GetString(paymentTypeOrdinal),
                };
                payments.Add(payment);
            }

            _logger?.LogDebug("Retrieved {Count} payments for session {SessionId}", payments.Count, sessionId);
            return payments;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error retrieving payments for session {SessionId}", sessionId);
            throw;
        }
    }

    public async Task ClearPaymentAsync(string sessionId)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
            throw new ArgumentNullException(nameof(sessionId));

        try
        {
            await using var db = new SqliteConnection(ConnectionString);
            await db.OpenAsync();

            await using var command = new SqliteCommand(DELETE_PAYMENT_SESSION_SQL, db);
            command.Parameters.AddWithValue("@SessionId", sessionId);

            await command.ExecuteNonQueryAsync();
            _logger?.LogDebug("Payment cleared for session {sessionId}", sessionId);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error clearing payment for session {SessionId}", sessionId);
            throw;
        }
    }

    // Hold
    public async Task HoldCartAsync(string sessionId, string userId)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
            throw new ArgumentNullException(nameof(sessionId));
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentNullException(nameof(userId));

        try
        {
            await using var db = new SqliteConnection(ConnectionString);

            await db.OpenAsync();

            try
            {
                // Insert into HoldCarts
                await using var holdCmd = new SqliteCommand(INSERT_HOLD_CART_SQL, db);
                holdCmd.Parameters.AddWithValue("@HoldId", userId);
                holdCmd.Parameters.AddWithValue("@SessionId", sessionId);
                await holdCmd.ExecuteNonQueryAsync();

                // Copy cart items to HoldCartItems
                await using var itemsCmd = new SqliteCommand(INSERT_HOLD_ITEM_SQL, db);
                itemsCmd.Parameters.AddWithValue("@HoldId", userId);
                itemsCmd.Parameters.AddWithValue("@SessionId", sessionId);
                await itemsCmd.ExecuteNonQueryAsync();

                // Borrar el carrito de compras
                await using var command = new SqliteCommand(DELETE_CART_SESSION_SQL, db);
                command.Parameters.AddWithValue("@SessionId", sessionId);
                await command.ExecuteNonQueryAsync();

                _logger?.LogDebug("Cart held successfully: HoldId={HoldId}", userId);
            }
            catch
            {
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error holding cart: SessionId={SessionId}, UserId={UserId}", sessionId, userId);
            throw;
        }
    }

    public async Task<List<HoldCartModel>> GetHeldCartsBySessionAsync(string sessionId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(sessionId))
                throw new ArgumentException("Session ID cannot be empty", nameof(sessionId));

            var heldCarts = new List<HoldCartModel>();
            await using var db = new SqliteConnection(ConnectionString);
            await db.OpenAsync();

            await using var command = new SqliteCommand(SELECT_HELD_CARTS_BY_SESSION_SQL, db);
            command.Parameters.AddWithValue("@SessionId", sessionId);

            await using var reader = await command.ExecuteReaderAsync();

            var holdIdOrdinal = reader.GetOrdinal("HoldId");

            while (await reader.ReadAsync())
            {
                var heldCart = new HoldCartModel
                {
                    HoldId = reader.GetString(holdIdOrdinal),
                };
                heldCarts.Add(heldCart);
            }

            _logger?.LogDebug("Retrieved {Count} held carts for session {SessionId}", heldCarts.Count, sessionId);
            return heldCarts;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error retrieving held carts for session {SessionId}", sessionId);
            throw;
        }
    }

    public async Task<HoldCartModel?> GetHeldCartsByIdAsync(string holdId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(holdId))
                throw new ArgumentException("User Id cannot be empty", nameof(holdId));

            HoldCartModel? heldCart = null;
            await using var db = new SqliteConnection(ConnectionString);
            await db.OpenAsync();

            await using var command = new SqliteCommand(SELECT_HELD_CARTS_BY_ID_SQL, db);
            command.Parameters.AddWithValue("@HoldId", holdId);

            await using var reader = await command.ExecuteReaderAsync();

            var holdIdOrdinal = reader.GetOrdinal("HoldId");

            if (await reader.ReadAsync())
            {
                heldCart = new HoldCartModel
                {
                    HoldId = reader.GetString(holdIdOrdinal),
                };
            }

            _logger?.LogDebug("Retrieved held cart for HoldId={HoldId}", holdId);

            return heldCart;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error retrieving held cart for HoldId={HoldId}", holdId);
            throw;
        }
    }

    public async Task<List<CartItem>> RetrieveHeldCartAsync(string holdId)
    {
        if (string.IsNullOrWhiteSpace(holdId))
            throw new ArgumentNullException(nameof(holdId));

        var items = new List<CartItem>();
        try
        {
            await using var db = new SqliteConnection(ConnectionString);
            await db.OpenAsync();

            await using var command = new SqliteCommand(SELECT_HELD_ITEMS_SQL, db);
            command.Parameters.AddWithValue("@HoldId", holdId);

            await using var reader = await command.ExecuteReaderAsync();
            
            // Cache column ordinals
            var numberOrdinal = reader.GetOrdinal("Number");
            var productIdOrdinal = reader.GetOrdinal("ProductId");
            var productNameOrdinal = reader.GetOrdinal("ProductName");
            var upcOrdinal = reader.GetOrdinal("UPC");
            var imageOrdinal = reader.GetOrdinal("Image");
            var unitPriceOrdinal = reader.GetOrdinal("UnitPrice");
            var quantityOrdinal = reader.GetOrdinal("Quantity");
            var weightOrdinal = reader.GetOrdinal("Weight");
            var taxOrdinal = reader.GetOrdinal("Tax");
            var promotionNameOrdinal = reader.GetOrdinal("PromotionName");
            var promotionValueOrdinal = reader.GetOrdinal("PromotionValue");
            var promotionLimitOrdinal = reader.GetOrdinal("PromotionLimit");
            var isEBTOrdinal = reader.GetOrdinal("IsEBT");

            while (await reader.ReadAsync())
            {
                var item = new CartItem
                {
                    Number = reader.GetInt32(numberOrdinal),
                    ProductId = reader.GetInt32(productIdOrdinal),
                    ProductName = reader.GetString(productNameOrdinal),
                    UPC = reader.IsDBNull(upcOrdinal) ? null : reader.GetString(upcOrdinal),
                    Image = reader.IsDBNull(imageOrdinal) ? null : reader.GetString(imageOrdinal),
                    UnitPrice = reader.GetDecimal(unitPriceOrdinal),
                    Quantity = reader.GetDouble(quantityOrdinal),
                    Weight = reader.GetDouble(weightOrdinal),
                    Tax = reader.GetDouble(taxOrdinal),
                    PromotionName = reader.IsDBNull(promotionNameOrdinal) ? null : reader.GetString(promotionNameOrdinal),
                    PromotionValue = reader.GetDouble(promotionValueOrdinal),
                    PromotionLimit = reader.GetDecimal(promotionLimitOrdinal),
                    IsEBT = reader.GetBoolean(isEBTOrdinal)
                };
                items.Add(item);
            }

            _logger?.LogDebug("Retrieved {Count} items from held cart {HoldId}", items.Count, holdId);
            return items;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error retrieving held cart items: HoldId={HoldId}", holdId);
            throw;
        }
    }

    public async Task DeleteHeldCartAsync(string holdId)
    {
        if (string.IsNullOrWhiteSpace(holdId))
            throw new ArgumentNullException(nameof(holdId));

        try
        {
            await using var db = new SqliteConnection(ConnectionString);

            await db.OpenAsync();

            try
            {
                // Delete hold items first due to foreign key constraint
                await using var deleteItemsCmd = new SqliteCommand(DELETE_HOLD_ITEMS_SQL, db);
                deleteItemsCmd.Parameters.AddWithValue("@HoldId", holdId);
                await deleteItemsCmd.ExecuteNonQueryAsync();

                // Delete hold cart
                await using var deleteHoldCmd = new SqliteCommand(DELETE_HOLD_CART_SQL, db);
                deleteHoldCmd.Parameters.AddWithValue("@HoldId", holdId);
                await deleteHoldCmd.ExecuteNonQueryAsync();

                _logger?.LogDebug("Held cart deleted successfully: HoldId={HoldId}", holdId);
            }
            catch
            {
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error deleting held cart: HoldId={HoldId}", holdId);
            throw;
        }
    }

    public async Task DropTablesAsync()
    {
        try
        {
            await using var db = new SqliteConnection(ConnectionString);
            await db.OpenAsync();

            var dropCartItems = "DROP TABLE IF EXISTS CartItems;";
            var dropPayments = "DROP TABLE IF EXISTS Payments;";
            var dropHoldItems = "DROP TABLE IF EXISTS HoldCartItems;";
            var dropHoldCarts = "DROP TABLE IF EXISTS HoldCarts;";

            await using (var cmd = new SqliteCommand(dropCartItems, db))
            {
                await cmd.ExecuteNonQueryAsync();
            }
            await using (var cmd = new SqliteCommand(dropPayments, db))
            {
                await cmd.ExecuteNonQueryAsync();
            }
            await using (var cmd = new SqliteCommand(dropHoldItems, db))
            {
                await cmd.ExecuteNonQueryAsync();
            }
            await using (var cmd = new SqliteCommand(dropHoldCarts, db))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            _logger?.LogInformation("Tables CartItems, Payments, HoldCartItems, and HoldCarts deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error deleting tables CartItems, Payments, HoldCartItems, and HoldCarts.");
            throw;
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        _semaphore.Dispose();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
