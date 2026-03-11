using OmadaPOS.Presentation.Styling;

namespace OmadaPOS.Views;

/// <summary>
/// Uniform base form for all POS popup dialogs.
/// Provides consistent header, sizing, footer and styling.
/// Subclasses only override abstract members — no Designer needed.
///
/// Sizes (W × H):
///   Compact  480 × 400  — messages, single-action dialogs
///   Medium   520 × 580  — forms with a few fields
///   Wide     660 × 660  — lists or multi-section content
/// </summary>
public abstract class POSDialog : Form
{
    public enum DialogSize { Compact, Medium, Wide, ExtraWide }

    private static readonly Dictionary<DialogSize, Size> _sizes = new()
    {
        { DialogSize.Compact,   new Size(480, 400) },
        { DialogSize.Medium,    new Size(520, 580) },
        { DialogSize.Wide,      new Size(660, 660) },
        { DialogSize.ExtraWide, new Size(960, 760) },
    };

    private const int HeaderH = 86;
    private const int FooterH = 64;

    // ── Abstract: subclasses define identity ──────────────────────────────────
    protected abstract Color  AccentColor { get; }
    protected abstract string Icon        { get; }
    protected abstract string Title       { get; }
    protected abstract string Subtitle    { get; }

    // ── Virtual: subclasses override to change behaviour ─────────────────────

    /// <summary>Predefined form size. Override to change.</summary>
    protected virtual DialogSize Size => DialogSize.Medium;

    /// <summary>
    /// Text for the right (confirm/primary) footer button.
    /// Return null to show a single centred CLOSE button instead.
    /// </summary>
    protected virtual string? ConfirmText => null;

    /// <summary>Text for the left (cancel/close) footer button.</summary>
    protected virtual string CancelText => "✕  CLOSE";

    /// <summary>
    /// Called when the confirm button is pressed.
    /// Return true to close the dialog, false to keep it open.
    /// </summary>
    protected virtual Task<bool> OnConfirmAsync() => Task.FromResult(true);

    /// <summary>
    /// Build and return the main content control.
    /// It will be stretched to fill the area between header and footer.
    /// </summary>
    protected abstract Control BuildContent();

    // ── Shared confirm button ref (for enabling/disabling) ────────────────────
    private Button? _btnConfirm;

    // Outer panel held as field so Load can insert the content after derived
    // constructor has finished assigning its fields.
    private TableLayoutPanel _outer = null!;

    // ── Constructor ───────────────────────────────────────────────────────────
    protected POSDialog()
    {
        DoubleBuffered  = true;

        var sz = _sizes[Size];
        FormBorderStyle = FormBorderStyle.None;
        StartPosition   = FormStartPosition.CenterScreen;
        base.Size       = sz;
        MinimumSize     = sz;
        MaximumSize     = sz;
        BackColor       = AppColors.BackgroundPrimary;
        TopMost         = true;

        // Rounded border on Paint
        Paint += (_, e) =>
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using var path   = ElegantButtonStyles.RoundedPath(new Rectangle(1, 1, Width - 2, Height - 2), 14);
            using var border = new Pen(Color.FromArgb(60, 0, 0, 0), 1.5f);
            e.Graphics.DrawPath(border, path);
        };

