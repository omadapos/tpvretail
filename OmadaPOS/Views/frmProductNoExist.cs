using OmadaPOS.Componentes;
using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Services;
using OmadaPOS.Presentation.Styling;
using System.Text.Json;

namespace OmadaPOS.Views;

/// <summary>
/// Shown when a scanned UPC is not found locally.
/// Looks up Open Food Facts for auto-fill, then lets the cashier
/// set price / category / flags before creating the product.
/// </summary>
public sealed class frmProductNoExist : POSDialog
{
    // ── Services ───────────────────────────────────────────────────────────────
    private readonly ICategoryService _categoryService;
    private readonly HttpClient       _http;
    private readonly string           _upc;

    // ── UI references ──────────────────────────────────────────────────────────
    private TextBox       _tbUpc        = null!;
    private TextBox       _tbName       = null!;
    private ComboBox      _cbCategory   = null!;
    private CheckBox      _chkTax       = null!;
    private CheckBox      _chkEbt       = null!;
    private CheckBox      _chkWic       = null!;
    private Label         _lblStatus    = null!;
    private NumericPadControl _numpad   = null!;

    // ── POSDialog identity ─────────────────────────────────────────────────────
    protected override Color  AccentColor => AppColors.Warning;
    protected override string Icon        => "⚠";
    protected override string Title       => "Producto no encontrado";
    protected override string Subtitle    => "Completa los datos para crear el producto";
    protected override DialogSize Size    => DialogSize.ExtraWide;
    protected override string? ConfirmText => "✔  GUARDAR PRODUCTO";
    protected override string  CancelText  => "✕  CANCELAR";

    // ── Constructor ────────────────────────────────────────────────────────────
    public frmProductNoExist(ICategoryService categoryService, HttpClient http, string upc)
    {
        _categoryService = categoryService;
        _http            = http;
        _upc             = upc;

        // Kick off data loading after the form is fully constructed.
        Load += async (_, _) => await LoadAllDataAsync();
    }

