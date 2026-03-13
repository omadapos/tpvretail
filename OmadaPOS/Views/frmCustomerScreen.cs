using OmadaPOS.Libreria.Services;
using OmadaPOS.Services;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace OmadaPOS.Views;

/// <summary>
/// Customer-facing secondary display — optimised for 1024×768 (4:3).
///
/// Two modes switch automatically based on cart state:
///
///   IDLE (cart empty)
///   ┌──────────────┬──────────────────────────────┐  header
///   │ Welcome card │  Banner carousel (full-tall)  │  body
///   └──────────────┴──────────────────────────────┘  footer
///
///   ACTIVE (cart has items)
///   ┌────────────────────────┬────────────────────┐  header
///   │  YOUR ORDER (ListView) │  Banner (sidebar)  │  body
///   └────────────────────────┴────────────────────┘  footer
///
/// The idle-card animates a pulsing glow on the WELCOME text.
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

    // Idle animation — drives a sine-wave alpha on the WELCOME glow
    private float    _pulseAngle  = 0f;

    // ── Controls ──────────────────────────────────────────────────────────────
    private ListView   _lvCart      = null!;
    private PictureBox _pbBannerAct = null!;   // active-mode sidebar
    private PictureBox _pbBannerIdle= null!;   // idle-mode right column
    private Label      _lblTotal    = null!;
    private Label      _lblSubTax   = null!;
    private Label      _lblItems    = null!;
    private Label      _lblWeight   = null!;
    private Label      _lblClock    = null!;
    private Label      _lblIdleClock= null!;   // big clock shown in idle card
    private Panel      _pnlGlow     = null!;   // custom-drawn pulsing WELCOME
    private Panel      _idlePanel   = null!;
    private Panel      _activePanel = null!;

    // ── Timers ────────────────────────────────────────────────────────────────
    private System.Windows.Forms.Timer? _timerBanner;
    private System.Windows.Forms.Timer? _timerClock;
    private System.Windows.Forms.Timer? _timerIdle;    // 50 ms — drives glow animation

    // ── Static GDI resources ─────────────────────────────────────────────────
    private static readonly Font _fontHero      = new("Consolas",  52F, FontStyle.Bold);
    private static readonly Font _fontMeta      = new("Segoe UI",  13F, FontStyle.Bold);
    private static readonly Font _fontItems     = new("Segoe UI",  16F, FontStyle.Bold);
    private static readonly Font _fontDetail    = new("Segoe UI",  12F);
    private static readonly Font _fontHeader    = new("Segoe UI",  15F, FontStyle.Bold);
    private static readonly Font _fontClock     = new("Consolas",  13F);
    private static readonly Font _fontList      = new("Segoe UI",  15F);
    private static readonly Font _fontIdleWelcome = new("Segoe UI", 52F, FontStyle.Bold);
    private static readonly Font _fontIdleSub   = new("Segoe UI",  15F);
    private static readonly Font _fontIdleClock = new("Consolas",  26F, FontStyle.Bold);
    private static readonly Font _fontIdleHint  = new("Segoe UI",  13F);

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
        root.RowStyles.Add(new RowStyle(SizeType.Absolute,  72));  // header
        root.RowStyles.Add(new RowStyle(SizeType.Percent,  100));  // body
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 190));  // footer

        root.Controls.Add(BuildHeader(), 0, 0);
        root.Controls.Add(BuildBody(),   0, 1);
        root.Controls.Add(BuildFooter(), 0, 2);

        Controls.Add(root);
    }

    // ── Header ────────────────────────────────────────────────────────────────
    private Panel BuildHeader()
    {
        var header = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.NavyDark,
            Padding   = new Padding(0),
        };
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
            Text      = DateTime.Now.ToString("ddd, MMM dd  ·  hh:mm tt"),
            Font      = _fontClock,
            ForeColor = AppColors.TextMuted,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Right,
            Width     = 280,
            TextAlign = ContentAlignment.MiddleRight,
            Padding   = new Padding(0, 0, 18, 0),
        };

        header.Controls.Add(lblWelcome);
        header.Controls.Add(_lblClock);
        header.Controls.Add(lblStore);

        return header;
    }

    // ── Body (container that holds both idle and active panels) ───────────────
    private Panel BuildBody()
    {
        var container = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = Color.Transparent,
            Padding   = new Padding(0),
            Margin    = new Padding(0),
        };

        _activePanel = BuildActivePanel();
        _idlePanel   = BuildIdlePanel();

        _activePanel.Dock = DockStyle.Fill;
        _idlePanel.Dock   = DockStyle.Fill;

        container.Controls.Add(_activePanel);
        container.Controls.Add(_idlePanel);  // added last → on top by default
        // Idle is shown first; RefreshCart() will switch as needed.

        return container;
    }

    // ── IDLE panel ────────────────────────────────────────────────────────────
    private Panel BuildIdlePanel()
    {
        var panel = new Panel
        {
            BackColor = AppColors.NavyDark,
            Padding   = new Padding(0),
            Margin    = new Padding(0),
        };

        var layout = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 2,
            RowCount    = 1,
            BackColor   = Color.Transparent,
            Padding     = new Padding(0),
            Margin      = new Padding(0),
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        layout.Controls.Add(BuildIdleCard(), 0, 0);
        layout.Controls.Add(BuildIdleBanner(), 1, 0);

        panel.Controls.Add(layout);
        return panel;
    }

    private Panel BuildIdleCard()
    {
        var card = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.NavyBase,
            Padding   = new Padding(32, 0, 24, 0),
        };
        // Right border separator
        card.Paint += (_, e) =>
        {
            using var pen = new Pen(Color.FromArgb(40, 255, 255, 255), 1f);
            e.Graphics.DrawLine(pen, card.Width - 1, 0, card.Width - 1, card.Height);
        };

        var inner = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 1,
            RowCount    = 5,
            BackColor   = Color.Transparent,
            Padding     = new Padding(0),
            Margin      = new Padding(0),
        };
        inner.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        inner.RowStyles.Add(new RowStyle(SizeType.Percent,  14)); // spacer
        inner.RowStyles.Add(new RowStyle(SizeType.Absolute,  36)); // store name
        inner.RowStyles.Add(new RowStyle(SizeType.Absolute,  80)); // WELCOME (glow panel)
        inner.RowStyles.Add(new RowStyle(SizeType.Absolute,  26)); // tagline
        inner.RowStyles.Add(new RowStyle(SizeType.Percent,  100)); // clock + hint

        // Store name
        var lblStore = new Label
        {
            Text      = "DAILY STOP",
            Font      = new Font("Segoe UI", 15F, FontStyle.Bold),
            ForeColor = AppColors.TextOnDarkSecondary,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
        };

        // Pulsing WELCOME — custom drawn
        _pnlGlow = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
        _pnlGlow.Paint += PaintWelcomeGlow;

        // Tagline
        var lblTag = new Label
        {
            Text      = "Please scan your items",
            Font      = _fontIdleHint,
            ForeColor = AppColors.TextOnDarkMuted,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.TopLeft,
        };

        // Clock + hint bottom
        var bottomPanel = BuildIdleBottomPanel();

        inner.Controls.Add(new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent }, 0, 0);
        inner.Controls.Add(lblStore,    0, 1);
        inner.Controls.Add(_pnlGlow,    0, 2);
        inner.Controls.Add(lblTag,      0, 3);
        inner.Controls.Add(bottomPanel, 0, 4);

        card.Controls.Add(inner);
        return card;
    }

    private Panel BuildIdleBottomPanel()
    {
        var panel = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 1,
            RowCount    = 2,
            BackColor   = Color.Transparent,
            Padding     = new Padding(0, 24, 0, 32),
            Margin      = new Padding(0),
        };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));  // clock
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));  // prompt

        _lblIdleClock = new Label
        {
            Text      = DateTime.Now.ToString("hh:mm tt"),
            Font      = _fontIdleClock,
            ForeColor = AppColors.AccentGreenLight,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.BottomLeft,
        };

        var lblHint = new Label
        {
            Text      = "Ready to serve you  ✦",
            Font      = new Font("Segoe UI", 11F, FontStyle.Italic),
            ForeColor = Color.FromArgb(80, 255, 255, 255),
            BackColor = Color.Transparent,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.TopLeft,
        };

        panel.Controls.Add(_lblIdleClock, 0, 0);
        panel.Controls.Add(lblHint,       0, 1);
        return panel;
    }

    // Custom GDI+ draw — WELCOME text with sine-wave glow intensity
    private void PaintWelcomeGlow(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode     = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

        var panel = (Panel)sender!;
        var rect  = panel.ClientRectangle;

        // Sine pulse — base 200, amplitude 55 → range [145, 255]
        double sin    = Math.Sin(_pulseAngle * Math.PI / 180.0);
        int    alpha  = 200 + (int)(55 * sin);
        var    color  = Color.FromArgb(alpha, AppColors.AccentGreen);

        // Drop shadow for depth
        using var shadow = new SolidBrush(Color.FromArgb(60, 0, 0, 0));
        var shadowRc = rect with { X = rect.X + 2, Y = rect.Y + 3 };
        TextRenderer.DrawText(g, "WELCOME", _fontIdleWelcome, shadowRc, Color.FromArgb(60, 0, 0, 0),
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

        TextRenderer.DrawText(g, "WELCOME", _fontIdleWelcome, rect, color,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
    }

    private Panel BuildIdleBanner()
    {
        var panel = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.NavyDark,
            Padding   = new Padding(0),
            Margin    = new Padding(0),
        };

        _pbBannerIdle = new PictureBox
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.NavyDark,
            SizeMode  = PictureBoxSizeMode.Zoom,
        };

        // When no banner: paint a subtle pattern
        _pbBannerIdle.Paint += (_, e) =>
        {
            if (_pbBannerIdle.Image != null) return;
            var g  = e.Graphics;
            var rc = _pbBannerIdle.ClientRectangle;

            // Diagonal stripe pattern (very subtle)
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using var pen = new Pen(Color.FromArgb(12, 255, 255, 255), 1f);
            for (int i = -rc.Height; i < rc.Width; i += 28)
                g.DrawLine(pen, i, 0, i + rc.Height, rc.Height);

            // Centered logo placeholder
            using var font = new Font("Segoe UI", 48F, FontStyle.Bold);
            TextRenderer.DrawText(g, "🏪", font, rc, Color.FromArgb(40, 255, 255, 255),
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        };

        panel.Controls.Add(_pbBannerIdle);
        return panel;
    }

    // ── ACTIVE panel ──────────────────────────────────────────────────────────
    private Panel BuildActivePanel()
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
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62));
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38));
        body.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        body.Controls.Add(BuildCartPanel(),    0, 0);
        body.Controls.Add(BuildBannerPanel(),  1, 0);

        return body;
    }

    private Control BuildCartPanel()
    {
        var panel = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = Color.FromArgb(248, 250, 252),
            Padding   = new Padding(0),
            Margin    = new Padding(0),
        };

        var subHeader = new Panel
        {
            Dock      = DockStyle.Top,
            Height    = 46,
            BackColor = AppColors.NavyBase,
        };
        subHeader.Paint += (_, e) =>
        {
            using var pen = new Pen(AppColors.AccentGreen, 2f);
            e.Graphics.DrawLine(pen, 0, subHeader.Height - 2, subHeader.Width, subHeader.Height - 2);
        };
        subHeader.Controls.Add(new Label
        {
            Text      = "YOUR ORDER",
            Font      = new Font("Segoe UI", 13F, FontStyle.Bold),
            ForeColor = AppColors.TextWhite,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
        });

        _lvCart = new ListView
        {
            Dock          = DockStyle.Fill,
            View          = View.Details,
            FullRowSelect = true,
            GridLines     = false,
            MultiSelect   = false,
            HideSelection = true,
            BackColor     = Color.FromArgb(255, 255, 255),
            ForeColor     = AppColors.TextPrimary,
            Font          = _fontList,
            BorderStyle   = BorderStyle.None,
            HeaderStyle   = ColumnHeaderStyle.Nonclickable,
            OwnerDraw     = true,
            UseCompatibleStateImageBehavior = false,
        };
        _lvCart.Columns.Add("#",       44,  HorizontalAlignment.Center);
        _lvCart.Columns.Add("Product", 220, HorizontalAlignment.Left);
        _lvCart.Columns.Add("Qty",     52,  HorizontalAlignment.Center);
        _lvCart.Columns.Add("Price",   100, HorizontalAlignment.Right);
        _lvCart.Columns.Add("Total",   100, HorizontalAlignment.Right);
        ListViewTheme.Apply(_lvCart);
        _lvCart.Resize += (_, _) => FillProductColumn(_lvCart, fillIdx: 1);

        panel.Controls.Add(_lvCart);
        panel.Controls.Add(subHeader);
        return panel;
    }

    private Panel BuildBannerPanel()
    {
        var panel = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.NavyDark,
            Padding   = new Padding(0),
            Margin    = new Padding(0),
        };

        _pbBannerAct = new PictureBox
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.NavyDark,
            SizeMode  = PictureBoxSizeMode.Zoom,
        };

        panel.Controls.Add(_pbBannerAct);
        return panel;
    }

    // ── Footer ────────────────────────────────────────────────────────────────
    private Control BuildFooter()
    {
        var footer = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.NavyBase,
            Padding   = new Padding(0),
            Margin    = new Padding(0),
        };
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
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48));
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
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 38));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 32));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 30));

        _lblItems = new Label
        {
            AutoSize  = false, Dock      = DockStyle.Fill,
            Text      = "Items: 0",
            Font      = _fontItems,
            ForeColor = AppColors.AccentGreen,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleLeft,
        };

        _lblSubTax = new Label
        {
            AutoSize  = false, Dock      = DockStyle.Fill,
            Text      = "Subtotal: $0.00   ·   Tax: $0.00",
            Font      = _fontDetail,
            ForeColor = Color.FromArgb(148, 163, 184),
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleLeft,
        };

        _lblWeight = new Label
        {
            AutoSize  = false, Dock      = DockStyle.Fill,
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
        inner.RowStyles.Add(new RowStyle(SizeType.Percent, 28));
        inner.RowStyles.Add(new RowStyle(SizeType.Percent, 72));

        var lblCaption = new Label
        {
            AutoSize  = false, Dock      = DockStyle.Fill,
            Text      = "TOTAL",
            Font      = _fontMeta,
            ForeColor = AppColors.TextMuted,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.BottomRight,
            Padding   = new Padding(0, 0, 0, 2),
        };

        _lblTotal = new Label
        {
            AutoSize  = false, Dock      = DockStyle.Fill,
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

    // ── ListView helpers ──────────────────────────────────────────────────────
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
        UpdateMode();   // switch idle ↔ active based on cart content
    }

    private void UpdateFooter()
    {
        if (_lblTotal  != null) _lblTotal.Text  = _grandTotal.ToString("C");
        if (_lblItems  != null) _lblItems.Text  = $"Items: {_itemCount:G}";
        if (_lblSubTax != null) _lblSubTax.Text = $"Subtotal: {_subtotal:C}   ·   Tax: {_taxTotal:C}";
    }

    // ── Idle / Active switching ───────────────────────────────────────────────
    private bool _isIdle = true;

    private void UpdateMode()
    {
        bool shouldBeIdle = _shoppingCart.Items.Count == 0;
        if (shouldBeIdle == _isIdle) return;

        _isIdle = shouldBeIdle;

        if (_isIdle)
        {
            _idlePanel.BringToFront();
            _timerIdle?.Start();
        }
        else
        {
            _activePanel.BringToFront();
            _timerIdle?.Stop();
        }
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
        string url = _bannerUrls[_bannerIndex];
        try { _pbBannerAct.LoadAsync(url);  } catch { }
        try { _pbBannerIdle.LoadAsync(url); } catch { }
    }

    // ── Timers ────────────────────────────────────────────────────────────────
    private void StartTimers()
    {
        _timerBanner = new System.Windows.Forms.Timer { Interval = 4_000 };
        _timerBanner.Tick += (_, _) =>
        {
            if (_bannerUrls.Length > 0)
            {
                _bannerIndex = (_bannerIndex + 1) % _bannerUrls.Length;
                ShowBanner();
            }
        };
        _timerBanner.Start();

        _timerClock = new System.Windows.Forms.Timer { Interval = 1_000 };
        _timerClock.Tick += (_, _) =>
        {
            string t = DateTime.Now.ToString("ddd, MMM dd  ·  hh:mm tt");
            if (_lblClock     != null) _lblClock.Text     = t;
            if (_lblIdleClock != null) _lblIdleClock.Text = DateTime.Now.ToString("hh:mm tt");
        };
        _timerClock.Start();

        // Idle animation — 50 ms ticks advance the pulse angle by 3°
        _timerIdle = new System.Windows.Forms.Timer { Interval = 50 };
        _timerIdle.Tick += (_, _) =>
        {
            _pulseAngle = (_pulseAngle + 3f) % 360f;
            _pnlGlow?.Invalidate();
        };
        if (_isIdle) _timerIdle.Start();   // only run when idle panel is visible
    }

    // ── Cleanup ───────────────────────────────────────────────────────────────
    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _shoppingCart.CartChanged    -= OnCartChanged;
        SharedData.WeightUnitChanged -= OnWeightChanged;

        _timerBanner?.Stop(); _timerBanner?.Dispose();
        _timerClock?.Stop();  _timerClock?.Dispose();
        _timerIdle?.Stop();   _timerIdle?.Dispose();

        base.OnFormClosed(e);
    }
}
