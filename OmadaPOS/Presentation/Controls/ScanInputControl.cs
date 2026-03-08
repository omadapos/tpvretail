using System.ComponentModel;
using OmadaPOS.Libreria.Utils;

namespace OmadaPOS.Presentation.Controls;

public class ScanInputControl : UserControl
{
    private readonly TextBox _textBox;

    private ScanInputControl(TextBox textBox)
    {
        _textBox = textBox ?? throw new ArgumentNullException(nameof(textBox));

        Dock = DockStyle.Fill;
        Margin = _textBox.Margin;
        Padding = new Padding(0);
        BackColor = Color.Transparent;

        Controls.Add(BuildScanPanel());
    }

    // Column 1 = scan zone in the 6-column header layout.
    // We use a fixed column instead of GetPositionFromControl so the Designer
    // can place textBox anywhere without breaking the layout on regeneration.
    public static ScanInputControl Attach(TableLayoutPanel headerLayout, TextBox textBox, int column = 1)
    {
        ArgumentNullException.ThrowIfNull(headerLayout);
        ArgumentNullException.ThrowIfNull(textBox);

        textBox.Parent?.Controls.Remove(textBox);

        var control = new ScanInputControl(textBox);
        headerLayout.Controls.Add(control, column, 0);

        return control;
    }

    public void ClearInput()
    {
        _textBox.Text = string.Empty;
        _textBox.Focus();
    }

    private Panel BuildScanPanel()
    {
        var scanPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppColors.SurfaceMuted,
            Margin = new Padding(8, 6, 8, 6),
            Cursor = Cursors.IBeam,
        };

        scanPanel.Paint += (s, e) =>
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            var rect = new Rectangle(1, 1, scanPanel.Width - 2, scanPanel.Height - 2);
            using var path   = ElegantButtonStyles.RoundedPath(rect, AppRadii.Compact);
            using var border = new Pen(AppBorders.Focus, AppBorders.Scan);
            g.DrawPath(border, path);
        };

        var iconLabel = new Label
        {
            Text      = "⬛",
            Font      = AppTypography.ListItem,
            ForeColor = AppColors.AccentGreen,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Left,
            Width     = 34,
            TextAlign = ContentAlignment.MiddleCenter,
            Cursor    = Cursors.IBeam,
        };
        iconLabel.Click += (s, e) => _textBox.Focus();

        var btnClear = new Button
        {
            Text      = "✕",
            Font      = AppTypography.KeyboardButton,
            ForeColor = AppColors.TextMuted,
            BackColor = Color.Transparent,
            FlatStyle = FlatStyle.Flat,
            Dock = DockStyle.Right,
            Width = 30,
            Cursor = Cursors.Hand,
            TabStop = false,
        };
        btnClear.FlatAppearance.BorderSize = 0;
        btnClear.FlatAppearance.MouseOverBackColor = Color.Transparent;
        btnClear.Click += (s, e) => ClearInput();

        _textBox.Dock = DockStyle.Fill;
        _textBox.BackColor       = AppColors.SurfaceMuted;
        _textBox.ForeColor       = AppColors.TextPrimary;
        _textBox.Font            = AppTypography.ScanInput;
        _textBox.BorderStyle     = BorderStyle.None;
        _textBox.TextAlign       = HorizontalAlignment.Center;
        _textBox.PlaceholderText = "Scan or type UPC...";
        _textBox.Margin          = AppSpacing.None;

        scanPanel.Controls.Add(_textBox);
        scanPanel.Controls.Add(btnClear);
        scanPanel.Controls.Add(iconLabel);
        scanPanel.Click += (s, e) => _textBox.Focus();

        return scanPanel;
    }
}
