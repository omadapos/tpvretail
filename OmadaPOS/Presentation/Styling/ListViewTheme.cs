using System.Drawing.Text;

namespace OmadaPOS.Presentation.Styling;

/// <summary>
/// Shared owner-draw theme for all ListView controls in the POS.
/// Applies dark-navy column headers with emerald accent line,
/// alternating white/near-white rows, hairline row dividers,
/// and Consolas for price/amount columns.
///
/// Replaces three near-identical copies that existed in:
///   CartListViewControl, frmSplit.AttachOwnerDraw, frmCustomerScreen.AttachListViewDraw
/// </summary>
public static class ListViewTheme
{
    // ── Shared palette ────────────────────────────────────────────────────────
    public static readonly Color RowEven     = Color.FromArgb(255, 255, 255);
    public static readonly Color RowOdd      = Color.FromArgb(248, 250, 252);
    public static readonly Color RowSelected = AppColors.AccentGreen;
    public static readonly Color HeaderBg    = Color.FromArgb(30, 41, 59);
    public static readonly Color RowBorder   = Color.FromArgb(20, 0, 0, 0);
    public static readonly Color TextDark    = Color.FromArgb(15, 23, 42);

    // ── Cached StringFormat instances ─────────────────────────────────────────
    private static readonly StringFormat _sfCenter = new() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
    private static readonly StringFormat _sfNear   = new() { Alignment = StringAlignment.Near,   LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
    private static readonly StringFormat _sfFar    = new() { Alignment = StringAlignment.Far,    LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };

    /// <summary>
    /// Applies the standard POS owner-draw theme to <paramref name="lv"/>.
    /// </summary>
    /// <param name="lv">The ListView to decorate.</param>
    /// <param name="numericColumns">
    /// Column indices that should use <c>Consolas</c> (right-aligned numeric values).
    /// Defaults to columns 3 and 4 (Price and Total in a standard 5-column cart).
    /// </param>
    public static void Apply(ListView lv, int[]? numericColumns = null)
    {
        numericColumns ??= [3, 4];

        lv.OwnerDraw    = true;
        lv.BackColor    = RowEven;
        lv.ForeColor    = TextDark;
        lv.BorderStyle  = BorderStyle.None;
        lv.GridLines    = false;
        lv.FullRowSelect = true;
        lv.HideSelection = false;

        // ── Column headers ────────────────────────────────────────────────────
        lv.DrawColumnHeader += (_, e) =>
        {
            var g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            using var bgBrush = new SolidBrush(HeaderBg);
            g.FillRectangle(bgBrush, e.Bounds);

            using var accent = new Pen(AppColors.AccentGreen, 3f);
            g.DrawLine(accent, e.Bounds.Left, e.Bounds.Bottom - 3,
                                e.Bounds.Right, e.Bounds.Bottom - 3);

            if (e.ColumnIndex > 0)
            {
                using var sep = new Pen(Color.FromArgb(35, 255, 255, 255), 1f);
                g.DrawLine(sep, e.Bounds.Left, e.Bounds.Top + 6,
                                e.Bounds.Left, e.Bounds.Bottom - 6);
            }

            using var textBrush = new SolidBrush(AppColors.TextWhite);
            var textRect = new Rectangle(e.Bounds.X + 4, e.Bounds.Y,
                                         e.Bounds.Width - 8, e.Bounds.Height);
            g.DrawString(e.Header?.Text ?? string.Empty,
                         AppTypography.ColumnHeader, textBrush, textRect, _sfCenter);
        };

        // ── Row background ────────────────────────────────────────────────────
        lv.DrawItem += (_, e) =>
        {
            bool selected = e.State.HasFlag(ListViewItemStates.Selected);
            var bg = selected ? RowSelected : (e.ItemIndex % 2 == 0 ? RowEven : RowOdd);
            using var brush = new SolidBrush(bg);
            e.Graphics.FillRectangle(brush, e.Bounds);
        };

        // ── Cell ──────────────────────────────────────────────────────────────
        lv.DrawSubItem += (_, e) =>
        {
            var g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            bool selected = e.Item!.Selected;
            var bg = selected ? RowSelected : (e.Item.Index % 2 == 0 ? RowEven : RowOdd);

            using var bgBrush = new SolidBrush(bg);
            g.FillRectangle(bgBrush, e.Bounds);

            using var divPen = new Pen(RowBorder, 1f);
            g.DrawLine(divPen, e.Bounds.Left, e.Bounds.Bottom - 1,
                               e.Bounds.Right, e.Bounds.Bottom - 1);

            bool isNumeric = Array.IndexOf(numericColumns, e.ColumnIndex) >= 0;
            Font font  = isNumeric ? AppTypography.ListItemNumber : AppTypography.ListItem;
            var  sf    = isNumeric ? _sfFar
                       : e.ColumnIndex == 1 ? _sfNear
                       : _sfCenter;

            Color fg = selected ? AppColors.TextWhite : TextDark;
            using var fgBrush = new SolidBrush(fg);

            var textRect = new Rectangle(e.Bounds.X + 5, e.Bounds.Y,
                                         e.Bounds.Width - 8, e.Bounds.Height);
            g.DrawString(e.SubItem!.Text, font, fgBrush, textRect, sf);
        };
    }
}
