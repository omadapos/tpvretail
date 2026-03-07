using OmadaPOS.Impresora;
using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Services;
using OmadaPOS.Libreria.Utils;

namespace OmadaPOS.Services.POS;

public sealed class CashDrawerOperationResult
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
}

public interface ICashDrawerService
{
    Task<CashDrawerOperationResult> OpenDrawerAsync();
}

public class CashDrawerService : ICashDrawerService
{
    private readonly IAdminSettingService _adminSettingService;
    private readonly IUserService _userService;

    public CashDrawerService(IAdminSettingService adminSettingService, IUserService userService)
    {
        _adminSettingService = adminSettingService ?? throw new ArgumentNullException(nameof(adminSettingService));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public async Task<CashDrawerOperationResult> OpenDrawerAsync()
    {
        var config = await _adminSettingService.LoadSettingById(WindowsIdProvider.GetMachineGuid());

        if (config == null || string.IsNullOrEmpty(config.PrinterName))
        {
            return new CashDrawerOperationResult
            {
                Success = false,
                ErrorMessage = "Printer not configured"
            };
        }

        byte[] openDrawerCommand = [0x1B, 0x70, 0x00, 0x3C, 0xFF];

        await _userService.Log(new LogDTO
        {
            AdminId = SessionManager.AdminId ?? 0,
            Phone = SessionManager.Phone,
            Info = "Open Drawer"
        });

        RawPrinterHelper.SendBytesToPrinter(config.PrinterName, openDrawerCommand);

        return new CashDrawerOperationResult { Success = true };
    }
}
