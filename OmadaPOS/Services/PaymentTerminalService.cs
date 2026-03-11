using Microsoft.Extensions.Logging;
using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Utils;
using POSLinkAdmin;
using POSLinkCore.CommunicationSetting;
using POSLinkSemiIntegration;
using POSLinkSemiIntegration.Transaction;

namespace OmadaPOS.Services;

public interface IPaymentTerminalService
{
    Task<Terminal> GetTerminalAsync(string ip, int port);
    Task<PaymentResponseModel> ProcessCreditPaymentAsync(Terminal terminal, DoCreditRequest request);
    Task<PaymentResponseModel> ProcessDebitPaymentAsync(Terminal terminal, DoDebitRequest request);
    Task<PaymentResponseModel> ProcessEBTPaymentAsync(Terminal terminal, DoEbtRequest request);
    Task<PaymentResponseModel> ProcessEBTBalanceAsync(Terminal terminal, DoEbtRequest request);
}

public class PaymentTerminalService : IPaymentTerminalService
{
    private readonly ILogger<PaymentTerminalService> _logger;
    private readonly POSLinkSemi _poslinkSemi;

    public PaymentTerminalService(ILogger<PaymentTerminalService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _poslinkSemi = POSLinkSemi.GetPOSLinkSemi();
    }

    public async Task<Terminal> GetTerminalAsync(string ip, int port)
    {
        try
        {
            var tcpSetting = new TcpSetting
            {
                Ip = ip,
                Port = port,
                Timeout = Constants.TIMEOUT
            };

            return await Task.Run(() => _poslinkSemi.GetTerminal(tcpSetting)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting terminal for IP {Ip} and port {Port}", ip, port);
            throw;
        }
    }

    public async Task<PaymentResponseModel> ProcessCreditPaymentAsync(Terminal terminal, DoCreditRequest request)
    {
        try
        {
            return await Task.Run(() =>
            {
                DoCreditResponse response;
                var result = terminal.Transaction.DoCredit(request, out response);
                return ProcessPaymentResponse(result, response);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing credit payment");
            throw;
        }
    }

    public async Task<PaymentResponseModel> ProcessDebitPaymentAsync(Terminal terminal, DoDebitRequest request)
    {
        try
        {
            return await Task.Run(() =>
            {
                DoDebitResponse response;
                var result = terminal.Transaction.DoDebit(request, out response);
                return ProcessPaymentResponse(result, response);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing debit payment");
            throw;
        }
    }

    public async Task<PaymentResponseModel> ProcessEBTBalanceAsync(Terminal terminal, DoEbtRequest request)
    {
        try
        {
            return await Task.Run(() =>
            {
                DoEbtResponse response;
                var result = terminal.Transaction.DoEbt(request, out response);
                return ProcessEBTBalanceResponse(result, response);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing EBT balance inquiry");
            throw;
        }
    }

    public async Task<PaymentResponseModel> ProcessEBTPaymentAsync(Terminal terminal, DoEbtRequest request)
    {
        try
        {
            return await Task.Run(() =>
            {
                DoEbtResponse response;
                var result = terminal.Transaction.DoEbt(request, out response);
                return ProcessPaymentResponse(result, response);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing EBT payment");
            throw;
        }
    }

    private PaymentResponseModel ProcessEBTBalanceResponse(ExecutionResult result, DoEbtResponse response)
    {
        var paymentResponse = new PaymentResponseModel { Success = false };

        if (result.GetErrorCode() != ExecutionResult.Code.Ok)
        {
            paymentResponse.MsgInfo = $"Error: {result.GetErrorCode()}";
            return paymentResponse;
        }

        string codePayment = GetResponseCode(response.ResponseCode);

        if (response.ResponseCode == "000000")
        {
            if (decimal.TryParse(response.AmountInformation.Balance2, out decimal balance))
            {
                paymentResponse.Balance = balance / 100;
                paymentResponse.Success = true;
            }
            else
            {
                paymentResponse.MsgInfo = "Error parsing balance";
            }
        }
        else
        {
            paymentResponse.MsgInfo = codePayment;
        }

        return paymentResponse;
    }

    private PaymentResponseModel ProcessPaymentResponse(ExecutionResult result, dynamic response)
    {
        var paymentResponse = new PaymentResponseModel { Success = false };

        if (result.GetErrorCode() != ExecutionResult.Code.Ok)
        {
            paymentResponse.MsgInfo = $"Error: {result.GetErrorCode()}";
            return paymentResponse;
        }

        string codePayment = GetResponseCode(response.ResponseCode);

        if (response.ResponseCode == "000000")
        {
            paymentResponse.MsgInfo = codePayment;
            paymentResponse.PaymentNumber = response.AccountInformation.Account;
            paymentResponse.PaymentCardHolder = response.AccountInformation.CardHolder;
            paymentResponse.PaymentCardType = response.AccountInformation.CardType.ToString();
            paymentResponse.PaymentReferenceNumber = response.TraceInformation.ReferenceNumber;
            paymentResponse.Success = true;

            if (decimal.TryParse(response.AmountInformation.Balance2, out decimal balance))
            {
                paymentResponse.Balance = balance / 100;
            }
        }
        else
        {
            paymentResponse.MsgInfo = codePayment;
        }

        return paymentResponse;
    }

    private string GetResponseCode(string responseCode)
    {
        if (Constants.LISTERRORS.TryGetValue(responseCode, out string? errorMessage))
        {
            return errorMessage;
        }
        return responseCode;
    }
}
