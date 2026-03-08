using OmadaPOS.Impresora;
using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Services;
using System.Drawing.Text;

namespace OmadaPOS.Views;

/// <summary>
/// Invoice history browser: filter by date, select an invoice to view details,
/// and reprint the receipt. Migrated from Designer to POSDialog.
/// </summary>
public sealed class frmPrintInvoice : POSDialog
{
    private readonly IOrderService  _orderService;
    private readonly IBranchService _branchService;

    private ListView     _lvInvoices = null!;
    private ListView     _lvProducts = null!;
    private DateTimePicker _dtFrom   = null!;
    private DateTimePicker _dtTo     = null!;
    private Button       _btnSearch  = null!;

    public frmPrintInvoice(IOrderService orderService, IBranchService branchService)
    {
        _orderService  = orderService;
        _branchService = branchService;

        Shown += async (_, _) => await LoadInvoicesAsync();
    }

    protected override Color      AccentColor => AppColors.SlateBlue;
    protected override string     Icon        => "🧾";
    protected override string     Title       => "Print Invoice";
    protected override string     Subtitle    => "Search and reprint past transactions";
    protected override DialogSize Size        => DialogSize.ExtraWide;
    protected override string?    ConfirmText => null;
    protected override string     CancelText  => "✕  CLOSE";

