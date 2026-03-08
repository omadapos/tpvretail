using OmadaPOS.Componentes;

namespace OmadaPOS.Presentation.Controls;

public class CartTotalsControl : UserControl
{
    private readonly RoundedPanel _panel;
    private readonly Label _labelSubtotalTitle;
    private readonly Label _labelTaxTitle;
    private readonly Label _labelTotalTitle;
    private readonly Label _labelSubtotalValue;
    private readonly Label _labelTaxValue;
    private readonly Label _labelTotalValue;

    private CartTotalsControl(
        RoundedPanel panel,
        Label labelSubtotalTitle,
        Label labelTaxTitle,
        Label labelTotalTitle,
        Label labelSubtotalValue,
        Label labelTaxValue,
        Label labelTotalValue)
    {
        _panel = panel;
        _labelSubtotalTitle = labelSubtotalTitle;
        _labelTaxTitle = labelTaxTitle;
        _labelTotalTitle = labelTotalTitle;
        _labelSubtotalValue = labelSubtotalValue;
        _labelTaxValue = labelTaxValue;
        _labelTotalValue = labelTotalValue;

        Dock = DockStyle.Fill;
        Margin = _panel.Margin;
        Padding = new Padding(0);
        BackColor = Color.Transparent;

        _panel.Dock = DockStyle.Fill;
        Controls.Add(_panel);
    }

    public static CartTotalsControl Attach(
        TableLayoutPanel parent,
        RoundedPanel panel,
        Label labelSubtotalTitle,
        Label labelTaxTitle,
        Label labelTotalTitle,
        Label labelSubtotalValue,
        Label labelTaxValue,
        Label labelTotalValue)
    {
        ArgumentNullException.ThrowIfNull(parent);
        ArgumentNullException.ThrowIfNull(panel);

        var pos = parent.GetPositionFromControl(panel);
        parent.Controls.Remove(panel);

        var control = new CartTotalsControl(
            panel,
            labelSubtotalTitle,
            labelTaxTitle,
            labelTotalTitle,
            labelSubtotalValue,
            labelTaxValue,
            labelTotalValue);

        parent.Controls.Add(control, pos.Column, pos.Row);
        return control;
    }

    public void ApplyTheme()
    {
        _panel.BackgroundStart = AppColors.NavyDark;
        _panel.BackgroundEnd   = AppColors.NavyBase;
        _panel.BorderColor     = AppBorders.AccentLine;
        _panel.CornerRadius    = AppRadii.Panel;
        _panel.Padding         = AppSpacing.TotalsInner;
        _panel.Margin          = AppSpacing.PanelMargin;

        _labelSubtotalTitle.Font      = AppTypography.RowLabel;
        _labelSubtotalTitle.ForeColor = AppColors.TextOnDarkMuted;
        _labelSubtotalTitle.BackColor = Color.Transparent;

        _labelTaxTitle.Font      = AppTypography.RowLabel;
        _labelTaxTitle.ForeColor = AppColors.TextOnDarkMuted;
        _labelTaxTitle.BackColor = Color.Transparent;

        _labelTotalTitle.Font      = AppTypography.SectionTitle;
        _labelTotalTitle.ForeColor = AppColors.TextWhite;
        _labelTotalTitle.BackColor = Color.Transparent;

        _labelSubtotalValue.Font      = AppTypography.AmountMono;
        _labelSubtotalValue.ForeColor = AppColors.TextOnDarkSecondary;
        _labelSubtotalValue.BackColor = Color.Transparent;

        _labelTaxValue.Font      = AppTypography.AmountMono;
        _labelTaxValue.ForeColor = AppColors.TextOnDarkMuted;
        _labelTaxValue.BackColor = Color.Transparent;

        _labelTotalValue.Font      = AppTypography.AmountGrand;
        _labelTotalValue.ForeColor = AppColors.AccentGreen;
        _labelTotalValue.BackColor = Color.Transparent;
    }

    public void UpdateTotals(decimal subtotal, decimal tax, decimal total)
    {
        _labelSubtotalValue.Text = subtotal.ToString("C");
        _labelTaxValue.Text      = tax.ToString("C");
        _labelTotalValue.Text    = total.ToString("C");
    }

    public void ResetTotals(bool includeGrandTotal)
    {
        _labelSubtotalValue.Text = "0.00";
        _labelTaxValue.Text = "0.00";

        if (includeGrandTotal)
            _labelTotalValue.Text = "0.00";
    }
}
