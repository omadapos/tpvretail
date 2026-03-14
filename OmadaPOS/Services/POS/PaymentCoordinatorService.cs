using OmadaPOS.Domain;
using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Services;
using OmadaPOS.Libreria.Utils;
using OmadaPOS.Models;

namespace OmadaPOS.Services.POS;

public sealed class CashSaleResult
{
    public bool IsValidAmount { get; init; }
    public string? ErrorMessage { get; init; }
    public OrderResponse? OrderResponse { get; init; }
}

public sealed class PaymentFlowResult
{
    public bool IsBalanceInquiry { get; init; }
    public PaymentResponseModel? PaymentResponse { get; init; }
    public OrderResponse? OrderResponse { get; init; }
}

public interface IPaymentCoordinatorService
{
    Task<CashSaleResult> ProcessCashSaleAsync(decimal totalGlobal, int inputValue, decimal changeValue, bool applyDiscount);
    Task<OrderResponse?> ProcessGiftCardAsync(decimal changeValue, bool applyDiscount);
    Task<PaymentFlowResult> ProcessTerminalPaymentAsync(PaymentType paymentType, decimal totalGlobal, bool applyDiscount);
    Task<PaymentResponseModel> ProcessPaymentAsync(string paymentType, decimal amount);
    Task<OrderResponse?> ProcessMultiplePaymentsAsync(decimal changeValue, bool applyDiscount);
    Task<OrderResponse?> PlaceOrderAsync(string paymentMethod, decimal changeAmount, bool applyDiscount);

    /// <summary>Call after saving terminal settings so next payment reloads config from SQLite.</summary>
    void InvalidateConfig();
}

public class PaymentCoordinatorService : IPaymentCoordinatorService
{
    private readonly IAdminSettingService _adminSettingService;
    private readonly IOrderService _orderService;
    private readonly IOrderApplicationService _orderApplicationService;
    private readonly IPaymentService _paymentService;
    private readonly IPaymentSplitService _paymentSplitService;
    private readonly IShoppingCart _shoppingCart;

    // Config is loaded once per service instance and cached — avoids 6 SQLite hits per payment.
    private AdminSetting? _cachedConfig;
    private readonly SemaphoreSlim _configLock = new(1, 1);

    public PaymentCoordinatorService(
        IAdminSettingService adminSettingService,
        IOrderService orderService,
        IOrderApplicationService orderApplicationService,
        IPaymentService paymentService,
        IPaymentSplitService paymentSplitService,
        IShoppingCart shoppingCart)
    {
        _adminSettingService = adminSettingService ?? throw new ArgumentNullException(nameof(adminSettingService));
        _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        _orderApplicationService = orderApplicationService ?? throw new ArgumentNullException(nameof(orderApplicationService));
        _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
        _paymentSplitService = paymentSplitService ?? throw new ArgumentNullException(nameof(paymentSplitService));
        _shoppingCart = shoppingCart ?? throw new ArgumentNullException(nameof(shoppingCart));
    }

    /// <summary>
    /// Returns the terminal config, hitting SQLite only on the first call.
    /// Call InvalidateConfig() if settings change at runtime.
    /// </summary>
    private async Task<AdminSetting?> GetConfigAsync()
    {
        if (_cachedConfig != null) return _cachedConfig;

        await _configLock.WaitAsync().ConfigureAwait(false);
        try
        {
            _cachedConfig ??= await _adminSettingService.LoadSettingById(WindowsIdProvider.GetMachineGuid()).ConfigureAwait(false);
            return _cachedConfig;
        }
        finally
        {
            _configLock.Release();
        }
    }

    /// <summary>Forces config to be reloaded on the next payment call (e.g. after saving settings).</summary>
    public void InvalidateConfig() => _cachedConfig = null;

    public async Task<CashSaleResult> ProcessCashSaleAsync(decimal totalGlobal, int inputValue, decimal changeValue, bool applyDiscount)
    {
        if (_shoppingCart.ItemCount <= 0 || (inputValue / 100.0m) < totalGlobal || inputValue <= 0.0)
        {
            return new CashSaleResult
            {
                IsValidAmount = false,
                ErrorMessage = "Please enter a valid amount."
            };
        }

        var config = await GetConfigAsync().ConfigureAwait(false);
        string terminal = config?.Terminal ?? string.Empty;

        var placeOrderModel = _orderApplicationService.BuildOrderModel(
            _shoppingCart.Items,
            changeValue,
            terminal,
            "Cash",
            0,
            applyDiscount
        );

        if (placeOrderModel == null)
            return new CashSaleResult { IsValidAmount = true };

        return new CashSaleResult
        {
            IsValidAmount = true,
            OrderResponse = await _orderService.PlaceOrder(placeOrderModel)
        };
    }

    public async Task<OrderResponse?> ProcessGiftCardAsync(decimal changeValue, bool applyDiscount)
    {
        if (_shoppingCart.ItemCount <= 0)
            return null;

        var config = await GetConfigAsync().ConfigureAwait(false);
        string terminal = config?.Terminal ?? string.Empty;

        var placeOrderModel = _orderApplicationService.BuildOrderModel(
            _shoppingCart.Items,
            changeValue,
            terminal,
            "GiftCard",
            0,
            applyDiscount
        );

        return placeOrderModel == null
            ? null
            : await _orderService.PlaceOrder(placeOrderModel).ConfigureAwait(false);
    }

