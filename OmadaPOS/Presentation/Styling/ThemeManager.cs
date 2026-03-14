using System.Drawing;
using System.Windows.Forms;

namespace OmadaPOS.Presentation.Styling;

/// <summary>
/// Centralized WinForms theme applicator for native controls that cannot be owner-drawn.
/// Complements the existing <see cref="ElegantButtonStyles"/> (for buttons) and
/// <see cref="ListViewTheme"/> (for ListViews) by providing theming for
/// TextBox, ComboBox, DateTimePicker, DataGridView, and Form itself.
///
/// Usage:
///   // In Form constructor or Load event:
///   ThemeManager.ApplyAll(this);
///
///   // Or target individual controls:
///   ThemeManager.ApplyInput(myTextBox);
///   ThemeManager.ApplyDataGridView(myGrid);
/// </summary>
public static class ThemeManager
{
    // ── Standard dimensions ───────────────────────────────────────────────────

    /// <summary>Standard height for TextBox / ComboBox inputs.</summary>
    public const int InputHeight = 32;

    /// <summary>Minimum height for normal (non-touch) buttons.</summary>
    public const int ButtonHeight = 36;

    /// <summary>Height for POS touch-zone buttons.</summary>
    public const int TouchHeight = 60;

    /// <summary>Standard DataGridView row height.</summary>
    public const int GridRowHeight = 32;

    // ── Input controls ────────────────────────────────────────────────────────

    /// <summary>
    /// Applies the standard POS theme to a <see cref="TextBox"/>.
    /// </summary>
    public static void ApplyInput(TextBox tb)
    {
        tb.BackColor    = AppColors.SurfaceCard;
        tb.ForeColor    = AppColors.TextPrimary;
        tb.Font         = AppTypography.Body;
        tb.BorderStyle  = BorderStyle.FixedSingle;

        tb.EnabledChanged += (s, _) =>
        {
            if (s is not TextBox t) return;
            t.BackColor = t.Enabled ? AppColors.SurfaceCard : AppColors.DisabledBackground;
            t.ForeColor = t.Enabled ? AppColors.TextPrimary : AppColors.DisabledText;
        };
    }

    /// <summary>
    /// Applies the standard POS theme to a <see cref="ComboBox"/>.
    /// </summary>
    public static void ApplyInput(ComboBox cb)
    {
        cb.FlatStyle = FlatStyle.Flat;
        cb.BackColor = AppColors.SurfaceCard;
        cb.ForeColor = AppColors.TextPrimary;
        cb.Font      = AppTypography.Body;

        cb.EnabledChanged += (s, _) =>
        {
            if (s is not ComboBox c) return;
            c.BackColor = c.Enabled ? AppColors.SurfaceCard : AppColors.DisabledBackground;
            c.ForeColor = c.Enabled ? AppColors.TextPrimary : AppColors.DisabledText;
        };
    }

    /// <summary>
    /// Applies the standard POS theme to a <see cref="DateTimePicker"/>.
    /// </summary>
    public static void ApplyInput(DateTimePicker dtp)
    {
        dtp.CalendarMonthBackground = AppColors.SurfaceCard;
        dtp.CalendarForeColor       = AppColors.TextPrimary;
        dtp.CalendarTitleBackColor  = AppColors.HeaderPrimary;
        dtp.CalendarTitleForeColor  = AppColors.TextWhite;
        dtp.Font                    = AppTypography.Body;
    }

    // ── DataGridView ──────────────────────────────────────────────────────────

