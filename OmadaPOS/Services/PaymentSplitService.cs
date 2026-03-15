using Microsoft.Extensions.Logging;
using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Utils;

namespace OmadaPOS.Services;

public interface IPaymentSplitService
{
    Task<List<PaymentModel>> GetSessionPaymentsAsync();
    Task<bool> CreatePaymentAsync(string paymentType, decimal amount);
    /// <summary>Removes the most recently added payment leg for the current session.</summary>
    Task RemoveLastPaymentAsync();
    void Clear();
}

public class PaymentSplitService(
    ISqliteManager sqliteManager, ILogger<PaymentSplitService> logger) : IPaymentSplitService
{
    private readonly object _lockObject = new();
    private readonly string _sessionId = WindowsIdProvider.GetMachineGuid();

    public async Task<List<PaymentModel>> GetSessionPaymentsAsync()
    {
        try
        {
            var payments = await sqliteManager.GetPaymentsAsync(_sessionId).ConfigureAwait(false);
            logger.LogDebug("Retrieved {Count} payments for session {SessionId}", payments.Count, _sessionId);
            return payments;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving payments for session {SessionId}", _sessionId);
            throw;
        }
    }

    public async Task<bool> CreatePaymentAsync(string paymentType, decimal amount)
    {
        try
        {
          
            await sqliteManager.SavePaymentAsync(_sessionId, amount, paymentType).ConfigureAwait(false);

            logger.LogInformation("Payment created: SessionId={SessionId}, Amount={Amount}, Type={PaymentType}",
                _sessionId, amount, paymentType);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating payment: SessionId={SessionId}, Amount={Amount}", _sessionId, amount);
            throw;
        }
    }

    public async Task RemoveLastPaymentAsync()
    {
        try
        {
            await sqliteManager.DeleteLastPaymentAsync(_sessionId).ConfigureAwait(false);
            logger.LogInformation("Last payment removed for session {SessionId}", _sessionId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error removing last payment for session {SessionId}", _sessionId);
            throw;
        }
    }

    public void Clear()
    {
        try
        {
            lock (_lockObject)
            {
                sqliteManager.ClearPaymentAsync(_sessionId)
                    .GetAwaiter().GetResult();
            }
        }
        catch (Exception)
        {
            // Log error or handle appropriately
        }
    }
}
