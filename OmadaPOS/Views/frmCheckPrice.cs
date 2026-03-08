using OmadaPOS.Libreria.Services;
using OmadaPOS.Services.Navigation;

namespace OmadaPOS.Views;

public sealed class frmCheckPrice : POSDialog
{
    private readonly ICategoryService  _categoryService;
    private readonly IWindowService    _windowService;

    private TextBox  _textUPC  = null!;
    private Label    _lblName  = null!;
    private Label    _lblPrice = null!;

    public frmCheckPrice(ICategoryService categoryService, IWindowService windowService)
    {
        _categoryService = categoryService;
        _windowService   = windowService;

        // Focus scan input after form is shown
        Shown += (_, _) => _textUPC.Focus();
    }

    protected override Color      AccentColor => AppColors.Info;
    protected override string     Icon        => "$";
    protected override string     Title       => "Check Price";
    protected override string     Subtitle    => "Scan or enter UPC to look up the price";
    protected override DialogSize Size        => DialogSize.Medium;
    protected override string     CancelText  => "✔  CLOSE";

    protected override Control BuildContent()
    {
        var panel = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.BackgroundPrimary,
            Padding   = new Padding(20, 16, 20, 8),
        };

        // ── UPC input ─────────────────────────────────────────────────────────
        var scanPanel = new Panel
        {
            Dock      = DockStyle.Top,
            Height    = 52,
            BackColor = AppColors.SurfaceMuted,
            Margin    = new Padding(0, 0, 0, 12),
        };
        scanPanel.Paint += (_, e) =>
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using var path   = ElegantButtonStyles.RoundedPath(new Rectangle(1, 1, scanPanel.Width - 2, scanPanel.Height - 2), 8);
            using var border = new Pen(AppColors.Info, 1.5f);
            e.Graphics.DrawPath(border, path);
        };

        _textUPC = new TextBox
        {
            Dock            = DockStyle.Fill,
            Font            = AppTypography.ScanInput,
            BackColor       = AppColors.SurfaceMuted,
            ForeColor       = AppColors.TextPrimary,
            BorderStyle     = BorderStyle.None,
            TextAlign       = HorizontalAlignment.Center,
            PlaceholderText = "Scan or type UPC…",
        };
        _textUPC.TextChanged += (_, _) => SearchProduct(_textUPC.Text);
        scanPanel.Controls.Add(_textUPC);

        // ── Product name ──────────────────────────────────────────────────────
        _lblName = new Label
        {
            Text      = "—",
            Font      = AppTypography.SectionTitle,
            ForeColor = AppColors.TextPrimary,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Top,
            Height    = 40,
            TextAlign = ContentAlignment.MiddleCenter,
        };

        // ── Price ─────────────────────────────────────────────────────────────
        _lblPrice = new Label
        {
            Text      = "—",
            Font      = new Font("Montserrat", 42F, FontStyle.Bold),
            ForeColor = AppColors.AccentGreen,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
        };

        panel.Controls.Add(_lblPrice);
        panel.Controls.Add(_lblName);
        panel.Controls.Add(scanPanel);
        return panel;
    }

    private async void SearchProduct(string upc)
    {
        if (string.IsNullOrWhiteSpace(upc)) return;
        try
        {
            if (upc.Length > 2 && upc.StartsWith("20"))
            {
                decimal price = decimal.Parse(upc.Substring(7, 4)) / 100;
                string  code  = upc.Substring(1, 5);
                var plu = await _categoryService.LoadProductByUPC_Promotion(code);
                if (plu != null) { _lblName.Text = plu.Name; _lblPrice.Text = price.ToString("C"); _textUPC.Clear(); }
            }
            else
            {
                var plu = await _categoryService.LoadProductByUPC_Promotion(upc);
                if (plu != null) { _lblName.Text = plu.Name; _lblPrice.Text = plu.Price?.ToString("C") ?? "—"; _textUPC.Clear(); }
            }
        }
        catch (Exception ex) { _windowService.OpenError(ex.Message); }
    }
}
