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

    private CancellationTokenSource? _searchCts;

    public frmCheckPrice(ICategoryService categoryService, IWindowService windowService)
    {
        _categoryService = categoryService;
        _windowService   = windowService;

        Shown    += (_, _) => _textUPC.Focus();
        FormClosed += (_, _) => { _searchCts?.Cancel(); _searchCts?.Dispose(); };
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
        _textUPC.TextChanged += async (_, _) =>
        {
            _searchCts?.Cancel();
            _searchCts?.Dispose();
            _searchCts = new CancellationTokenSource();
            var token = _searchCts.Token;
            try
            {
                await Task.Delay(250, token);
                await SearchProductAsync(_textUPC.Text, token);
            }
            catch (OperationCanceledException) { }
        };
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
            Font      = new Font("Consolas", 42F, FontStyle.Bold),
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

    // Weight-embedded barcode format "20CCCCCPPPP?" requires at least 11 chars:
    // pos 0   = "2", pos 1 = "0", pos 1..5 = PLU code (5 digits), pos 7..10 = price (4 digits).
    private const int WeightBarcodeMinLength = 11;

    private async Task SearchProductAsync(string upc, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(upc)) return;
        try
        {
            if (upc.Length >= WeightBarcodeMinLength && upc.StartsWith("20"))
            {
                decimal price = decimal.Parse(upc.Substring(7, 4)) / 100;
                string  code  = upc.Substring(1, 5);
                ct.ThrowIfCancellationRequested();
                var plu = await _categoryService.LoadProductByUPC_Promotion(code);
                if (plu != null && !ct.IsCancellationRequested)
                {
                    _lblName.Text  = plu.Name;
                    _lblPrice.Text = price.ToString("C");
                    _textUPC.Clear();
                }
            }
            else
            {
                ct.ThrowIfCancellationRequested();
                var plu = await _categoryService.LoadProductByUPC_Promotion(upc);
                if (plu != null && !ct.IsCancellationRequested)
                {
                    _lblName.Text  = plu.Name;
                    _lblPrice.Text = plu.Price?.ToString("C") ?? "—";
                    _textUPC.Clear();
                }
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex) { _windowService.OpenError(ex.Message); }
    }
}