        // Build the shell (header + placeholder content row + footer).
        // BuildContent() is intentionally NOT called here — it is deferred to
        // OnLoad so that derived-class constructors can assign their fields
        // before BuildContent() reads them.
        _outer = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 1,
            RowCount    = 3,
            BackColor   = Color.Transparent,
            Padding     = new Padding(0),
            Margin      = new Padding(0),
        };
        _outer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        _outer.RowStyles.Add(new RowStyle(SizeType.Absolute, HeaderH));
        _outer.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        _outer.RowStyles.Add(new RowStyle(SizeType.Absolute, FooterH));

        _outer.Controls.Add(BuildHeader(), 0, 0);
        _outer.Controls.Add(BuildFooter(), 0, 2);

        Controls.Add(_outer);

        // Insert content after all constructors have run.
        Load += (_, _) => _outer.Controls.Add(BuildContent(), 0, 1);
    }

    // ── Header ────────────────────────────────────────────────────────────────
    private Panel BuildHeader()
    {
        var header = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.NavyDark,
            Padding   = new Padding(0),
        };

        header.Paint += (_, e) =>
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using var accent = new SolidBrush(AccentColor);
            e.Graphics.FillRectangle(accent, 0, 0, 5, header.Height);
            using var line = new Pen(AccentColor, 2f);
            e.Graphics.DrawLine(line, 0, header.Height - 1, header.Width, header.Height - 1);
        };

        var lblIcon = new Label
        {
            Text      = Icon,
            Font      = new Font("Segoe UI", 22F, FontStyle.Bold),
            ForeColor = AccentColor,
            BackColor = Color.Transparent,
            Size      = new Size(60, HeaderH),
            Location  = new Point(12, 0),
            TextAlign = ContentAlignment.MiddleCenter,
        };

        var lblTitle = new Label
        {
            Text      = Title,
            Font      = AppTypography.SectionTitle,
            ForeColor = AppColors.TextWhite,
            BackColor = Color.Transparent,
            Location  = new Point(76, 10),
            Size      = new Size(Width - 84, 36),
            TextAlign = ContentAlignment.BottomLeft,
        };

        var lblSub = new Label
        {
            Text      = Subtitle,
            Font      = AppTypography.Caption,
            ForeColor = AppColors.TextMuted,
            BackColor = Color.Transparent,
            Location  = new Point(78, 48),
            Size      = new Size(Width - 86, 28),
            TextAlign = ContentAlignment.TopLeft,
        };

        header.Controls.Add(lblSub);
        header.Controls.Add(lblTitle);
        header.Controls.Add(lblIcon);
        return header;
    }

    // ── Footer ────────────────────────────────────────────────────────────────
    private Panel BuildFooter()
    {
        var footer = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.SurfaceMuted,
            Padding   = new Padding(0),
        };

        if (ConfirmText is string confirmLabel)
        {
            // Two buttons: CANCEL (40%) + CONFIRM (60%)
            var layout = new TableLayoutPanel
            {
                Dock        = DockStyle.Fill,
                ColumnCount = 2,
                RowCount    = 1,
                BackColor   = Color.Transparent,
                Padding     = new Padding(0),
                Margin      = new Padding(0),
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var btnCancel = new Button { Text = CancelText, Dock = DockStyle.Fill, Margin = new Padding(8, 8, 4, 8) };
            ElegantButtonStyles.Style(btnCancel, AppColors.Danger, AppColors.TextWhite, fontSize: 16f);
            btnCancel.Click += (_, _) => Close();

            _btnConfirm = new Button { Text = confirmLabel, Dock = DockStyle.Fill, Margin = new Padding(4, 8, 8, 8) };
            ElegantButtonStyles.Style(_btnConfirm, AccentColor, AppColors.TextWhite, fontSize: 16f);
            _btnConfirm.Click += BtnConfirm_Click;

            layout.Controls.Add(btnCancel,   0, 0);
            layout.Controls.Add(_btnConfirm, 1, 0);
            footer.Controls.Add(layout);
        }
        else
        {
            // Single centred CLOSE button
            var btnClose = new Button { Text = CancelText, Dock = DockStyle.Fill, Margin = new Padding(80, 8, 80, 8) };
            ElegantButtonStyles.Style(btnClose, AppColors.SlateBlue, AppColors.TextWhite, fontSize: 16f);
            btnClose.Click += (_, _) => Close();
            footer.Controls.Add(btnClose);
        }

        return footer;
    }

    // ── Confirm handler ───────────────────────────────────────────────────────
    private async void BtnConfirm_Click(object? sender, EventArgs e)
    {
        if (_btnConfirm != null) _btnConfirm.Enabled = false;
        try
        {
            if (await OnConfirmAsync()) Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            if (!IsDisposed && _btnConfirm != null) _btnConfirm.Enabled = true;
        }
    }

    // ── Helper: create styled input field (label + textbox) ──────────────────
    protected static Panel FieldPanel(string labelText, out TextBox textBox,
        string placeholder = "", bool readOnly = false)
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

        var tb = new TextBox
        {
            Dock            = DockStyle.Fill,
            Font            = AppTypography.Body,
            BackColor       = AppColors.SurfaceMuted,
            ForeColor       = AppColors.TextPrimary,
            BorderStyle     = BorderStyle.FixedSingle,
            PlaceholderText = placeholder,
            ReadOnly        = readOnly,
        };

        panel.Controls.Add(tb);
        panel.Controls.Add(lbl);
        textBox = tb;
        return panel;
    }
}
