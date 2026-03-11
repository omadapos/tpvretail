using OmadaPOS.Impresora;
using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Services;
using OmadaPOS.Libreria.Utils;
using System.Drawing.Text;

namespace OmadaPOS.Views;

/// <summary>
/// Invoice history browser — search by date range, view item details, reprint receipts.
///
/// Invoice list columns:
///   Invoice # | Date & Time | Subtotal | Tax | Total | Payment | [🖨 Print]
///
/// Detail columns (right panel, populated on row selection):
///   Product | Qty | Unit Price | Tax | Line Total
/// </summary>
public sealed class frmPrintInvoice : POSDialog
{
    private readonly IOrderService        _orderService;
    private readonly IBranchService       _branchService;
    private readonly IAdminSettingService _adminSettingService;

    // ── Controls ──────────────────────────────────────────────────────────────
    private ListView       _lvInvoices  = null!;
    private ListView       _lvDetails   = null!;
    private DateTimePicker _dtFrom      = null!;
    private DateTimePicker _dtTo        = null!;
    private Button         _btnSearch   = null!;
    private Label          _lblSummary  = null!;  // selected-invoice summary strip
    private TextBox        _tbScan      = null!;  // barcode scan input

    // ── Column index constants ────────────────────────────────────────────────
    private const int COL_INV_NUM     = 0;
    private const int COL_INV_DATE    = 1;
    private const int COL_INV_SUB     = 2;
    private const int COL_INV_TAX     = 3;
    private const int COL_INV_TOTAL   = 4;
    private const int COL_INV_PAY     = 5;
    private const int COL_INV_PRINT   = 6;  // always last

    public frmPrintInvoice(IOrderService orderService, IBranchService branchService, IAdminSettingService adminSettingService)
    {
        _orderService        = orderService;
        _branchService       = branchService;
        _adminSettingService = adminSettingService;

        Shown += async (_, _) => await LoadInvoicesAsync();
    }

    protected override Color      AccentColor => AppColors.SlateBlue;
    protected override string     Icon        => "🧾";
    protected override string     Title       => "Invoice History";
    protected override string     Subtitle    => "Search · View Details · Reprint";
    protected override DialogSize Size        => DialogSize.ExtraWide;
    protected override string?    ConfirmText => null;
    protected override string     CancelText  => "✕  CLOSE";

