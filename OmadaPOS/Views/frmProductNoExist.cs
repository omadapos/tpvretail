using OmadaPOS.Componentes;
using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Services;
using OmadaPOS.Presentation.Styling;
using System.Text.Json;

namespace OmadaPOS.Views;

/// <summary>
/// Shown when a scanned UPC is not found in the local backend.
/// Automatically queries Open Food Facts to pre-fill name, brand and image.
/// The cashier only needs to enter the price and confirm.
/// </summary>
public sealed class frmProductNoExist : POSDialog
{
    // ── Services ───────────────────────────────────────────────────────────────
    private readonly ICategoryService _categoryService;
    private readonly HttpClient       _http;
    private readonly string           _upc;

    // ── UI refs ────────────────────────────────────────────────────────────────
    private PictureBox        _pic        = null!;
    private Label             _lblSource  = null!;
    private Label             _lblStatus  = null!;
    private TextBox           _tbName     = null!;
    private Label             _lblBrand   = null!;
    private ComboBox          _cbCategory = null!;
    private Button            _btnTax     = null!;
    private Button            _btnEbt     = null!;
    private Button            _btnWic     = null!;
    private NumericPadControl _numpad     = null!;

    // ── Toggle state ───────────────────────────────────────────────────────────
    private bool _taxOn = true;
    private bool _ebtOn = false;
    private bool _wicOn = false;

    // ── POSDialog identity ─────────────────────────────────────────────────────
    protected override Color      AccentColor => AppColors.Warning;
    protected override string     Icon        => "⚠";
    protected override string     Title       => "Producto no encontrado";
    protected override string     Subtitle    => "Verifica los datos y asigna el precio de venta";
    protected override DialogSize Size        => DialogSize.ExtraWide;
    protected override string?    ConfirmText => "✔  GUARDAR PRODUCTO";
    protected override string     CancelText  => "✕  CANCELAR";

    public frmProductNoExist(ICategoryService categoryService, HttpClient http, string upc)
    {
        _categoryService = categoryService;
        _http            = http;
        _upc             = upc;

        Load += async (_, _) => await LoadAllDataAsync();
    }

