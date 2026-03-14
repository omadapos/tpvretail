using OmadaPOS.Presentation.Styling;
using OmadaPOS.Services;

namespace OmadaPOS.Views;

/// <summary>
/// Compact supervisor-PIN dialog.
/// Shown before allowing the cashier to exit the POS application.
/// Returns DialogResult.OK only when the correct PIN is entered.
/// </summary>
public sealed class frmSupervisorPin : Form
{
    // ── State ─────────────────────────────────────────────────────────────────
    private string _entered = "";
    private const int MaxLen = 4;

    private CancellationTokenSource? _flashCts;

    // ── UI refs ───────────────────────────────────────────────────────────────
    private readonly Label _displayLabel;
    private readonly Label _statusLabel;

    // ── Constructor ───────────────────────────────────────────────────────────
    public frmSupervisorPin()
    {
        FormBorderStyle = FormBorderStyle.None;
        StartPosition   = FormStartPosition.CenterScreen;
        BackColor       = AppColors.NavyDark;
        Size            = new Size(380, 530);
        TopMost         = true;

        // Rounded border via Paint
        Paint += (_, e) =>
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using var path   = ElegantButtonStyles.RoundedPath(new Rectangle(1, 1, Width - 2, Height - 2), 16);
            using var border = new Pen(AppColors.AccentGreen, 2f);
            e.Graphics.DrawPath(border, path);
        };

        // ── Header ────────────────────────────────────────────────────────────
        var lblIcon = new Label
        {
            Text      = "⏻",
            Font      = new Font("Segoe UI", 28F),
            ForeColor = AppColors.Danger,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleCenter,
            Dock      = DockStyle.Top,
            Height    = 64,
        };

        var lblTitle = new Label
        {
            Text      = "Supervisor Required",
            Font      = AppTypography.SectionTitle,
            ForeColor = AppColors.TextWhite,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleCenter,
            Dock      = DockStyle.Top,
            Height    = 32,
        };

        var lblSubtitle = new Label
        {
            Text      = "Enter the 4-digit supervisor PIN",
            Font      = AppTypography.Caption,
            ForeColor = AppColors.TextMuted,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleCenter,
            Dock      = DockStyle.Top,
            Height    = 24,
        };

        // ── PIN display — shows ● for each entered digit ──────────────────────
        _displayLabel = new Label
        {
            Text      = DotDisplay(0),
            Font      = new Font("Segoe UI", 28F, FontStyle.Bold),
            ForeColor = AppColors.AccentGreen,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleCenter,
            Dock      = DockStyle.Top,
            Height    = 60,
        };

        // ── Status (wrong PIN message) ─────────────────────────────────────────
        _statusLabel = new Label
        {
            Text      = "",
            Font      = AppTypography.BodySmall,
            ForeColor = AppColors.Danger,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleCenter,
            Dock      = DockStyle.Top,
            Height    = 24,
        };

        // ── Numeric pad ───────────────────────────────────────────────────────
        var pad = BuildPad();
        pad.Dock = DockStyle.Top;
        pad.Height = 280;

