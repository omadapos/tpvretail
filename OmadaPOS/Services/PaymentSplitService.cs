using Microsoft.Extensions.Logging;
using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Utils;

namespace OmadaPOS.Services;

public interface IPaymentSplitService
{
    Task<List<PaymentModel>> GetSessionPaymentsAsync();

    Task<bool> CreatePaymentAsync(string paymentType, decimal amount);

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
            var payments = await sqliteManager.GetPaymentsAsync(_sessionId);
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
          
            await sqliteManager.SavePaymentAsync(_sessionId, amount, paymentType);

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