    // ── Root layout: image | info | numpad ─────────────────────────────────────
    protected override Control BuildContent()
    {
        var root = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 3,
            RowCount    = 2,
            BackColor   = Color.Transparent,
            Padding     = new Padding(12, 8, 12, 4),
            Margin      = Padding.Empty,
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));  // image – fixed
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));  // info  – takes remaining
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260));  // numpad – fixed
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));        // content
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));        // status bar

        root.Controls.Add(BuildImagePanel(),  0, 0);
        root.Controls.Add(BuildInfoPanel(),   1, 0);
        root.Controls.Add(BuildNumpadPanel(), 2, 0);

        _lblStatus = new Label
        {
            Dock      = DockStyle.Fill,
            Font      = AppTypography.Caption,
            ForeColor = AppColors.TextMuted,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleLeft,
            Text      = "🔍  Buscando información del producto…",
            Padding   = new Padding(6, 0, 0, 0),
        };
        root.Controls.Add(_lblStatus, 0, 1);
        root.SetColumnSpan(_lblStatus, 3);

        return root;
    }

    // ── Image panel ────────────────────────────────────────────────────────────
    private Panel BuildImagePanel()
    {
        var panel = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.SurfaceMuted,
            Margin    = new Padding(0, 0, 10, 0),
        };
        panel.Paint += (_, e) =>
        {
            using var pen = new Pen(AppColors.SeparatorOnLight, 1);
            e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
        };

        _pic = new PictureBox
        {
            Dock     = DockStyle.Fill,
            SizeMode = PictureBoxSizeMode.Zoom,
            BackColor = Color.Transparent,
        };
        _pic.Paint += DrawImagePlaceholder;

        _lblSource = new Label
        {
            Dock      = DockStyle.Bottom,
            Height    = 28,
            Text      = "Buscando en catálogo…",
            Font      = AppTypography.Caption,
            ForeColor = AppColors.TextMuted,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleCenter,
        };

        panel.Controls.Add(_pic);
        panel.Controls.Add(_lblSource);
        return panel;
    }

    private void DrawImagePlaceholder(object? sender, PaintEventArgs e)
    {
        if (_pic.Image != null) return;
        var g = e.Graphics;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        using var sf   = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        using var bf   = new SolidBrush(AppColors.TextMuted);
        using var font = new Font("Segoe UI", 9F);

        // Simple icon placeholder
        g.DrawString("Sin imagen", font, bf,
            new RectangleF(0, 0, _pic.Width, _pic.Height), sf);
    }

    // ── Info panel: TableLayoutPanel with explicit rows ─────────────────────────
    private Panel BuildInfoPanel()
    {
        var outer = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = Color.Transparent,
            Padding   = new Padding(10, 0, 10, 0),
        };

        var tbl = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 1,
            RowCount    = 10,
            BackColor   = Color.Transparent,
            Padding     = Padding.Empty,
            Margin      = Padding.Empty,
        };
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        // Row heights
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));  // 0 UPC
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));  // 1 label "NOMBRE"
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));  // 2 name textbox
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));  // 3 brand
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 8F));   // 4 separator
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));  // 5 label "CATEGORÍA"
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));  // 6 combobox
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 8F));   // 7 separator
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));  // 8 label "IMPUESTOS"
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));  // 9 toggle buttons

        // Row 0 — UPC badge
        var lblUpc = new Label
        {
            Dock      = DockStyle.Fill,
            Text      = $"UPC   {_upc}",
            Font      = AppTypography.ScanInput,
            ForeColor = AppColors.AccentGreen,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding   = new Padding(0, 4, 0, 4),
        };
        tbl.Controls.Add(lblUpc, 0, 0);

        // Row 1 — Label "NOMBRE"
        tbl.Controls.Add(FieldLabel("NOMBRE DEL PRODUCTO"), 0, 1);

        // Row 2 — Name textbox
        _tbName = new TextBox
        {
            Dock            = DockStyle.Fill,
            Font            = AppTypography.Body,
            ForeColor       = AppColors.TextPrimary,
            BackColor       = AppColors.SurfaceMuted,
            BorderStyle     = BorderStyle.FixedSingle,
            PlaceholderText = "Escribe el nombre del producto…",
        };
        tbl.Controls.Add(_tbName, 0, 2);

        // Row 3 — Brand
        _lblBrand = new Label
        {
            Dock      = DockStyle.Fill,
            Text      = "Marca: —",
            Font      = AppTypography.Body,
            ForeColor = AppColors.TextSecondary,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleLeft,
        };
        tbl.Controls.Add(_lblBrand, 0, 3);

        // Row 4 — Separator
        tbl.Controls.Add(MakeSep(), 0, 4);

        // Row 5 — Label "CATEGORÍA"
        tbl.Controls.Add(FieldLabel("CATEGORÍA"), 0, 5);

        // Row 6 — Category combobox
        _cbCategory = new ComboBox
        {
            Dock          = DockStyle.Fill,
            Font          = AppTypography.Body,
            FlatStyle     = FlatStyle.Flat,
            BackColor     = AppColors.SurfaceMuted,
            ForeColor     = AppColors.TextPrimary,
            DropDownStyle = ComboBoxStyle.DropDownList,
        };
        tbl.Controls.Add(_cbCategory, 0, 6);

        // Row 7 — Separator
        tbl.Controls.Add(MakeSep(), 0, 7);

        // Row 8 — Label "IMPUESTOS"
        tbl.Controls.Add(FieldLabel("IMPUESTOS / RESTRICCIONES"), 0, 8);

        // Row 9 — Toggle buttons
        var toggleRow = new FlowLayoutPanel
        {
            Dock          = DockStyle.Fill,
            BackColor     = Color.Transparent,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents  = false,
            Padding       = new Padding(0, 2, 0, 2),
        };
        _btnTax = MakeToggle("TAX 7%", _taxOn);
        _btnEbt = MakeToggle("EBT",    _ebtOn);
        _btnWic = MakeToggle("WIC",    _wicOn);
        _btnTax.Click += (_, _) => { _taxOn = !_taxOn; RefreshToggle(_btnTax, _taxOn); };
        _btnEbt.Click += (_, _) => { _ebtOn = !_ebtOn; RefreshToggle(_btnEbt, _ebtOn); };
        _btnWic.Click += (_, _) => { _wicOn = !_wicOn; RefreshToggle(_btnWic, _wicOn); };
        toggleRow.Controls.Add(_btnTax);
        toggleRow.Controls.Add(_btnEbt);
        toggleRow.Controls.Add(_btnWic);
        tbl.Controls.Add(toggleRow, 0, 9);

        outer.Controls.Add(tbl);
        return outer;
    }

    // ── Numpad panel ───────────────────────────────────────────────────────────
    private Panel BuildNumpadPanel()
    {
        var panel = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.SurfaceMuted,
            Padding   = new Padding(10, 8, 10, 8),
            Margin    = new Padding(10, 0, 0, 0),
        };
        panel.Paint += (_, e) =>
        {
            using var pen = new Pen(AppColors.SeparatorOnLight, 1);
            e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
        };

        var lblTitle = new Label
        {
            Dock      = DockStyle.Top,
            Height    = 34,
            Text      = "PRECIO DE VENTA",
            Font      = AppTypography.SectionTitle,
            ForeColor = AppColors.TextPrimary,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleCenter,
        };

        var lblHint = new Label
        {
            Dock      = DockStyle.Top,
            Height    = 20,
            Text      = "Solo ingresa el precio",
            Font      = AppTypography.Caption,
            ForeColor = AppColors.TextMuted,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleCenter,
        };

        _numpad = new NumericPadControl(NumericPadControl.PadMode.Money)
        {
            Dock = DockStyle.Fill,
        };

        panel.Controls.Add(_numpad);
        panel.Controls.Add(lblHint);
        panel.Controls.Add(lblTitle);
        return panel;
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
        catch { }
    }

    private async Task LookupProductAsync()
    {
        SetStatus("🔍  Buscando en base de datos local…");
        try
        {
            var local = await _categoryService.LoadProductInfoByUPC(_upc);
            if (local != null && !string.IsNullOrWhiteSpace(local.Name))
            {
                ApplyData(local.Name, brand: null, source: "base de datos local");
                return;
            }
        }
        catch { }

        SetStatus("🌐  Buscando en Open Food Facts…");
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(12));

            using var req = new HttpRequestMessage(HttpMethod.Get,
                $"https://world.openfoodfacts.org/api/v2/product/{_upc}" +
                "?fields=product_name,product_name_en,brands,categories_tags,image_front_url");
            req.Headers.UserAgent.ParseAdd("OmadaPOS/1.0 (internal-use)");

            using var resp = await _http.SendAsync(req, cts.Token);
            if (!resp.IsSuccessStatusCode)
            {
                SetStatus("ℹ  No encontrado en catálogo global. Escribe el nombre manualmente.");
                return;
            }

            string json = await resp.Content.ReadAsStringAsync(cts.Token);
            using var doc  = JsonDocument.Parse(json);
            var       root = doc.RootElement;

            if (!root.TryGetProperty("status", out var st) || st.GetInt32() != 1
                || !root.TryGetProperty("product", out var p))
            {
                SetStatus("ℹ  No encontrado en catálogo global. Escribe el nombre manualmente.");
                return;
            }

            string  name     = GetStr(p, "product_name") ?? GetStr(p, "product_name_en") ?? string.Empty;
            string? brand    = GetStr(p, "brands");
            string? imageUrl = GetStr(p, "image_front_url");

            bool isBev = false;
            if (p.TryGetProperty("categories_tags", out var tags) && tags.ValueKind == JsonValueKind.Array)
                foreach (var t in tags.EnumerateArray())
                {
                    var s = t.GetString();
                    if (s != null && (s.Contains("beverages") || s.Contains("snacks") || s.Contains("drinks")))
                    { isBev = true; break; }
                }

            if (string.IsNullOrWhiteSpace(name))
            {
                SetStatus("ℹ  No encontrado. Escribe el nombre manualmente.");
                return;
            }

            ApplyData(name, brand, source: "Open Food Facts");
            if (isBev) Invoke(() => { _ebtOn = false; RefreshToggle(_btnEbt, false); });

            if (!string.IsNullOrWhiteSpace(imageUrl))
                await LoadImageAsync(imageUrl, cts.Token);

            SetStatus("✔  Datos cargados desde Open Food Facts. Solo ingresa el precio y guarda.");
        }
        catch (OperationCanceledException) { SetStatus("⚠  Tiempo de espera agotado. Escribe el nombre manualmente."); }
        catch                              { SetStatus("⚠  Sin acceso al catálogo global. Escribe el nombre manualmente."); }
    }

    private void ApplyData(string name, string? brand, string source)
    {
        Invoke(() =>
        {
            if (string.IsNullOrWhiteSpace(_tbName.Text)) _tbName.Text = name;
            _lblBrand.Text       = string.IsNullOrWhiteSpace(brand) ? "Marca: —" : $"Marca: {brand}";
            _lblSource.Text      = $"✔  {source}";
            _lblSource.ForeColor = AppColors.AccentGreen;
        });
    }

    private async Task LoadImageAsync(string url, CancellationToken ct)
    {
        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.UserAgent.ParseAdd("OmadaPOS/1.0 (internal-use)");
            using var res = await _http.SendAsync(req, ct);
            if (!res.IsSuccessStatusCode) return;

            byte[] bytes = await res.Content.ReadAsByteArrayAsync(ct);
            using var ms  = new System.IO.MemoryStream(bytes);
            var img = Image.FromStream(ms);

            if (_pic.IsHandleCreated)
                _pic.Invoke(() => { _pic.Image = img; _pic.Invalidate(); });
            else
                _pic.Image = img;
        }
        catch { }
    }

    // ── Confirm ────────────────────────────────────────────────────────────────
    protected override async Task<bool> OnConfirmAsync()
    {
        string  name  = _tbName.Text.Trim();
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

        int catId = _cbCategory.SelectedValue is int id ? id : 0;

        await _categoryService.SaveProduct(new ProductCreateModel
        {
            Name           = name,
            Short_Name     = name.Length > 20 ? name[..20] : name,
            Description    = name,
            Price          = (double)price,
            Status         = 1,
            BranchId       = SessionManager.BranchId,
            Tax            = _taxOn ? 7 : 0,
            CategoryId     = catId,
            Category_Ids   = catId,
            Display_Addons = false,
            Display_Sides  = false,
            Upc            = _upc,
            Ebt            = _ebtOn,
            Wic            = _wicOn,
            Stock          = 0,
            Cost           = 0,
        });
        return true;
    }

    // ── Helpers ────────────────────────────────────────────────────────────────
    private static Label FieldLabel(string text) => new()
    {
        Dock      = DockStyle.Fill,
        Text      = text,
        Font      = AppTypography.RowLabel,
        ForeColor = AppColors.TextSecondary,
        BackColor = Color.Transparent,
        TextAlign = ContentAlignment.BottomLeft,
        Padding   = new Padding(0, 0, 0, 2),
    };

    private static Panel MakeSep() => new()
    {
        Dock      = DockStyle.Fill,
        BackColor = AppColors.SurfaceMuted,
        Margin    = new Padding(0, 2, 0, 2),
    };

    private static Button MakeToggle(string label, bool active) => new()
    {
        Text      = label,
        Width     = 86,
        Height    = 38,
        Font      = AppTypography.RowLabel,
        ForeColor = active ? AppColors.TextWhite  : AppColors.TextSecondary,
        BackColor = active ? AppColors.AccentGreen : AppColors.SurfaceMuted,
        FlatStyle = FlatStyle.Flat,
        Cursor    = Cursors.Hand,
        Margin    = new Padding(0, 0, 8, 0),
        FlatAppearance =
        {
            BorderSize  = 1,
            BorderColor = active ? AppColors.AccentGreen : AppColors.SurfaceMuted,
        },
    };

    private static void RefreshToggle(Button btn, bool active)
    {
        btn.ForeColor = active ? AppColors.TextWhite  : AppColors.TextSecondary;
        btn.BackColor = active ? AppColors.AccentGreen : AppColors.SurfaceMuted;
        btn.FlatAppearance.BorderColor = active ? AppColors.AccentGreen : AppColors.SurfaceMuted;
    }

    private static string? GetStr(JsonElement el, string key)
        => el.TryGetProperty(key, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;

    private void SetStatus(string msg)
    {
        if (_lblStatus.IsHandleCreated) _lblStatus.Invoke(() => _lblStatus.Text = msg);
        else _lblStatus.Text = msg;
    }
}
