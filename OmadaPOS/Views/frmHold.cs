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
    private ListBox _listBox   = null!;
    private Button  _btnHoldIt = null!;

    // Color tags for held carts (up to 10 simultaneous holds)
    private static readonly string[] _colorTags =
        ["Red","Blue","Green","Yellow","Orange","Purple","Pink","Brown","Gray","Black"];

    // Mapping from tag name → display color (matches Bootstrap palette for familiarity)
    private static readonly Dictionary<string, Color> _tagColors = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Red"]    = Color.FromArgb(220,  53,  69),
        ["Blue"]   = Color.FromArgb( 13, 110, 253),
        ["Green"]  = Color.FromArgb( 25, 135,  84),
        ["Yellow"] = Color.FromArgb(230, 160,   0),
        ["Orange"] = Color.FromArgb(253, 126,  20),
        ["Purple"] = Color.FromArgb(111,  66, 193),
        ["Pink"]   = Color.FromArgb(214,  51, 132),
        ["Brown"]  = Color.FromArgb(139,  69,  19),
        ["Gray"]   = Color.FromArgb(108, 117, 125),
        ["Black"]  = Color.FromArgb( 33,  37,  41),
    };

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

    // ── Footer: two buttons — HOLD IT (primary) + CANCEL ─────────────────────
    // We override BuildContent to include the buttons so we have full control
    // of layout (list above, buttons below), and return null ConfirmText so
    // POSDialog renders a single CLOSE which we replace below.
    protected override string? ConfirmText => null;
    protected override string  CancelText  => "✕  CLOSE";

    protected override Control BuildContent()
    {
        var outer = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 1,
            RowCount    = 2,
            BackColor   = AppColors.BackgroundPrimary,
            Padding     = new Padding(16, 12, 16, 10),
        };
        outer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        outer.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));

        // ── Held carts list (owner-draw: coloured circle + cart info) ────────
        _listBox = new ListBox
        {
            Dock           = DockStyle.Fill,
            Font           = AppTypography.Body,
            BackColor      = AppColors.SurfaceCard,
            ForeColor      = AppColors.TextPrimary,
            BorderStyle    = BorderStyle.FixedSingle,
            IntegralHeight = false,
            DrawMode       = DrawMode.OwnerDrawFixed,
            ItemHeight     = 52,
            DataSource     = _heldCarts,
        };
        _listBox.DrawItem             += ListBox_DrawItem;
        _listBox.SelectedIndexChanged += ListBox_SelectionChanged;

        // ── Button row ────────────────────────────────────────────────────────
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

        btnRow.Controls.Add(btnCancel,   0, 0);
        btnRow.Controls.Add(_btnHoldIt,  1, 0);

        outer.Controls.Add(_listBox, 0, 0);
        outer.Controls.Add(btnRow,   0, 1);
        return outer;
    }

    // ── Owner-draw: coloured circle + label + subtitle ───────────────────────
    private void ListBox_DrawItem(object? sender, DrawItemEventArgs e)
    {
        if (e.Index < 0 || e.Index >= _heldCarts.Count) return;

        var cart     = _heldCarts[e.Index];
        var g        = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        bool selected = (e.State & DrawItemState.Selected) != 0;

        // Background
        using var bgBrush = new SolidBrush(selected ? AppColors.NavyBase : AppColors.SurfaceCard);
        g.FillRectangle(bgBrush, e.Bounds);

        // Coloured circle on the left
        var dotColor = _tagColors.TryGetValue(cart.HoldId, out var c) ? c : Color.Gray;
        int dotSize  = 32;
        int dotX     = e.Bounds.X + 12;
        int dotY     = e.Bounds.Y + (e.Bounds.Height - dotSize) / 2;
        var dotRect  = new Rectangle(dotX, dotY, dotSize, dotSize);
        using var dotBrush = new SolidBrush(dotColor);
        g.FillEllipse(dotBrush, dotRect);

        // First letter of the tag inside the circle
        var initial = cart.HoldId.Length > 0 ? cart.HoldId[0].ToString() : "?";
        using var initFont  = new Font("Segoe UI", 11f, FontStyle.Bold);
        using var initBrush = new SolidBrush(Color.White);
        var initSize = g.MeasureString(initial, initFont);
        g.DrawString(initial, initFont, initBrush,
            dotX + (dotSize - initSize.Width)  / 2,
            dotY + (dotSize - initSize.Height) / 2);

        // Main label: "Cart: Red"
        int textX   = dotX + dotSize + 12;
        int textY   = e.Bounds.Y + 7;
        using var titleBrush = new SolidBrush(selected ? Color.White : AppColors.TextPrimary);
        using var titleFont  = new Font("Segoe UI", 11f, FontStyle.Bold);
        g.DrawString($"Cart: {cart.HoldId}", titleFont, titleBrush, textX, textY);

        // Subtitle: "3 items · 02:45 PM"
        string sub = $"{cart.ItemCount} item{(cart.ItemCount != 1 ? "s" : "")}  ·  {cart.LastModified:hh:mm tt}";
        using var subBrush = new SolidBrush(selected ? Color.FromArgb(200, 255, 255, 255) : AppColors.TextSecondary);
        using var subFont  = new Font("Segoe UI", 9f);
        g.DrawString(sub, subFont, subBrush, textX, textY + 22);

        // Thin bottom separator
        if (!selected)
        {
            using var sepPen = new Pen(Color.FromArgb(20, 0, 0, 0));
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

            _btnHoldIt.Enabled = _shoppingCart.ItemCount > 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading held carts: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ── Restore selected cart ─────────────────────────────────────────────────
    private async void ListBox_SelectionChanged(object? sender, EventArgs e)
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

            if (confirm != DialogResult.Yes)
            {
                _listBox.SelectedIndex = -1;
                return;
            }

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
        try
        {
            _btnHoldIt.Enabled = false;
            string tag = "";
            foreach (var color in _colorTags)
            {
                var existing = await _holdService.GetHeldCartsByIdAsync(color);
                if (existing == null) { tag = color; break; }
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
            _btnHoldIt.Enabled = true;
        }
    }
}
