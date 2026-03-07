using OmadaPOS.Presentation.Styling;

namespace OmadaPOS.Presentation.Controls;

public class POSHeaderControl : UserControl
{
    private readonly TableLayoutPanel _headerLayout;
    private readonly Button           _buttonPrint;
    private readonly Button           _buttonSettings;
    private readonly Button           _buttonClose;
    private Label?                    _lblOrder;

    private POSHeaderControl(
        TableLayoutPanel headerLayout,
        Button buttonPrint,
        Button buttonSettings,
        Button buttonClose)
    {
        _headerLayout   = headerLayout;
        _buttonPrint    = buttonPrint;
        _buttonSettings = buttonSettings;
        _buttonClose    = buttonClose;

        Dock      = DockStyle.Fill;
        Margin    = _headerLayout.Margin;
        Padding   = new Padding(0);
        BackColor = Color.Transparent;

        _headerLayout.Dock = DockStyle.Fill;
        Controls.Add(_headerLayout);
    }

    public static POSHeaderControl Attach(
        TableLayoutPanel mainLayout,
        TableLayoutPanel headerLayout,
        Button buttonPrint,
        Button buttonSettings,
        Button buttonClose)
    {
        ArgumentNullException.ThrowIfNull(mainLayout);
        ArgumentNullException.ThrowIfNull(headerLayout);

        var pos = mainLayout.GetPositionFromControl(headerLayout);
        mainLayout.Controls.Remove(headerLayout);

        var control = new POSHeaderControl(
            headerLayout,
            buttonPrint,
            buttonSettings,
            buttonClose);

        mainLayout.Controls.Add(control, pos.Column, pos.Row);
        return control;
    }

    public void ApplyTheme()
    {
        BackColor               = AppColors.NavyDark;
        _headerLayout.BackColor = AppColors.NavyDark;
        _headerLayout.Margin    = AppSpacing.None;
        _headerLayout.Padding   = new Padding(0);

        // Accent line at bottom of header
        _headerLayout.Paint += (_, e) =>
        {
            using var pen = new Pen(AppColors.AccentGreen, 2f);
            e.Graphics.DrawLine(pen, 0, _headerLayout.Height - 1, _headerLayout.Width, _headerLayout.Height - 1);
        };

        // ── Column widths ────────────────────────────────────────────────────────
        // [220px Zone1-Brand] [100% Scan] [90px Print] [90px Settings] [200px User] [90px Exit]
        _headerLayout.ColumnStyles[0] = new ColumnStyle(SizeType.Absolute, 220);
        _headerLayout.ColumnStyles[1] = new ColumnStyle(SizeType.Percent, 100);
        _headerLayout.ColumnStyles[2] = new ColumnStyle(SizeType.Absolute, 90);
        _headerLayout.ColumnStyles[3] = new ColumnStyle(SizeType.Absolute, 90);
        _headerLayout.ColumnStyles[4] = new ColumnStyle(SizeType.Absolute, 200);
        _headerLayout.ColumnStyles[5] = new ColumnStyle(SizeType.Absolute, 90);

        // ── Zone 1: Brand + Order number ────────────────────────────────────────
        var pnlZone1 = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = Color.Transparent,
            Padding   = new Padding(16, 0, 8, 0),
        };

        var lblBrand = new Label
        {
            Text      = "● Omada POS",
            Font      = AppTypography.AppHeader,
            ForeColor = AppColors.TextWhite,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Top,
            Height    = 32,
            TextAlign = ContentAlignment.BottomLeft,
        };

        _lblOrder = new Label
        {
            Text      = "# ------",
            Font      = new Font("Consolas", 10F, FontStyle.Regular),
            ForeColor = AppColors.AccentGreenLight,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.TopLeft,
        };

        // Bottom-first for DockStyle.Top stacking: lblBrand on top, _lblOrder fills remainder
        pnlZone1.Controls.Add(_lblOrder);
        pnlZone1.Controls.Add(lblBrand);

        _headerLayout.Controls.Add(pnlZone1, 0, 0);

        // ── Zone 2: Scan input ───────────────────────────────────────────────────
        // textBoxUPC already at col 1; styled by ScanInputControl — nothing to do here.

        // ── Zone 3: Print ────────────────────────────────────────────────────────
        _buttonPrint.Text   = "🖨";
        _buttonPrint.Margin = new Padding(2, 8, 2, 8);
        _buttonPrint.Font   = AppTypography.HeaderIcon;

        // ── Zone 4: Settings ─────────────────────────────────────────────────────
        _buttonSettings.Visible = true;
        _buttonSettings.Text    = "⚙";
        _buttonSettings.Margin  = new Padding(2, 8, 2, 8);
        _buttonSettings.Font    = AppTypography.HeaderIcon;

        // ── Zone 6: Exit ─────────────────────────────────────────────────────────
        _buttonClose.Text   = "⏻";
        _buttonClose.Margin = new Padding(2, 8, 2, 8);
        _buttonClose.Font   = AppTypography.HeaderIconLg;
    }

    public void SetInvoiceDisplay(int orderId)
    {
        if (_lblOrder != null)
            _lblOrder.Text = $"# {orderId:D5}";
    }
}
