using OmadaPOS.Componentes;

namespace OmadaPOS.Presentation.Controls;

public class PaymentPanelControl : UserControl
{
    private readonly TableLayoutPanel _paymentPanel;
    private readonly TableLayoutPanel _paymentHeader;
    private readonly KeyPaymentControl _keyPaymentControl;
    private readonly RoundedPanel _summaryPanel;
    private readonly TableLayoutPanel _summaryLayout;
    private readonly Label _labelTender;
    private readonly Label _labelDue;
    private readonly Label _labelInputValue;
    private readonly Label _labelChangeValue;
    private readonly TableLayoutPanel _paymentButtons;
    private readonly SplitContainer _splitScale;
    private readonly TableLayoutPanel _weightLayout;
    private readonly Label _labelWeight;
    private readonly Label _labelProduct;
    private readonly Label _labelStatus;
    private readonly PictureBox _pictureBox;

    private PaymentPanelControl(
        TableLayoutPanel paymentPanel,
        TableLayoutPanel paymentHeader,
        KeyPaymentControl keyPaymentControl,
        RoundedPanel summaryPanel,
        TableLayoutPanel summaryLayout,
        Label labelTender,
        Label labelDue,
        Label labelInputValue,
        Label labelChangeValue,
        TableLayoutPanel paymentButtons,
        SplitContainer splitScale,
        TableLayoutPanel weightLayout,
        Label labelWeight,
        Label labelProduct,
        Label labelStatus,
        PictureBox pictureBox)
    {
        _paymentPanel   = paymentPanel;
        _paymentHeader  = paymentHeader;
        _keyPaymentControl = keyPaymentControl;
        _summaryPanel = summaryPanel;
        _summaryLayout = summaryLayout;
        _labelTender = labelTender;
        _labelDue = labelDue;
        _labelInputValue = labelInputValue;
        _labelChangeValue = labelChangeValue;
        _paymentButtons = paymentButtons;
        _splitScale = splitScale;
        _weightLayout = weightLayout;
        _labelWeight = labelWeight;
        _labelProduct = labelProduct;
        _labelStatus = labelStatus;
        _pictureBox = pictureBox;

        Dock = DockStyle.Fill;
        Margin = _paymentPanel.Margin;
        Padding = new Padding(0);
        BackColor = Color.Transparent;

        _paymentPanel.Dock = DockStyle.Fill;
        Controls.Add(_paymentPanel);
    }

    public static PaymentPanelControl Attach(
        TableLayoutPanel mainLayout,
        TableLayoutPanel paymentPanel,
        TableLayoutPanel paymentHeader,
        KeyPaymentControl keyPaymentControl,
        RoundedPanel summaryPanel,
        TableLayoutPanel summaryLayout,
        Label labelTender,
        Label labelDue,
        Label labelInputValue,
        Label labelChangeValue,
        TableLayoutPanel paymentButtons,
        SplitContainer splitScale,
        TableLayoutPanel weightLayout,
        Label labelWeight,
        Label labelProduct,
        Label labelStatus,
        PictureBox pictureBox)
    {
        ArgumentNullException.ThrowIfNull(mainLayout);
        ArgumentNullException.ThrowIfNull(paymentPanel);

        var pos = mainLayout.GetPositionFromControl(paymentPanel);
        mainLayout.Controls.Remove(paymentPanel);

        var control = new PaymentPanelControl(
            paymentPanel,
            paymentHeader,
            keyPaymentControl,
            summaryPanel,
            summaryLayout,
            labelTender,
            labelDue,
            labelInputValue,
            labelChangeValue,
            paymentButtons,
            splitScale,
            weightLayout,
            labelWeight,
            labelProduct,
            labelStatus,
            pictureBox);

        mainLayout.Controls.Add(control, pos.Column, pos.Row);
        return control;
    }

    public void ApplyTheme()
    {
        _paymentPanel.BackColor  = AppColors.BackgroundPrimary;
        _paymentPanel.Padding    = AppSpacing.PaymentColumn;

        _paymentHeader.BackColor = AppColors.BackgroundPrimary;
        _keyPaymentControl.BackColor = AppColors.BackgroundPrimary;

        _summaryPanel.BackgroundStart = AppColors.BackgroundSecondary;
        _summaryPanel.BackgroundEnd   = AppColors.SurfaceMuted;
        _summaryPanel.BorderColor     = AppBorders.PanelLight;
        _summaryPanel.ShadowColor     = AppShadows.Subtle;
        _summaryPanel.CornerRadius    = AppRadii.Panel;
        _summaryPanel.Padding         = AppSpacing.SummaryInner;
        _summaryPanel.Margin          = AppSpacing.PanelMargin;

        _summaryLayout.BackColor = Color.Transparent;

        _labelTender.Font      = AppTypography.PaymentLabel;
        _labelTender.ForeColor = AppColors.SlateBlue;
        _labelTender.BackColor = Color.Transparent;

        _labelDue.Font      = AppTypography.PaymentLabel;
        _labelDue.ForeColor = AppColors.SlateBlue;
        _labelDue.BackColor = Color.Transparent;

        _labelInputValue.Font      = AppTypography.AmountDisplay;
        _labelInputValue.ForeColor = AppColors.NavyDark;
        _labelInputValue.BackColor = Color.Transparent;

        _labelChangeValue.Font      = AppTypography.AmountDisplay;
        _labelChangeValue.ForeColor = AppColors.AccentGreen;
        _labelChangeValue.BackColor = Color.Transparent;

        _paymentButtons.BackColor = AppColors.BackgroundPrimary;
        _paymentButtons.Padding   = AppSpacing.ButtonGroup;

        _weightLayout.BackColor = AppColors.NavyDark;
        _weightLayout.Padding   = AppSpacing.ScaleSection;

        _labelWeight.Font      = AppTypography.WeightDisplay;
        _labelWeight.ForeColor = AppColors.Warning;
        _labelWeight.BackColor = Color.Transparent;

        _labelProduct.Font      = AppTypography.Body;
        _labelProduct.ForeColor = AppColors.TextWhite;
        _labelProduct.BackColor = Color.Transparent;

        _labelStatus.Font      = AppTypography.Caption;
        _labelStatus.ForeColor = AppColors.TextMuted;
        _labelStatus.BackColor = Color.Transparent;

        _pictureBox.BackColor = AppColors.NavyDark;
    }

    public void UpdatePaymentValues(decimal inputValue, decimal changeValue)
    {
        _labelInputValue.Text = inputValue.ToString("N2");
        _labelChangeValue.Text = changeValue.ToString("N2");
    }

    public void ResetPaymentValues()
    {
        _labelInputValue.Text = "0.00";
        _labelChangeValue.Text = "0.00";
    }
}