    protected override Control BuildContent()
    {
        var outer = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 1,
            RowCount    = 2,
            BackColor   = AppColors.BackgroundPrimary,
            Padding     = new Padding(0),
        };
        outer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));  // filter bar
        outer.RowStyles.Add(new RowStyle(SizeType.Percent, 100));  // lists

        // ── Filter bar ────────────────────────────────────────────────────────
        var filterBar = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 4,
            RowCount    = 1,
            BackColor   = AppColors.SurfaceMuted,
            Padding     = new Padding(12, 8, 12, 8),
            Margin      = new Padding(0),
        };
        filterBar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
        filterBar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
        filterBar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10)); // spacer
        filterBar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
        filterBar.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        _dtFrom = new DateTimePicker
        {
            Dock   = DockStyle.Fill,
            Format = DateTimePickerFormat.Short,
            Font   = AppTypography.Body,
            Margin = new Padding(0, 0, 4, 0),
        };

        _dtTo = new DateTimePicker
        {
            Dock   = DockStyle.Fill,
            Format = DateTimePickerFormat.Short,
            Font   = AppTypography.Body,
            Margin = new Padding(0, 0, 4, 0),
        };

        var spacer = new Label { Dock = DockStyle.Fill, BackColor = Color.Transparent };

        _btnSearch = new Button { Text = "🔍  SEARCH", Dock = DockStyle.Fill, Margin = new Padding(0) };
        ElegantButtonStyles.Style(_btnSearch, AppColors.SlateBlue, AppColors.TextWhite, fontSize: 13f);
        _btnSearch.Click += async (_, _) => await SearchAsync();

        filterBar.Controls.Add(_dtFrom,    0, 0);
        filterBar.Controls.Add(_dtTo,      1, 0);
        filterBar.Controls.Add(spacer,     2, 0);
        filterBar.Controls.Add(_btnSearch, 3, 0);

        // ── Two-column list area ──────────────────────────────────────────────
        var lists = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 2,
            RowCount    = 2,
            BackColor   = AppColors.BackgroundPrimary,
            Padding     = new Padding(0),
        };
        lists.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
        lists.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
        lists.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));   // column headings
        lists.RowStyles.Add(new RowStyle(SizeType.Percent, 100));   // lists

        var lblLeft  = SectionLabel("Invoices");
        var lblRight = SectionLabel("Invoice Details");

        _lvInvoices = BuildListView();
        InitInvoiceColumns();
        _lvInvoices.SelectedIndexChanged += async (_, _) => await LoadDetailsAsync();
        _lvInvoices.MouseClick += LvInvoices_MouseClick;

        _lvProducts = BuildListView();
        InitProductColumns();

        lists.Controls.Add(lblLeft,     0, 0);
        lists.Controls.Add(lblRight,    1, 0);
        lists.Controls.Add(_lvInvoices, 0, 1);
        lists.Controls.Add(_lvProducts, 1, 1);

        outer.Controls.Add(filterBar, 0, 0);
        outer.Controls.Add(lists,     0, 1);

        return outer;
    }

    // ── List initialisation ───────────────────────────────────────────────────
    private static Label SectionLabel(string text) => new()
    {
        Text      = text,
        Font      = AppTypography.RowLabel,
        ForeColor = AppColors.TextSecondary,
        BackColor = AppColors.SurfaceMuted,
        Dock      = DockStyle.Fill,
        Padding   = new Padding(12, 0, 0, 0),
        TextAlign = ContentAlignment.MiddleLeft,
    };

    private static ListView BuildListView() => new()
    {
        Dock              = DockStyle.Fill,
        View              = View.Details,
        FullRowSelect     = true,
        GridLines         = false,
        MultiSelect       = false,
        BackColor         = Color.White,
        ForeColor         = AppColors.TextPrimary,
        Font              = AppTypography.Body,
        BorderStyle       = BorderStyle.None,
        HeaderStyle       = ColumnHeaderStyle.Nonclickable,
        UseCompatibleStateImageBehavior = false,
        OwnerDraw         = true,
    };

    private void InitInvoiceColumns()
    {
        _lvInvoices.Columns.Add("#",         80,  HorizontalAlignment.Center);
        _lvInvoices.Columns.Add("Invoice",   90,  HorizontalAlignment.Center);
        _lvInvoices.Columns.Add("Amount",   100,  HorizontalAlignment.Right);
        _lvInvoices.Columns.Add("Date",     160,  HorizontalAlignment.Center);
        _lvInvoices.Columns.Add("Change",   100,  HorizontalAlignment.Right);
        _lvInvoices.Columns.Add("",          72,  HorizontalAlignment.Center); // print
        AttachOwnerDraw(_lvInvoices, printColIndex: 5);
        _lvInvoices.Resize += (_, _) => DistributeColumns(_lvInvoices);
    }

    private void InitProductColumns()
    {
        _lvProducts.Columns.Add("Id",      60, HorizontalAlignment.Center);
        _lvProducts.Columns.Add("Product", 180, HorizontalAlignment.Left);
        _lvProducts.Columns.Add("Price",   80,  HorizontalAlignment.Right);
        AttachOwnerDraw(_lvProducts);
        _lvProducts.Resize += (_, _) => DistributeColumns(_lvProducts);
    }

    // ── Owner-draw: navy header + alternating rows ────────────────────────────
    private static void AttachOwnerDraw(ListView lv, int printColIndex = -1)
    {
        lv.DrawColumnHeader += (_, e) =>
        {
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            using var bg = new SolidBrush(AppColors.NavyDark);
            e.Graphics.FillRectangle(bg, e.Bounds);

            using var tb = new SolidBrush(AppColors.TextWhite);
            using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            e.Graphics.DrawString(e.Header.Text, AppTypography.RowLabel, tb, e.Bounds, sf);
        };

        lv.DrawItem += (_, e) =>
        {
            bool isAlt = e.ItemIndex % 2 == 1;
            bool isSel = (e.State & ListViewItemStates.Selected) == ListViewItemStates.Selected;
            var  bg    = isSel ? AppColors.NavyBase : isAlt ? Color.FromArgb(247, 249, 252) : Color.White;
            using var br = new SolidBrush(bg);
            e.Graphics.FillRectangle(br, e.Bounds);
        };

        lv.DrawSubItem += (_, e) =>
        {
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            bool isSel = (e.ItemState & ListViewItemStates.Selected) != 0;

            // Print button column
            if (printColIndex >= 0 && e.ColumnIndex == printColIndex)
            {
                using var btnBrush = new SolidBrush(AppColors.AccentGreen);
                var btnRect = new Rectangle(e.Bounds.X + 4, e.Bounds.Y + 3, e.Bounds.Width - 8, e.Bounds.Height - 6);
                using var path = ElegantButtonStyles.RoundedPath(btnRect, 6);
                e.Graphics.FillPath(btnBrush, path);
                using var tw = new SolidBrush(Color.White);
                using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                e.Graphics.DrawString("🖨 Print", AppTypography.Caption, tw, btnRect, sf);
                return;
            }

            Color fg = isSel ? AppColors.TextWhite : AppColors.TextPrimary;
            using var textBrush = new SolidBrush(fg);
            using var fmt       = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
            var rect = new Rectangle(e.Bounds.X + 4, e.Bounds.Y, e.Bounds.Width - 8, e.Bounds.Height);
            e.Graphics.DrawString(e.SubItem.Text, lv.Font, textBrush, rect, fmt);
        };
    }

    private static void DistributeColumns(ListView lv)
    {
        int total = lv.ClientSize.Width;
        if (lv.Columns.Count == 0 || total <= 0) return;

        // Keep last column fixed if it's a print button
        bool hasPrintCol = lv.Columns.Count == 6;
        int  printW      = hasPrintCol ? 72 : 0;
        int  rem         = total - printW;
        int  perCol      = rem / (lv.Columns.Count - (hasPrintCol ? 1 : 0));

        for (int i = 0; i < lv.Columns.Count; i++)
        {
            if (hasPrintCol && i == lv.Columns.Count - 1)
                lv.Columns[i].Width = printW;
            else
                lv.Columns[i].Width = perCol;
        }
    }

    // ── Data loading ──────────────────────────────────────────────────────────
    private async Task LoadInvoicesAsync(string? from = null, string? to = null)
    {
        try
        {
            _btnSearch.Enabled = false;
            _lvInvoices.Items.Clear();
            Cursor = Cursors.WaitCursor;

            var list = from != null && to != null
                ? await _orderService.GetOrderTop(from, to)
                : await _orderService.GetOrderTop();

            if (list == null) return;

            foreach (var item in list)
            {
                var lvi = new ListViewItem(item.Id.ToString());
                lvi.SubItems.Add(item.Consecutivo.ToString());
                lvi.SubItems.Add(item.Order_Amount.ToString("N2"));
                lvi.SubItems.Add(item.Created_At.ToString("yyyy-MM-dd HH:mm"));
                lvi.SubItems.Add(item.Devuelta.ToString("N2"));
                lvi.SubItems.Add(""); // print button cell
                lvi.Tag = item.Id;
                _lvInvoices.Items.Add(lvi);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading invoices:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Cursor = Cursors.Default;
            _btnSearch.Enabled = true;
        }
    }

    private async Task SearchAsync()
    {
        string from = _dtFrom.Value.ToString("yyyyMMdd");
        string to   = _dtTo.Value.ToString("yyyyMMdd");
        await LoadInvoicesAsync(from, to);
    }

    private async Task LoadDetailsAsync()
    {
        if (_lvInvoices.SelectedItems.Count == 0) return;
        if (!int.TryParse(_lvInvoices.SelectedItems[0].Tag?.ToString(), out int orderId)) return;

        try
        {
            _lvProducts.Items.Clear();
            Cursor = Cursors.WaitCursor;

            var list = await _orderService.GetOrderDetailsByOrderId(orderId);
            if (list == null) return;

            foreach (var item in list)
            {
                var lvi = new ListViewItem(item.Product_Id.ToString());
                lvi.SubItems.Add(item.Product_Name);
                lvi.SubItems.Add(item.Price.ToString("N2"));
                _lvProducts.Items.Add(lvi);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading details:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    // ── Print on click of the 🖨 column ───────────────────────────────────────
    private async void LvInvoices_MouseClick(object? sender, MouseEventArgs e)
    {
        var info = _lvInvoices.HitTest(e.Location);
        if (info.Item == null || info.SubItem == null) return;
        if (info.Item.SubItems.IndexOf(info.SubItem) != 5) return; // not print column

        if (!int.TryParse(info.Item.Tag?.ToString(), out int orderId)) return;

        try
        {
            Cursor = Cursors.WaitCursor;
            var order  = await _orderService.GetOrderById(orderId);
            if (order == null) return;

            var branch  = await _branchService.LoadBranch(SessionManager.BranchId ?? 0);
            var details = await _orderService.GetOrderDetailsByOrderId(orderId);
            if (branch != null)
            {
                var ticket = new Ticket(orderId, order.Consecutivo, order, details,
                    SessionManager.Name, null, branch.Address, branch.Name);
                ticket.Print();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Print error:\n{ex.Message}", "Print Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }
}
