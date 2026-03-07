using System.ComponentModel;
using OmadaPOS.Libreria.Utils;

namespace OmadaPOS.Presentation.Controls;

public class ScanInputControl : UserControl
{
    private readonly TextBox _textBox;
    private readonly Panel _scanPanel;

    private ScanInputControl(TextBox textBox)
    {
        _textBox = textBox ?? throw new ArgumentNullException(nameof(textBox));

        Dock = DockStyle.Fill;
        Margin = _textBox.Margin;
        Padding = new Padding(0);
        BackColor = Color.Transparent;

        _scanPanel = BuildScanPanel();
        Controls.Add(_scanPanel);
    }

    public static ScanInputControl Attach(TableLayoutPanel headerLayout, TextBox textBox)
    {
        ArgumentNullException.ThrowIfNull(headerLayout);
        ArgumentNullException.ThrowIfNull(textBox);

        var pos = headerLayout.GetPositionFromControl(textBox);
        headerLayout.Controls.Remove(textBox);

        var control = new ScanInputControl(textBox);
        headerLayout.Controls.Add(control, pos.Column, pos.Row);

        return control;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string TextValue
    {
        get => _textBox.Text;
        set => _textBox.Text = value;
    }

    public void FocusInput() => _textBox.Focus();

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
            BackColor = AppColors.NavyBase,
            Margin = new Padding(4, 5, 4, 5),
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
        _textBox.BackColor       = AppColors.NavyBase;
        _textBox.ForeColor       = AppColors.AccentGreen;
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
