using OmadaPOS.Presentation.Styling;
using OmadaPOS.Services;

namespace OmadaPOS.Views;

/// <summary>
/// Diagnostics panel accessible from the Settings menu.
/// Shows real-time hardware status and the last N lines of the current log file.
/// </summary>
public sealed class frmDiagnostics : Form
{
    private readonly ZebraScannerService? _scanner;

    // ── Controls ──────────────────────────────────────────────────────────────
    private Label  _lblScannerStatus  = null!;
    private Label  _lblScaleStatus    = null!;
    private Label  _lblPrinterStatus  = null!;
    private Label  _lblApiStatus      = null!;
    private TextBox _tbLog            = null!;
    private Label  _lblLogPath        = null!;
    private System.Windows.Forms.Timer? _refreshTimer;

    public frmDiagnostics(ZebraScannerService? scanner = null)
    {
        _scanner        = scanner;
        Text            = "System Diagnostics";
        Size            = new Size(860, 640);
        MinimumSize     = new Size(700, 500);
        StartPosition   = FormStartPosition.CenterParent;
        BackColor       = AppColors.BackgroundPrimary;
        FormBorderStyle = FormBorderStyle.Sizable;
        Font            = AppTypography.Body;

        BuildLayout();

        Load    += (_, _) => { RefreshStatus(); LoadLog(); StartTimer(); };
        Disposed += (_, _) => _refreshTimer?.Dispose();
    }

    // ── Layout ────────────────────────────────────────────────────────────────

