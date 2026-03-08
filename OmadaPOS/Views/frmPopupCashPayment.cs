using OmadaPOS.Impresora;
using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Services;

namespace OmadaPOS.Views;

/// <summary>
/// Shown (modeless) immediately after a cash payment is processed.
/// Displays the change due and provides a one-click receipt print.
/// </summary>
public sealed class frmPopupCashPayment : POSDialog
{
    private readonly IOrderService   _orderService;
    private readonly IBranchService  _branchService;

    private readonly int     _orderId;
    private readonly int     _consecutivo;
    private readonly decimal _devuelta;

    private Button _btnPrint = null!;

    public frmPopupCashPayment(
        IOrderService  orderService,
        IBranchService branchService,
        int orderId, int consecutivo, decimal devuelta)
    {
        _orderService  = orderService;
        _branchService = branchService;
        _orderId       = orderId;
        _consecutivo   = consecutivo;
        _devuelta      = devuelta;
    }

    protected override Color      AccentColor => AppColors.AccentGreen;
    protected override string     Icon        => "💵";
    protected override string     Title       => "Cash Payment";
    protected override string     Subtitle    => "Transaction complete — give change to customer";
    protected override DialogSize Size        => DialogSize.Medium;
    protected override string?    ConfirmText => "🖨  PRINT RECEIPT";
    protected override string     CancelText  => "✕  CLOSE";

    protected override Control BuildContent()
    {
        var outer = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 1,
            RowCount    = 2,
            BackColor   = AppColors.BackgroundPrimary,
            Padding     = new Padding(24, 20, 24, 12),
        };
        outer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        outer.RowStyles.Add(new RowStyle(SizeType.Percent,  60)); // change card
        outer.RowStyles.Add(new RowStyle(SizeType.Percent,  40)); // invoice info

        // ── Change Amount Card — TableLayoutPanel so rows are exact ───────────
        var changeCard = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 1,
            RowCount    = 2,
            BackColor   = Color.FromArgb(236, 253, 245),
            Margin      = new Padding(0, 0, 0, 12),
        };
        changeCard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        changeCard.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));  // "CHANGE DUE" label
        changeCard.RowStyles.Add(new RowStyle(SizeType.Percent, 100));  // amount

        var lblLegend = new Label
        {
            AutoSize  = false,
            Dock      = DockStyle.Fill,
            Text      = "CHANGE DUE",
            Font      = AppTypography.RowLabel,
            ForeColor = AppColors.TextSecondary,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.BottomCenter,
        };

        var lblAmount = new Label
        {
            AutoSize  = false,
            Dock      = DockStyle.Fill,
            Text      = _devuelta.ToString("C"),
            Font      = new Font("Montserrat", 44F, FontStyle.Bold),
            ForeColor = AppColors.AccentGreen,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleCenter,
        };

        changeCard.Controls.Add(lblLegend, 0, 0);
        changeCard.Controls.Add(lblAmount, 0, 1);

        // ── Invoice info row ──────────────────────────────────────────────────
        var invoiceRow = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 2,
            RowCount    = 1,
            BackColor   = Color.Transparent,
            Margin      = new Padding(0),
        };
        invoiceRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 48));
        invoiceRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        invoiceRow.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var lblIcon = new Label
        {
            AutoSize  = false,
            Dock      = DockStyle.Fill,
            Text      = "🧾",
            Font      = new Font("Segoe UI Emoji", 22F),
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleCenter,
        };

        var lblInvoice = new Label
        {
            AutoSize  = false,
            Dock      = DockStyle.Fill,
            Text      = $"Invoice  #  {_consecutivo}",
            Font      = AppTypography.SectionTitle,
            ForeColor = AppColors.TextPrimary,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleLeft,
        };

        invoiceRow.Controls.Add(lblIcon,    0, 0);
        invoiceRow.Controls.Add(lblInvoice, 1, 0);

        outer.Controls.Add(changeCard, 0, 0);
        outer.Controls.Add(invoiceRow, 0, 1);

        return outer;
    }

    protected override async Task<bool> OnConfirmAsync()
    {
        try
        {
            var order        = await _orderService.GetOrderById(_orderId);
            var orderDetails = await _orderService.GetOrderDetailsByOrderId(_orderId);

            if (order == null || orderDetails == null)
            {
                MessageBox.Show("Could not retrieve order information.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            var branch = await _branchService.LoadBranch(SessionManager.BranchId ?? 0);
            if (branch != null)
            {
                var ticket = new Ticket(_orderId, order.Consecutivo, order, orderDetails,
                    SessionManager.Name, null, branch.Address, branch.FooterMsg);
                ticket.Print();
            }

            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Print error:\n{ex.Message}", "Print Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }
}
