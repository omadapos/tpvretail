namespace OmadaPOS.Views;

public sealed class frmPaymentStatus : POSDialog
{
    private readonly string _message;
    private readonly bool   _success;

    public frmPaymentStatus(string msg, bool success = true)
    {
        _message = msg;
        _success = success;
    }

    protected override Color      AccentColor => _success ? AppColors.AccentGreen : AppColors.Danger;
    protected override string     Icon        => _success ? "✔" : "✕";
    protected override string     Title       => _success ? "Payment Approved" : "Payment Declined";
    protected override string     Subtitle    => "Transaction result";
    protected override DialogSize Size        => DialogSize.Compact;
    protected override string     CancelText  => "✕  CLOSE";

    protected override Control BuildContent()
    {
        var panel = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.BackgroundPrimary,
            Padding   = new Padding(24, 20, 24, 10),
        };

        var lbl = new Label
        {
            Text      = _message,
            Font      = AppTypography.SectionTitle,
            ForeColor = _success ? AppColors.TextPrimary : AppColors.Danger,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
        };

        panel.Controls.Add(lbl);
        return panel;
    }
}
