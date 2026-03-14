using OmadaPOS.Impresora;
using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Services;
using OmadaPOS.Services;

namespace OmadaPOS.Views;

public sealed class frmCierreDiario : POSDialog
{
    private readonly IOrderService  _orderService;
    private readonly IBranchService _branchService;
    private DateTimePicker _picker = null!;

    public frmCierreDiario(IOrderService orderService, IBranchService branchService)
    {
        _orderService  = orderService;
        _branchService = branchService;
        Shown += (_, _) => ThemeManager.ApplyAll(this);
    }

    protected override Color      AccentColor => AppColors.Danger;
    protected override string     Icon        => "⏻";
    protected override string     Title       => "Close Day";
    protected override string     Subtitle    => "Select date and confirm daily closing";
    protected override DialogSize Size        => DialogSize.Compact;
    protected override string?    ConfirmText => "⏻  CLOSE DAY";
    protected override string     CancelText  => "✕  CANCEL";

    protected override Control BuildContent()
    {
        var panel = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.BackgroundPrimary,
            Padding   = new Padding(28, 24, 28, 8),
        };

        var lbl = new Label
        {
            Text      = "Select closing date:",
            Font      = AppTypography.Body,
            ForeColor = AppColors.TextSecondary,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Top,
            Height    = 28,
        };

        _picker = new DateTimePicker
        {
            Dock      = DockStyle.Top,
            Font      = new Font("Segoe UI", 16F),
            Value     = DateTime.Today,
            Format    = DateTimePickerFormat.Short,
        };

        panel.Controls.Add(_picker);
        panel.Controls.Add(lbl);
        return panel;
    }

    protected override async Task<bool> OnConfirmAsync()
    {
        string dateLabel = _picker.Value.ToString("MMMM dd, yyyy");

        // Step 1: Confirm the operation with a clear warning
        var confirm = MessageBox.Show(
            $"You are about to close the day for:\n\n  📅  {dateLabel}\n\n" +
            "This will generate the Z-Report and close the current shift.\n" +
            "This action cannot be undone.\n\n" +
            "Supervisor authorization is required to proceed.",
            "Confirm Day Close",
            MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

        if (confirm != DialogResult.Yes) return false;

        // Step 2: Require supervisor PIN
        using var pinDialog = new frmSupervisorPin();
        if (pinDialog.ShowDialog(this) != DialogResult.OK) return false;

        // Step 3: Execute
        var sDate  = _picker.Value.ToString("yyyyMMdd");
        var cierre = await _orderService.CierreDiario(sDate, SessionManager.UserName);
        if (cierre == null) return false;

        var branch = await _branchService.LoadBranch(SessionManager.BranchId ?? 0);
        if (branch != null)
        {
            try
            {
                new TicketCierre(cierre, branch.Name ?? AppConstants.AppName, branch.Address).Print();
            }
            catch (Exception printEx)
            {
                MessageBox.Show(
                    $"Day closed successfully, but the report could not print:\n{printEx.Message}",
                    "Print Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        return true;
    }
}
