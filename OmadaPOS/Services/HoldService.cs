using Microsoft.Extensions.Logging;
using OmadaPOS.Models;

namespace OmadaPOS.Services;

public interface IHoldService
{
    /// <summary>
    /// Holds the current cart for a user with a given name
    /// </summary>
    /// <param name="sessionId">Current session ID</param>
    /// <param name="userId">User ID who is holding the cart</param>
    /// <returns>True if the cart was held successfully</returns>
    Task<bool> HoldCartAsync(string sessionId, string userId);

    /// <summary>
    /// Gets all held carts for a specific session
    /// </summary>
    /// <param name="sessionId">Session ID to get held carts for</param>
    /// <returns>List of held carts for the session</returns>
    Task<List<HoldCartModel>> GetHeldCartsBySessionAsync(string sessionId);

    Task<HoldCartModel?> GetHeldCartsByIdAsync(string holdId);

    /// <summary>
    /// Retrieves the items from a specific held cart
    /// </summary>
    /// <param name="holdId">ID of the held cart to retrieve</param>
    /// <returns>List of cart items from the held cart</returns>
    Task<List<CartItem>> RetrieveHeldCartAsync(string holdId);

    /// <summary>
    /// Deletes a held cart and its items
    /// </summary>
    /// <param name="holdId">ID of the held cart to delete</param>
    /// <returns>True if the cart was deleted successfully</returns>
    Task<bool> DeleteHeldCartAsync(string holdId);
}

public class HoldService : IHoldService
{
    private readonly ISqliteManager _sqliteManager;
    private readonly ILogger<HoldService>? _logger;

    public HoldService(ISqliteManager sqliteManager, ILogger<HoldService>? logger = null)
    {
        _sqliteManager = sqliteManager ?? throw new ArgumentNullException(nameof(sqliteManager));
        _logger = logger;
    }

    public async Task<bool> HoldCartAsync(string sessionId, string userId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(sessionId))
                throw new ArgumentException("Session ID cannot be empty", nameof(sessionId));
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            // Verify that the cart has items before holding
            var cartItems = await _sqliteManager.GetCartItemsAsync(sessionId).ConfigureAwait(false);
            if (!cartItems.Any())
            {
                _logger?.LogWarning("Attempted to hold an empty cart: SessionId={SessionId}, UserId={UserId}", 
                    sessionId, userId);
                return false;
            }

            await _sqliteManager.HoldCartAsync(sessionId, userId).ConfigureAwait(false);
            _logger?.LogInformation("Cart held successfully: SessionId={SessionId}, UserId={UserId}", 
                sessionId, userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error holding cart: SessionId={SessionId}, UserId={UserId}", 
                sessionId, userId);
            throw;
        }
    }

    public async Task<List<HoldCartModel>> GetHeldCartsBySessionAsync(string sessionId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(sessionId))
                throw new ArgumentException("Session ID cannot be empty", nameof(sessionId));

            var heldCarts = await _sqliteManager.GetHeldCartsBySessionAsync(sessionId).ConfigureAwait(false);
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
                throw new ArgumentException("Session ID cannot be empty", nameof(holdId));

            var heldCarts = await _sqliteManager.GetHeldCartsByIdAsync(holdId).ConfigureAwait(false);
            _logger?.LogDebug("Retrieved {HoldId}", holdId);
            return heldCarts;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error retrieving {HoldId}", holdId);
            throw;
        }
    }

    public async Task<List<CartItem>> RetrieveHeldCartAsync(string holdId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(holdId))
                throw new ArgumentException("Hold ID cannot be empty", nameof(holdId));

            var cartItems = await _sqliteManager.RetrieveHeldCartAsync(holdId).ConfigureAwait(false);
            _logger?.LogDebug("Retrieved {Count} items from held cart {HoldId}", cartItems.Count, holdId);
            return cartItems;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error retrieving held cart items: HoldId={HoldId}", holdId);
            throw;
        }
    }

    public async Task<bool> DeleteHeldCartAsync(string holdId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(holdId))
                throw new ArgumentException("Hold ID cannot be empty", nameof(holdId));

            await _sqliteManager.DeleteHeldCartAsync(holdId).ConfigureAwait(false);
            _logger?.LogInformation("Held cart deleted successfully: HoldId={HoldId}", holdId);
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error deleting held cart: HoldId={HoldId}", holdId);
            throw;
        }
    }

    
} 
