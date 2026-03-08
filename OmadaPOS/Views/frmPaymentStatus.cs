namespace OmadaPOS.Views;

public sealed class frmPaymentStatus : POSDialog
{
    private readonly string _message;

    public frmPaymentStatus(string msg)
    {
        _message = msg;
    }

    protected override Color      AccentColor => AppColors.AccentGreen;
    protected override string     Icon        => "✔";
    protected override string     Title       => "Payment Status";
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
            ForeColor = AppColors.TextPrimary,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
        };

        panel.Controls.Add(lbl);
        return panel;
    }
}
