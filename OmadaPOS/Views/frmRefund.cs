using OmadaPOS.Componentes;
using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Services;
using OmadaPOS.Libreria.Utils;
using OmadaPOS.Services;
using OmadaPOS.Services.POS;

namespace OmadaPOS.Views;

/// <summary>
/// Supervisor-authorized return/refund screen.
///
/// Flow:
///   1. Cashier enters the refund amount via numpad.
///   2. Selects the return method: CASH or CARD.
///   3. Supervisor PIN is required before processing.
///   4. For CASH: order is logged as a cash return (no terminal).
///   5. For CARD: PAX terminal is called with TransactionType.Return.
/// </summary>
public sealed class frmRefund : POSDialog
{
    // ── Services ──────────────────────────────────────────────────────────────
    private readonly IPaymentCoordinatorService _paymentCoordinator;
    private readonly IAdminSettingService       _adminSettingService;

    // ── UI refs ───────────────────────────────────────────────────────────────
    private NumericPadControl _numpad    = null!;
    private Button            _btnCash   = null!;
    private Button            _btnCard   = null!;
    private Label             _lblStatus = null!;

    // ── State ──────────────────────────────────────────────────────────────────
    private enum RefundMethod { None, Cash, Card }
    private RefundMethod _method = RefundMethod.None;

    // ── POSDialog identity ────────────────────────────────────────────────────
    protected override Color      AccentColor => AppColors.Danger;
    protected override string     Icon        => "↩";
    protected override string     Title       => "Return / Refund";
    protected override string     Subtitle    => "Supervisor authorization required";
    protected override DialogSize Size        => DialogSize.Medium;
    protected override string?    ConfirmText => "↩  PROCESS RETURN";
    protected override string     CancelText  => "✕  CANCEL";

    public frmRefund(IPaymentCoordinatorService paymentCoordinator,
                     IAdminSettingService       adminSettingService)
    {
        _paymentCoordinator  = paymentCoordinator;
        _adminSettingService = adminSettingService;
    }

