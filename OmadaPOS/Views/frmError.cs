namespace OmadaPOS.Views;

public sealed class frmError : POSDialog
{
    private readonly string _message;

    public frmError(string errMsg)
    {
        _message = errMsg;
    }

    protected override Color       AccentColor => AppColors.Danger;
    protected override string      Icon        => "⚠";
    protected override string      Title       => "Error";
    protected override string      Subtitle    => "An unexpected error occurred";
    protected override DialogSize  Size        => DialogSize.Compact;
    protected override string      CancelText  => "✕  CLOSE";

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
            Font      = AppTypography.Body,
            ForeColor = AppColors.TextPrimary,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
        };

        panel.Controls.Add(lbl);
        return panel;
    }
}
