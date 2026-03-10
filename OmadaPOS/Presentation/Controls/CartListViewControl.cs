using OmadaPOS.Models;

namespace OmadaPOS.Presentation.Controls;

public sealed class CartListRenderResult
{
    public decimal SubTotal { get; init; }
    public decimal TaxTotal { get; init; }
    public decimal Total { get; init; }
}

public class CartListViewControl : UserControl
{
    private readonly ListView _listView;

    private CartListViewControl(ListView listView)
    {
        _listView = listView ?? throw new ArgumentNullException(nameof(listView));

        Dock = DockStyle.Fill;
        Margin = _listView.Margin;
        Padding = new Padding(0);
        BackColor = Color.Transparent;

        _listView.Dock = DockStyle.Fill;
        Controls.Add(_listView);
    }

    public static CartListViewControl Attach(TableLayoutPanel parent, ListView listView)
    {
        ArgumentNullException.ThrowIfNull(parent);
        ArgumentNullException.ThrowIfNull(listView);

        var pos = parent.GetPositionFromControl(listView);
        parent.Controls.Remove(listView);

        var control = new CartListViewControl(listView);
        parent.Controls.Add(control, pos.Column, pos.Row);

        return control;
    }

    // ── Static cached colors & fonts ─────────────────────────────────────────
    private static readonly Color _rowEven     = Color.FromArgb(255, 255, 255);      // white
    private static readonly Color _rowOdd      = Color.FromArgb(248, 250, 252);      // barely-off-white
    private static readonly Color _rowSelected = Color.FromArgb(16, 185, 129);       // emerald accent
    private static readonly Color _headerBg    = Color.FromArgb(30, 41, 59);         // deep navy
    private static readonly Color _rowBorder   = Color.FromArgb(20, 0, 0, 0);        // 8% black separator
    private static readonly Color _numFg       = Color.FromArgb(15, 23, 42);         // dark for numbers