    // ── POSDialog: build content ───────────────────────────────────────────────
    protected override Control BuildContent()
    {
        var root = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 2,
            RowCount    = 1,
            BackColor   = Color.Transparent,
            Margin      = Padding.Empty,
            Padding     = new Padding(16, 12, 16, 8),
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 56F)); // fields
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 44F)); // numpad
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        root.Controls.Add(BuildFieldsPanel(), 0, 0);
        root.Controls.Add(BuildNumpadPanel(), 1, 0);

        return root;
    }

    // ── Left panel: form fields ────────────────────────────────────────────────
    private Panel BuildFieldsPanel()
    {
        var panel = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = Color.Transparent,
            Padding   = new Padding(0, 0, 14, 0),
        };

        // Status bar (bottom)
        _lblStatus = new Label
        {
            Dock      = DockStyle.Bottom,
            Height    = 26,
            Font      = AppTypography.Caption,
            ForeColor = AppColors.TextMuted,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleLeft,
            Text      = "Buscando información del producto…",
        };

        // Checkboxes row
        var chkRow = new FlowLayoutPanel
        {
            Dock          = DockStyle.Bottom,
            Height        = 46,
            BackColor     = Color.Transparent,
            FlowDirection = FlowDirection.LeftToRight,
            Padding       = new Padding(0, 6, 0, 0),
        };
        _chkTax = MakeCheckBox("Tax (7%)");
        _chkEbt = MakeCheckBox("EBT");
        _chkWic = MakeCheckBox("WIC");
        chkRow.Controls.AddRange(new Control[] { _chkTax, _chkEbt, _chkWic });

        // Category
        var catBlock = MakeLabeledComboBox("Categoría", out _cbCategory);

        // Name
        var nameBlock = FieldPanel("Nombre del producto", out _tbName, "Escribe el nombre…");

        // UPC (read-only)
        var upcBlock = FieldPanel("UPC / Código de barras", out _tbUpc, readOnly: true);
        _tbUpc.Text      = _upc;
        _tbUpc.Font      = AppTypography.ScanInput;
        _tbUpc.ForeColor = AppColors.AccentGreen;

        // Stack top-down (reverse order due to DockStyle.Top)
        panel.Controls.Add(_lblStatus);
        panel.Controls.Add(chkRow);
        panel.Controls.Add(catBlock);
        panel.Controls.Add(nameBlock);
        panel.Controls.Add(upcBlock);

        return panel;
    }

    // ── Right panel: numeric pad ───────────────────────────────────────────────
    private Panel BuildNumpadPanel()
    {
        var wrapper = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.SurfaceMuted,
            Padding   = new Padding(10),
        };
        wrapper.Paint += (_, e) =>
        {
            using var pen = new Pen(AppColors.SlateBlue, AppBorders.Thin);
            e.Graphics.DrawRectangle(pen, 0, 0, wrapper.Width - 1, wrapper.Height - 1);
        };

        var lblPriceTitle = new Label
        {
            Text      = "PRECIO DE VENTA",
            Font      = AppTypography.RowLabel,
            ForeColor = AppColors.TextSecondary,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Top,
            Height    = 32,
            TextAlign = ContentAlignment.BottomCenter,
            Padding   = new Padding(0, 0, 0, 4),
        };

        _numpad = new NumericPadControl(NumericPadControl.PadMode.Money)
        {
            Dock = DockStyle.Fill,
        };

        wrapper.Controls.Add(_numpad);
        wrapper.Controls.Add(lblPriceTitle);
        return wrapper;
    }

    // ── Data loading ───────────────────────────────────────────────────────────
    private async Task LoadAllDataAsync()
    {
        await Task.WhenAll(LoadCategoriesAsync(), LookupProductAsync());
    }

    private async Task LoadCategoriesAsync()
    {
        try
        {
            var cats = await _categoryService.LoadCategories();
            if (cats == null || cats.Count == 0) return;

            _cbCategory.Invoke(() =>
            {
                _cbCategory.DataSource    = cats;
                _cbCategory.DisplayMember = "Name";
                _cbCategory.ValueMember   = "Id";
                _cbCategory.SelectedIndex = 0;
            });
        }
        catch
        {
            // Categories are optional — user can still create the product
        }
    }

    private async Task LookupProductAsync()
    {
        // Step 1 — local API (already done by caller, but try again for extra info)
        SetStatus("🔍  Buscando en base de datos local…");
        try
        {
            var local = await _categoryService.LoadProductInfoByUPC(_upc);
            if (local != null && !string.IsNullOrWhiteSpace(local.Name))
            {
                SetField(_tbName, local.Name);
                SetStatus($"✔  Encontrado en base de datos local");
                return;
            }
        }
        catch { /* continue to external lookup */ }

        // Step 2 — Open Food Facts (free, no API key required)
        SetStatus("🌐  Buscando en catálogo global (Open Food Facts)…");
        try
        {
            using var cts      = new CancellationTokenSource(TimeSpan.FromSeconds(8));
            string    url      = $"https://world.openfoodfacts.org/api/v0/product/{_upc}.json";
            using var response = await _http.GetAsync(url, cts.Token);

            if (response.IsSuccessStatusCode)
            {
                string  json = await response.Content.ReadAsStringAsync(cts.Token);
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (root.TryGetProperty("status", out var status) && status.GetInt32() == 1
                    && root.TryGetProperty("product", out var product))
                {
                    string name = GetJsonString(product, "product_name")
                               ?? GetJsonString(product, "product_name_en")
                               ?? string.Empty;

                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        SetField(_tbName, name);
                        SetStatus($"✔  Encontrado en Open Food Facts");

                        // Auto-check EBT/tax hints from product categories
                        string cats = GetJsonString(product, "categories_tags") ?? "";
                        if (cats.Contains("beverages") || cats.Contains("snacks"))
                            SetCheck(_chkEbt, false);

                        return;
                    }
                }
            }

            SetStatus("ℹ  No encontrado en catálogo global. Ingresa el nombre manualmente.");
        }
        catch (OperationCanceledException)
        {
            SetStatus("⚠  La búsqueda externa tardó demasiado. Ingresa el nombre manualmente.");
        }
        catch
        {
            SetStatus("⚠  Sin conexión al catálogo global. Ingresa el nombre manualmente.");
        }
    }

    // ── POSDialog: confirm action ──────────────────────────────────────────────
    protected override async Task<bool> OnConfirmAsync()
    {
        string name  = _tbName.Text.Trim();
        decimal price = _numpad.ValueDecimal;

        if (string.IsNullOrWhiteSpace(name))
        {
            MessageBox.Show("Por favor escribe el nombre del producto.", "Nombre requerido",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        if (price <= 0)
        {
            MessageBox.Show("Por favor ingresa un precio mayor a cero.", "Precio requerido",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        int categoryId = _cbCategory.SelectedValue is int id ? id : 0;

        var model = new ProductCreateModel
        {
            Name          = name,
            Short_Name    = name.Length > 20 ? name[..20] : name,
            Description   = name,
            Price         = (double)price,
            Status        = 1,
            BranchId      = SessionManager.BranchId,
            Tax           = _chkTax.Checked ? 7 : 0,
            CategoryId    = categoryId,
            Category_Ids  = categoryId,
            Display_Addons = false,
            Display_Sides  = false,
            Upc           = _upc,
            Ebt           = _chkEbt.Checked,
            Wic           = _chkWic.Checked,
            Stock         = 0,
            Cost          = 0,
        };

        await _categoryService.SaveProduct(model);
        return true;
    }

    // ── Helpers ────────────────────────────────────────────────────────────────
    private static Panel MakeLabeledComboBox(string labelText, out ComboBox comboBox)
    {
        var panel = new Panel { Dock = DockStyle.Top, Height = 66, Padding = new Padding(0, 0, 0, 4) };

        var lbl = new Label
        {
            Text      = labelText,
            Font      = AppTypography.RowLabel,
            ForeColor = AppColors.TextSecondary,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Top,
            Height    = 22,
        };

        var cb = new ComboBox
        {
            Dock          = DockStyle.Fill,
            Font          = AppTypography.Body,
            FlatStyle     = FlatStyle.Flat,
            BackColor     = AppColors.SurfaceMuted,
            ForeColor     = AppColors.TextPrimary,
            DropDownStyle = ComboBoxStyle.DropDownList,
        };

        panel.Controls.Add(cb);
        panel.Controls.Add(lbl);
        comboBox = cb;
        return panel;
    }

    private static CheckBox MakeCheckBox(string text) =>
        new()
        {
            Text      = text,
            Font      = AppTypography.Body,
            ForeColor = AppColors.TextPrimary,
            BackColor = Color.Transparent,
            AutoSize  = true,
            Margin    = new Padding(0, 0, 18, 0),
        };

    private static string? GetJsonString(JsonElement element, string key)
        => element.TryGetProperty(key, out var v) && v.ValueKind == JsonValueKind.String
            ? v.GetString()
            : null;

    private void SetStatus(string message)
    {
        if (_lblStatus.IsHandleCreated)
            _lblStatus.Invoke(() => _lblStatus.Text = message);
        else
            _lblStatus.Text = message;
    }

    private static void SetField(TextBox tb, string value)
    {
        if (tb.IsHandleCreated)
            tb.Invoke(() => { if (string.IsNullOrWhiteSpace(tb.Text)) tb.Text = value; });
        else if (string.IsNullOrWhiteSpace(tb.Text))
            tb.Text = value;
    }

    private static void SetCheck(CheckBox cb, bool value)
    {
        if (cb.IsHandleCreated)
            cb.Invoke(() => cb.Checked = value);
        else
            cb.Checked = value;
    }
}
