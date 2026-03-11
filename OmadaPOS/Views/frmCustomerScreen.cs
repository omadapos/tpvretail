using OmadaPOS.Libreria.Services;
using OmadaPOS.Services;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace OmadaPOS.Views;

/// <summary>
/// Customer-facing secondary display — optimized for 1024×768 LED screen (4:3).
///
/// Lifecycle contract:
///   • Opened by frmHome.Load (after cashier logs in), positioned on the secondary screen.
///   • Closed by frmHome.FormClosed — never by the customer.
///   • User-initiated close attempts (Alt+F4, etc.) are silently blocked.
///   • Registered as Singleton in DI — always the same instance per app session.
///
/// Layout (1024 × 768):
///   ┌────────────────────────────────────────────┐  64px  Header
///   ├──────────────────────┬─────────────────────┤
///   │  🛒 YOUR ORDER       │   BANNER IMAGE       │  fill  Body
///   │  ListView (55%)      │   Carousel  (45%)    │
///   ├──────────────────────┴─────────────────────┤
///   │  Items: 5    Subtotal: $52.30  Tax: $3.66  │  180px Footer
///   │  ⚖ 0.0 lb                   TOTAL $55.96  │
///   └────────────────────────────────────────────┘
/// </summary>
public sealed class frmCustomerScreen : Form
{
    // ── Services ──────────────────────────────────────────────────────────────
    private readonly IBannerService _bannerService;
    private readonly IShoppingCart  _shoppingCart;

    // ── State ─────────────────────────────────────────────────────────────────
    private string[] _bannerUrls  = [];
    private int      _bannerIndex = 0;
    private decimal  _subtotal    = 0;
    private decimal  _taxTotal    = 0;
    private decimal  _grandTotal  = 0;
    private double   _itemCount   = 0;

    // ── Controls ──────────────────────────────────────────────────────────────
    private ListView   _lvCart    = null!;
    private PictureBox _pbBanner  = null!;
    private Label      _lblTotal  = null!;
    private Label      _lblSubTax = null!;
    private Label      _lblItems  = null!;
    private Label      _lblWeight = null!;
    private Label      _lblClock  = null!;

    // ── Timers ────────────────────────────────────────────────────────────────
    private System.Windows.Forms.Timer? _timerBanner;
    private System.Windows.Forms.Timer? _timerClock;

    // ── Static GDI resources ─────────────────────────────────────────────────
    private static readonly Font _fontHero       = new("Consolas",  52F, FontStyle.Bold);  // grand total
    private static readonly Font _fontMeta       = new("Segoe UI",  13F, FontStyle.Bold);  // "TOTAL" caption
    private static readonly Font _fontItems      = new("Segoe UI",  16F, FontStyle.Bold);  // "Items: N"
    private static readonly Font _fontDetail     = new("Segoe UI",  12F);                  // subtotal/tax line
    private static readonly Font _fontHeader     = new("Segoe UI",  15F, FontStyle.Bold);  // store name
    private static readonly Font _fontClock      = new("Consolas",  13F);                  // clock
    private static readonly Font _fontList       = new("Segoe UI",  15F);                  // list rows — readable at distance
    private static readonly Font _fontListHdr    = new("Segoe UI",  12F, FontStyle.Bold);  // column headers