    public void Configure()
    {
        _listView.View          = View.Details;
        _listView.FullRowSelect  = true;
        _listView.GridLines      = false;
        _listView.MultiSelect    = false;
        _listView.HideSelection  = false;

        if (_listView.Columns.Count == 0)
        {
            _listView.Columns.Add("#",       80);
            _listView.Columns.Add("Product", 200);
            _listView.Columns.Add("Qty",     70);
            _listView.Columns.Add("Price",   90);
            _listView.Columns.Add("Total",   90);
        }

        _listView.BackColor = _rowEven;
        _listView.ForeColor = AppColors.TextPrimary;
        _listView.Font      = AppTypography.ListItem;

        foreach (ColumnHeader col in _listView.Columns)
            col.TextAlign = HorizontalAlignment.Center;

        _listView.OwnerDraw = true;

        // ── Column headers ────────────────────────────────────────────────────
        _listView.DrawColumnHeader += (_, e) =>
        {
            var g = e.Graphics;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            using var bgBrush = new SolidBrush(_headerBg);
            g.FillRectangle(bgBrush, e.Bounds);

            // Emerald accent line at bottom
            using var accent = new Pen(AppColors.AccentGreen, 3f);
            g.DrawLine(accent, e.Bounds.Left, e.Bounds.Bottom - 3, e.Bounds.Right, e.Bounds.Bottom - 3);

            // Thin vertical column separator
            using var sep = new Pen(Color.FromArgb(40, 255, 255, 255), 1f);
            if (e.ColumnIndex > 0)
                g.DrawLine(sep, e.Bounds.Left, e.Bounds.Top + 6, e.Bounds.Left, e.Bounds.Bottom - 6);

            using var textBrush = new SolidBrush(AppColors.TextWhite);
            using var sf = new StringFormat
            {
                Alignment     = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming      = StringTrimming.EllipsisCharacter,
            };
            var textRect = new Rectangle(e.Bounds.X + 4, e.Bounds.Y, e.Bounds.Width - 8, e.Bounds.Height);
            g.DrawString(e.Header?.Text ?? string.Empty, AppTypography.ColumnHeader, textBrush, textRect, sf);
        };

        // ── Row fill — MUST fill here so sub-items inherit correct bg ─────────
        _listView.DrawItem += (_, e) =>
        {
            bool  selected = e.State.HasFlag(ListViewItemStates.Selected);
            Color bg       = selected ? _rowSelected
                           : (e.ItemIndex % 2 == 0 ? _rowEven : _rowOdd);

            using var brush = new SolidBrush(bg);
            e.Graphics.FillRectangle(brush, e.Bounds);
        };

        // ── Cell — fill background FIRST then draw text ───────────────────────
        // Without filling here Windows overpaints each cell with its default blue.
        _listView.DrawSubItem += (_, e) =>
        {
            var g = e.Graphics;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            bool  selected = e.Item!.Selected;
            Color bg       = selected ? _rowSelected
                           : (e.Item.Index % 2 == 0 ? _rowEven : _rowOdd);

            // 1. Fill the cell background (prevents system blue from showing)
            using var bgBrush = new SolidBrush(bg);
            g.FillRectangle(bgBrush, e.Bounds);

            // 2. Bottom divider — subtle hairline
            using var divPen = new Pen(_rowBorder, 1f);
            g.DrawLine(divPen, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);

            // 3. Text color — white on selection, dark on normal rows
            Color fg = selected ? Color.White : _numFg;

            // Col 1 → Product name (Segoe UI, left-aligned)
            // Col 3,4 → Price/Total (Consolas, right-aligned, slightly larger)
            // Others → centered
            Font            font  = (e.ColumnIndex == 3 || e.ColumnIndex == 4)
                                        ? AppTypography.ListItemNumber
                                        : AppTypography.ListItem;
            StringAlignment align = e.ColumnIndex == 1 ? StringAlignment.Near
                                  : e.ColumnIndex >= 3 ? StringAlignment.Far
                                  :                      StringAlignment.Center;

            using var sf = new StringFormat
            {
                Alignment     = align,
                LineAlignment = StringAlignment.Center,
                Trimming      = StringTrimming.EllipsisCharacter,
            };
            var textRect = new Rectangle(
                e.Bounds.X + 4,
                e.Bounds.Y,
                e.Bounds.Width - 8,
                e.Bounds.Height);

            using var fgBrush = new SolidBrush(fg);
            g.DrawString(e.SubItem!.Text, font, fgBrush, textRect, sf);
        };

        AdjustColumns();
        _listView.Resize += (_, _) => AdjustColumns();
    }

    public CartListRenderResult UpdateCartItems(IReadOnlyList<CartItem> items)
    {
        _listView.Items.Clear();

        decimal subTotal = 0.0m;
        decimal taxTotal = 0.0m;
        decimal total = 0.0m;

        foreach (var item in items)
        {
            var listItem = new ListViewItem(
            [
                item.Number.ToString(),
                item.ProductName,
                item.Quantity.ToString(),
                item.UnitPrice.ToString("N2"),
                item.Subtotal.ToString("N2")
            ])
            {
                Tag = item.ProductId
            };

            _listView.Items.Add(listItem);

            subTotal += item.Subtotal;
            taxTotal += item.TaxAmount;
            total += item.Total;
        }

        return new CartListRenderResult
        {
            SubTotal = subTotal,
            TaxTotal = taxTotal,
            Total = total
        };
    }

    private void AdjustColumns()
    {
        int w = _listView.ClientSize.Width;
        if (w <= 0 || _listView.Columns.Count != 5) return;

        // Fixed narrow columns for # and Qty; Price/Total get guaranteed minimums;
        // Product absorbs the remaining space.
        int numW     = Math.Max(28, (int)(w * 0.08));   // #
        int qtyW     = Math.Max(36, (int)(w * 0.10));   // Qty
        int priceW   = Math.Max(72, (int)(w * 0.20));   // Price
        int totalW   = Math.Max(72, (int)(w * 0.20));   // Total
        int productW = Math.Max(50, w - numW - qtyW - priceW - totalW); // Product

        _listView.BeginUpdate();
        _listView.Columns[0].Width = numW;
        _listView.Columns[1].Width = productW;
        _listView.Columns[2].Width = qtyW;
        _listView.Columns[3].Width = priceW;
        _listView.Columns[4].Width = totalW;
        _listView.EndUpdate();
    }
}
