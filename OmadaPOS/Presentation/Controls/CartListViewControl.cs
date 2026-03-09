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

    public void Configure()
    {
        _listView.View          = View.Details;
        _listView.FullRowSelect  = true;
        _listView.GridLines      = false;   // removed — alternating rows provide visual separation
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

        _listView.BackColor = AppColors.BackgroundSecondary;
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

            // Rich header — slightly elevated from panel background
            using var bgBrush = new SolidBrush(AppColors.NavyLight);
            g.FillRectangle(bgBrush, e.Bounds);

            // Emerald accent line at bottom of header
            using var accentPen = new Pen(AppColors.AccentGreen, 2f);
            g.DrawLine(accentPen, e.Bounds.Left, e.Bounds.Bottom - 2, e.Bounds.Right, e.Bounds.Bottom - 2);

            // Thin separator between columns
            using var sep = new Pen(AppBorders.SeparatorOnDark, AppBorders.Thin);
            g.DrawLine(sep, e.Bounds.Right - 1, e.Bounds.Top + 4, e.Bounds.Right - 1, e.Bounds.Bottom - 4);

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

        // ── Row backgrounds (alternating stripe) ─────────────────────────────
        _listView.DrawItem += (_, e) =>
        {
            bool selected = e.State.HasFlag(ListViewItemStates.Selected);
            Color bg = selected
                ? AppColors.Info                                          // blue selection
                : (e.ItemIndex % 2 == 0
                    ? AppColors.BackgroundSecondary                       // dark stripe A
                    : AppColors.SurfaceMuted);                            // dark stripe B

            using var brush = new SolidBrush(bg);
            e.Graphics.FillRectangle(brush, e.Bounds);
        };

        // ── Cell text ─────────────────────────────────────────────────────────
        _listView.DrawSubItem += (_, e) =>
        {
            var g = e.Graphics;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            bool  selected = e.Item!.Selected;
            Color fg       = selected ? AppColors.TextWhite : AppColors.TextPrimary;

            // Price (col 3) and Total (col 4) → right-aligned; Product (col 1) → left
            StringAlignment align = e.ColumnIndex == 1 ? StringAlignment.Near
                                  : e.ColumnIndex >= 3 ? StringAlignment.Far
                                  :                      StringAlignment.Center;

            using var sf = new StringFormat
            {
                Alignment     = align,
                LineAlignment = StringAlignment.Center,
                Trimming      = StringTrimming.EllipsisCharacter,
            };

            // Minimal horizontal padding so prices have full column width
            var textRect = new Rectangle(
                e.Bounds.X + 2,
                e.Bounds.Y,
                e.Bounds.Width - 4,
                e.Bounds.Height);

            using var brush = new SolidBrush(fg);
            g.DrawString(e.SubItem!.Text, AppTypography.ListItem, brush, textRect, sf);
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