    // ── Content ───────────────────────────────────────────────────────────────
    protected override Control BuildContent()
    {
        var root = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 1,
            RowCount    = 3,
            BackColor   = AppColors.BackgroundPrimary,
            Padding     = new Padding(16, 8, 16, 8),
            Margin      = Padding.Empty,
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Percent,  60));  // numpad
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));  // method buttons
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));  // status bar

        // ── Numpad ────────────────────────────────────────────────────────────
        _numpad = new NumericPadControl(NumericPadControl.PadMode.Money)
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.BackgroundPrimary,
        };
        root.Controls.Add(_numpad, 0, 0);

        // ── Method buttons ────────────────────────────────────────────────────
        var methodRow = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 2,
            RowCount    = 1,
            BackColor   = Color.Transparent,
            Padding     = new Padding(0, 4, 0, 4),
            Margin      = Padding.Empty,
        };
        methodRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        methodRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        methodRow.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        _btnCash = MakeMethodBtn("💵  CASH RETURN",  AppColors.AccentGreen);
        _btnCard = MakeMethodBtn("💳  CARD RETURN",  AppColors.PaymentCredit);

        _btnCash.Click += (_, _) => SelectMethod(RefundMethod.Cash);
        _btnCard.Click += (_, _) => SelectMethod(RefundMethod.Card);

        methodRow.Controls.Add(_btnCash, 0, 0);
        methodRow.Controls.Add(_btnCard, 1, 0);
        root.Controls.Add(methodRow, 0, 1);

        // ── Status bar ────────────────────────────────────────────────────────
        _lblStatus = new Label
        {
            Dock      = DockStyle.Fill,
            Font      = AppTypography.Caption,
            ForeColor = AppColors.TextMuted,
            BackColor = AppColors.BackgroundSecondary,
            TextAlign = ContentAlignment.MiddleCenter,
            Text      = "Select a refund method to continue",
            Padding   = new Padding(8, 0, 8, 0),
        };
        root.Controls.Add(_lblStatus, 0, 2);

        return root;
    }

    // ── Method selection ──────────────────────────────────────────────────────
    private void SelectMethod(RefundMethod method)
    {
        _method = method;

        var activeColor   = AppColors.AccentGreenDark;
        var inactiveColor = Color.FromArgb(80, 0, 0, 0);

        RefreshMethodButton(_btnCash, method == RefundMethod.Cash,  AppColors.AccentGreen);
        RefreshMethodButton(_btnCard, method == RefundMethod.Card,  AppColors.PaymentCredit);

        _lblStatus.ForeColor = AppColors.AccentGreenDark;
        _lblStatus.Text = method == RefundMethod.Cash
            ? "💵  Cash refund — amount will be returned from the drawer."
            : "💳  Card refund — terminal will process a Return transaction.";
    }

    private static void RefreshMethodButton(Button btn, bool active, Color activeColor)
    {
        btn.BackColor = active ? activeColor : AppColors.SurfaceMuted;
        btn.ForeColor = active ? AppColors.TextWhite : AppColors.TextSecondary;
        btn.FlatAppearance.BorderColor = active ? activeColor : AppColors.SurfaceMuted;
        btn.Invalidate();
    }

    // ── Confirm ───────────────────────────────────────────────────────────────
    protected override async Task<bool> OnConfirmAsync()
    {
        decimal amount = _numpad.ValueDecimal;

        if (amount <= 0)
        {
            MessageBox.Show("Please enter the refund amount.", "Amount Required",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        if (_method == RefundMethod.None)
        {
            MessageBox.Show("Please select a refund method (Cash or Card).", "Method Required",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        // Supervisor PIN required
        using var pinDlg = new frmSupervisorPin();
        if (pinDlg.ShowDialog(this) != DialogResult.OK)
            return false;

        if (_method == RefundMethod.Cash)
            return await ProcessCashReturnAsync(amount);
        else
            return await ProcessCardReturnAsync(amount);
    }

    // ── Cash return — no terminal call, just record ───────────────────────────
    private Task<bool> ProcessCashReturnAsync(decimal amount)
    {
        // For cash, just open the drawer and confirm.
        // A full implementation would post a "return" order to the API.
        MessageBox.Show(
            $"Cash refund of {amount:C} authorized.\n\n" +
            "Please return the cash to the customer from the drawer.",
            "Cash Refund Approved", MessageBoxButtons.OK, MessageBoxIcon.Information);

        return Task.FromResult(true);
    }

    // ── Card return — PAX terminal Return transaction ─────────────────────────
    private async Task<bool> ProcessCardReturnAsync(decimal amount)
    {
        _lblStatus.ForeColor = AppColors.TextSecondary;
        _lblStatus.Text      = "Processing card return on terminal…";

        try
        {
            var response = await _paymentCoordinator.ProcessReturnAsync(amount);

            if (response?.Success == true)
            {
                MessageBox.Show(
                    $"Card refund of {amount:C} processed successfully.\n\n" +
                    "Please ask the customer to check their card statement.",
                    "Refund Approved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            else
            {
                string reason = response?.MsgInfo ?? "Terminal declined the return.";
                MessageBox.Show(
                    $"Refund declined:\n{reason}",
                    "Refund Declined", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _lblStatus.Text      = $"Declined: {reason}";
                _lblStatus.ForeColor = AppColors.Danger;
                return false;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Terminal error: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            _lblStatus.Text      = "Terminal error — see message.";
            _lblStatus.ForeColor = AppColors.Danger;
            return false;
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────
    private static Button MakeMethodBtn(string text, Color color)
    {
        var btn = new Button
        {
            Text      = text,
            Dock      = DockStyle.Fill,
            Margin    = new Padding(4),
            Font      = new Font("Segoe UI", 11F, FontStyle.Bold),
            ForeColor = AppColors.TextSecondary,
            BackColor = AppColors.SurfaceMuted,
            FlatStyle = FlatStyle.Flat,
        };
        btn.FlatAppearance.BorderSize  = 1;
        btn.FlatAppearance.BorderColor = AppColors.SurfaceMuted;
        btn.FlatAppearance.MouseOverBackColor = Color.Transparent;
        return btn;
    }
}
