using OmadaPOS.Presentation.Styling;
using System.Drawing.Drawing2D;

namespace OmadaPOS.Presentation.Controls;

/// <summary>
/// Slim, self-contained header bar.
/// Zones: [Brand/Cashier | Scan input + Product name | ☰ Menu | ✕ Exit]
/// </summary>
public sealed class POSHeaderControl : UserControl
{
    // ── Public events ──────────────────────────────────────────────────────────
    public event EventHandler? SettingsRequested;
    public event EventHandler? DailyCloseRequested;
    public event EventHandler? InvoiceRequested;
    public event EventHandler? LogoutRequested;
    public event EventHandler? ExitRequested;
    public event EventHandler? AgeAuditLogRequested;
    public event EventHandler? DiagnosticsRequested;

    // ── Internal state ─────────────────────────────────────────────────────────
    private readonly Label _lblCashier;
    private readonly Label _lblProductName;
    private Label? _lblScannerDot;
    private Label? _lblScaleDot;

    // ── Header palette — corporate navy chrome ─────────────────────────────────
    private static readonly Color _headerBg    = AppColors.HeaderPrimary;    // #1F4E79 navy
    private static readonly Color _headerBg2   = AppColors.HeaderSecondary; // #163B6D navy scan zone
    private static readonly Color _btnMenuBg   = AppColors.HeaderDark;      // #0F2D56 navy menu btn
    private static readonly Color _btnExitBg   = Color.FromArgb(185, 28, 28);   // red — exit button bg
    private static readonly Color _accentLine  = AppColors.AccentGreen;
    private static readonly Color _iconColor   = Color.FromArgb(220, 230, 245); // near-white icon

    // ── Factory ────────────────────────────────────────────────────────────────
    public static POSHeaderControl Attach(
        TableLayoutPanel mainLayout,
        TableLayoutPanel oldHeaderLayout,
        TextBox          textBoxUPC)
    {
        ArgumentNullException.ThrowIfNull(mainLayout);
        ArgumentNullException.ThrowIfNull(oldHeaderLayout);
        ArgumentNullException.ThrowIfNull(textBoxUPC);

        mainLayout.SuspendLayout();
        mainLayout.Controls.Remove(oldHeaderLayout);
        oldHeaderLayout.Dispose();

        var header = new POSHeaderControl(textBoxUPC);
        mainLayout.Controls.Add(header, 0, 0);

        mainLayout.ResumeLayout(true);
        return header;
    }

