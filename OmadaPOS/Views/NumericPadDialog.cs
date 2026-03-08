using OmadaPOS.Componentes;
using OmadaPOS.Presentation.Styling;

namespace OmadaPOS.Views;

/// <summary>
/// Uniform base form for all numeric-pad popup dialogs.
/// Subclasses only override the abstract members — the layout,
/// sizing and styling are handled here once and applied consistently.
///
/// Fixed size: 480 × 700 px, touch-optimised for 15" Elo terminals.
/// Layout (top→bottom):
///   [Header  90px ] — accent bar + icon + title + subtitle
///   [Pad    fill  ] — NumericPadControl (mode set by subclass)
///   [Footer  68px ] — CANCEL · CONFIRM
/// </summary>
public abstract class NumericPadDialog : Form
{
    // ── Layout constants ──────────────────────────────────────────────────────
    private const int FormW      = 480;
    private const int FormH      = 700;
    private const int HeaderH    = 90;
    private const int FooterH    = 68;

    // ── Shared control refs ───────────────────────────────────────────────────
    protected readonly NumericPadControl Pad;
    private   readonly Button            _btnConfirm;

    // ── Abstract configuration ────────────────────────────────────────────────

    /// <summary>Numeric pad mode (Integer or Money).</summary>
    protected abstract NumericPadControl.PadMode PadMode { get; }

    /// <summary>Header accent color and confirm-button color.</summary>
    protected abstract Color AccentColor { get; }

    /// <summary>Header icon glyph (e.g. "#", "⌕", "$").</summary>
    protected abstract string Icon { get; }

    /// <summary>Main title shown in the header.</summary>
    protected abstract string Title { get; }

    /// <summary>Subtitle / hint shown below the title.</summary>
    protected abstract string Subtitle { get; }

    /// <summary>Text on the confirm button (e.g. "✔  CONFIRM").</summary>
    protected abstract string ConfirmText { get; }

    /// <summary>
    /// Executed when the user presses the confirm button.
    /// Return <c>true</c> to close the dialog, <c>false</c> to keep it open.
    /// </summary>
    protected abstract Task<bool> OnConfirmAsync(NumericPadControl pad);

    // ── Constructor ───────────────────────────────────────────────────────────
    protected NumericPadDialog()
    {
        // ── Form chrome ───────────────────────────────────────────────────────
        FormBorderStyle = FormBorderStyle.None;
        StartPosition   = FormStartPosition.CenterParent;
        Size            = new Size(FormW, FormH);
        MinimumSize     = new Size(FormW, FormH);
        MaximumSize     = new Size(FormW, FormH);
        BackColor       = AppColors.BackgroundPrimary;
        TopMost         = true;

        // Rounded border
        Paint += (_, e) =>
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using var path   = ElegantButtonStyles.RoundedPath(new Rectangle(1, 1, Width - 2, Height - 2), 14);
            using var border = new Pen(Color.FromArgb(60, 255, 255, 255), 1.5f);
            e.Graphics.DrawPath(border, path);
        };

        // ── Outer layout ──────────────────────────────────────────────────────
        var outer = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 1,
            RowCount    = 3,
            BackColor   = Color.Transparent,
            Margin      = new Padding(0),
            Padding     = new Padding(0),
        };
        outer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        outer.RowStyles.Add(new RowStyle(SizeType.Absolute, HeaderH));
        outer.RowStyles.Add(new RowStyle(SizeType.Percent,  100));
        outer.RowStyles.Add(new RowStyle(SizeType.Absolute, FooterH));

        // ── Header ────────────────────────────────────────────────────────────
        var header = BuildHeader();
        outer.Controls.Add(header, 0, 0);

        // ── Numeric pad ───────────────────────────────────────────────────────
        Pad = new NumericPadControl(PadMode) { Dock = DockStyle.Fill, Margin = new Padding(0) };
        outer.Controls.Add(Pad, 0, 1);

        // ── Footer ────────────────────────────────────────────────────────────
        _btnConfirm = new Button { Text = ConfirmText, Dock = DockStyle.Fill, Margin = new Padding(4, 8, 8, 8) };
        ElegantButtonStyles.Style(_btnConfirm, AccentColor, AppColors.TextWhite, fontSize: 18f);
        _btnConfirm.Click += BtnConfirm_Click;

        var btnCancel = new Button { Text = "✕  CANCEL", Dock = DockStyle.Fill, Margin = new Padding(8, 8, 4, 8) };
        ElegantButtonStyles.Style(btnCancel, AppColors.Danger, AppColors.TextWhite, fontSize: 18f);
        btnCancel.Click += (_, _) => Close();

        var footer = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 2,
            RowCount    = 1,
            BackColor   = AppColors.SurfaceMuted,
            Padding     = new Padding(0),
            Margin      = new Padding(0),
        };
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
        footer.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        footer.Controls.Add(btnCancel,   0, 0);
        footer.Controls.Add(_btnConfirm, 1, 0);
        outer.Controls.Add(footer, 0, 2);

        Controls.Add(outer);
    }

    // ── Header builder ────────────────────────────────────────────────────────
    private Panel BuildHeader()
    {
        var header = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.NavyDark,
            Padding   = new Padding(0),
        };

        // Accent line at bottom + left accent bar on Paint
        header.Paint += (_, e) =>
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using var accentBrush = new SolidBrush(AccentColor);
            e.Graphics.FillRectangle(accentBrush, 0, 0, 5, header.Height);
            using var linePen = new Pen(AccentColor, 2f);
            e.Graphics.DrawLine(linePen, 0, header.Height - 1, header.Width, header.Height - 1);
        };

        var lblIcon = new Label
        {
            Text      = Icon,
            Font      = new Font("Segoe UI", 22F, FontStyle.Bold),
            ForeColor = AccentColor,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleCenter,
            Size      = new Size(60, HeaderH),
            Location  = new Point(12, 0),
        };

        var lblTitle = new Label
        {
            Text      = Title,
            Font      = AppTypography.SectionTitle,
            ForeColor = AppColors.TextWhite,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.BottomLeft,
            Location  = new Point(76, 12),
            Size      = new Size(FormW - 80, 36),
        };

        var lblSub = new Label
        {
            Text      = Subtitle,
            Font      = AppTypography.Caption,
            ForeColor = AppColors.TextMuted,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.TopLeft,
            Location  = new Point(78, 50),
            Size      = new Size(FormW - 82, 28),
        };

        header.Controls.Add(lblSub);
        header.Controls.Add(lblTitle);
        header.Controls.Add(lblIcon);
        return header;
    }

    // ── Confirm handler ───────────────────────────────────────────────────────
    private async void BtnConfirm_Click(object? sender, EventArgs e)
    {
        _btnConfirm.Enabled = false;
        try
        {
            bool close = await OnConfirmAsync(Pad);
            if (close) Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            if (!IsDisposed) _btnConfirm.Enabled = true;
        }
    }
}
