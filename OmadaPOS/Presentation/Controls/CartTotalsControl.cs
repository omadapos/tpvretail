using OmadaPOS.Componentes;
using OmadaPOS.Presentation.Styling;

namespace OmadaPOS.Presentation.Controls;

/// <summary>
/// Self-contained totals card for the POS cart column.
/// Owns its own RoundedPanel, TableLayoutPanel, and all labels.
/// Call <see cref="Attach"/> to insert it into a parent grid; then wire
/// <see cref="UpdateTotals"/> / <see cref="ResetTotals"/> as needed.
/// </summary>
public sealed class CartTotalsControl : UserControl
{
    // ── Value labels (updated at runtime) ────────────────────────────────────
    private readonly Label _lblSubtotalValue;
    private readonly Label _lblTaxValue;
    private readonly Label _lblTotalValue;

    // ── Colors (dark-chrome card) ─────────────────────────────────────────────
    private static readonly Color _bg       = Color.FromArgb(15,  23,  42);  // navy dark
    private static readonly Color _separator = Color.FromArgb(55, 255, 255, 255);

    // ── Constructor ───────────────────────────────────────────────────────────
    private CartTotalsControl()
    {
        DoubleBuffered = true;
        Dock           = DockStyle.Fill;
        Margin         = new Padding(6, 4, 6, 4);
        Padding        = new Padding(0);
        BackColor      = Color.Transparent;

        // ── Dark rounded card ─────────────────────────────────────────────────
        var card = new RoundedPanel
        {
            Dock            = DockStyle.Fill,
            BackgroundStart = AppColors.NavyDark,
            BackgroundEnd   = AppColors.NavyBase,
            BorderColor     = AppBorders.AccentLine,
            CornerRadius    = AppRadii.Panel,
            Padding         = new Padding(0),
        };

        // ── Outer table: Subtotal | Tax | ── separator ── | Total ────────────
        //   Row 0: Subtotal row  (fixed 38px)
        //   Row 1: Tax row       (fixed 34px)
        //   Row 2: Total hero    (fills remaining — ample room for 26pt Consolas)
        var tbl = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 2,
            RowCount    = 3,
            BackColor   = Color.Transparent,
            Padding     = new Padding(18, 10, 18, 6),
            Margin      = new Padding(0),
        };
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));   // subtotal
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));   // tax
        tbl.RowStyles.Add(new RowStyle(SizeType.Percent,  100));  // total — fills rest

        // Subtotal row
        tbl.Controls.Add(Label("Subtotal", AppColors.TextOnDarkMuted,      AppTypography.RowLabel,  ContentAlignment.MiddleLeft),  0, 0);
        _lblSubtotalValue = Label("$0.00",  AppColors.TextOnDarkSecondary,  AppTypography.AmountMono, ContentAlignment.MiddleRight);
        tbl.Controls.Add(_lblSubtotalValue, 1, 0);

        // Tax row
        tbl.Controls.Add(Label("Tax",    AppColors.TextOnDarkMuted,  AppTypography.RowLabel,  ContentAlignment.MiddleLeft),  0, 1);
        _lblTaxValue = Label("$0.00",    AppColors.TextOnDarkMuted,  AppTypography.AmountMono, ContentAlignment.MiddleRight);
        tbl.Controls.Add(_lblTaxValue, 1, 1);

        // Total hero — spans both columns
        var totalRow = BuildTotalRow(out _lblTotalValue);
        tbl.Controls.Add(totalRow, 0, 2);
        tbl.SetColumnSpan(totalRow, 2);

        card.Controls.Add(tbl);
        Controls.Add(card);
    }

    // ── Total hero row ────────────────────────────────────────────────────────
    private static Panel BuildTotalRow(out Label valueLabel)
    {
        var panel = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = Color.Transparent,
            Padding   = new Padding(0),
            Margin    = new Padding(0),
        };

        // Hairline separator at the top
        panel.Paint += (_, e) =>
        {
            using var pen = new Pen(_separator, 1f);
            e.Graphics.DrawLine(pen, 0, 2, panel.Width, 2);
        };

        var inner = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 2,
            RowCount    = 1,
            BackColor   = Color.Transparent,
            Padding     = new Padding(0, 6, 0, 4),
            Margin      = new Padding(0),
        };
        inner.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38));
        inner.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62));
        inner.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var lblCaption = Label("TOTAL", AppColors.TextWhite,   AppTypography.SectionTitle, ContentAlignment.MiddleLeft);
        valueLabel     = Label("$0.00", AppColors.AccentGreen, AppTypography.AmountGrand,  ContentAlignment.MiddleRight);

        inner.Controls.Add(lblCaption, 0, 0);
        inner.Controls.Add(valueLabel, 1, 0);

        panel.Controls.Add(inner);
        return panel;
    }

    // ── Label factory ─────────────────────────────────────────────────────────
    private static Label Label(string text, Color fg, Font font, ContentAlignment align) =>
        new()
        {
            Text      = text,
            ForeColor = fg,
            Font      = font,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Fill,
            TextAlign = align,
            AutoSize  = false,
        };

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Creates a <see cref="CartTotalsControl"/> and inserts it into
    /// <paramref name="parent"/> at the specified grid cell.
    /// The old control at that position (if any) is removed first.
    /// </summary>
    public static CartTotalsControl Attach(TableLayoutPanel parent, int column, int row)
    {
        ArgumentNullException.ThrowIfNull(parent);

        // Remove whatever was at this cell (old designer placeholder)
        var existing = parent.GetControlFromPosition(column, row);
        if (existing != null)
            parent.Controls.Remove(existing);

        var ctrl = new CartTotalsControl();
        parent.Controls.Add(ctrl, column, row);
        return ctrl;
    }

    /// <summary>Updates all three displayed amounts.</summary>
    public void UpdateTotals(decimal subtotal, decimal tax, decimal total)
    {
        _lblSubtotalValue.Text = subtotal.ToString("C");
        _lblTaxValue.Text      = tax.ToString("C");
        _lblTotalValue.Text    = total.ToString("C");
    }

    /// <summary>Resets displayed amounts to zero.</summary>
    public void ResetTotals(bool includeGrandTotal)
    {
        _lblSubtotalValue.Text = "$0.00";
        _lblTaxValue.Text      = "$0.00";
        if (includeGrandTotal)
            _lblTotalValue.Text = "$0.00";
    }

    /// <summary>No-op kept for call-site compatibility; theme is baked in.</summary>
    public void ApplyTheme() { }
}