        // ── Cancel button ─────────────────────────────────────────────────────
        var btnCancel = new Button
        {
            Text      = "✕  CANCEL",
            Dock      = DockStyle.Bottom,
            Height    = 48,
            Margin    = new Padding(16, 0, 16, 12),
            FlatStyle = FlatStyle.Flat,
            Font      = AppTypography.PaymentLabel,
            ForeColor = AppColors.TextMuted,
            BackColor = Color.Transparent,
            Cursor    = Cursors.Default,
        };
        btnCancel.FlatAppearance.BorderColor    = AppColors.TextMuted;
        btnCancel.FlatAppearance.BorderSize     = 1;
        btnCancel.FlatAppearance.MouseOverBackColor = Color.Transparent;
        btnCancel.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };

        // ── Compose — bottom-first for DockStyle.Top stacking ─────────────────
        Controls.Add(btnCancel);
        Controls.Add(pad);
        Controls.Add(_statusLabel);
        Controls.Add(_displayLabel);
        Controls.Add(lblSubtitle);
        Controls.Add(lblTitle);
        Controls.Add(lblIcon);

        // Keep dialog on top if something else briefly steals focus
        Deactivate += (_, _) => { if (!IsDisposed) Activate(); };

        // Cancel any pending flash task when the form closes to avoid
        // ObjectDisposedException races with Invoke() after Dispose().
        FormClosed += (_, _) => { _flashCts?.Cancel(); _flashCts?.Dispose(); };
    }

    // ── PIN pad grid (3 cols × 4 rows) ────────────────────────────────────────
    private TableLayoutPanel BuildPad()
    {
        var grid = new TableLayoutPanel
        {
            ColumnCount = 3,
            RowCount    = 4,
            BackColor   = Color.Transparent,
            Margin      = new Padding(16, 8, 16, 8),
            Padding     = new Padding(0),
        };
        for (int c = 0; c < 3; c++)
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
        for (int r = 0; r < 4; r++)
            grid.RowStyles.Add(new RowStyle(SizeType.Percent, 25f));

        // Rows 0-2: 7 8 9 / 4 5 6 / 1 2 3
        (string text, string tag)[] rows012 =
        [
            ("7","7"),("8","8"),("9","9"),
            ("4","4"),("5","5"),("6","6"),
            ("1","1"),("2","2"),("3","3"),
        ];
        for (int i = 0; i < 9; i++)
        {
            var (t, tag) = rows012[i];
            grid.Controls.Add(MakeDigit(t, tag), i % 3, i / 3);
        }

        // Row 3: ⌫ | 0 | ✓
        grid.Controls.Add(MakeAction("⌫", "bs",  AppColors.Warning), 0, 3);
        grid.Controls.Add(MakeDigit("0", "0"),                        1, 3);
        grid.Controls.Add(MakeAction("✓", "ok",  AppColors.AccentGreen), 2, 3);

        return grid;
    }

    private Button MakeDigit(string text, string tag)
    {
        var btn = new Button { Text = text, Tag = tag, Dock = DockStyle.Fill, Margin = new Padding(4) };
        ElegantButtonStyles.Style(btn, AppColors.NavyBase, AppColors.TextWhite, fontSize: 26f);
        btn.Click += OnButtonClick;
        return btn;
    }

    private Button MakeAction(string text, string tag, Color color)
    {
        var btn = new Button { Text = text, Tag = tag, Dock = DockStyle.Fill, Margin = new Padding(4) };
        ElegantButtonStyles.Style(btn, color, AppColors.TextWhite, fontSize: 22f);
        btn.Click += OnButtonClick;
        return btn;
    }

    // ── Click logic ───────────────────────────────────────────────────────────
    private void OnButtonClick(object? sender, EventArgs e)
    {
        if (sender is not Button btn) return;
        var tag = btn.Tag?.ToString() ?? "";

        switch (tag)
        {
            case "bs":
                if (_entered.Length > 0) _entered = _entered[..^1];
                RefreshDisplay();
                break;

            case "ok":
                Confirm();
                break;

            default:
                if (_entered.Length < MaxLen)
                {
                    _entered += tag;
                    RefreshDisplay();
                    // Auto-confirm when MaxLen reached
                    if (_entered.Length == MaxLen)
                        Confirm();
                }
                break;
        }
    }

    private void Confirm()
    {
        if (SupervisorConfig.Verify(_entered))
        {
            DialogResult = DialogResult.OK;
            Close();
            return;
        }

        // Wrong PIN — flash red, reset after 1.2s
        _displayLabel.ForeColor = AppColors.Danger;
        _displayLabel.Text      = "✕  ✕  ✕  ✕";
        _statusLabel.Text       = "Incorrect PIN — try again";

        _flashCts?.Cancel();
        _flashCts?.Dispose();
        _flashCts = new CancellationTokenSource();
        var token = _flashCts.Token;

        Task.Delay(1200, token).ContinueWith(_ =>
        {
            if (IsDisposed || token.IsCancellationRequested) return;
            try
            {
                Invoke(() =>
                {
                    _entered = "";
                    _statusLabel.Text = "";
                    RefreshDisplay();
                });
            }
            catch (ObjectDisposedException) { }
        }, TaskScheduler.Default);
    }

    private void RefreshDisplay()
    {
        _displayLabel.ForeColor = AppColors.AccentGreen;
        _displayLabel.Text      = DotDisplay(_entered.Length);
    }

    private static string DotDisplay(int filled) =>
        string.Join("  ", Enumerable.Range(0, MaxLen).Select(i => i < filled ? "●" : "○"));
}