    public async Task<PaymentFlowResult> ProcessTerminalPaymentAsync(PaymentType paymentType, decimal totalGlobal, bool applyDiscount)
    {
        var config = await GetConfigAsync().ConfigureAwait(false);

        string terminal = config?.Terminal ?? string.Empty;
        int port = config?.Port ?? 0;
        string ip = config?.IP ?? string.Empty;

        var consecutivo = await _orderService.LoadLastConsecutivoPayment().ConfigureAwait(false);

        if (paymentType == PaymentType.EBTBalance)
        {
            var balanceResponse = await _paymentService.GetEBTBalanceAsync(new PaymentRequest
            {
                Ip = ip,
                Port = port,
                Terminal = terminal,
                Amount = 0,
                EcrRefNumber = consecutivo.ToString(),
            });

            return new PaymentFlowResult
            {
                IsBalanceInquiry = true,
                PaymentResponse = balanceResponse
            };
        }

        if (_shoppingCart.ItemCount <= 0)
            return new PaymentFlowResult();

        var placeOrderModel = _orderApplicationService.BuildOrderModel(
            _shoppingCart.Items,
            0,
            terminal,
            paymentType.ToString(),
            0,
            applyDiscount
        );

        if (placeOrderModel == null)
            return new PaymentFlowResult();

        // Apply Cash Discount service fee: add to terminal amount (cents) and order model (dollars).
        decimal serviceFee  = SurchargePolicy.GetFeeAmount((decimal)placeOrderModel.Order_Amount, paymentType);
        placeOrderModel.Service_Fee   = (double)serviceFee;
        placeOrderModel.Order_Amount += (double)serviceFee;

        var totalPayment = SurchargePolicy.Apply(totalGlobal * 100, paymentType);

        System.Diagnostics.Debug.WriteLine(
            $"[Surcharge] CashDiscountEnabled={SessionManager.CashDiscountEnabled} | " +
            $"PaymentType={paymentType} | " +
            $"CartTotal={totalGlobal:C} | " +
            $"Fee={serviceFee:C} | " +
            $"TerminalCents={totalPayment}");

        var paymentResponse = await _paymentService.ProcessPaymentAsync(paymentType, new PaymentRequest
        {
            Ip = ip,
            Port = port,
            Terminal = terminal,
            Amount = totalPayment,
            EcrRefNumber = consecutivo.ToString(),
        });

        if (paymentResponse != null && paymentResponse.Success)
        {
            placeOrderModel.Balance = paymentResponse.Balance;

            return new PaymentFlowResult
            {
                PaymentResponse = paymentResponse,
                OrderResponse = await _orderService.PlaceOrder(placeOrderModel)
            };
        }

        return new PaymentFlowResult
        {
            PaymentResponse = paymentResponse
        };
    }

    public async Task<PaymentResponseModel> ProcessPaymentAsync(string paymentType, decimal amount)
    {
        var config = await GetConfigAsync().ConfigureAwait(false);
        if (config == null)
            throw new InvalidOperationException("Payment terminal configuration not found");

        var request = new PaymentRequest
        {
            Ip = config.IP ?? string.Empty,
            Port = config.Port ?? 0,
            Amount = amount * 100,
            EcrRefNumber = (await _orderService.LoadLastConsecutivoPayment()).ToString()
        };

        return paymentType switch
        {
            "CREDIT_CARD" => await _paymentService.ProcessPaymentAsync(PaymentType.Credit, request),
            "DEBIT_CARD" => await _paymentService.ProcessPaymentAsync(PaymentType.Debit, request),
            "EBT" => await _paymentService.ProcessPaymentAsync(PaymentType.EBT, request),
            "EBT_BALANCE" => await _paymentService.GetEBTBalanceAsync(request),
            _ => throw new ArgumentException($"Unsupported payment type: {paymentType}")
        };
    }

    public async Task<OrderResponse?> ProcessMultiplePaymentsAsync(decimal changeValue, bool applyDiscount)
    {
        var config = await GetConfigAsync().ConfigureAwait(false);
        string terminal = config?.Terminal ?? string.Empty;

        List<PlaceOrderPayment> payments = [];
        var pays = await _paymentSplitService.GetSessionPaymentsAsync().ConfigureAwait(false);

        foreach (var pay in pays)
        {
            if (pay.Total > 0)
            {
                payments.Add(new PlaceOrderPayment
                {
                    Tipo = pay.PaymentType,
                    Total = pay.Total
                });
            }
        }

        var placeOrderModel = _orderApplicationService.BuildMultipleOrderModel(
            _shoppingCart.Items,
            changeValue,
            terminal,
            payments,
            0,
            applyDiscount
        );

        return placeOrderModel == null
            ? null
            : await _orderService.PlaceOrderMultiple(placeOrderModel).ConfigureAwait(false);
    }

    public async Task<OrderResponse?> PlaceOrderAsync(string paymentMethod, decimal changeAmount, bool applyDiscount)
    {
        var config = await GetConfigAsync().ConfigureAwait(false);
        var terminal = config?.Terminal ?? string.Empty;

        var orderModel = _orderApplicationService.BuildOrderModel(
            _shoppingCart.Items,
            changeAmount,
            terminal,
            paymentMethod,
            0,
            applyDiscount);

        if (orderModel == null)
            throw new InvalidOperationException("Failed to create order model");

        return await _orderService.PlaceOrder(orderModel).ConfigureAwait(false);
    }
}
