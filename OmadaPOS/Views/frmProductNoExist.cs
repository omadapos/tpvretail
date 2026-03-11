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

    // ── Root layout ────────────────────────────────────────────────────────────
    //  Left  (200px): product image + source badge
    //  Center (flex):  all product fields in a clean card
    //  Right (290px):  price numpad
    //  Bottom (30px):  lookup status bar
    protected override Control BuildContent()
    {
        var root = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 3,
            RowCount    = 2,
            BackColor   = Color.Transparent,
            Padding     = new Padding(14, 10, 14, 6),
            Margin      = Padding.Empty,
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200F));  // image
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,  100F));  // info
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 290F));  // numpad
        root.RowStyles.Add(new RowStyle(SizeType.Percent,  100F));        // content
        root.RowStyles.Add(new RowStyle(SizeType.Absolute,  30F));        // status bar

        root.Controls.Add(BuildImagePanel(),  0, 0);
        root.Controls.Add(BuildInfoPanel(),   1, 0);
        root.Controls.Add(BuildNumpadPanel(), 2, 0);

        // ── Status bar ────────────────────────────────────────────────────────
        var statusBar = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.SurfaceMuted,
            Margin    = new Padding(0, 4, 0, 0),
        };

        // Thin top separator line
        statusBar.Paint += (_, e) =>
        {
            using var pen = new Pen(AppColors.SeparatorOnLight, 1);
            e.Graphics.DrawLine(pen, 0, 0, statusBar.Width, 0);
        };

        _lblStatus = new Label
        {
            Dock      = DockStyle.Fill,
            Font      = new Font("Segoe UI", 9F, FontStyle.Italic),
            ForeColor = AppColors.TextMuted,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleLeft,
            Text      = "🔍  Buscando información del producto…",
            Padding   = new Padding(8, 0, 0, 0),
        };
        statusBar.Controls.Add(_lblStatus);
        root.Controls.Add(statusBar, 0, 1);
        root.SetColumnSpan(statusBar, 3);

        return root;
    }

    // ── Image panel ────────────────────────────────────────────────────────────
    private Panel BuildImagePanel()
    {
        var outer = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = Color.Transparent,
            Margin    = new Padding(0, 0, 12, 0),
        };

        // Card with rounded-looking border via Paint
        var card = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.SurfaceMuted,
        };

        // Rounded border
        card.Paint += (_, e) =>
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using var pen = new Pen(AppColors.SeparatorOnLight, 1.5f);
            e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
        };

        // Photo area (fills card except source label at bottom)
        _pic = new PictureBox
        {
            Dock      = DockStyle.Fill,
            SizeMode  = PictureBoxSizeMode.Zoom,
            BackColor = Color.Transparent,
            Padding   = new Padding(10),
        };
        _pic.Paint += DrawImagePlaceholder;

        // Source badge at bottom of image card
        _lblSource = new Label
        {
            Dock      = DockStyle.Bottom,
            Height    = 32,
            Text      = "Buscando en catálogo…",
            Font      = new Font("Segoe UI", 9F, FontStyle.Italic),
            ForeColor = AppColors.TextMuted,
            BackColor = Color.FromArgb(8, 0, 0, 0),
            TextAlign = ContentAlignment.MiddleCenter,
        };

        card.Controls.Add(_pic);
        card.Controls.Add(_lblSource);
        outer.Controls.Add(card);
        return outer;
    }

    private static readonly Font _placeholderFont = new("Segoe UI", 10F);

    private void DrawImagePlaceholder(object? sender, PaintEventArgs e)
    {
        if (_pic.Image != null) return;
        var g  = e.Graphics;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        g.SmoothingMode     = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        var bounds = new RectangleF(0, 0, _pic.Width, _pic.Height);
        using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        using var bf = new SolidBrush(Color.FromArgb(80, AppColors.TextMuted));

        // Camera icon (Unicode)
        using var iconFont = new Font("Segoe UI", 36F);
        g.DrawString("📷", iconFont, bf,
            new RectangleF(0, bounds.Height / 2 - 48, bounds.Width, 52), sf);

        using var bf2 = new SolidBrush(AppColors.TextMuted);
        g.DrawString("Sin imagen", _placeholderFont, bf2,
            new RectangleF(0, bounds.Height / 2 + 12, bounds.Width, 28), sf);
    }

    // ── Info panel ─────────────────────────────────────────────────────────────
    private Panel BuildInfoPanel()
    {
        var outer = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = Color.Transparent,
            Padding   = new Padding(0, 0, 12, 0),
        };

        var tbl = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 1,
            RowCount    = 9,
            BackColor   = Color.Transparent,
            Padding     = Padding.Empty,
            Margin      = Padding.Empty,
        };
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        // Fixed pixel heights — sized to prevent clipping at 96 DPI
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));  // 0: UPC badge
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));  // 1: label NOMBRE
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 46F));  // 2: name textbox
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));  // 3: brand + line
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));  // 4: label CATEGORIA
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));  // 5: combo
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 10F));  // 6: spacer
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));  // 7: label IMPUESTOS
        tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));  // 8: toggles (fills remaining)

        // ── Row 0 — UPC badge ─────────────────────────────────────────────────
        var upcRow = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
        upcRow.Paint += (_, e) =>
        {
            using var pen = new Pen(AppColors.SeparatorOnLight, 1);
            e.Graphics.DrawLine(pen, 0, upcRow.Height - 1, upcRow.Width, upcRow.Height - 1);
        };
        var lblUpc = new Label
        {
            Dock      = DockStyle.Fill,
            Text      = $"UPC  {_upc}",
            Font      = AppTypography.ScanInput,
            ForeColor = AppColors.AccentGreen,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleLeft,
        };
        upcRow.Controls.Add(lblUpc);
        tbl.Controls.Add(upcRow, 0, 0);

        // ── Row 1 — Label "NOMBRE DEL PRODUCTO" ───────────────────────────────
        tbl.Controls.Add(FieldLabel("NOMBRE DEL PRODUCTO"), 0, 1);

        // ── Row 2 — Name textbox ──────────────────────────────────────────────
        _tbName = new TextBox
        {
            Dock            = DockStyle.Fill,
            Font            = AppTypography.Body,
            ForeColor       = AppColors.TextPrimary,
            BackColor       = Color.White,
            BorderStyle     = BorderStyle.FixedSingle,
            PlaceholderText = "Escribe el nombre del producto…",
            Margin          = new Padding(0, 2, 0, 2),
        };
        tbl.Controls.Add(_tbName, 0, 2);

        // ── Row 3 — Brand ─────────────────────────────────────────────────────
        _lblBrand = new Label
        {
            Dock      = DockStyle.Fill,
            Text      = "Marca: —",
            Font      = AppTypography.BodySmall,
            ForeColor = AppColors.TextSecondary,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleLeft,
        };
        tbl.Controls.Add(_lblBrand, 0, 3);

        // ── Row 4 — Label "CATEGORÍA" ─────────────────────────────────────────
        tbl.Controls.Add(FieldLabel("CATEGORÍA"), 0, 4);

        // ── Row 5 — Category combobox ─────────────────────────────────────────
        _cbCategory = new ComboBox
        {
            Dock          = DockStyle.Fill,
            Font          = AppTypography.Body,
            FlatStyle     = FlatStyle.Flat,
            BackColor     = Color.White,
            ForeColor     = AppColors.TextPrimary,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Margin        = new Padding(0, 2, 0, 2),
        };
        tbl.Controls.Add(_cbCategory, 0, 5);

        // ── Row 6 — Spacer ────────────────────────────────────────────────────
        tbl.Controls.Add(new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent }, 0, 6);

        // ── Row 7 — Label "IMPUESTOS / RESTRICCIONES" ────────────────────────
        tbl.Controls.Add(FieldLabel("IMPUESTOS / RESTRICCIONES"), 0, 7);

        // ── Row 8 — Toggle buttons (TAX / EBT / WIC) ─────────────────────────
        var toggleRow = new FlowLayoutPanel
        {
            Dock          = DockStyle.Fill,
            BackColor     = Color.Transparent,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents  = false,
            Padding       = new Padding(0, 4, 0, 4),
        };
        _btnTax = MakeToggle("TAX 7%", _taxOn);
        _btnEbt = MakeToggle("EBT",    _ebtOn);
        _btnWic = MakeToggle("WIC",    _wicOn);
        _btnTax.Click += (_, _) => { _taxOn = !_taxOn; RefreshToggle(_btnTax, _taxOn); };
        _btnEbt.Click += (_, _) => { _ebtOn = !_ebtOn; RefreshToggle(_btnEbt, _ebtOn); };
        _btnWic.Click += (_, _) => { _wicOn = !_wicOn; RefreshToggle(_btnWic, _wicOn); };
        toggleRow.Controls.AddRange([_btnTax, _btnEbt, _btnWic]);
        tbl.Controls.Add(toggleRow, 0, 8);

        outer.Controls.Add(tbl);
        return outer;
    }

    // ── Numpad panel ───────────────────────────────────────────────────────────
    private Panel BuildNumpadPanel()
    {
        // Outer border card
        var card = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.NavyDark,
            Margin    = Padding.Empty,
            Padding   = new Padding(0),
        };

        // Header strip inside the numpad card
        var header = new Panel
        {
            Dock      = DockStyle.Top,
            Height    = 62,
            BackColor = AppColors.NavyBase,
            Padding   = new Padding(0),
        };

        // Emerald accent line at bottom of header
        header.Paint += (_, e) =>
        {
            using var pen = new Pen(AppColors.AccentGreen, 2f);
            e.Graphics.DrawLine(pen, 0, header.Height - 2, header.Width, header.Height - 2);
        };

        var lblTitle = new Label
        {
            Dock      = DockStyle.Top,
            Height    = 38,
            Text      = "PRECIO DE VENTA",
            Font      = AppTypography.SectionTitle,
            ForeColor = AppColors.TextWhite,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.BottomCenter,
            Padding   = new Padding(0, 0, 0, 2),
        };

        var lblHint = new Label
        {
            Dock      = DockStyle.Top,
            Height    = 22,
            Text      = "Solo ingresa el precio",
            Font      = new Font("Segoe UI", 9F, FontStyle.Italic),
            ForeColor = AppColors.TextOnDarkMuted,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.TopCenter,
        };

        header.Controls.Add(lblHint);
        header.Controls.Add(lblTitle);

        _numpad = new NumericPadControl(NumericPadControl.PadMode.Money)
        {
            Dock    = DockStyle.Fill,
            Padding = new Padding(8, 8, 8, 8),
        };

        card.Controls.Add(_numpad);
        card.Controls.Add(header);
        return card;
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
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[frmProductNoExist] LoadCategories failed: {ex.Message}");
        }
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
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[frmProductNoExist] Local product lookup failed: {ex.Message}");
        }

        SetStatus("🌐  Buscando en Open Food Facts…");
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(12));

            using var req = new HttpRequestMessage(HttpMethod.Get,
                $"https://world.openfoodfacts.org/api/v2/product/{_upc}" +
                "?fields=product_name,product_name_en,brands,categories_tags,image_front_url");
            req.Headers.UserAgent.ParseAdd(AppConstants.UserAgent);

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
            req.Headers.UserAgent.ParseAdd(AppConstants.UserAgent);
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
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[frmProductNoExist] Image download failed: {ex.Message}");
        }
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
        Font      = new Font("Segoe UI", 9F, FontStyle.Bold),
        ForeColor = AppColors.TextSecondary,
        BackColor = Color.Transparent,
        TextAlign = ContentAlignment.BottomLeft,
        Padding   = new Padding(1, 0, 0, 2),
    };

    private static readonly Color _toggleActiveBg   = AppColors.AccentGreen;
    private static readonly Color _toggleInactiveBg = Color.FromArgb(240, 242, 246);
    private static readonly Color _toggleActiveFg   = AppColors.TextWhite;
    private static readonly Color _toggleInactiveFg = AppColors.TextSecondary;
    private static readonly Color _toggleActiveBd   = AppColors.AccentGreenDark;
    private static readonly Color _toggleInactiveBd = Color.FromArgb(210, 215, 225);

    private static Button MakeToggle(string label, bool active) => new()
    {
        Text      = label,
        Width     = 90,
        Height    = 44,
        Font      = new Font("Segoe UI", 11F, FontStyle.Bold),
        ForeColor = active ? _toggleActiveFg   : _toggleInactiveFg,
        BackColor = active ? _toggleActiveBg   : _toggleInactiveBg,
        FlatStyle = FlatStyle.Flat,
        Cursor    = Cursors.Hand,
        Margin    = new Padding(0, 0, 10, 0),
        FlatAppearance =
        {
            BorderSize               = 1,
            BorderColor              = active ? _toggleActiveBd : _toggleInactiveBd,
            MouseOverBackColor       = Color.Transparent,
            MouseDownBackColor       = Color.Transparent,
        },
    };

    private static void RefreshToggle(Button btn, bool active)
    {
        btn.ForeColor = active ? _toggleActiveFg   : _toggleInactiveFg;
        btn.BackColor = active ? _toggleActiveBg   : _toggleInactiveBg;
        btn.FlatAppearance.BorderColor = active ? _toggleActiveBd : _toggleInactiveBd;
        btn.Invalidate();
    }

    private static string? GetStr(JsonElement el, string key)
        => el.TryGetProperty(key, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;

    private void SetStatus(string msg)
    {
        if (_lblStatus.IsHandleCreated) _lblStatus.Invoke(() => _lblStatus.Text = msg);
        else _lblStatus.Text = msg;
    }
}
