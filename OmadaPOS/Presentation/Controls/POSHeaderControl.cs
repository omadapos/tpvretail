using OmadaPOS.Presentation.Styling;

namespace OmadaPOS.Presentation.Controls;

/// <summary>
/// Slim, self-contained header bar.
/// Zones: [Brand/Cashier | Scan input + Product name | ⚙ Config menu | ✕ Exit]
/// </summary>
public sealed class POSHeaderControl : UserControl
{
    // ── Public events ──────────────────────────────────────────────────────────
    public event EventHandler? SettingsRequested;
    public event EventHandler? DailyCloseRequested;
    public event EventHandler? InvoiceRequested;
    public event EventHandler? LogoutRequested;
    public event EventHandler? ExitRequested;

    // ── Internal state ─────────────────────────────────────────────────────────
    private readonly Label   _lblCashier;
    private readonly Label   _lblProductName;

    // ── Factory ────────────────────────────────────────────────────────────────
    /// <summary>
    /// Replaces <paramref name="oldHeaderLayout"/> (and the product-strip row below it)
    /// with this control inside <paramref name="mainLayout"/>.
    /// </summary>
    public static POSHeaderControl Attach(
        TableLayoutPanel mainLayout,
        TableLayoutPanel oldHeaderLayout,
        TextBox          textBoxUPC)
    {
        ArgumentNullException.ThrowIfNull(mainLayout);
        ArgumentNullException.ThrowIfNull(oldHeaderLayout);
        ArgumentNullException.ThrowIfNull(textBoxUPC);

        mainLayout.SuspendLayout();

        // Remove and dispose the placeholder header layout (and its children).
        // The Designer already places tableLayoutPanelMain at row 1 with the
        // correct 2-row structure, so no further row surgery is needed.
        mainLayout.Controls.Remove(oldHeaderLayout);
        oldHeaderLayout.Dispose();

        // Insert the new self-contained header at row 0.
        var header = new POSHeaderControl(textBoxUPC);
        mainLayout.Controls.Add(header, 0, 0);

        mainLayout.ResumeLayout(true);
        return header;
    }