    private void BuildLayout()
    {
        // Header
        var header = new Panel
        {
            Dock      = DockStyle.Top,
            Height    = 56,
            BackColor = AppColors.NavyBase,
            Padding   = new Padding(16, 0, 16, 0),
        };
        header.Controls.Add(new Label
        {
            Text      = "🔧  System Diagnostics",
            Font      = new Font("Segoe UI", 14F, FontStyle.Bold),
            ForeColor = AppColors.TextWhite,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Left,
            Width     = 400,
            TextAlign = ContentAlignment.MiddleLeft,
        });

        var btnRefresh = new Button
        {
            Text  = "↺ Refresh",
            Dock  = DockStyle.Right,
            Width = 110,
        };
        ElegantButtonStyles.Style(btnRefresh, AppColors.SlateBlue, AppColors.TextWhite, fontSize: 11f);
        btnRefresh.Click += (_, _) => { RefreshStatus(); LoadLog(); };
        header.Controls.Add(btnRefresh);

        // Status bar bottom
        var statusBar = new Panel
        {
            Dock      = DockStyle.Bottom,
            Height    = 40,
            BackColor = AppColors.SurfaceMuted,
            Padding   = new Padding(16, 0, 0, 0),
        };
        _lblLogPath = new Label
        {
            Dock      = DockStyle.Fill,
            Font      = new Font("Segoe UI", 8.5F),
            ForeColor = AppColors.TextSecondary,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleLeft,
        };
        var btnClose = new Button { Text = "Close", Dock = DockStyle.Right, Width = 100, Height = 40 };
        ElegantButtonStyles.Style(btnClose, AppColors.Danger, AppColors.TextWhite, fontSize: 11f);
        btnClose.Click += (_, _) => Close();
        statusBar.Controls.Add(_lblLogPath);
        statusBar.Controls.Add(btnClose);

        // Root splitter: hardware card (top) + log (fill)
        var root = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 1,
            RowCount    = 2,
            BackColor   = Color.Transparent,
            Padding     = new Padding(12),
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 160));
        root.RowStyles.Add(new RowStyle(SizeType.Percent,  100));

        root.Controls.Add(BuildHardwareCard(), 0, 0);
        root.Controls.Add(BuildLogPanel(),     0, 1);

        Controls.Add(root);
        Controls.Add(statusBar);
        Controls.Add(header);
    }

    private Panel BuildHardwareCard()
    {
        var card = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.SurfaceCard,
            Padding   = new Padding(16, 12, 16, 12),
            Margin    = new Padding(0, 0, 0, 8),
        };

        var title = new Label
        {
            Text      = "Hardware Status",
            Font      = new Font("Segoe UI", 11F, FontStyle.Bold),
            ForeColor = AppColors.TextPrimary,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Top,
            Height    = 28,
        };

        var grid = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 4,
            RowCount    = 2,
            BackColor   = Color.Transparent,
        };
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
        grid.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        // Column headers
        AddGridLabel(grid, "🔍  Barcode Scanner", 0, 0, header: true);
        AddGridLabel(grid, "⚖  Scale",            1, 0, header: true);
        AddGridLabel(grid, "🖨  Receipt Printer",  2, 0, header: true);
        AddGridLabel(grid, "🌐  API Server",       3, 0, header: true);

        // Status values
        _lblScannerStatus = AddGridLabel(grid, "—", 0, 1);
        _lblScaleStatus   = AddGridLabel(grid, "—", 1, 1);
        _lblPrinterStatus = AddGridLabel(grid, "—", 2, 1);
        _lblApiStatus     = AddGridLabel(grid, "—", 3, 1);

        card.Controls.Add(grid);
        card.Controls.Add(title);
        return card;
    }

    private static Label AddGridLabel(TableLayoutPanel grid, string text, int col, int row, bool header = false)
    {
        var lbl = new Label
        {
            Text      = text,
            Font      = header
                ? new Font("Segoe UI", 9F, FontStyle.Bold)
                : new Font("Segoe UI", 14F, FontStyle.Bold),
            ForeColor = header ? AppColors.TextSecondary : AppColors.TextPrimary,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
        };
        grid.Controls.Add(lbl, col, row);
        return lbl;
    }

    private Panel BuildLogPanel()
    {
        var panel = new Panel { Dock = DockStyle.Fill };

        var logHeader = new Panel
        {
            Dock      = DockStyle.Top,
            Height    = 32,
            BackColor = AppColors.NavyDark,
        };
        logHeader.Controls.Add(new Label
        {
            Text      = "  📋  Recent Log Entries  (last 200 lines of today's log)",
            Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            ForeColor = AppColors.TextWhite,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
        });

        _tbLog = new TextBox
        {
            Dock        = DockStyle.Fill,
            Multiline   = true,
            ScrollBars  = ScrollBars.Vertical,
            ReadOnly    = true,
            Font        = new Font("Consolas", 8.5F),
            BackColor   = Color.FromArgb(18, 24, 38),
            ForeColor   = Color.FromArgb(180, 220, 200),
            BorderStyle = BorderStyle.None,
            WordWrap    = false,
        };

        panel.Controls.Add(_tbLog);
        panel.Controls.Add(logHeader);
        return panel;
    }

    // ── Hardware status ───────────────────────────────────────────────────────

    private void RefreshStatus()
    {
        UpdateStatusLabel(_lblScannerStatus, _scanner?.IsConnected);
        UpdateStatusLabel(_lblScaleStatus,   _scanner?.IsScaleConnected);
        RefreshPrinterStatus();
        RefreshApiStatus();
    }

    private static void UpdateStatusLabel(Label lbl, bool? connected)
    {
        if (lbl == null) return;
        if (connected == null)
        {
            lbl.Text      = "N/A";
            lbl.ForeColor = AppColors.TextMuted;
        }
        else if (connected == true)
        {
            lbl.Text      = "✔  Connected";
            lbl.ForeColor = AppColors.AccentGreen;
        }
        else
        {
            lbl.Text      = "✕  Disconnected";
            lbl.ForeColor = AppColors.Danger;
        }
    }

    private void RefreshPrinterStatus()
    {
        try
        {
            var ps        = new System.Drawing.Printing.PrinterSettings();
            bool hasDef   = !string.IsNullOrWhiteSpace(ps.PrinterName);
            _lblPrinterStatus.Text      = hasDef ? $"✔  {ps.PrinterName}" : "✕  No default";
            _lblPrinterStatus.ForeColor = hasDef ? AppColors.AccentGreen : AppColors.Danger;
            _lblPrinterStatus.Font      = new Font("Segoe UI", 10F, FontStyle.Bold);
        }
        catch
        {
            _lblPrinterStatus.Text      = "? Unknown";
            _lblPrinterStatus.ForeColor = AppColors.Warning;
        }
    }

    private void RefreshApiStatus()
    {
        bool hasToken = !string.IsNullOrWhiteSpace(OmadaPOS.Libreria.Models.SessionManager.Token);
        _lblApiStatus.Text      = hasToken ? "✔  Authenticated" : "✕  Not signed in";
        _lblApiStatus.ForeColor = hasToken ? AppColors.AccentGreen : AppColors.Warning;
    }

    // ── Log viewer ────────────────────────────────────────────────────────────

    private void LoadLog()
    {
        try
        {
            var logsDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "OmadaPOS", "logs");

            // Today's log file has date-stamp suffix (Serilog RollingInterval.Day)
            string dateStamp = DateTime.Now.ToString("yyyyMMdd");
            var logFile = Directory.EnumerateFiles(logsDir, $"*{dateStamp}*.log")
                                   .OrderByDescending(File.GetLastWriteTime)
                                   .FirstOrDefault();

            if (logFile == null || !File.Exists(logFile))
            {
                _tbLog.Text  = $"No log file found for today ({dateStamp}) in:\n{logsDir}";
                _lblLogPath.Text = $"Log dir: {logsDir}";
                return;
            }

            _lblLogPath.Text = $"Log: {logFile}";

            // Read last 200 lines without locking the file
            string[] allLines;
            using (var fs = new FileStream(logFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(fs))
                allLines = sr.ReadToEnd().Split('\n');

            var last200 = allLines.TakeLast(200).ToArray();
            _tbLog.Text  = string.Join(Environment.NewLine, last200);

            // Scroll to end
            _tbLog.SelectionStart = _tbLog.Text.Length;
            _tbLog.ScrollToCaret();
        }
        catch (Exception ex)
        {
            _tbLog.Text = $"Error reading log: {ex.Message}";
        }
    }

    private void StartTimer()
    {
        _refreshTimer = new System.Windows.Forms.Timer { Interval = 15_000 };
        _refreshTimer.Tick += (_, _) => { RefreshStatus(); LoadLog(); };
        _refreshTimer.Start();
    }
}
