using System.ComponentModel;
using OmadaPOS.Presentation.Styling;

namespace OmadaPOS.Componentes;

/// <summary>
/// Unified numeric entry control — replaces KeyPadControl, KeyPadMoneyControl and KeyPaymentControl.
/// </summary>
public sealed class NumericPadControl : UserControl
{
    // ── Mode ─────────────────────────────────────────────────────────────────

    public enum PadMode
    {
        /// <summary>String / integer — quantity, UPC lookup. Has backspace. Pull via TextValue.</summary>
        Integer,

        /// <summary>Dollar amount in cents. Has 00 and Clear. Pull via ValueDecimal / ValueCents.</summary>
        Money,

        /// <summary>Dollar amount in cents + $10/$20/$50/$100 presets. Push via ValueChanged.</summary>
        MoneyWithBills,
    }

    // ── State ─────────────────────────────────────────────────────────────────

    private int    _cents = 0;
    private string _text  = "";

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>Fires whenever the entered value changes (any mode).</summary>
    public event EventHandler? ValueChanged;

    /// <summary>Current value in cents — Money / MoneyWithBills modes.</summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int ValueCents
    {
        get => _cents;
        set { _cents = value; RefreshDisplay(); ValueChanged?.Invoke(this, EventArgs.Empty); }
    }

    /// <summary>Current value as decimal dollars — Money / MoneyWithBills modes.</summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public decimal ValueDecimal => _cents / 100m;

    /// <summary>Current text — Integer mode.</summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string TextValue
    {
        get => _text;
        set { _text = value; RefreshDisplay(); }
    }

    /// <summary>Reset to zero / empty without firing ValueChanged.</summary>
    public void Reset()
    {
        _cents = 0;
        _text  = "";
        RefreshDisplay();
    }

    // ── Private refs ──────────────────────────────────────────────────────────

    private readonly PadMode _mode;
    private readonly Label   _displayLabel;

    // ── Constructor ───────────────────────────────────────────────────────────

    public NumericPadControl(PadMode mode = PadMode.Money)
    {
        _mode = mode;

        Dock      = DockStyle.Fill;
        Margin    = new Padding(0);
        Padding   = new Padding(0);
        BackColor = AppColors.BackgroundPrimary;

        // ── Outer layout — display (20%) + pad (80%) ─────────────────────────
        bool showDisplay = mode != PadMode.MoneyWithBills;

        var outer = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 1,
            RowCount    = 2,
            BackColor   = Color.Transparent,
            Margin      = new Padding(0),
            Padding     = new Padding(4),
        };
        outer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        outer.RowStyles.Add(new RowStyle(showDisplay ? SizeType.Percent : SizeType.Absolute,
                                         showDisplay ? 20f : 0f));
        outer.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        // ── Display ───────────────────────────────────────────────────────────
        _displayLabel = new Label
        {
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleRight,
            Font      = AppTypography.KeypadDisplay,
            ForeColor = AppColors.AccentGreen,
            BackColor = AppColors.NavyDark,
            Padding   = new Padding(0, 0, 16, 0),
            Visible   = showDisplay,
        };
        _displayLabel.Paint += (_, e) =>
        {
            using var pen = new Pen(AppColors.AccentGreen, 3);
            e.Graphics.DrawLine(pen, 8, _displayLabel.Height - 4,
                                    _displayLabel.Width - 8, _displayLabel.Height - 4);
        };

        var displayWrapper = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.NavyDark,
            Margin    = new Padding(0, 0, 0, showDisplay ? 4 : 0),
            Visible   = showDisplay,
        };
        displayWrapper.Controls.Add(_displayLabel);
        outer.Controls.Add(displayWrapper, 0, 0);

        // ── Button pad ────────────────────────────────────────────────────────
        int cols = mode == PadMode.MoneyWithBills ? 4 : 3;
        var pad  = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = cols,
            RowCount    = 4,
            BackColor   = Color.Transparent,
            Margin      = new Padding(0),
            Padding     = new Padding(0),
        };
        float pct = 100f / cols;
        for (int c = 0; c < cols; c++)
            pad.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, pct));
        for (int r = 0; r < 4; r++)
            pad.RowStyles.Add(new RowStyle(SizeType.Percent, 25));

        // Digit grid 7-8-9 / 4-5-6 / 1-2-3
        (string text, string tag, int row, int col)[] digits =
        [
            ("7","7",0,0), ("8","8",0,1), ("9","9",0,2),
            ("4","4",1,0), ("5","5",1,1), ("6","6",1,2),
            ("1","1",2,0), ("2","2",2,1), ("3","3",2,2),
        ];
        foreach (var (t, tag, row, col) in digits)
            pad.Controls.Add(Digit(t, tag), col, row);

        // Bottom row — varies by mode
        pad.Controls.Add(Digit("0", "0"), 1, 3);

        if (mode == PadMode.Integer)
        {
            pad.Controls.Add(Digit("00", "00"), 0, 3);
            pad.Controls.Add(Action("⌫", "bs", ElegantButtonStyles.WarningOrange), 2, 3);
        }
        else
        {
            pad.Controls.Add(Digit("00", "00"), 0, 3);
            pad.Controls.Add(Action("⌫  C", "*",  ElegantButtonStyles.AlertRed),  2, 3);
        }

        // Bill presets — 4th column
        if (mode == PadMode.MoneyWithBills)
        {
            (string txt, string tag)[] bills = [("$10","1000"),("$20","2000"),("$50","5000"),("$100","10000")];
            for (int i = 0; i < 4; i++)
                pad.Controls.Add(Action(bills[i].txt, bills[i].tag, ElegantButtonStyles.CashGreen), 3, i);
        }

        outer.Controls.Add(pad, 0, 1);
        Controls.Add(outer);
        RefreshDisplay();
    }

    // ── Button factories ──────────────────────────────────────────────────────

    private Button Digit(string text, string tag)
    {
        var btn = new Button { Text = text, Tag = tag, Dock = DockStyle.Fill, Margin = new Padding(3) };
        ElegantButtonStyles.Style(btn, ElegantButtonStyles.Keypad, fontSize: 30f);
        btn.Click += OnClick;
        return btn;
    }

    private Button Action(string text, string tag, Color color)
    {
        var btn = new Button { Text = text, Tag = tag, Dock = DockStyle.Fill, Margin = new Padding(3) };
        ElegantButtonStyles.Style(btn, color, fontSize: 26f);
        btn.Click += OnClick;
        return btn;
    }

    // ── Click handler ─────────────────────────────────────────────────────────

    private void OnClick(object? sender, EventArgs e)
    {
        if (sender is not Button btn) return;
        var tag = btn.Tag?.ToString() ?? "";

        if (_mode == PadMode.Integer)
        {
            switch (tag)
            {
                case "bs": if (_text.Length > 0) _text = _text[..^1]; break;
                default:   _text += tag; break;
            }
        }
        else
        {
            switch (tag)
            {
                case "*":                                                       // clear
                    _cents = 0; break;
                case "1000": case "2000": case "5000": case "10000":           // bill preset
                    if (int.TryParse(tag, out int bill)) _cents = bill; break;
                default:                                                        // digit / 00
                    if (int.TryParse(_cents.ToString() + tag, out int next)) _cents = next; break;
            }
        }

        RefreshDisplay();
        ValueChanged?.Invoke(this, EventArgs.Empty);
    }

    // ── Display ───────────────────────────────────────────────────────────────

    private void RefreshDisplay()
    {
        _displayLabel.Text = _mode == PadMode.Integer
            ? _text
            : $"$ {_cents / 100m:N2}";
    }
}