    // ── Constructor ────────────────────────────────────────────────────────────
    private POSHeaderControl(TextBox textBoxUPC)
    {
        Dock      = DockStyle.Fill;
        Margin    = Padding.Empty;
        Padding   = Padding.Empty;
        BackColor = AppColors.BackgroundSecondary;

        // Bottom accent separator
        Paint += (_, e) =>
        {
            using var pen = new Pen(AppColors.SeparatorOnDark, 1f);
            e.Graphics.DrawLine(pen, 0, Height - 1, Width, Height - 1);
        };

        // ── Root layout: 4 columns ─────────────────────────────────────────────
        var layout = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 4,
            RowCount    = 1,
            Margin      = Padding.Empty,
            Padding     = new Padding(0, 0, 4, 0),
            BackColor   = Color.Transparent,
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F)); // Brand/Cashier
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,  100F)); // Scan + Product
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));  // ⚙ Config
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 54F));  // ✕ Exit
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        // ── Zone 1: Brand + cashier name ──────────────────────────────────────
        var zone1 = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = Color.Transparent,
            Padding   = new Padding(14, 6, 8, 6),
        };

        var lblBrand = new Label
        {
            Text      = "● OMADA POS",
            Font      = new Font("Segoe UI", 12F, FontStyle.Bold),
            ForeColor = AppColors.TextPrimary,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Top,
            Height    = 34,
            TextAlign = ContentAlignment.BottomLeft,
        };

        _lblCashier = new Label
        {
            Text      = "—",
            Font      = new Font("Segoe UI", 9F),
            ForeColor = AppColors.TextSecondary,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.TopLeft,
        };

        // Dock.Top stacking: add Fill first, then Top so lblBrand renders on top
        zone1.Controls.Add(_lblCashier);
        zone1.Controls.Add(lblBrand);
        layout.Controls.Add(zone1, 0, 0);

        // ── Zone 2: Scan input (top row) + product name (bottom row) ──────────
        var zone2 = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            RowCount    = 2,
            ColumnCount = 1,
            Margin      = Padding.Empty,
            Padding     = new Padding(0, 8, 8, 8),
            BackColor   = Color.Transparent,
        };
        zone2.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
        zone2.RowStyles.Add(new RowStyle(SizeType.Percent,  100F));
        zone2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        // Styled scan input wrapper
        var scanWrapper = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.BackgroundPrimary,
            Margin    = Padding.Empty,
            Padding   = new Padding(8, 0, 0, 0),
        };
        scanWrapper.Paint += (_, e) =>
        {
            var r = new Rectangle(0, 0, scanWrapper.Width - 1, scanWrapper.Height - 1);
            using var pen = new Pen(AppColors.SeparatorOnDark, 1f);
            e.Graphics.DrawRectangle(pen, r);
        };

        textBoxUPC.Parent?.Controls.Remove(textBoxUPC);
        textBoxUPC.Dock          = DockStyle.Fill;
        textBoxUPC.BackColor     = AppColors.BackgroundPrimary; // Transparent not allowed on native TextBox
        textBoxUPC.BorderStyle   = BorderStyle.None;
        textBoxUPC.Font          = new Font("Segoe UI", 12F);
        textBoxUPC.ForeColor     = AppColors.TextPrimary;
        textBoxUPC.PlaceholderText = "Scan UPC...";
        textBoxUPC.TextAlign     = HorizontalAlignment.Left;
        scanWrapper.Controls.Add(textBoxUPC);
        zone2.Controls.Add(scanWrapper, 0, 0);

        _lblProductName = new Label
        {
            Text      = "Ready to scan…",
            Font      = new Font("Segoe UI", 10F, FontStyle.Italic),
            ForeColor = AppColors.TextSecondary,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding   = new Padding(2, 0, 0, 0),
        };
        zone2.Controls.Add(_lblProductName, 0, 1);
        layout.Controls.Add(zone2, 1, 0);

        // ── Zone 3: Config (dropdown menu) ────────────────────────────────────
        var btnConfig = MakeHeaderButton("⚙", AppColors.SlateBlue);
        btnConfig.Click += BtnConfig_Click;
        layout.Controls.Add(btnConfig, 2, 0);

        // ── Zone 4: Exit (closes entire application) ───────────────────────────
        var btnExit = MakeHeaderButton("✕", AppColors.Danger);
        btnExit.Click += (_, _) => ExitRequested?.Invoke(this, EventArgs.Empty);
        layout.Controls.Add(btnExit, 3, 0);

        Controls.Add(layout);
    }

    // ── Config dropdown ────────────────────────────────────────────────────────
    private void BtnConfig_Click(object sender, EventArgs e)
    {
        var menu = new ContextMenuStrip();
        StyleContextMenu(menu);
        menu.Items.Add("⚙  Configuración",    null, (_, _) => SettingsRequested?.Invoke(this, EventArgs.Empty));
        menu.Items.Add("🖨  Reimprimir factura", null, (_, _) => InvoiceRequested?.Invoke(this, EventArgs.Empty));
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("📅  Cierre diario",    null, (_, _) => DailyCloseRequested?.Invoke(this, EventArgs.Empty));
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("🚪  Cerrar sesión",    null, (_, _) => LogoutRequested?.Invoke(this, EventArgs.Empty));

        var btn = (Control)sender;
        menu.Show(btn, new Point(0, btn.Height));
    }

    private static void StyleContextMenu(ContextMenuStrip menu)
    {
        menu.Font            = new Font("Segoe UI", 11F);
        menu.BackColor       = AppColors.BackgroundPrimary;
        menu.ForeColor       = AppColors.TextPrimary;
        menu.ShowImageMargin = false;
        menu.RenderMode      = ToolStripRenderMode.System;
    }

    // ── Public API ─────────────────────────────────────────────────────────────
    public void UpdateCashier(string name)
        => _lblCashier.Text = string.IsNullOrWhiteSpace(name) ? "—" : $"👤 {name}";

    public void UpdateProductName(string name)
        => _lblProductName.Text = string.IsNullOrWhiteSpace(name) ? "Ready to scan…" : name;

    /// <summary>No-op kept for call-site compatibility during migration.</summary>
    public void SetInvoiceDisplay(int orderId) { }

    // ── Helpers ────────────────────────────────────────────────────────────────
    private static Button MakeHeaderButton(string text, Color backColor) =>
        new()
        {
            Text      = text,
            Dock      = DockStyle.Fill,
            Margin    = new Padding(2, 10, 2, 10),
            Font      = new Font("Segoe UI", 17F),
            ForeColor = AppColors.TextWhite,
            BackColor = backColor,
            FlatStyle = FlatStyle.Flat,
            Cursor    = Cursors.Hand,
            FlatAppearance = { BorderSize = 0 },
        };
}
