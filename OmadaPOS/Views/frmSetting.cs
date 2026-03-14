using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Services;
using OmadaPOS.Libreria.Utils;
using OmadaPOS.Services.POS;

namespace OmadaPOS.Views;

public sealed class frmSetting : POSDialog
{
    private readonly IAdminSettingService       _adminSettingService;
    private readonly IPaymentCoordinatorService _paymentCoordinatorService;

    private TextBox _tbIP          = null!;
    private TextBox _tbPort        = null!;
    private TextBox _tbTerminal    = null!;
    private TextBox _tbPrinter     = null!;
    private Label   _lblWindowsId  = null!;

    public frmSetting(IAdminSettingService adminSettingService, IPaymentCoordinatorService paymentCoordinatorService)
    {
        _adminSettingService       = adminSettingService;
        _paymentCoordinatorService = paymentCoordinatorService;
        Shown += async (_, _) => await LoadSettingsAsync();
    }

    protected override Color      AccentColor => AppColors.SlateBlue;
    protected override string     Icon        => "⚙";
    protected override string     Title       => "Settings";
    protected override string     Subtitle    => "Terminal configuration";
    protected override DialogSize Size        => DialogSize.Medium;
    protected override string?    ConfirmText => "💾  SAVE";
    protected override string     CancelText  => "✕  CANCEL";

    protected override Control BuildContent()
    {
        var scroll = new Panel
        {
            Dock        = DockStyle.Fill,
            BackColor   = AppColors.BackgroundPrimary,
            Padding     = new Padding(20, 12, 20, 8),
            AutoScroll  = true,
        };

        var f1 = FieldPanel("IP Address",    out _tbIP,       "192.168.1.0");
        var f2 = FieldPanel("Port",          out _tbPort,     "10009");
        var f3 = FieldPanel("Terminal ID",   out _tbTerminal, "POS");
        var f4 = FieldPanel("Printer Name",  out _tbPrinter,  "RONGTA");

        // Windows/Machine ID (read-only footer label)
        _lblWindowsId = new Label
        {
            Text      = "Machine ID: loading…",
            Font      = AppTypography.Caption,
            ForeColor = AppColors.TextMuted,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Top,
            Height    = 24,
            TextAlign = ContentAlignment.MiddleCenter,
        };

        // Stack top-down (Controls.Add reverses order for DockStyle.Top)
        scroll.Controls.Add(_lblWindowsId);
        scroll.Controls.Add(f4);
        scroll.Controls.Add(f3);
        scroll.Controls.Add(f2);
        scroll.Controls.Add(f1);
        return scroll;
    }

    private async Task LoadSettingsAsync()
    {
        try
        {
            string? windowsId = WindowsIdProvider.GetMachineGuid();
            _lblWindowsId.Text = $"Machine ID: {windowsId ?? "N/A"}";

            if (string.IsNullOrEmpty(windowsId)) return;

            var s = await _adminSettingService.LoadSettingById(windowsId);
            if (s == null) return;

            _tbIP.Text      = s.IP          ?? "";
            _tbPort.Text    = s.Port?.ToString() ?? "";
            _tbTerminal.Text= s.Terminal    ?? "";
            _tbPrinter.Text = s.PrinterName ?? "";
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error loading settings: " + ex.Message);
        }
    }

    protected override async Task<bool> OnConfirmAsync()
    {
        // Validate port
        if (!int.TryParse(_tbPort.Text, out int port) || port < 1 || port > 65535)
        {
            MessageBox.Show(
                "Port must be a number between 1 and 65535.",
                "Invalid Port", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        bool saved = await _adminSettingService.UpdateSetting(new AdminSetting
        {
            WindowsId   = WindowsIdProvider.GetMachineGuid(),
            IP          = _tbIP.Text,
            Port        = port,
            Terminal    = _tbTerminal.Text,
            PrinterName = _tbPrinter.Text,
        });

        if (!saved)
        {
            MessageBox.Show(
                "Settings could not be saved. Please try again.",
                "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        // Force the payment service to reload config on next payment call
        _paymentCoordinatorService.InvalidateConfig();

        MessageBox.Show(
            "Settings saved successfully.",
            "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);

        return true;
    }
}
