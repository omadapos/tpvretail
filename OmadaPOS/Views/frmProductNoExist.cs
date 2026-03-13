using System.Drawing.Drawing2D;
using System.Drawing.Text;
using OmadaPOS.Componentes;
using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Services;
using OmadaPOS.Presentation.Styling;
using OmadaPOS.Services;

namespace OmadaPOS.Views;

/// <summary>
/// Shown when a scanned UPC is not found in the local backend.
/// Queries the OmadaPOS global product catalog to pre-fill name, brand,
/// category, EBT/WIC flags, description and image.
/// The cashier only needs to enter the price and confirm.
/// </summary>
public sealed class frmProductNoExist : POSDialog
{
    // ── Services ───────────────────────────────────────────────────────────────
    private readonly ICategoryService        _categoryService;
    private readonly IExternalProductService _externalProduct;
    private readonly HttpClient              _http;
    private readonly string                  _upc;

    // ── UI refs ────────────────────────────────────────────────────────────────
    private PictureBox        _pic         = null!;
    private Label             _lblSourceBadge = null!;   // bottom of image card
    private Label             _lblStatus   = null!;      // bottom status bar
    private TextBox           _tbName      = null!;
    private Label             _lblBrand    = null!;
    private Label             _lblSizeWeight = null!;
    private Label             _lblDesc     = null!;
    private ComboBox          _cbCategory  = null!;
    private Button            _btnTax      = null!;
    private Button            _btnEbt      = null!;
    private Button            _btnWic      = null!;
    private NumericPadControl _numpad      = null!;

    // ── State ──────────────────────────────────────────────────────────────────
    private bool _taxOn = true;
    private bool _ebtOn = false;
    private bool _wicOn = false;
    private ExternalProductInfo? _apiInfo;

    // ── POSDialog identity ─────────────────────────────────────────────────────
    protected override Color      AccentColor => AppColors.Warning;
    protected override string     Icon        => "⚠";
    protected override string     Title       => "Producto no encontrado";
    protected override string     Subtitle    => "Verifica los datos, asigna el precio y guarda";
    protected override DialogSize Size        => DialogSize.ExtraWide;
    protected override string?    ConfirmText => "✔  GUARDAR PRODUCTO";
    protected override string     CancelText  => "✕  CANCELAR";

    public frmProductNoExist(ICategoryService categoryService, IExternalProductService externalProduct,
                             HttpClient http, string upc)
    {
        _categoryService = categoryService;
        _externalProduct = externalProduct;
        _http            = http;
        _upc             = upc;

        Load += async (_, _) => await LoadAllDataAsync();
    }