    /// <summary>
    /// Applies the full standard POS theme to a <see cref="DataGridView"/>.
    /// Header: dark navy. Selection: light blue. Alternating rows. No visual styles.
    /// </summary>
    public static void ApplyDataGridView(DataGridView dgv)
    {
        dgv.EnableHeadersVisualStyles = false;
        dgv.BorderStyle               = BorderStyle.None;
        dgv.CellBorderStyle           = DataGridViewCellBorderStyle.SingleHorizontal;
        dgv.RowHeadersVisible         = false;
        dgv.AllowUserToResizeRows     = false;
        dgv.GridColor                 = AppColors.ListViewGridLine;
        dgv.BackgroundColor           = AppColors.SurfaceCard;

        // Column headers
        dgv.ColumnHeadersDefaultCellStyle.BackColor  = AppColors.NavyBase;
        dgv.ColumnHeadersDefaultCellStyle.ForeColor  = AppColors.TextWhite;
        dgv.ColumnHeadersDefaultCellStyle.Font       = AppTypography.ColumnHeader;
        dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = AppColors.NavyBase;
        dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = AppColors.TextWhite;
        dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        dgv.ColumnHeadersHeight         = 34;

        // Row defaults
        dgv.DefaultCellStyle.Font            = AppTypography.Body;
        dgv.DefaultCellStyle.BackColor        = AppColors.SurfaceCard;
        dgv.DefaultCellStyle.ForeColor        = AppColors.TextPrimary;
        dgv.DefaultCellStyle.SelectionBackColor = AppColors.ListViewSelection;
        dgv.DefaultCellStyle.SelectionForeColor = AppColors.ListViewSelectionText;
        dgv.DefaultCellStyle.Padding          = new Padding(4, 0, 4, 0);

        // Alternating rows
        dgv.AlternatingRowsDefaultCellStyle.BackColor        = AppColors.BackgroundPrimary;
        dgv.AlternatingRowsDefaultCellStyle.ForeColor        = AppColors.TextPrimary;
        dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = AppColors.ListViewSelection;
        dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor = AppColors.ListViewSelectionText;

        // Row height
        dgv.RowTemplate.Height = GridRowHeight;

        // Selection mode
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
    }

    // ── Form ──────────────────────────────────────────────────────────────────

    /// <summary>
    /// Sets the form background to the standard canvas color.
    /// </summary>
    public static void ApplyForm(Form f)
    {
        f.BackColor = AppColors.BackgroundPrimary;
    }

    // ── Disabled state ────────────────────────────────────────────────────────

    /// <summary>
    /// Forces disabled visual appearance on any control.
    /// </summary>
    public static void ApplyDisabled(Control c)
    {
        c.BackColor = AppColors.DisabledBackground;
        c.ForeColor = AppColors.DisabledText;
    }

    // ── Recursive "apply to all" ──────────────────────────────────────────────

    /// <summary>
    /// Recursively walks all controls under <paramref name="root"/> and applies
    /// theme helpers by control type. Safe to call from Form constructor or Load event.
    /// Buttons are intentionally skipped (handled by <see cref="ElegantButtonStyles"/>).
    /// ListViews are intentionally skipped (handled by <see cref="ListViewTheme"/>).
    /// </summary>
    public static void ApplyAll(Control root)
    {
        if (root is Form f)
            ApplyForm(f);

        foreach (Control c in root.Controls)
        {
            switch (c)
            {
                case TextBox tb when tb.Name != "_txtScan" && !tb.Multiline:
                    ApplyInput(tb);
                    break;
                case TextBox tb when tb.Multiline:
                    tb.BackColor   = AppColors.SurfaceCard;
                    tb.ForeColor   = AppColors.TextPrimary;
                    tb.Font        = AppTypography.Body;
                    tb.BorderStyle = BorderStyle.FixedSingle;
                    break;
                case ComboBox cb:
                    ApplyInput(cb);
                    break;
                case DateTimePicker dtp:
                    ApplyInput(dtp);
                    break;
                case DataGridView dgv:
                    ApplyDataGridView(dgv);
                    break;
                case TabPage tp:
                    tp.BackColor = AppColors.BackgroundPrimary;
                    tp.Font      = AppTypography.Body;
                    ApplyAll(tp);
                    continue;
                case TabControl tc:
                    tc.Font = AppTypography.Body;
                    ApplyAll(tc);
                    continue;
                case Panel panel:
                    // Recurse into panels but don't override their BackColor —
                    // panels are styled individually by each form.
                    ApplyAll(panel);
                    continue;
                case GroupBox gb:
                    gb.ForeColor = AppColors.TextPrimary;
                    gb.Font      = AppTypography.Body;
                    ApplyAll(gb);
                    continue;
                case Label lbl:
                    if (lbl.ForeColor == SystemColors.ControlText)
                        lbl.ForeColor = AppColors.TextPrimary;
                    if (lbl.Font.Size <= 9f && lbl.Font.Name == "Microsoft Sans Serif")
                        lbl.Font = AppTypography.Body;
                    break;
            }

            if (c.Controls.Count > 0 && c is not DataGridView && c is not ListView)
                ApplyAll(c);
        }
    }
}
