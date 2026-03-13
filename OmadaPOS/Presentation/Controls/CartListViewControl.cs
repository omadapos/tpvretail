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

        Dock      = DockStyle.Fill;
        Margin    = _listView.Margin;
        Padding   = new Padding(0);
        BackColor = Color.Transparent;

        _listView.Dock = DockStyle.Fill;

        // Enable double-buffering on the native ListView control.
        // WinForms exposes this only via the protected SetStyle mechanism,
        // so we reach it through reflection — the standard WinForms idiom.
        typeof(ListView)
            .GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic)
            ?.SetValue(_listView, true);

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
        _listView.View       = View.Details;
        _listView.MultiSelect = false;
        _listView.Font       = AppTypography.ListItem;

        if (_listView.Columns.Count == 0)
        {
            _listView.Columns.Add("#",       80);
            _listView.Columns.Add("Product", 200);
            _listView.Columns.Add("Qty",     70);
            _listView.Columns.Add("Price",   90);
            _listView.Columns.Add("Total",   90);
        }

        foreach (ColumnHeader col in _listView.Columns)
            col.TextAlign = HorizontalAlignment.Center;

        // Shared owner-draw: dark-navy headers, alternating rows, Consolas for cols 3 & 4
        ListViewTheme.Apply(_listView);

        AdjustColumns();
        _listView.Resize += (_, _) => AdjustColumns();
    }

    public CartListRenderResult UpdateCartItems(IReadOnlyList<CartItem> items)
    {
        decimal subTotal = 0m;
        decimal taxTotal = 0m;
        decimal total    = 0m;

        // Build all ListViewItem objects before touching the control so the
        // single BeginUpdate/EndUpdate block suppresses every intermediate repaint.
        var lviArray = new ListViewItem[items.Count];
        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            lviArray[i] = new ListViewItem(
            [
                item.Number.ToString(),
                item.ProductName,
                item.Quantity.ToString(),
                item.UnitPrice.ToString("N2"),
                item.Subtotal.ToString("N2"),
            ])
            { Tag = item.ProductId };

            subTotal += item.Subtotal;
            taxTotal += item.TaxAmount;
            total    += item.Total;
        }

        // BeginUpdate prevents any repaints until EndUpdate — eliminates flicker
        _listView.BeginUpdate();
        try
        {
            _listView.Items.Clear();
            _listView.Items.AddRange(lviArray);   // single batch insert
        }
        finally
        {
            _listView.EndUpdate();   // triggers exactly one repaint
        }

        return new CartListRenderResult
        {
            SubTotal = subTotal,
            TaxTotal = taxTotal,
            Total    = total,
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
