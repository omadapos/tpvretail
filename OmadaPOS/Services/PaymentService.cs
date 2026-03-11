using Microsoft.Extensions.Logging;
using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Utils;
using POSLinkAdmin.Const;
using POSLinkAdmin.Util;
using POSLinkSemiIntegration;
using POSLinkSemiIntegration.Transaction;
using POSLinkSemiIntegration.Util;

namespace OmadaPOS.Services;

public interface IPaymentService
{
    Task<PaymentResponseModel> ProcessPaymentAsync(
        PaymentType paymentType,
        PaymentRequest request);
    Task<PaymentResponseModel> GetEBTBalanceAsync(PaymentRequest request);
}

public class PaymentService : IPaymentService
{
    private readonly ILogger<PaymentService> _logger;
    private readonly IPaymentTerminalService _terminalService;

    public PaymentService(
        ILogger<PaymentService> logger,
        IPaymentTerminalService terminalService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _terminalService = terminalService ?? throw new ArgumentNullException(nameof(terminalService));
    }

    public async Task<PaymentResponseModel> ProcessPaymentAsync(
        PaymentType paymentType,
        PaymentRequest request)
    {
        try
        {
            ValidatePaymentRequest(request);

            var terminal = await _terminalService.GetTerminalAsync(request.Ip, request.Port).ConfigureAwait(false);
            var response = paymentType switch
            {
                PaymentType.Credit => await ProcessCreditPaymentAsync(terminal, request),
                PaymentType.Debit => await ProcessDebitPaymentAsync(terminal, request),
                PaymentType.EBT => await ProcessEBTPaymentAsync(terminal, request),
                _ => throw new ArgumentException($"Unsupported payment type: {paymentType}")
            };

            LogPaymentResponse(response, paymentType);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment of type {PaymentType}", paymentType);
            return new PaymentResponseModel { Success = false, MsgInfo = ex.Message };
        }
    }

    public async Task<PaymentResponseModel> GetEBTBalanceAsync(PaymentRequest request)
    {
        try
        {
            ValidatePaymentRequest(request, validateAmount: false);

            var terminal = await _terminalService.GetTerminalAsync(request.Ip, request.Port).ConfigureAwait(false);
            var response = await ProcessEBTBalanceAsync(terminal, request).ConfigureAwait(false);

            LogPaymentResponse(response, PaymentType.EBTBalance);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting EBT balance");
            return new PaymentResponseModel { Success = false, MsgInfo = ex.Message };
        }

    }

    private async Task<PaymentResponseModel> ProcessCreditPaymentAsync(Terminal terminal, PaymentRequest request)
    {
        var paymentRequest = new DoCreditRequest
        {
            AmountInformation = new AmountRequest { TransactionAmount = request.Amount.ToString("###") },
            TraceInformation = new TraceRequest
            {
                EcrReferenceNumber = request.EcrRefNumber,
                InvoiceNumber = request.EcrRefNumber
            },
            TransactionType = TransactionType.Sale
        };

        return await _terminalService.ProcessCreditPaymentAsync(terminal, paymentRequest).ConfigureAwait(false);
    }

    private async Task<PaymentResponseModel> ProcessDebitPaymentAsync(Terminal terminal, PaymentRequest request)
    {
        var paymentRequest = new DoDebitRequest
        {
            AmountInformation = new AmountRequest { TransactionAmount = request.Amount.ToString("###") },
            TraceInformation = new TraceRequest { EcrReferenceNumber = request.EcrRefNumber },
            TransactionType = TransactionType.Sale
        };

        return await _terminalService.ProcessDebitPaymentAsync(terminal, paymentRequest).ConfigureAwait(false);
    }

    private async Task<PaymentResponseModel> ProcessEBTPaymentAsync(Terminal terminal, PaymentRequest request)
    {
        var paymentRequest = new DoEbtRequest
        {
            AmountInformation = new AmountRequest { TransactionAmount = request.Amount.ToString("###") },
            TraceInformation = new TraceRequest { EcrReferenceNumber = request.EcrRefNumber },
            TransactionType = TransactionType.Sale
        };

        return await _terminalService.ProcessEBTPaymentAsync(terminal, paymentRequest).ConfigureAwait(false);
    }

    private async Task<PaymentResponseModel> ProcessEBTBalanceAsync(Terminal terminal, PaymentRequest request)
    {
        var balanceRequest = new DoEbtRequest
        {
            AmountInformation = new AmountRequest { TransactionAmount = "0" },
            TraceInformation = new TraceRequest { EcrReferenceNumber = request.EcrRefNumber },
            TransactionType = TransactionType.Inquiry
        };

        return await _terminalService.ProcessEBTBalanceAsync(terminal, balanceRequest).ConfigureAwait(false);
    }

    private void ValidatePaymentRequest(PaymentRequest request, bool validateAmount = true)
    {
        if (string.IsNullOrWhiteSpace(request.Ip))
            throw new ArgumentException("IP address is required", nameof(request));

        if (request.Port <= 0)
            throw new ArgumentException("Invalid port number", nameof(request));

        if (string.IsNullOrWhiteSpace(request.EcrRefNumber))
            throw new ArgumentException("ECR reference number is required", nameof(request));

        if (validateAmount && request.Amount <= 0)
            throw new ArgumentException("Amount must be greater than zero", nameof(request));
    }

    private void LogPaymentResponse(PaymentResponseModel response, PaymentType paymentType)
    {
        if (response.Success)
        {
            _logger.LogInformation(
                "Payment processed successfully. Type: {PaymentType}, Amount: {Amount}, Reference: {Reference}",
                paymentType,
                response.PaymentReferenceNumber,
                response.PaymentNumber);
        }
        else
        {
            _logger.LogWarning(
                "Payment processing failed. Type: {PaymentType}, Error: {Error}",
                paymentType,
                response.MsgInfo);
        }
    }

}