    // ── Constructor ───────────────────────────────────────────────────────────
    public frmCustomerScreen(IBannerService bannerService, IShoppingCart shoppingCart)
    {
        DoubleBuffered = true;
        _bannerService = bannerService;
        _shoppingCart  = shoppingCart;

        InitForm();

        _shoppingCart.CartChanged    += OnCartChanged;
        SharedData.WeightUnitChanged += OnWeightChanged;

        Load += async (_, _) =>
        {
            await LoadBannersAsync();
            StartTimers();
            RefreshCart();
        };
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Positions this form on the secondary (customer) monitor.
    /// Falls back to a 1024×768 window if only one screen is connected.
    /// </summary>
    public void PositionOnSecondaryScreen()
    {
        var secondary = Screen.AllScreens.FirstOrDefault(s => !s.Primary);
        if (secondary != null)
        {
            StartPosition = FormStartPosition.Manual;
            Bounds        = secondary.Bounds;
            TopMost       = true;
        }
        else
        {
            // Single-monitor — show as a resizable test window
            StartPosition = FormStartPosition.Manual;
            Size          = new Size(1024, 768);
            Location      = new Point(Screen.PrimaryScreen!.WorkingArea.Right - 1040, 20);
            TopMost       = false;
        }
    }

    // ── Form construction ─────────────────────────────────────────────────────
    private void InitForm()
    {
        FormBorderStyle = FormBorderStyle.None;
        BackColor       = AppColors.NavyDark;
        Text            = "Customer Display";

        // Block customer-initiated close (Alt+F4). Programmatic Close() uses
        // CloseReason.None and passes through normally.
        FormClosing += (_, e) =>
        {
            if (e.CloseReason == CloseReason.UserClosing)
                e.Cancel = true;
        };

        var root = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 1,
            RowCount    = 3,
            BackColor   = Color.Transparent,
            Padding     = new Padding(0),
            Margin      = new Padding(0),
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 76));   // header — taller, more presence
        root.RowStyles.Add(new RowStyle(SizeType.Percent,  100)); // body
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 190)); // footer — extra space for big total

        root.Controls.Add(BuildHeader(), 0, 0);
        root.Controls.Add(BuildBody(),   0, 1);
        root.Controls.Add(BuildFooter(), 0, 2);

        Controls.Add(root);
    }

    // ── Header (64px) ─────────────────────────────────────────────────────────
    private Panel BuildHeader()
    {
        var header = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.NavyDark,
            Padding   = new Padding(0),
        };
        // Emerald accent line at the bottom
        header.Paint += (_, e) =>
        {
            using var accent = new SolidBrush(AppColors.AccentGreen);
            e.Graphics.FillRectangle(accent, 0, header.Height - 3, header.Width, 3);
        };

        var lblStore = new Label
        {
            Text      = "🏪  DAILY STOP",
            Font      = _fontHeader,
            ForeColor = AppColors.TextWhite,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Left,
            Width     = 260,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding   = new Padding(18, 0, 0, 0),
        };

        var lblWelcome = new Label
        {
            Text      = "✦  WELCOME  ✦",
            Font      = new Font("Segoe UI", 12F, FontStyle.Bold),
            ForeColor = AppColors.AccentGreen,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
        };

        _lblClock = new Label
        {
            Text      = DateTime.Now.ToString("ddd, MMM dd  ·  HH:mm:ss"),
            Font      = _fontClock,
            ForeColor = AppColors.TextMuted,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Right,
            Width     = 280,
            TextAlign = ContentAlignment.MiddleRight,
            Padding   = new Padding(0, 0, 18, 0),
        };

        header.Controls.Add(lblWelcome);   // fill — must be added first so Dock.Fill works
        header.Controls.Add(_lblClock);
        header.Controls.Add(lblStore);

        return header;
    }

    // ── Body (fill — 2 columns) ───────────────────────────────────────────────
    private Control BuildBody()
    {
        var body = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 2,
            RowCount    = 1,
            BackColor   = Color.Transparent,
            Padding     = new Padding(0),
            Margin      = new Padding(0),
        };
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62)); // cart — more room for items
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38)); // banner
        body.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        body.Controls.Add(BuildCartPanel(),   0, 0);
        body.Controls.Add(BuildBannerPanel(), 1, 0);

        return body;
    }

    // ── Cart panel ─────────────────────────────────────────────────────────────
    private Control BuildCartPanel()
    {
        var panel = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = Color.FromArgb(248, 250, 252),
            Padding   = new Padding(0),
            Margin    = new Padding(0),
        };

        // Sub-header "YOUR ORDER" — prominent, high-contrast
        var subHeader = new Panel
        {
            Dock      = DockStyle.Top,
            Height    = 48,
            BackColor = AppColors.NavyBase,
        };
        // Emerald bottom accent on sub-header
        subHeader.Paint += (_, e) =>
        {
            using var pen = new Pen(AppColors.AccentGreen, 2f);
            e.Graphics.DrawLine(pen, 0, subHeader.Height - 2, subHeader.Width, subHeader.Height - 2);
        };
        var lblTitle = new Label
        {
            Text      = "YOUR ORDER",
            Font      = new Font("Segoe UI", 13F, FontStyle.Bold),
            ForeColor = AppColors.TextWhite,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Padding   = new Padding(0, 0, 0, 2),
        };
        subHeader.Controls.Add(lblTitle);

        // Cart list — larger font for customer-facing 1024×768 screen
        _lvCart = new ListView
        {
            Dock              = DockStyle.Fill,
            View              = View.Details,
            FullRowSelect     = true,
            GridLines         = false,
            MultiSelect       = false,
            HideSelection     = true,
            BackColor         = Color.FromArgb(255, 255, 255),
            ForeColor         = AppColors.TextPrimary,
            Font              = _fontList,
            BorderStyle       = BorderStyle.None,
            HeaderStyle       = ColumnHeaderStyle.Nonclickable,
            OwnerDraw         = true,
            UseCompatibleStateImageBehavior = false,
        };
        _lvCart.Columns.Add("#",       44,  HorizontalAlignment.Center);
        _lvCart.Columns.Add("Product", 220, HorizontalAlignment.Left);
        _lvCart.Columns.Add("Qty",     52,  HorizontalAlignment.Center);
        _lvCart.Columns.Add("Price",   100, HorizontalAlignment.Right);
        _lvCart.Columns.Add("Total",   100, HorizontalAlignment.Right);
        AttachListViewDraw(_lvCart);
        _lvCart.Resize += (_, _) => FillProductColumn(_lvCart, fillIdx: 1);

        panel.Controls.Add(_lvCart);
        panel.Controls.Add(subHeader);

        return panel;
    }

    // ── Banner panel ──────────────────────────────────────────────────────────
    private Panel BuildBannerPanel()
    {
        var panel = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.NavyDark,
            Padding   = new Padding(0),
            Margin    = new Padding(0),
        };

        _pbBanner = new PictureBox
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.NavyDark,
            SizeMode  = PictureBoxSizeMode.Zoom,
        };

        panel.Controls.Add(_pbBanner);
        return panel;
    }

    // ── Footer (180px) ────────────────────────────────────────────────────────
    private Control BuildFooter()
    {
        var footer = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.NavyBase,
            Padding   = new Padding(0),
            Margin    = new Padding(0),
        };
        // Emerald accent line at the top
        footer.Paint += (_, e) =>
        {
            using var accent = new SolidBrush(AppColors.AccentGreen);
            e.Graphics.FillRectangle(accent, 0, 0, footer.Width, 4);
        };

        var grid = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 2,
            RowCount    = 1,
            BackColor   = Color.Transparent,
            Padding     = new Padding(0),
            Margin      = new Padding(0),
        };
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52)); // details
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48)); // total
        grid.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        grid.Controls.Add(BuildFooterDetails(), 0, 0);
        grid.Controls.Add(BuildFooterTotal(),   1, 0);

        footer.Controls.Add(grid);
        return footer;
    }

    private Control BuildFooterDetails()
    {
        var panel = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 1,
            RowCount    = 3,
            BackColor   = Color.Transparent,
            Padding     = new Padding(24, 16, 12, 12),
            Margin      = new Padding(0),
        };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 38)); // items count
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 32)); // subtotal + tax
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 30)); // weight

        _lblItems = new Label
        {
            AutoSize  = false,
            Dock      = DockStyle.Fill,
            Text      = "Items: 0",
            Font      = _fontItems,
            ForeColor = AppColors.AccentGreen,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleLeft,
        };

        _lblSubTax = new Label
        {
            AutoSize  = false,
            Dock      = DockStyle.Fill,
            Text      = "Subtotal: $0.00   ·   Tax: $0.00",
            Font      = _fontDetail,
            ForeColor = Color.FromArgb(148, 163, 184),
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleLeft,
        };

        _lblWeight = new Label
        {
            AutoSize  = false,
            Dock      = DockStyle.Fill,
            Text      = $"⚖  {SharedData.WeightUnit}",
            Font      = _fontDetail,
            ForeColor = AppColors.Warning,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleLeft,
        };

        panel.Controls.Add(_lblItems,  0, 0);
        panel.Controls.Add(_lblSubTax, 0, 1);
        panel.Controls.Add(_lblWeight, 0, 2);

        return panel;
    }

    private Control BuildFooterTotal()
    {
        var panel = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = Color.Transparent,
            Padding   = new Padding(16, 10, 24, 10),
        };
        // Subtle vertical separator on the left
        panel.Paint += (_, e) =>
        {
            using var sep = new Pen(Color.FromArgb(50, 255, 255, 255), 1f);
            e.Graphics.DrawLine(sep, 8, 20, 8, panel.Height - 20);
        };

        var inner = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 1,
            RowCount    = 2,
            BackColor   = Color.Transparent,
            Padding     = new Padding(0),
            Margin      = new Padding(0),
        };
        inner.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        inner.RowStyles.Add(new RowStyle(SizeType.Percent, 30)); // "TOTAL" label
        inner.RowStyles.Add(new RowStyle(SizeType.Percent, 70)); // amount

        var lblCaption = new Label
        {
            AutoSize  = false,
            Dock      = DockStyle.Fill,
            Text      = "TOTAL",
            Font      = _fontMeta,
            ForeColor = AppColors.TextMuted,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.BottomRight,
            Padding   = new Padding(0, 0, 0, 2),
        };

        _lblTotal = new Label
        {
            AutoSize  = false,
            Dock      = DockStyle.Fill,
            Text      = "$0.00",
            Font      = _fontHero,
            ForeColor = AppColors.AccentGreen,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.TopRight,
        };

        inner.Controls.Add(lblCaption, 0, 0);
        inner.Controls.Add(_lblTotal,  0, 1);

        panel.Controls.Add(inner);
        return panel;
    }

    // ── ListView owner-draw — delegated to shared ListViewTheme ──────────────
    private static void AttachListViewDraw(ListView lv)
        => ListViewTheme.Apply(lv);

    private static void FillProductColumn(ListView lv, int fillIdx)
    {
        if (lv.Columns.Count == 0) return;
        int total  = lv.ClientSize.Width;
        int fixedW = 0;
        for (int i = 0; i < lv.Columns.Count; i++)
            if (i != fillIdx) fixedW += lv.Columns[i].Width;
        lv.Columns[fillIdx].Width = Math.Max(total - fixedW, 60);
    }

    // ── Cart data ─────────────────────────────────────────────────────────────
    private void OnCartChanged(object? sender, EventArgs e)
    {
        if (InvokeRequired) { Invoke(RefreshCart); return; }
        RefreshCart();
    }

    private void RefreshCart()
    {
        _lvCart.Items.Clear();
        _subtotal  = 0;
        _taxTotal  = 0;
        _itemCount = 0;

        foreach (var item in _shoppingCart.Items)
        {
            _subtotal  += item.Subtotal;
            _taxTotal  += item.Total - item.Subtotal;
            _itemCount += item.Quantity;

            var lvi = new ListViewItem(item.Number.ToString());
            lvi.SubItems.Add(item.ProductName);
            lvi.SubItems.Add(item.Quantity.ToString());
            lvi.SubItems.Add(item.UnitPrice.ToString("C"));
            lvi.SubItems.Add(item.Total.ToString("C"));
            _lvCart.Items.Add(lvi);
        }

        _grandTotal = _subtotal + _taxTotal;
        UpdateFooter();
    }

    private void UpdateFooter()
    {
        if (_lblTotal  != null) _lblTotal.Text  = _grandTotal.ToString("C");
        if (_lblItems  != null) _lblItems.Text  = $"Items: {_itemCount:G}";
        if (_lblSubTax != null) _lblSubTax.Text = $"Subtotal: {_subtotal:C}   ·   Tax: {_taxTotal:C}";
    }

    // ── Weight ────────────────────────────────────────────────────────────────
    private void OnWeightChanged(string newUnit)
    {
        void Update() => _lblWeight.Text = $"⚖  {newUnit}";
        if (InvokeRequired) Invoke(Update);
        else Update();
    }

    // ── Banners ───────────────────────────────────────────────────────────────
    private async Task LoadBannersAsync()
    {
        try
        {
            var list    = await _bannerService.LoadBanners();
            _bannerUrls = list.Select(b => b.Image).ToArray();
            if (_bannerUrls.Length > 0)
                ShowBanner();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CustomerScreen] Banner load failed: {ex.Message}");
        }
    }

    private void ShowBanner()
    {
        if (_bannerUrls.Length == 0) return;
        try { _pbBanner.LoadAsync(_bannerUrls[_bannerIndex]); }
        catch { /* silent — bad URL or network issue */ }
    }

    // ── Timers ────────────────────────────────────────────────────────────────
    private void StartTimers()
    {
        _timerBanner = new System.Windows.Forms.Timer { Interval = 4000 };
        _timerBanner.Tick += (_, _) =>
        {
            if (_bannerUrls.Length > 0)
            {
                _bannerIndex = (_bannerIndex + 1) % _bannerUrls.Length;
                ShowBanner();
            }
        };
        _timerBanner.Start();

        _timerClock = new System.Windows.Forms.Timer { Interval = 1000 };
        _timerClock.Tick += (_, _) =>
            _lblClock.Text = DateTime.Now.ToString("ddd, MMM dd  ·  HH:mm:ss");
        _timerClock.Start();
    }

    // ── Cleanup ───────────────────────────────────────────────────────────────
    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _shoppingCart.CartChanged    -= OnCartChanged;
        SharedData.WeightUnitChanged -= OnWeightChanged;

        _timerBanner?.Stop();
        _timerBanner?.Dispose();
        _timerClock?.Stop();
        _timerClock?.Dispose();

        base.OnFormClosed(e);
    }
}