    // ══════════════════════════════════════════════════════════════════════════
    // ROOT LAYOUT
    // ══════════════════════════════════════════════════════════════════════════
    //
    //  ┌─────────────────────────────────────────────────────────────────────┐
    //  │  Col 0 (240px) │  Col 1 (flex)                │  Col 2 (280px)     │
    //  │  Image card    │  Product info sections        │  Price numpad      │
    //  │                │                               │                    │
    //  ├─────────────────────────────────────────────────────────────────────┤
    //  │  Status bar (spans all 3 columns, 28px)                             │
    //  └─────────────────────────────────────────────────────────────────────┘
    //
    protected override Control BuildContent()
    {
        var root = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 3,
            RowCount    = 2,
            BackColor   = AppColors.BackgroundPrimary,
            Padding     = new Padding(14, 12, 14, 4),
            Margin      = Padding.Empty,
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 240F));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,  100F));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 280F));
        root.RowStyles.Add(new RowStyle(SizeType.Percent,  100F));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute,  28F));

        root.Controls.Add(BuildImageCard(),  0, 0);
        root.Controls.Add(BuildInfoPanel(),  1, 0);
        root.Controls.Add(BuildNumpadPanel(), 2, 0);

        // ── Status bar ────────────────────────────────────────────────────────
        _lblStatus = new Label
        {
            Dock      = DockStyle.Fill,
            Font      = AppTypography.Caption,
            ForeColor = AppColors.TextMuted,
            BackColor = AppColors.BackgroundSecondary,
            TextAlign = ContentAlignment.MiddleLeft,
            Text      = "🔍  Consultando catálogo de productos…",
            Padding   = new Padding(10, 0, 0, 0),
        };
        root.Controls.Add(_lblStatus, 0, 1);
        root.SetColumnSpan(_lblStatus, 3);

        return root;
    }

    // ══════════════════════════════════════════════════════════════════════════
    // LEFT — IMAGE CARD
    // ══════════════════════════════════════════════════════════════════════════
    //
    //  ┌──────────────────────┐
    //  │  UPC  00012345678    │  ← green chip at top
    //  │                      │
    //  │     [product photo]  │  ← fills most of card
    //  │                      │
    //  │  ✔ Catálogo OmadaPOS │  ← source badge at bottom
    //  └──────────────────────┘
    //
    private Panel BuildImageCard()
    {
        var outer = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = Color.Transparent,
            Margin    = new Padding(0, 0, 12, 0),
        };

        // White rounded card
        var card = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = Color.White,
        };
        card.Paint += (_, e) =>
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using var pen = new Pen(Color.FromArgb(220, 226, 235), 1.5f);
            g.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
        };

        // ── UPC chip at top ───────────────────────────────────────────────────
        var upcChip = new Panel
        {
            Dock      = DockStyle.Top,
            Height    = 40,
            BackColor = Color.FromArgb(240, 253, 244),   // very light green tint
            Padding   = new Padding(10, 0, 10, 0),
        };
        upcChip.Paint += (_, e) =>
        {
            using var pen = new Pen(Color.FromArgb(187, 247, 208), 1f);
            e.Graphics.DrawLine(pen, 0, upcChip.Height - 1, upcChip.Width, upcChip.Height - 1);
        };
        upcChip.Controls.Add(new Label
        {
            Dock      = DockStyle.Fill,
            Text      = $"UPC  {_upc}",
            Font      = AppTypography.ScanInput,
            ForeColor = AppColors.AccentGreenDark,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleCenter,
        });

        // ── Product image ─────────────────────────────────────────────────────
        _pic = new PictureBox
        {
            Dock      = DockStyle.Fill,
            SizeMode  = PictureBoxSizeMode.Zoom,
            BackColor = Color.White,
            Padding   = new Padding(12),
        };
        _pic.Paint += DrawImagePlaceholder;

        // ── Source badge at bottom ────────────────────────────────────────────
        _lblSourceBadge = new Label
        {
            Dock      = DockStyle.Bottom,
            Height    = 34,
            Text      = "Buscando en catálogo…",
            Font      = new Font("Segoe UI", 9F, FontStyle.Italic),
            ForeColor = AppColors.TextMuted,
            BackColor = AppColors.SurfaceMuted,
            TextAlign = ContentAlignment.MiddleCenter,
        };
        _lblSourceBadge.Paint += (_, e) =>
        {
            using var pen = new Pen(Color.FromArgb(220, 226, 235), 1f);
            e.Graphics.DrawLine(pen, 0, 0, _lblSourceBadge.Width, 0);
        };

        card.Controls.Add(_pic);
        card.Controls.Add(_lblSourceBadge);
        card.Controls.Add(upcChip);
        outer.Controls.Add(card);
        return outer;
    }

    // ── Image placeholder drawn when no photo is available ────────────────────
    private static readonly Font _iconFont  = new("Segoe UI Emoji", 40F);
    private static readonly Font _hintFont  = new("Segoe UI", 10F, FontStyle.Regular);

    private void DrawImagePlaceholder(object? sender, PaintEventArgs e)
    {
        if (_pic.Image != null) return;
        var g = e.Graphics;
        g.SmoothingMode    = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        float cx = _pic.Width / 2f;
        float cy = _pic.Height / 2f;
        using var sfC = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        using var bMuted = new SolidBrush(Color.FromArgb(100, AppColors.TextMuted));
        using var bText  = new SolidBrush(AppColors.TextMuted);

        g.DrawString("📦", _iconFont, bMuted,
            new RectangleF(0, cy - 54, _pic.Width, 60), sfC);
        g.DrawString("Sin imagen disponible", _hintFont, bText,
            new RectangleF(0, cy + 12, _pic.Width, 24), sfC);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // CENTER — INFO PANEL
    // ══════════════════════════════════════════════════════════════════════════
    //
    //  ┌─────────────────────────────────────────────┐
    //  │  NOMBRE DEL PRODUCTO                        │  section header
    //  │  ┌─────────────────────────────────────┐   │
    //  │  │ Sun chips minis                     │   │  editable textbox
    //  │  └─────────────────────────────────────┘   │
    //  │  Marca: Sun Chips           42.5 g          │  chips inline
    //  │  ┌─────────────────────────────────────┐   │
    //  │  │ Chips multigrano en miniatura…      │   │  description (read-only)
    //  │  └─────────────────────────────────────┘   │
    //  ├─────────────────────────────────────────────┤  separator
    //  │  CATEGORÍA                                  │
    //  │  [  Snacks & Chips                     ▼ ] │
    //  ├─────────────────────────────────────────────┤  separator
    //  │  IMPUESTOS / RESTRICCIONES                  │
    //  │  [TAX 7%]  [EBT]  [WIC]                    │
    //  └─────────────────────────────────────────────┘
    //
    private Panel BuildInfoPanel()
    {
        var outer = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = Color.Transparent,
            Padding   = new Padding(0, 0, 10, 0),
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

        // Row heights — carefully sized to avoid clipping at 96 DPI
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));  // 0: section label NOMBRE
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 46F));  // 1: name textbox
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));  // 2: brand + size chips
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F));  // 3: description block
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 10F));  // 4: section separator
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));  // 5: section label CATEGORÍA
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 46F));  // 6: category combo
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 10F));  // 7: section separator
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));  // 8: section label IMPUESTOS
        tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));  // 9: toggles — fills rest

        // ── Row 0: Section header NOMBRE ─────────────────────────────────────
        tbl.Controls.Add(SectionLabel("NOMBRE DEL PRODUCTO"), 0, 0);

        // ── Row 1: Name textbox ───────────────────────────────────────────────
        _tbName = new TextBox
        {
            Dock            = DockStyle.Fill,
            Font            = new Font("Segoe UI", 14F, FontStyle.Regular),
            ForeColor       = AppColors.TextPrimary,
            BackColor       = Color.White,
            BorderStyle     = BorderStyle.FixedSingle,
            PlaceholderText = "Escribe el nombre del producto…",
            Margin          = new Padding(0, 2, 0, 4),
        };
        tbl.Controls.Add(_tbName, 0, 1);

        // ── Row 2: Brand + size chips ─────────────────────────────────────────
        var metaRow = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = Color.Transparent,
            Padding   = new Padding(0, 2, 0, 2),
        };
        _lblBrand = new Label
        {
            Dock      = DockStyle.Left,
            AutoSize  = false,
            Width     = 210,
            Text      = "Marca: —",
            Font      = AppTypography.BodySmall,
            ForeColor = AppColors.TextSecondary,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleLeft,
        };
        _lblSizeWeight = new Label
        {
            Dock      = DockStyle.Fill,
            Text      = string.Empty,
            Font      = AppTypography.BodySmall,
            ForeColor = AppColors.TextMuted,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleRight,
        };
        metaRow.Controls.Add(_lblSizeWeight);   // fill first
        metaRow.Controls.Add(_lblBrand);
        tbl.Controls.Add(metaRow, 0, 2);

        // ── Row 3: Description block (read-only, italic) ──────────────────────
        _lblDesc = new Label
        {
            Dock      = DockStyle.Fill,
            Text      = string.Empty,
            Font      = new Font("Segoe UI", 10F, FontStyle.Italic),
            ForeColor = AppColors.TextSecondary,
            BackColor = Color.FromArgb(248, 250, 252),
            TextAlign = ContentAlignment.TopLeft,
            Padding   = new Padding(8, 6, 8, 4),
            AutoSize  = false,
        };
        _lblDesc.Paint += (_, e) =>
        {
            using var pen = new Pen(Color.FromArgb(220, 226, 235), 1f);
            e.Graphics.DrawRectangle(pen, 0, 0, _lblDesc.Width - 1, _lblDesc.Height - 1);
        };
        tbl.Controls.Add(_lblDesc, 0, 3);

        // ── Row 4: Separator ──────────────────────────────────────────────────
        tbl.Controls.Add(SectionSeparator(), 0, 4);

        // ── Row 5: Section header CATEGORÍA ──────────────────────────────────
        tbl.Controls.Add(SectionLabel("CATEGORÍA"), 0, 5);

        // ── Row 6: Category combo ─────────────────────────────────────────────
        _cbCategory = new ComboBox
        {
            Dock          = DockStyle.Fill,
            Font          = AppTypography.Body,
            FlatStyle     = FlatStyle.Flat,
            BackColor     = Color.White,
            ForeColor     = AppColors.TextPrimary,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Margin        = new Padding(0, 2, 0, 4),
        };
        tbl.Controls.Add(_cbCategory, 0, 6);

        // ── Row 7: Separator ──────────────────────────────────────────────────
        tbl.Controls.Add(SectionSeparator(), 0, 7);

        // ── Row 8: Section header IMPUESTOS ──────────────────────────────────
        tbl.Controls.Add(SectionLabel("IMPUESTOS / RESTRICCIONES"), 0, 8);

        // ── Row 9: Toggle buttons ─────────────────────────────────────────────
        var flagRow = new FlowLayoutPanel
        {
            Dock          = DockStyle.Fill,
            BackColor     = Color.Transparent,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents  = false,
            Padding       = new Padding(0, 8, 0, 4),
        };
        _btnTax = MakeToggle("TAX 7%", _taxOn);
        _btnEbt = MakeToggle("EBT",    _ebtOn);
        _btnWic = MakeToggle("WIC",    _wicOn);
        _btnTax.Click += (_, _) => { _taxOn = !_taxOn; RefreshToggle(_btnTax, _taxOn); };
        _btnEbt.Click += (_, _) => { _ebtOn = !_ebtOn; RefreshToggle(_btnEbt, _ebtOn); };
        _btnWic.Click += (_, _) => { _wicOn = !_wicOn; RefreshToggle(_btnWic, _wicOn); };
        flagRow.Controls.AddRange([_btnTax, _btnEbt, _btnWic]);
        tbl.Controls.Add(flagRow, 0, 9);

        outer.Controls.Add(tbl);
        return outer;
    }

    // ══════════════════════════════════════════════════════════════════════════
    // RIGHT — PRICE NUMPAD
    // ══════════════════════════════════════════════════════════════════════════
    private Panel BuildNumpadPanel()
    {
        var card = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.NavyDark,
            Margin    = new Padding(4, 0, 0, 0),
        };

        // Header
        var header = new Panel
        {
            Dock      = DockStyle.Top,
            Height    = 68,
            BackColor = AppColors.NavyBase,
        };
        header.Paint += (_, e) =>
        {
            using var pen = new Pen(AppColors.AccentGreen, 2f);
            e.Graphics.DrawLine(pen, 0, header.Height - 2, header.Width, header.Height - 2);
        };

        // Title label (bottom-aligned inside header)
        header.Controls.Add(new Label
        {
            Dock      = DockStyle.Bottom,
            Height    = 24,
            Text      = "Solo ingresa el precio y guarda",
            Font      = new Font("Segoe UI", 9F, FontStyle.Italic),
            ForeColor = AppColors.TextOnDarkMuted,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.BottomCenter,
            Padding   = new Padding(0, 0, 0, 4),
        });
        header.Controls.Add(new Label
        {
            Dock      = DockStyle.Fill,
            Text      = "PRECIO DE VENTA",
            Font      = AppTypography.SectionTitle,
            ForeColor = AppColors.TextWhite,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleCenter,
        });

        _numpad = new NumericPadControl(NumericPadControl.PadMode.Money)
        {
            Dock    = DockStyle.Fill,
            Padding = new Padding(6, 8, 6, 8),
        };

        card.Controls.Add(_numpad);
        card.Controls.Add(header);
        return card;
    }

    // ══════════════════════════════════════════════════════════════════════════
    // DATA LOADING  (logic unchanged — only UI methods above were redesigned)
    // ══════════════════════════════════════════════════════════════════════════

    private async Task LoadAllDataAsync()
    {
        var catsTask = LoadCategoriesAsync();
        var apiTask  = LookupProductAsync();
        await Task.WhenAll(catsTask, apiTask);

        if (_apiInfo != null)
            TryAutoMatchCategory(_apiInfo.CategoryName);
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
            System.Diagnostics.Debug.WriteLine($"[frmProductNoExist] LoadCategories: {ex.Message}");
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
                ApplyLocalData(local.Name);
                return;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[frmProductNoExist] LocalLookup: {ex.Message}");
        }

        SetStatus("🌐  Consultando catálogo OmadaPOS…");
        try
        {
            using var cts  = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            var       info = await _externalProduct.LookupByUpcAsync(_upc, cts.Token).ConfigureAwait(false);

            if (info == null)
            {
                SetStatus("ℹ  No encontrado en catálogo. Escribe el nombre manualmente.");
                return;
            }

            _apiInfo = info;
            ApplyApiData(info);

            if (!string.IsNullOrWhiteSpace(info.ImageUrl))
                await LoadImageAsync(info.ImageUrl, cts.Token).ConfigureAwait(false);

            var statusMsg = info.RequiresAgeVerification
                ? "⚠  Producto con restricción de edad — se pedirá verificación al vender."
                : "✔  Datos cargados desde catálogo OmadaPOS. Ingresa el precio y guarda.";
            SetStatus(statusMsg);
        }
        catch (OperationCanceledException) { SetStatus("⚠  Tiempo agotado. Escribe el nombre manualmente."); }
        catch                              { SetStatus("⚠  Sin conexión al catálogo. Escribe el nombre manualmente."); }
    }

    // ── Apply helpers ─────────────────────────────────────────────────────────

    private void ApplyLocalData(string name)
    {
        Invoke(() =>
        {
            if (string.IsNullOrWhiteSpace(_tbName.Text)) _tbName.Text = name;
            SetSourceBadge("✔  Base de datos local", AppColors.AccentGreen);
        });
    }

    private void ApplyApiData(ExternalProductInfo info)
    {
        Invoke(() =>
        {
            if (string.IsNullOrWhiteSpace(_tbName.Text))
                _tbName.Text = info.Name;

            // Brand chip
            _lblBrand.Text = string.IsNullOrWhiteSpace(info.Brand)
                ? "Marca: —"
                : $"Marca: {info.Brand}";

            // Size / weight inline
            var parts = new List<string>(2);
            if (!string.IsNullOrWhiteSpace(info.Size))        parts.Add(info.Size);
            if (!string.IsNullOrWhiteSpace(info.WeightGrams)) parts.Add(info.WeightGrams);
            _lblSizeWeight.Text = string.Join("  ·  ", parts);

            // Description — prefer Spanish
            var desc = info.DescriptionEs ?? info.DescriptionEn;
            _lblDesc.Text = string.IsNullOrWhiteSpace(desc) ? string.Empty : desc;

            // EBT / WIC auto-toggle
            if (info.EbtEligible != _ebtOn) { _ebtOn = info.EbtEligible; RefreshToggle(_btnEbt, _ebtOn); }
            if (info.WicEligible != _wicOn) { _wicOn = info.WicEligible; RefreshToggle(_btnWic, _wicOn); }

            SetSourceBadge("✔  Catálogo OmadaPOS", AppColors.AccentGreen);
        });
    }

    private void SetSourceBadge(string text, Color fg)
    {
        _lblSourceBadge.Text      = text;
        _lblSourceBadge.ForeColor = fg;
    }

    private void TryAutoMatchCategory(string? apiCategoryName)
    {
        if (string.IsNullOrWhiteSpace(apiCategoryName)) return;
        if (_cbCategory.Items.Count == 0) return;

        _cbCategory.Invoke(() =>
        {
            var needle = apiCategoryName.Trim().ToLowerInvariant();
            for (int i = 0; i < _cbCategory.Items.Count; i++)
            {
                var localName = (_cbCategory.Items[i] as dynamic)?.Name?.ToString()?.ToLowerInvariant()
                                ?? string.Empty;
                if (localName.Contains(needle) || needle.Contains(localName))
                {
                    _cbCategory.SelectedIndex = i;
                    return;
                }
            }
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
            System.Diagnostics.Debug.WriteLine($"[frmProductNoExist] ImageLoad: {ex.Message}");
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

        int    catId       = _cbCategory.SelectedValue is int id ? id : 0;
        string description = _apiInfo?.DescriptionEs ?? _apiInfo?.DescriptionEn ?? name;

        await _categoryService.SaveProduct(new ProductCreateModel
        {
            Name           = name,
            Short_Name     = name.Length > 20 ? name[..20] : name,
            Description    = description,
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

    // ══════════════════════════════════════════════════════════════════════════
    // UI FACTORY HELPERS
    // ══════════════════════════════════════════════════════════════════════════

    private static Label SectionLabel(string text) => new()
    {
        Dock      = DockStyle.Fill,
        Text      = text,
        Font      = new Font("Segoe UI", 9F, FontStyle.Bold),
        ForeColor = AppColors.TextSecondary,
        BackColor = Color.Transparent,
        TextAlign = ContentAlignment.BottomLeft,
        Padding   = new Padding(0, 0, 0, 2),
    };

    private static Panel SectionSeparator()
    {
        var p = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
        p.Paint += (_, e) =>
        {
            using var pen = new Pen(Color.FromArgb(220, 226, 235), 1f);
            e.Graphics.DrawLine(pen, 0, p.Height / 2, p.Width, p.Height / 2);
        };
        return p;
    }

    // ── Toggle buttons ─────────────────────────────────────────────────────────
    private static readonly Color _tActiveBg  = AppColors.AccentGreen;
    private static readonly Color _tInactiveBg = Color.FromArgb(241, 245, 249);
    private static readonly Color _tActiveFg  = Color.White;
    private static readonly Color _tInactiveFg = AppColors.TextSecondary;
    private static readonly Color _tActiveBd  = AppColors.AccentGreenDark;
    private static readonly Color _tInactiveBd = Color.FromArgb(203, 213, 225);

    private static Button MakeToggle(string text, bool active) => new()
    {
        Text      = text,
        Width     = 100,
        Height    = 48,
        Font      = new Font("Segoe UI", 12F, FontStyle.Bold),
        ForeColor = active ? _tActiveFg   : _tInactiveFg,
        BackColor = active ? _tActiveBg   : _tInactiveBg,
        FlatStyle = FlatStyle.Flat,
        Margin    = new Padding(0, 0, 12, 0),
        FlatAppearance =
        {
            BorderSize        = 1,
            BorderColor       = active ? _tActiveBd  : _tInactiveBd,
            MouseOverBackColor = Color.Transparent,
            MouseDownBackColor = Color.Transparent,
        },
    };

    private static void RefreshToggle(Button btn, bool active)
    {
        btn.ForeColor = active ? _tActiveFg  : _tInactiveFg;
        btn.BackColor = active ? _tActiveBg  : _tInactiveBg;
        btn.FlatAppearance.BorderColor = active ? _tActiveBd : _tInactiveBd;
        btn.Invalidate();
    }

    private void SetStatus(string msg)
    {
        if (_lblStatus.IsHandleCreated)
            _lblStatus.Invoke(() => _lblStatus.Text = msg);
        else
            _lblStatus.Text = msg;
    }
}
