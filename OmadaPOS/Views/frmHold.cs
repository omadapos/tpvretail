using OmadaPOS.Libreria.Utils;
using OmadaPOS.Models;
using OmadaPOS.Services;
using OmadaPOS.Services.Navigation;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace OmadaPOS.Views;

public sealed class frmHold : POSDialog
{
    private readonly IHoldService             _holdService;
    private readonly IShoppingCart            _shoppingCart;
    private readonly IHomeInteractionService  _homeInteractionService;

    private readonly BindingList<HoldCartModel> _heldCarts = new();
    private ListBox  _listBox      = null!;
    private Button   _btnHoldIt    = null!;
    private Button   _btnRestore   = null!;
    private ListView _lvPreview    = null!;
    private Label    _lblMeta      = null!;
    private Label    _lblTotal     = null!;
    private Panel    _previewPanel = null!;

    // Color tags for held carts (up to 10 simultaneous holds)
    private static readonly string[] _colorTags =
        ["Red","Blue","Green","Yellow","Orange","Purple","Pink","Brown","Gray","Black"];

    // Mapping from tag name → display color
    private static readonly Dictionary<string, Color> _tagColors = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Red"]    = AppColors.Danger,
        ["Blue"]   = AppColors.PaymentCredit,
        ["Green"]  = AppColors.AccentGreen,
        ["Yellow"] = AppColors.Warning,
        ["Orange"] = Color.FromArgb(253, 126,  20),
        ["Purple"] = AppColors.PaymentGiftCard,
        ["Pink"]   = Color.FromArgb(214,  51, 132),
        ["Brown"]  = Color.FromArgb(139,  69,  19),
        ["Gray"]   = AppColors.SurfaceMuted,
        ["Black"]  = AppColors.BackgroundPrimary,
    };

    // Fonts cached as static fields — never allocate in DrawItem
    private static readonly Font _drawInitFont  = new("Segoe UI", 11f, FontStyle.Bold);
    private static readonly Font _drawTitleFont = new("Segoe UI", 11f, FontStyle.Bold);
    private static readonly Font _drawSubFont   = new("Segoe UI",  9f);

    public frmHold(
        IHoldService holdService,
        IShoppingCart shoppingCart,
        IHomeInteractionService homeInteractionService)
    {
        _holdService            = holdService;
        _shoppingCart           = shoppingCart;
        _homeInteractionService = homeInteractionService;

        Shown += async (_, _) => await LoadAsync();
    }

    protected override Color      AccentColor => AppColors.Warning;
    protected override string     Icon        => "⏸";
    protected override string     Title       => "Hold Cart";
    protected override string     Subtitle    => "Save current cart or restore a previous one";
    protected override DialogSize Size        => DialogSize.Wide;

    protected override string? ConfirmText => null;
    protected override string  CancelText  => "✕  CLOSE";

    protected override Control BuildContent()
    {
        // ── Root: two columns ────────────────────────────────────────────────
        var root = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 2,
            RowCount    = 1,
            BackColor   = AppColors.BackgroundPrimary,
            Padding     = new Padding(16, 12, 16, 10),
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        root.Controls.Add(BuildLeftColumn(),  0, 0);
        root.Controls.Add(BuildRightColumn(), 1, 0);
        return root;
    }

    // ── LEFT column: hold list + hold-it / cancel buttons ────────────────────
    private Control BuildLeftColumn()
    {
        var outer = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 1,
            RowCount    = 2,
            BackColor   = Color.Transparent,
            Padding     = new Padding(0, 0, 8, 0),
        };
        outer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        outer.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));

        _listBox = new ListBox
        {
            Dock           = DockStyle.Fill,
            Font           = AppTypography.Body,
            BackColor      = AppColors.SurfaceCard,
            ForeColor      = AppColors.TextPrimary,
            BorderStyle    = BorderStyle.FixedSingle,
            IntegralHeight = false,
            DrawMode       = DrawMode.OwnerDrawFixed,
            ItemHeight     = 60,
            DataSource     = _heldCarts,
        };
        _listBox.DrawItem             += ListBox_DrawItem;
        _listBox.SelectedIndexChanged += ListBox_SelectionChanged;

        var btnRow = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 2,
            RowCount    = 1,
            BackColor   = Color.Transparent,
            Padding     = new Padding(0),
            Margin      = new Padding(0),
        };
        btnRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        btnRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        btnRow.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var btnCancel = new Button { Text = "✕  CANCEL", Dock = DockStyle.Fill, Margin = new Padding(0, 4, 4, 0) };
        ElegantButtonStyles.Style(btnCancel, AppColors.Danger, AppColors.TextWhite, fontSize: 14f);
        btnCancel.Click += (_, _) => Close();

        _btnHoldIt = new Button { Text = "⏸  HOLD CART", Dock = DockStyle.Fill, Margin = new Padding(4, 4, 0, 0) };
        ElegantButtonStyles.Style(_btnHoldIt, AppColors.Warning, AppColors.TextWhite, fontSize: 14f);
        _btnHoldIt.Click += HoldIt_Click;

        btnRow.Controls.Add(btnCancel,  0, 0);
        btnRow.Controls.Add(_btnHoldIt, 1, 0);

        outer.Controls.Add(_listBox, 0, 0);
        outer.Controls.Add(btnRow,   0, 1);
        return outer;
    }

    // ── RIGHT column: item preview ────────────────────────────────────────────
    private Control BuildRightColumn()
    {
        _previewPanel = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.SurfaceCard,
            Padding   = new Padding(0, 0, 0, 60),
        };

        // Header
        var header = new Panel
        {
            Dock      = DockStyle.Top,
            Height    = 44,
            BackColor = AppColors.NavyBase,
        };
        header.Controls.Add(new Label
        {
            Text      = "🛒  Cart Preview",
            Font      = new Font("Segoe UI", 11F, FontStyle.Bold),
            ForeColor = AppColors.TextWhite,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
        });

        // Meta info (cashier, date)
        _lblMeta = new Label
        {
            Text      = "Select a held cart to preview its items.",
            Dock      = DockStyle.Top,
            Height    = 38,
            Font      = AppTypography.Body,
            ForeColor = AppColors.TextSecondary,
            BackColor = Color.FromArgb(248, 250, 252),
            TextAlign = ContentAlignment.MiddleCenter,
            Padding   = new Padding(8, 0, 8, 0),
        };

        // Item list
        _lvPreview = new ListView
        {
            Dock          = DockStyle.Fill,
            View          = View.Details,
            FullRowSelect = true,
            GridLines     = false,
            MultiSelect   = false,
            HideSelection = true,
            BackColor     = AppColors.SurfaceCard,
            ForeColor     = AppColors.TextPrimary,
            Font          = new Font("Segoe UI", 9.5f),
            BorderStyle   = BorderStyle.None,
            HeaderStyle   = ColumnHeaderStyle.Nonclickable,
            UseCompatibleStateImageBehavior = false,
        };
        _lvPreview.Columns.Add("Product", 180, HorizontalAlignment.Left);
        _lvPreview.Columns.Add("Qty",      50, HorizontalAlignment.Center);
        _lvPreview.Columns.Add("Price",    70, HorizontalAlignment.Right);
        _lvPreview.Columns.Add("Total",    70, HorizontalAlignment.Right);
        _lvPreview.Resize += (_, _) =>
        {
            int fixed_ = 50 + 70 + 70;
            _lvPreview.Columns[0].Width = Math.Max(_lvPreview.ClientSize.Width - fixed_ - 4, 100);
        };
        ListViewTheme.Apply(_lvPreview);

        // Footer: total + restore button
        var footer = new Panel
        {
            Dock      = DockStyle.Bottom,
            Height    = 60,
            BackColor = AppColors.NavyDark,
            Padding   = new Padding(8, 8, 8, 8),
        };

        _lblTotal = new Label
        {
            Text      = "",
            Dock      = DockStyle.Fill,
            Font      = new Font("Segoe UI", 13F, FontStyle.Bold),
            ForeColor = AppColors.TextWhite,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleLeft,
        };

        _btnRestore = new Button
        {
            Text    = "▶  RESTORE",
            Dock    = DockStyle.Right,
            Width   = 130,
            Enabled = false,
        };
        ElegantButtonStyles.Style(_btnRestore, AppColors.AccentGreen, AppColors.TextWhite, fontSize: 13f);
        _btnRestore.Click += async (_, _) => await RestoreSelectedAsync();

        footer.Controls.Add(_lblTotal);
        footer.Controls.Add(_btnRestore);

        _previewPanel.Controls.Add(_lvPreview);
        _previewPanel.Controls.Add(_lblMeta);
        _previewPanel.Controls.Add(header);
        _previewPanel.Controls.Add(footer);
        return _previewPanel;
    }

    // ── Owner-draw: coloured circle + label + subtitle ───────────────────────
    private void ListBox_DrawItem(object? sender, DrawItemEventArgs e)
    {
        if (e.Index < 0 || e.Index >= _heldCarts.Count) return;

        var cart = _heldCarts[e.Index];
        var g    = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        bool selected = (e.State & DrawItemState.Selected) != 0;

        using var bgBrush = new SolidBrush(selected ? AppColors.NavyBase : AppColors.SurfaceCard);
        g.FillRectangle(bgBrush, e.Bounds);

        // Coloured circle
        var dotColor = _tagColors.TryGetValue(cart.HoldId, out var c) ? c : AppColors.TextMuted;
        int dotSize  = 36;
        int dotX     = e.Bounds.X + 12;
        int dotY     = e.Bounds.Y + (e.Bounds.Height - dotSize) / 2;
        var dotRect  = new Rectangle(dotX, dotY, dotSize, dotSize);
        using var dotBrush = new SolidBrush(dotColor);
        g.FillEllipse(dotBrush, dotRect);

        // Initial letter inside circle
        var initial  = cart.HoldId.Length > 0 ? cart.HoldId[0].ToString() : "?";
        using var initBrush = new SolidBrush(AppColors.TextWhite);
        var initSize = g.MeasureString(initial, _drawInitFont);
        g.DrawString(initial, _drawInitFont, initBrush,
            dotX + (dotSize - initSize.Width)  / 2,
            dotY + (dotSize - initSize.Height) / 2);

        int textX = dotX + dotSize + 12;
        int textY = e.Bounds.Y + 7;

        // Line 1: "Cart: Red  —  $12.50"
        using var titleBrush = new SolidBrush(selected ? AppColors.TextWhite : AppColors.TextPrimary);
        string title = cart.ItemTotal > 0
            ? $"Cart: {cart.HoldId}   ·   {cart.ItemTotal:C}"
            : $"Cart: {cart.HoldId}";
        g.DrawString(title, _drawTitleFont, titleBrush, textX, textY);

        // Line 2: "3 items  ·  02:45 PM  ·  John"
        string sub = $"{cart.ItemCount} item{(cart.ItemCount != 1 ? "s" : "")}  ·  {cart.LastModified:MM/dd hh:mm tt}";
        if (!string.IsNullOrWhiteSpace(cart.CashierName)) sub += $"  ·  {cart.CashierName}";
        using var subBrush = new SolidBrush(selected ? AppColors.OverlayLight : AppColors.TextSecondary);
        g.DrawString(sub, _drawSubFont, subBrush, textX, textY + 24);

        if (!selected)
        {
            using var sepPen = new Pen(AppColors.ShadowSubtle);
            g.DrawLine(sepPen, e.Bounds.Left + 8, e.Bounds.Bottom - 1, e.Bounds.Right - 8, e.Bounds.Bottom - 1);
        }
    }

    // ── Load held carts ───────────────────────────────────────────────────────
    private async Task LoadAsync()
    {
        try
        {
            await _shoppingCart.LoadCartAsync();
            var carts = await _holdService.GetHeldCartsBySessionAsync(WindowsIdProvider.GetMachineGuid());
            _heldCarts.Clear();
            foreach (var c in carts) _heldCarts.Add(c);

            _btnHoldIt.Enabled  = _shoppingCart.ItemCount > 0;
            _btnRestore.Enabled = false;
            ClearPreview();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading held carts: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ── Preview selected cart items ───────────────────────────────────────────
    private async void ListBox_SelectionChanged(object? sender, EventArgs e)
    {
        if (_listBox.SelectedIndex < 0) { ClearPreview(); return; }

        var selected = (HoldCartModel)_listBox.SelectedItem!;
        try
        {
            var items = await _holdService.RetrieveHeldCartPreviewAsync(selected.HoldId);

            _lvPreview.Items.Clear();
            foreach (var item in items)
            {
                var total = item.UnitPrice * (decimal)item.Quantity;
                var lvi   = new ListViewItem(item.ProductName);
                lvi.SubItems.Add(item.Quantity.ToString("G"));
                lvi.SubItems.Add(item.UnitPrice.ToString("C"));
                lvi.SubItems.Add(total.ToString("C"));
                _lvPreview.Items.Add(lvi);
            }

            // Meta label
            string meta = $"Held on {selected.LastModified:MM/dd/yyyy  hh:mm tt}";
            if (!string.IsNullOrWhiteSpace(selected.CashierName)) meta += $"   by {selected.CashierName}";
            _lblMeta.Text = meta;

            _lblTotal.Text      = $"Total:  {selected.ItemTotal:C}   ({selected.ItemCount} items)";
            _btnRestore.Enabled = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading cart preview: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ClearPreview()
    {
        _lvPreview.Items.Clear();
        _lblMeta.Text       = "Select a held cart to preview its items.";
        _lblTotal.Text      = "";
        _btnRestore.Enabled = false;
    }

    // ── Restore selected cart ─────────────────────────────────────────────────
    private async Task RestoreSelectedAsync()
    {
        if (_listBox.SelectedIndex < 0) return;
        try
        {
            var selected = (HoldCartModel)_listBox.SelectedItem!;

            string warning = _shoppingCart.ItemCount > 0
                ? $"Restore cart \"{selected.HoldId}\"?\nThis will replace the current cart ({_shoppingCart.ItemCount} item{(_shoppingCart.ItemCount != 1 ? "s" : "")})."
                : $"Restore cart \"{selected.HoldId}\"?";

            var confirm = MessageBox.Show(warning, "Restore Hold",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1);

            if (confirm != DialogResult.Yes) return;

            var items = await _holdService.RetrieveHeldCartAsync(selected.HoldId);

            _shoppingCart.Clear();
            foreach (var item in items) _shoppingCart.AddItem(item);

            await _holdService.DeleteHeldCartAsync(selected.HoldId);
            _homeInteractionService.RequestCartRefresh();
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error restoring cart: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ── Save current cart to hold ─────────────────────────────────────────────
    private async void HoldIt_Click(object? sender, EventArgs e)
    {
        if (_shoppingCart.ItemCount <= 0) return;

        _btnHoldIt.Enabled = false;
        try
        {
            string tag = "";
            foreach (var color in _colorTags)
            {
                var existing = await _holdService.GetHeldCartsByIdAsync(color);
                if (existing == null) { tag = color; break; }
            }

            if (tag == "")
            {
                MessageBox.Show("All hold slots are in use. Please restore a held cart first.",
                    "Hold Slots Full", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool ok = await _holdService.HoldCartAsync(WindowsIdProvider.GetMachineGuid(), tag);
            if (!ok)
                MessageBox.Show("Could not save cart. Please try again.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving cart: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            if (!IsDisposed && !Disposing)
                _btnHoldIt.Enabled = true;
        }
    }
}