    // ── Constructor ────────────────────────────────────────────────────────────
    private POSHeaderControl(TextBox textBoxUPC)
    {
        SetStyle(ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.AllPaintingInWmPaint  |
                 ControlStyles.UserPaint, true);
        UpdateStyles();

        Dock      = DockStyle.Fill;
        Margin    = Padding.Empty;
        Padding   = Padding.Empty;
        BackColor = _headerBg;

        // Emerald accent line at the very bottom
        Paint += (_, e) =>
        {
            using var pen = new Pen(_accentLine, 2f);
            e.Graphics.DrawLine(pen, 0, Height - 2, Width, Height - 2);
        };

        // ── Root layout: 4 columns ─────────────────────────────────────────────
        var layout = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 4,
            RowCount    = 1,
            Margin      = Padding.Empty,
            Padding     = Padding.Empty,
            BackColor   = Color.Transparent,
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 190F)); // Brand/Cashier
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,  100F)); // Scan + Product
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 64F));  // ☰ Menu
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 56F));  // ✕ Exit
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        // ── Zone 1: Brand + cashier ────────────────────────────────────────────
        var zone1 = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = Color.Transparent,
            Padding   = new Padding(16, 6, 8, 6),
        };
        // Right-side divider line
        zone1.Paint += (_, e) =>
        {
            using var p = new Pen(Color.FromArgb(30, 255, 255, 255), 1f);
            e.Graphics.DrawLine(p, zone1.Width - 1, 8, zone1.Width - 1, zone1.Height - 8);
        };

        var lblBrand = new Label
        {
            Text      = AppConstants.AppName,
            Font      = new Font("Segoe UI", 11F, FontStyle.Bold),
            ForeColor = AppColors.TextWhite,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Top,
            Height    = 30,
            TextAlign = ContentAlignment.BottomLeft,
        };

        _lblCashier = new Label
        {
            Text      = "—",
            Font      = new Font("Segoe UI", 9F),
            ForeColor = AppColors.TextOnDarkSecondary,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Top,
            Height    = 18,
            TextAlign = ContentAlignment.TopLeft,
        };

        // Hardware status dots — shown at the bottom of zone 1
        var statusPanel = new Panel
        {
            Dock      = DockStyle.Bottom,
            Height    = 18,
            BackColor = Color.Transparent,
        };

        _lblScannerDot = new Label
        {
            Text      = "⬤ SCN",
            Font      = new Font("Segoe UI", 7.5F),
            ForeColor = Color.FromArgb(100, 148, 163, 184),  // muted — unknown at start
            BackColor = Color.Transparent,
            AutoSize  = true,
            Location  = new Point(0, 2),
        };

        _lblScaleDot = new Label
        {
            Text      = "⬤ SCL",
            Font      = new Font("Segoe UI", 7.5F),
            ForeColor = Color.FromArgb(100, 148, 163, 184),
            BackColor = Color.Transparent,
            AutoSize  = true,
            Location  = new Point(52, 2),
        };

        statusPanel.Controls.Add(_lblScannerDot);
        statusPanel.Controls.Add(_lblScaleDot);

        zone1.Controls.Add(statusPanel);
        zone1.Controls.Add(_lblCashier);
        zone1.Controls.Add(lblBrand);
        layout.Controls.Add(zone1, 0, 0);

        // ── Zone 2: Scan input + product name ─────────────────────────────────
        var zone2 = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            RowCount    = 2,
            ColumnCount = 1,
            Margin      = Padding.Empty,
            Padding     = new Padding(0, 8, 12, 8),
            BackColor   = Color.Transparent,
        };
        zone2.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
        zone2.RowStyles.Add(new RowStyle(SizeType.Percent,  100F));
        zone2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        // Scan input wrapper — slightly lighter navy background
        var scanWrapper = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = _headerBg2,
            Margin    = Padding.Empty,
            Padding   = new Padding(10, 0, 0, 0),
        };
        scanWrapper.Paint += (_, e) =>
        {
            var r = new Rectangle(0, 0, scanWrapper.Width - 1, scanWrapper.Height - 1);
            using var pen = new Pen(Color.FromArgb(50, 255, 255, 255), 1f);
            e.Graphics.DrawRectangle(pen, r);
        };

        textBoxUPC.Parent?.Controls.Remove(textBoxUPC);
        textBoxUPC.Dock          = DockStyle.Fill;
        textBoxUPC.BackColor     = _headerBg2;
        textBoxUPC.BorderStyle   = BorderStyle.None;
        textBoxUPC.Font          = new Font("Segoe UI", 12F);
        textBoxUPC.ForeColor     = AppColors.TextWhite;
        textBoxUPC.PlaceholderText = "Scan UPC…";
        textBoxUPC.TextAlign     = HorizontalAlignment.Left;
        scanWrapper.Controls.Add(textBoxUPC);
        zone2.Controls.Add(scanWrapper, 0, 0);

        _lblProductName = new Label
        {
            Text      = "Ready to scan…",
            Font      = new Font("Segoe UI", 9F, FontStyle.Italic),
            ForeColor = AppColors.TextOnDarkMuted,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding   = new Padding(2, 0, 0, 0),
        };
        zone2.Controls.Add(_lblProductName, 0, 1);
        layout.Controls.Add(zone2, 1, 0);

        // ── Zone 3: Menu button (☰) ────────────────────────────────────────────
        var btnMenu = MakeIconPanel(_btnMenuBg, DrawHamburger);
        btnMenu.Click += BtnConfig_Click;
        new ToolTip().SetToolTip(btnMenu, "Menú");
        layout.Controls.Add(btnMenu, 2, 0);

        // ── Zone 4: Exit button (✕) ────────────────────────────────────────────
        var btnExit = MakeIconPanel(_btnExitBg, DrawClose);
        btnExit.Click += (_, _) => ExitRequested?.Invoke(this, EventArgs.Empty);
        new ToolTip().SetToolTip(btnExit, "Salir del sistema");
        layout.Controls.Add(btnExit, 3, 0);

        Controls.Add(layout);
    }

    // ── Config dropdown ────────────────────────────────────────────────────────
    private void BtnConfig_Click(object? sender, EventArgs e)
    {
        var menu = new ContextMenuStrip();
        StyleContextMenu(menu);
        menu.Items.Add("Settings",        null, (_, _) => SettingsRequested?.Invoke(this, EventArgs.Empty));
        menu.Items.Add("Reprint Receipt",      null, (_, _) => InvoiceRequested?.Invoke(this, EventArgs.Empty));
        menu.Items.Add("Age Verification Log", null, (_, _) => AgeAuditLogRequested?.Invoke(this, EventArgs.Empty));
        menu.Items.Add("Diagnostics",          null, (_, _) => DiagnosticsRequested?.Invoke(this, EventArgs.Empty));
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Daily Close",          null, (_, _) => DailyCloseRequested?.Invoke(this, EventArgs.Empty));
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Sign Out",        null, (_, _) => LogoutRequested?.Invoke(this, EventArgs.Empty));

        var ctrl = (Control)(sender ?? this);
        menu.Show(ctrl, new Point(0, ctrl.Height));
    }

    private static void StyleContextMenu(ContextMenuStrip menu)
    {
        menu.Font            = new Font("Segoe UI", 11F);
        menu.BackColor       = Color.FromArgb(30, 41, 59);
        menu.ForeColor       = AppColors.TextWhite;
        menu.ShowImageMargin = false;
        menu.RenderMode      = ToolStripRenderMode.System;
        menu.Padding         = new Padding(0, 4, 0, 4);
    }

    // ── Public API ─────────────────────────────────────────────────────────────
    public void UpdateCashier(string name)
        => _lblCashier.Text = string.IsNullOrWhiteSpace(name) ? "—" : name;

    public void UpdateProductName(string name)
        => _lblProductName.Text = string.IsNullOrWhiteSpace(name) ? "Ready to scan…" : name;

    /// <summary>Refreshes the scanner status dot. Call from the UI thread.</summary>
    public void UpdateScannerStatus(bool connected)
    {
        if (_lblScannerDot == null) return;
        _lblScannerDot.ForeColor = connected
            ? AppColors.AccentGreen
            : Color.FromArgb(220, 38, 38);   // red
        _lblScannerDot.Text = connected ? "⬤ SCN" : "⬤ SCN";
        new ToolTip().SetToolTip(_lblScannerDot, connected ? "Scanner: connected" : "Scanner: disconnected");
    }

    /// <summary>Refreshes the scale status dot. Call from the UI thread.</summary>
    public void UpdateScaleStatus(bool connected)
    {
        if (_lblScaleDot == null) return;
        _lblScaleDot.ForeColor = connected
            ? AppColors.AccentGreen
            : Color.FromArgb(220, 38, 38);
        new ToolTip().SetToolTip(_lblScaleDot, connected ? "Scale: connected" : "Scale: disconnected");
    }

    // ── Icon panel factory ─────────────────────────────────────────────────────
    // Using Panel instead of Button: fully owner-drawn, no system chrome.
    private static Panel MakeIconPanel(Color bg, Action<Graphics, Rectangle> drawIcon)
    {
        var p = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = bg,
            Margin    = new Padding(1, 10, 4, 10),
        };

        p.Paint += (_, e) =>
        {
            var g = e.Graphics;
            g.SmoothingMode     = SmoothingMode.AntiAlias;
            g.CompositingQuality = CompositingQuality.HighQuality;

            var r = new Rectangle(0, 0, p.Width - 1, p.Height - 1);

            // Rounded background
            using var path = RoundedRect(r, 8);
            using var bgBr = new SolidBrush(bg);
            g.FillPath(bgBr, path);

            // Subtle inner highlight at top edge
            using var gloss = new Pen(Color.FromArgb(20, 255, 255, 255), 1f);
            g.DrawLine(gloss, r.Left + 8, r.Top + 1, r.Right - 8, r.Top + 1);

            // Icon
            drawIcon(g, new Rectangle(0, 0, p.Width, p.Height));
        };

        // Pressed feedback — momentary darken on MouseDown
        p.MouseDown += (_, _) =>
        {
            p.BackColor = DarkenColor(bg, 0.15f);
            p.Invalidate();
        };
        p.MouseUp += (_, _) =>
        {
            p.BackColor = bg;
            p.Invalidate();
        };

        return p;
    }

    // ── Icon painters ──────────────────────────────────────────────────────────
    private static void DrawHamburger(Graphics g, Rectangle bounds)
    {
        int cx = bounds.Width  / 2;
        int cy = bounds.Height / 2;
        int w  = 18;     // line width
        int gap = 5;     // gap between lines

        using var pen = new Pen(_iconColor, 2.2f) { StartCap = LineCap.Round, EndCap = LineCap.Round };

        // Three horizontal lines centered
        g.DrawLine(pen, cx - w / 2, cy - gap,     cx + w / 2, cy - gap);
        g.DrawLine(pen, cx - w / 2, cy,            cx + w / 2, cy);
        g.DrawLine(pen, cx - w / 2, cy + gap,     cx + w / 2, cy + gap);
    }

    private static void DrawClose(Graphics g, Rectangle bounds)
    {
        int cx = bounds.Width  / 2;
        int cy = bounds.Height / 2;
        int r  = 9;   // half-size of the × arms

        using var pen = new Pen(Color.White, 2.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round };

        // × — two diagonal lines
        g.DrawLine(pen, cx - r, cy - r, cx + r, cy + r);
        g.DrawLine(pen, cx + r, cy - r, cx - r, cy + r);
    }

    // ── GDI+ helpers ──────────────────────────────────────────────────────────
    private static GraphicsPath RoundedRect(Rectangle r, int radius)
    {
        int d    = radius * 2;
        var path = new GraphicsPath();
        path.AddArc(r.X,         r.Y,          d, d, 180, 90);
        path.AddArc(r.Right - d, r.Y,          d, d, 270, 90);
        path.AddArc(r.Right - d, r.Bottom - d, d, d,   0, 90);
        path.AddArc(r.X,         r.Bottom - d, d, d,  90, 90);
        path.CloseFigure();
        return path;
    }

    private static Color DarkenColor(Color c, float factor)
        => Color.FromArgb(c.A,
            Math.Max(0, (int)(c.R * (1 - factor))),
            Math.Max(0, (int)(c.G * (1 - factor))),
            Math.Max(0, (int)(c.B * (1 - factor))));
}
