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
        _listView.View = View.Details;
        _listView.FullRowSelect = true;
        _listView.GridLines = true;
        _listView.MultiSelect = false;

        if (_listView.Columns.Count == 0)
        {
            _listView.Columns.Add("#", 80);
            _listView.Columns.Add("Product", 200);
            _listView.Columns.Add("Quantity", 80);
            _listView.Columns.Add("Price", 100);
            _listView.Columns.Add("Subtotal", 100);
        }

        _listView.BackColor = AppColors.BackgroundSecondary;
        _listView.ForeColor = AppColors.TextPrimary;
        _listView.Font      = AppTypography.ListItem;

        foreach (ColumnHeader column in _listView.Columns)
            column.TextAlign = HorizontalAlignment.Center;

        _listView.OwnerDraw = true;
        _listView.DrawColumnHeader += (s, e) =>
        {
            var g = e.Graphics;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            using var bgBrush = new SolidBrush(AppColors.NavyBase);
            g.FillRectangle(bgBrush, e.Bounds);

            using var sep = new Pen(AppBorders.SeparatorOnDark, AppBorders.Thin);
            g.DrawLine(sep, e.Bounds.Right - 1, e.Bounds.Top + 4, e.Bounds.Right - 1, e.Bounds.Bottom - 4);

            var       headerFont = AppTypography.ColumnHeader;   // static shared — do NOT dispose
            using var textBrush = new SolidBrush(AppColors.TextWhite);
            using var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            };
            var textRect = new Rectangle(e.Bounds.X + 4, e.Bounds.Y, e.Bounds.Width - 8, e.Bounds.Height);
            g.DrawString(e.Header?.Text ?? string.Empty, headerFont, textBrush, textRect, sf);
        };
        _listView.DrawItem += (s, e) => e.DrawDefault = true;
        _listView.DrawSubItem += (s, e) => e.DrawDefault = true;

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
        int totalWidth = _listView.ClientSize.Width;
        int columnCount = _listView.Columns.Count;
        if (columnCount == 0) return;

        int columnWidth = totalWidth / columnCount;
        int lastColumnWidth = totalWidth - (columnWidth * (columnCount - 1));

        for (int i = 0; i < columnCount; i++)
            _listView.Columns[i].Width = (i == columnCount - 1) ? lastColumnWidth : columnWidth;
    }
}
