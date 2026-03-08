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
        BackColor               = AppColors.BackgroundSecondary;
        _headerLayout.BackColor = AppColors.BackgroundSecondary;
        _headerLayout.Margin    = AppSpacing.None;
        _headerLayout.Padding   = new Padding(0);

        // Accent line at bottom + subtle shadow
        _headerLayout.Paint += (_, e) =>
        {
            using var pen    = new Pen(AppColors.AccentGreen, 2f);
            using var shadow = new Pen(AppColors.ShadowSubtle, 1f);
            e.Graphics.DrawLine(pen,    0, _headerLayout.Height - 1, _headerLayout.Width, _headerLayout.Height - 1);
            e.Graphics.DrawLine(shadow, 0, _headerLayout.Height - 2, _headerLayout.Width, _headerLayout.Height - 2);
        };

        // ── Ensure exactly 6 columns (Designer may have reset ColumnCount) ────────
        // Layout: [220px Brand] [% Scan] [90px Print] [90px Settings] [200px User] [90px Exit]
        _headerLayout.ColumnCount = 6;
        while (_headerLayout.ColumnStyles.Count < 6)
            _headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));

        _headerLayout.ColumnStyles[0] = new ColumnStyle(SizeType.Absolute, 220);
        _headerLayout.ColumnStyles[1] = new ColumnStyle(SizeType.Percent,  100);
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
            ForeColor = AppColors.TextPrimary,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Top,
            Height    = 32,
            TextAlign = ContentAlignment.BottomLeft,
        };

        _lblOrder = new Label
        {
            Text      = "# ------",
            Font      = new Font("Consolas", 10F, FontStyle.Regular),
            ForeColor = AppColors.AccentGreen,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.TopLeft,
        };

        // DockStyle.Top stacking: add Fill first, then Top — lblBrand ends on top
        pnlZone1.Controls.Add(_lblOrder);
        pnlZone1.Controls.Add(lblBrand);
        PlaceInHeader(pnlZone1, 0);

        // ── Zone 2: Scan input — handled by ScanInputControl (col 1) ─────────────

        // ── Zone 3: Print ────────────────────────────────────────────────────────
        _buttonPrint.Text   = "🖨";
        _buttonPrint.Dock   = DockStyle.Fill;
        _buttonPrint.Margin = new Padding(2, 6, 2, 6);
        ElegantButtonStyles.Style(_buttonPrint, AppColors.SlateBlue, AppColors.TextWhite, radius: AppRadii.Compact, fontSize: 18f);
        PlaceInHeader(_buttonPrint, 2);

        // ── Zone 4: Settings ─────────────────────────────────────────────────────
        _buttonSettings.Text   = "⚙";
        _buttonSettings.Dock   = DockStyle.Fill;
        _buttonSettings.Margin = new Padding(2, 6, 2, 6);
        ElegantButtonStyles.Style(_buttonSettings, AppColors.SlateBlue, AppColors.TextWhite, radius: AppRadii.Compact, fontSize: 18f);
        PlaceInHeader(_buttonSettings, 3);

        // ── Zone 5: User session — handled by UserSessionControl (col 4) ─────────

        // ── Zone 6: Exit ─────────────────────────────────────────────────────────
        _buttonClose.Text   = "⏻";
        _buttonClose.Dock   = DockStyle.Fill;
        _buttonClose.Margin = new Padding(2, 6, 2, 6);
        ElegantButtonStyles.Style(_buttonClose, AppColors.Danger, AppColors.TextWhite, radius: AppRadii.Compact, fontSize: 20f);
        PlaceInHeader(_buttonClose, 5);
    }

    /// <summary>
    /// Moves <paramref name="ctrl"/> to <paramref name="column"/> in the header layout,
    /// removing it from its current parent first. This is Designer-regeneration-safe.
    /// </summary>
    private void PlaceInHeader(Control ctrl, int column)
    {
        ctrl.Parent?.Controls.Remove(ctrl);
        _headerLayout.Controls.Add(ctrl, column, 0);
    }

    public void SetInvoiceDisplay(int orderId)
    {
        if (_lblOrder != null)
            _lblOrder.Text = $"# {orderId:D5}";
    }
}
