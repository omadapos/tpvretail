using OmadaPOS.Libreria.Utils;

namespace OmadaPOS.Services.POS;

public sealed class HoldCartState
{
    public int Count { get; init; }
}

public interface IHomeHoldCartService
{
    Task<HoldCartState> GetCurrentSessionStateAsync();
}

public class HoldCartService : IHomeHoldCartService
{
    private readonly IHoldService _holdService;

    public HoldCartService(IHoldService holdService)
    {
        _holdService = holdService ?? throw new ArgumentNullException(nameof(holdService));
    }

    public async Task<HoldCartState> GetCurrentSessionStateAsync()
    {
        var sessionId = WindowsIdProvider.GetMachineGuid();
        var heldCarts = await _holdService.GetHeldCartsBySessionAsync(sessionId);

        return new HoldCartState
        {
            Count = heldCarts?.Count ?? 0
        };
    }
}