    // ── Layout ────────────────────────────────────────────────────────────────
    protected override Control BuildContent()
    {
        var root = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 1,
            RowCount    = 3,
            BackColor   = AppColors.BackgroundPrimary,
            Padding     = new Padding(0),
            Margin      = new Padding(0),
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));  // filter bar
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));  // lists
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));  // summary strip

        root.Controls.Add(BuildFilterBar(), 0, 0);
        root.Controls.Add(BuildLists(),     0, 1);
        root.Controls.Add(BuildSummary(),   0, 2);

        return root;
    }

    // ── Filter bar ────────────────────────────────────────────────────────────
    private Control BuildFilterBar()
    {
        var bar = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.SurfaceMuted,
            Padding   = new Padding(14, 10, 14, 10),
        };
        bar.Paint += (_, e) =>
        {
            using var bottom = new Pen(AppColors.ShadowSubtle, 1f);
            e.Graphics.DrawLine(bottom, 0, bar.Height - 1, bar.Width, bar.Height - 1);
        };

        // Left group: From — To
        var lblFrom = FilterLabel("From:");
        _dtFrom = new DateTimePicker
        {
            Format = DateTimePickerFormat.Short,
            Font   = AppTypography.Body,
            Width  = 120,
            Value  = DateTime.Today,
        };

        var lblTo = FilterLabel("To:");
        _dtTo = new DateTimePicker
        {
            Format = DateTimePickerFormat.Short,
            Font   = AppTypography.Body,
            Width  = 120,
            Value  = DateTime.Today,
        };

        _btnSearch = new Button { Text = "🔍  SEARCH", Width = 130, Dock = DockStyle.Right };
        ElegantButtonStyles.Style(_btnSearch, AppColors.SlateBlue, AppColors.TextWhite, fontSize: 12f);
        _btnSearch.Click += async (_, _) => await SearchAsync();

        var btnToday = new Button { Text = "🗓  TODAY", Width = 110, Dock = DockStyle.Right };
        ElegantButtonStyles.Style(btnToday, AppColors.NavyBase, AppColors.TextWhite, fontSize: 12f);
        btnToday.Click += async (_, _) =>
        {
            _dtFrom.Value = DateTime.Today;
            _dtTo.Value   = DateTime.Today;
            await SearchAsync();
        };

        // ── Barcode scan input (right side, before buttons) ─────────────────
        var lblScan = FilterLabel("║▌▌▌ Scan:");
        lblScan.ForeColor = AppColors.AccentGreen;
        lblScan.Font      = new Font("Consolas", 9F, FontStyle.Bold);

        _tbScan = new TextBox
        {
            Width       = 160,
            Font        = new Font("Consolas", 12F, FontStyle.Bold),
            BackColor   = AppColors.NavyDark,
            ForeColor   = AppColors.AccentGreen,
            BorderStyle = BorderStyle.FixedSingle,
            PlaceholderText = "Scan receipt…",
            MaxLength   = 20,
        };
        _tbScan.KeyDown += async (_, e) =>
        {
            if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                await FindByBarcodeAsync(_tbScan.Text.Trim());
                _tbScan.Clear();
            }
        };

        var btnScanSearch = new Button { Text = "▶", Width = 36, Dock = DockStyle.Right };
        ElegantButtonStyles.Style(btnScanSearch, AppColors.AccentGreen, AppColors.NavyDark, fontSize: 14f);
        btnScanSearch.Click += async (_, _) =>
        {
            await FindByBarcodeAsync(_tbScan.Text.Trim());
            _tbScan.Clear();
        };

        // Arrange left-to-right manually since Panel + Dock.Left is simplest
        int x = 0;
        void Place(Control c, int gap = 6) { c.Left = x; c.Top = (bar.Height - c.Height) / 2; bar.Controls.Add(c); x += c.Width + gap; }

        Place(lblFrom,     4);
        Place(_dtFrom,    16);
        Place(lblTo,       4);
        Place(_dtTo,      20);
        Place(lblScan,     4);
        Place(_tbScan,     2);
        Place(btnScanSearch, 0);

        bar.Controls.Add(_btnSearch);
        bar.Controls.Add(btnToday);

        // Auto-focus scan box when form opens
        Shown += (_, _) => _tbScan.Focus();

        return bar;
    }

    private static Label FilterLabel(string text) => new()
    {
        Text      = text,
        Font      = new Font("Segoe UI", 9F, FontStyle.Bold),
        ForeColor = AppColors.TextSecondary,
        BackColor = Color.Transparent,
        AutoSize  = true,
    };

    // ── Two-column list area ──────────────────────────────────────────────────
    private Control BuildLists()
    {
        var grid = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 2,
            RowCount    = 2,
            BackColor   = AppColors.BackgroundPrimary,
            Padding     = new Padding(0),
            Margin      = new Padding(0),
        };
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58)); // invoices
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42)); // details
        grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 34)); // section headers
        grid.RowStyles.Add(new RowStyle(SizeType.Percent,  100)); // lists

        grid.Controls.Add(BuildSectionHeader("📋  Invoices",        AppColors.NavyDark, false), 0, 0);
        grid.Controls.Add(BuildSectionHeader("🔎  Invoice Details",  AppColors.NavyBase, true),  1, 0);

        _lvInvoices = BuildListView();
        InitInvoiceColumns();
        _lvInvoices.SelectedIndexChanged += async (_, _) => await LoadDetailsAsync();
        _lvInvoices.MouseClick           += LvInvoices_MouseClick;
        grid.Controls.Add(_lvInvoices, 0, 1);

        _lvDetails = BuildListView();
        InitDetailColumns();
        grid.Controls.Add(_lvDetails, 1, 1);

        return grid;
    }

    private static Panel BuildSectionHeader(string text, Color bg, bool rightSide)
    {
        var panel = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = bg,
        };
        if (rightSide)
        {
            panel.Paint += (_, e) =>
            {
                using var sep = new Pen(AppColors.SeparatorOnDark, 1f);
                e.Graphics.DrawLine(sep, 0, 4, 0, panel.Height - 4);
            };
        }

        var lbl = new Label
        {
            Text      = text,
            Font      = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = AppColors.TextWhite,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Padding   = new Padding(12, 0, 0, 0),
        };
        panel.Controls.Add(lbl);
        return panel;
    }

    // ── Summary strip (bottom) ────────────────────────────────────────────────
    private Control BuildSummary()
    {
        var panel = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.NavyDark,
            Padding   = new Padding(16, 0, 16, 0),
        };
        panel.Paint += (_, e) =>
        {
            using var top = new Pen(AppColors.AccentGreen, 2f);
            e.Graphics.DrawLine(top, 0, 0, panel.Width, 0);
        };

        _lblSummary = new Label
        {
            Text      = "Select an invoice to see its summary",
            Font      = new Font("Segoe UI", 9F),
            ForeColor = AppColors.TextMuted,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
        };
        panel.Controls.Add(_lblSummary);
        return panel;
    }

    // ── Column definitions ────────────────────────────────────────────────────
    private void InitInvoiceColumns()
    {
        _lvInvoices.Columns.Add("Invoice #", 88,  HorizontalAlignment.Center);
        _lvInvoices.Columns.Add("Date",      155, HorizontalAlignment.Center);
        _lvInvoices.Columns.Add("Subtotal",  90,  HorizontalAlignment.Right);
        _lvInvoices.Columns.Add("Tax",       72,  HorizontalAlignment.Right);
        _lvInvoices.Columns.Add("Total",     90,  HorizontalAlignment.Right);
        _lvInvoices.Columns.Add("Payment",   80,  HorizontalAlignment.Center);
        _lvInvoices.Columns.Add("",          64,  HorizontalAlignment.Center); // print button
        AttachOwnerDraw(_lvInvoices, printColIndex: COL_INV_PRINT);
        _lvInvoices.Resize += (_, _) => DistributeInvoiceColumns();
    }

    private void InitDetailColumns()
    {
        _lvDetails.Columns.Add("Product",    200, HorizontalAlignment.Left);
        _lvDetails.Columns.Add("Qty",         52, HorizontalAlignment.Center);
        _lvDetails.Columns.Add("Unit Price",  90, HorizontalAlignment.Right);
        _lvDetails.Columns.Add("Tax",         72, HorizontalAlignment.Right);
        _lvDetails.Columns.Add("Line Total",  90, HorizontalAlignment.Right);
        AttachOwnerDraw(_lvDetails);
        _lvDetails.Resize += (_, _) => DistributeDetailColumns();
    }

    // ── ListView factory ──────────────────────────────────────────────────────
    private static ListView BuildListView() => new()
    {
        Dock              = DockStyle.Fill,
        View              = View.Details,
        FullRowSelect     = true,
        GridLines         = false,
        MultiSelect       = false,
        BackColor         = AppColors.SurfaceCard,
        ForeColor         = AppColors.TextPrimary,
        Font              = AppTypography.Body,
        BorderStyle       = BorderStyle.None,
        HeaderStyle       = ColumnHeaderStyle.Nonclickable,
        UseCompatibleStateImageBehavior = false,
        OwnerDraw         = true,
    };

    // ── Owner-draw ────────────────────────────────────────────────────────────
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
            var  bg    = isSel ? AppColors.NavyBase
                       : isAlt ? AppColors.SurfaceMuted
                       : AppColors.SurfaceCard;
            using var br = new SolidBrush(bg);
            e.Graphics.FillRectangle(br, e.Bounds);
        };

        lv.DrawSubItem += (_, e) =>
        {
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            bool isSel = (e.ItemState & ListViewItemStates.Selected) != 0;

            // Print button cell
            if (printColIndex >= 0 && e.ColumnIndex == printColIndex)
            {
                using var btnBrush = new SolidBrush(isSel ? AppColors.AccentGreenDark : AppColors.AccentGreen);
                var btnRect = new Rectangle(e.Bounds.X + 4, e.Bounds.Y + 3, e.Bounds.Width - 8, e.Bounds.Height - 6);
                using var path = ElegantButtonStyles.RoundedPath(btnRect, 5);
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.FillPath(btnBrush, path);
                using var tw = new SolidBrush(AppColors.TextWhite);
                using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                e.Graphics.DrawString("🖨", AppTypography.Caption, tw, btnRect, sf);
                return;
            }

            // Numeric columns: right-align; others center
            var colAlign  = e.ColumnIndex < lv.Columns.Count
                            ? lv.Columns[e.ColumnIndex].TextAlign
                            : HorizontalAlignment.Left;
            var strAlign  = colAlign == HorizontalAlignment.Right  ? StringAlignment.Far
                          : colAlign == HorizontalAlignment.Center ? StringAlignment.Center
                          : StringAlignment.Near;

            Color fg = isSel ? AppColors.TextWhite : AppColors.TextPrimary;
            using var textBrush = new SolidBrush(fg);
            using var fmt       = new StringFormat
            {
                Alignment     = strAlign,
                LineAlignment = StringAlignment.Center,
                Trimming      = StringTrimming.EllipsisCharacter,
            };
            int pad = colAlign == HorizontalAlignment.Right ? 8 : 4;
            var rect = new Rectangle(e.Bounds.X + 4, e.Bounds.Y, e.Bounds.Width - pad - 4, e.Bounds.Height);
            e.Graphics.DrawString(e.SubItem.Text, lv.Font, textBrush, rect, fmt);
        };
    }

    // ── Column distribution ───────────────────────────────────────────────────
    private void DistributeInvoiceColumns()
    {
        int total  = _lvInvoices.ClientSize.Width;
        int printW = 64;
        int payW   = 80;
        int taxW   = 72;
        int fixed_ = printW + payW + taxW;
        int dateW  = 155;
        int amtW   = 90;
        // Invoice # takes remaining
        int invoiceW = Math.Max(total - fixed_ - dateW - amtW * 2, 70);

        int[] widths = { invoiceW, dateW, amtW, taxW, amtW, payW, printW };
        for (int i = 0; i < Math.Min(widths.Length, _lvInvoices.Columns.Count); i++)
            _lvInvoices.Columns[i].Width = widths[i];
    }

    private void DistributeDetailColumns()
    {
        int total  = _lvDetails.ClientSize.Width;
        int qtyW   = 52;
        int taxW   = 72;
        int priceW = 90;
        int totW   = 90;
        int nameW  = Math.Max(total - qtyW - taxW - priceW - totW, 80);

        int[] widths = { nameW, qtyW, priceW, taxW, totW };
        for (int i = 0; i < Math.Min(widths.Length, _lvDetails.Columns.Count); i++)
            _lvDetails.Columns[i].Width = widths[i];
    }

    // ── Data loading ──────────────────────────────────────────────────────────
    private async Task LoadInvoicesAsync(string? from = null, string? to = null)
    {
        try
        {
            _btnSearch.Enabled = false;
            _lvInvoices.Items.Clear();
            _lvDetails.Items.Clear();
            _lblSummary.Text = "Loading…";
            Cursor = Cursors.WaitCursor;

            var list = from != null && to != null
                ? await _orderService.GetOrderTop(from, to)
                : await _orderService.GetOrderTop();

            if (list == null || list.Count == 0)
            {
                _lblSummary.Text = "No invoices found for the selected period.";
                return;
            }

            foreach (var item in list)
            {
                var lvi = new ListViewItem(item.Consecutivo.ToString());
                lvi.SubItems.Add(item.Created_At.ToString("MM/dd/yyyy  HH:mm"));
                lvi.SubItems.Add(item.SubTotal.ToString("C"));
                lvi.SubItems.Add(item.Total_Tax_Amount.ToString("C"));
                lvi.SubItems.Add(item.Order_Amount.ToString("C"));
                lvi.SubItems.Add(FormatPayment(item.Payment_Method));
                lvi.SubItems.Add(""); // print button
                lvi.Tag = item;       // store full model for details + print
                _lvInvoices.Items.Add(lvi);
            }

            _lblSummary.Text = $"{list.Count} invoice{(list.Count == 1 ? "" : "s")} found.  Select one to view details and reprint.";
        }
        catch (Exception ex)
        {
            _lblSummary.Text = $"Error: {ex.Message}";
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

    // ── Barcode scan lookup ───────────────────────────────────────────────────
    /// <summary>
    /// Called when the cashier scans a receipt's CODE128 barcode (e.g. "00001234").
    /// 1. Tries to find the invoice in the currently loaded list.
    /// 2. If not found, expands the search to the last 90 days and tries again.
    /// 3. Selects, highlights, and loads the invoice detail panel.
    /// </summary>
    private async Task FindByBarcodeAsync(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return;

        // Strip leading zeros and parse consecutivo
        if (!int.TryParse(raw.TrimStart('0'), out int consecutivo) || consecutivo <= 0)
        {
            _lblSummary.Text = $"⚠  Invalid barcode: \"{raw}\"";
            return;
        }

        // ── Step 1: search in the already-loaded list ─────────────────────────
        if (SelectInvoiceByConsecutivo(consecutivo)) return;

        // ── Step 2: not visible — load a wider date range and retry ───────────
        _lblSummary.Text = $"Looking up Invoice #{consecutivo}…";
        Cursor = Cursors.WaitCursor;
        try
        {
            string from = DateTime.Today.AddDays(-90).ToString("yyyyMMdd");
            string to   = DateTime.Today.ToString("yyyyMMdd");
            await LoadInvoicesAsync(from, to);

            if (!SelectInvoiceByConsecutivo(consecutivo))
                _lblSummary.Text = $"⚠  Invoice #{consecutivo} not found in the last 90 days.";
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    /// <summary>
    /// Finds the row with the given consecutivo, selects it, scrolls to it,
    /// and triggers the detail load. Returns true if found.
    /// </summary>
    private bool SelectInvoiceByConsecutivo(int consecutivo)
    {
        foreach (ListViewItem lvi in _lvInvoices.Items)
        {
            if (lvi.Tag is not OrderModel order) continue;
            if (order.Consecutivo != consecutivo) continue;

            // Flash highlight and select
            _lvInvoices.SelectedItems.Clear();
            lvi.Selected = true;
            lvi.Focused  = true;
            _lvInvoices.EnsureVisible(lvi.Index);
            _lvInvoices.Focus();

            // Highlight row briefly with accent color using a quick timer
            var origColor = lvi.BackColor;
            lvi.BackColor = AppColors.AccentGreen;
            var t = new System.Windows.Forms.Timer { Interval = 450 };
            t.Tick += (_, _) =>
            {
                lvi.BackColor = origColor;
                t.Stop(); t.Dispose();
            };
            t.Start();

            _lblSummary.Text = $"✔  Invoice #{consecutivo} found — {order.Created_At:MM/dd/yyyy HH:mm}";
            return true;
        }
        return false;
    }

    private async Task LoadDetailsAsync()
    {
        if (_lvInvoices.SelectedItems.Count == 0) return;
        if (_lvInvoices.SelectedItems[0].Tag is not OrderModel order) return;

        try
        {
            _lvDetails.Items.Clear();
            Cursor = Cursors.WaitCursor;

            var list = await _orderService.GetOrderDetailsByOrderId(order.Id);
            if (list == null) return;

            double lineTotal = 0;
            foreach (var item in list)
            {
                double sub = item.Price * item.Quantity;
                lineTotal += sub + item.Tax_Amount;

                var lvi = new ListViewItem(item.Product_Name ?? "—");
                lvi.SubItems.Add(item.Quantity % 1 == 0
                    ? ((int)item.Quantity).ToString()
                    : item.Quantity.ToString("N2"));
                lvi.SubItems.Add(item.Price.ToString("C"));
                lvi.SubItems.Add(item.Tax_Amount.ToString("C"));
                lvi.SubItems.Add((sub + item.Tax_Amount).ToString("C"));
                _lvDetails.Items.Add(lvi);
            }

            // Update summary strip with full invoice breakdown
            _lblSummary.Text =
                $"Invoice #{order.Consecutivo}   ·   " +
                $"Subtotal: {order.SubTotal:C}   " +
                $"Tax: {order.Total_Tax_Amount:C}   " +
                $"Total: {order.Order_Amount:C}   " +
                $"Payment: {FormatPayment(order.Payment_Method)}   " +
                (order.Devuelta > 0 ? $"Change: {order.Devuelta:C}   " : "") +
                $"Date: {order.Created_At:MM/dd/yyyy HH:mm}";
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
        if (info.Item.SubItems.IndexOf(info.SubItem) != COL_INV_PRINT) return;
        if (info.Item.Tag is not OrderModel order) return;

        try
        {
            Cursor = Cursors.WaitCursor;
            var branch  = await _branchService.LoadBranch(SessionManager.BranchId ?? 0);
            var details = await _orderService.GetOrderDetailsByOrderId(order.Id);
            if (branch != null)
            {
                var settings = await _adminSettingService.LoadSettingById(WindowsIdProvider.GetMachineGuid());
                var rePrinter = new ReceiptPrinter(
                    order,
                    details,
                    cashier:      SessionManager.Name ?? "",
                    storeName:    branch.Name    ?? AppConstants.AppName,
                    storeAddress: branch.Address ?? "",
                    storePhone:   branch.Contact,
                    footerMsg:    branch.FooterMsg);

                string? configuredPrinter = settings?.PrinterName;
                if (!string.IsNullOrWhiteSpace(configuredPrinter))
                    rePrinter.PrintTo(configuredPrinter);
                else
                    rePrinter.Print();
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

    // ── Helper ────────────────────────────────────────────────────────────────
    private static string FormatPayment(string? method) =>
        method?.ToUpperInvariant() switch
        {
            "CASH"   => "💵 Cash",
            "CREDIT" => "💳 Credit",
            "DEBIT"  => "💳 Debit",
            "EBT"    => "🏦 EBT",
            "SPLIT"  => "⚡ Split",
            null     => "—",
            _        => method,
        };
}
