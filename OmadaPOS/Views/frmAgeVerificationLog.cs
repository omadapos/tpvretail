using OmadaPOS.Presentation.Styling;
using OmadaPOS.Services;

namespace OmadaPOS.Views;

/// <summary>
/// Read-only viewer for the age-verification audit log stored in SQLite.
/// Accessible from the Settings / Diagnostics header menu.
/// Only the last N days of records are loaded to keep the grid fast.
/// </summary>
public sealed class frmAgeVerificationLog : Form
{
    private readonly ISqliteManager _sqlite;

    private DataGridView  _grid        = null!;
    private ComboBox      _cbDays      = null!;
    private Label         _lblCount    = null!;

    public frmAgeVerificationLog(ISqliteManager sqlite)
    {
        _sqlite         = sqlite;
        Text            = "Age Verification Audit Log";
        Size            = new Size(1020, 640);
        MinimumSize     = new Size(800, 500);
        StartPosition   = FormStartPosition.CenterParent;
        BackColor       = AppColors.BackgroundPrimary;
        FormBorderStyle = FormBorderStyle.Sizable;
        Font            = AppTypography.Body;

        BuildLayout();

        Load += async (_, _) => await LoadAsync();
    }

    // ── Layout ────────────────────────────────────────────────────────────────

    private void BuildLayout()
    {
        // Header bar
        var header = new Panel
        {
            Dock      = DockStyle.Top,
            Height    = 56,
            BackColor = AppColors.NavyBase,
            Padding   = new Padding(16, 0, 16, 0),
        };

        var lblTitle = new Label
        {
            Text      = "🔞  Age Verification Audit Log",
            Font      = new Font("Segoe UI", 14F, FontStyle.Bold),
            ForeColor = AppColors.TextWhite,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Left,
            TextAlign = ContentAlignment.MiddleLeft,
            AutoSize  = false,
            Width     = 400,
        };

        var filterPanel = new Panel
        {
            Dock      = DockStyle.Right,
            Width     = 280,
            BackColor = Color.Transparent,
        };

        var lblDays = new Label
        {
            Text      = "Show last:",
            Font      = AppTypography.Body,
            ForeColor = AppColors.TextWhite,
            BackColor = Color.Transparent,
            AutoSize  = true,
            Location  = new Point(0, 18),
        };

        _cbDays = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font          = AppTypography.Body,
            BackColor     = AppColors.SurfaceMuted,
            ForeColor     = AppColors.TextPrimary,
            Location      = new Point(78, 14),
            Width         = 100,
        };
        _cbDays.Items.AddRange(["7 days", "14 days", "30 days", "60 days", "90 days"]);
        _cbDays.SelectedIndex = 2; // default 30 days

        var btnRefresh = new Button
        {
            Text     = "↺ Refresh",
            Location = new Point(188, 12),
            Width    = 90,
            Height   = 32,
        };
        ElegantButtonStyles.Style(btnRefresh, AppColors.SlateBlue, AppColors.TextWhite, fontSize: 11f);
        btnRefresh.Click += async (_, _) => await LoadAsync();

        filterPanel.Controls.Add(lblDays);
        filterPanel.Controls.Add(_cbDays);
        filterPanel.Controls.Add(btnRefresh);

        header.Controls.Add(filterPanel);
        header.Controls.Add(lblTitle);

        // Status bar
        var statusBar = new Panel
        {
            Dock      = DockStyle.Bottom,
            Height    = 36,
            BackColor = AppColors.SurfaceMuted,
            Padding   = new Padding(16, 0, 0, 0),
        };
        _lblCount = new Label
        {
            Dock      = DockStyle.Fill,
            Font      = AppTypography.Body,
            ForeColor = AppColors.TextSecondary,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleLeft,
        };
        var btnClose = new Button { Text = "Close", Dock = DockStyle.Right, Width = 100, Height = 36 };
        ElegantButtonStyles.Style(btnClose, AppColors.Danger, AppColors.TextWhite, fontSize: 11f);
        btnClose.Click += (_, _) => Close();
        statusBar.Controls.Add(_lblCount);
        statusBar.Controls.Add(btnClose);

        // Grid
        _grid = new DataGridView
        {
            Dock                    = DockStyle.Fill,
            AutoSizeColumnsMode     = DataGridViewAutoSizeColumnsMode.None,
            AllowUserToAddRows      = false,
            AllowUserToDeleteRows   = false,
            ReadOnly                = true,
            SelectionMode           = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect             = false,
            RowHeadersVisible       = false,
            CellBorderStyle         = DataGridViewCellBorderStyle.SingleHorizontal,
            BackgroundColor         = AppColors.BackgroundPrimary,
            GridColor               = AppColors.ListViewGridLine,
            BorderStyle             = BorderStyle.None,
        };

        ApplyGridTheme(_grid);
        BuildColumns();

        Controls.Add(_grid);
        Controls.Add(statusBar);
        Controls.Add(header);
    }

    private void BuildColumns()
    {
        _grid.Columns.Clear();
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "ColId",      HeaderText = "#",           Width = 50,  ReadOnly = true });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "ColDate",    HeaderText = "Date/Time",   Width = 145, ReadOnly = true });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "ColCashier", HeaderText = "Cashier",     Width = 130, ReadOnly = true });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "ColMethod",  HeaderText = "Method",      Width = 110, ReadOnly = true });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "ColResult",  HeaderText = "Result",      Width = 110, ReadOnly = true });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "ColPassed",  HeaderText = "21+",         Width = 60,  ReadOnly = true });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "ColIdType",  HeaderText = "ID Type",     Width = 110, ReadOnly = true });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "ColIdToken", HeaderText = "ID Token",    Width = 110, ReadOnly = true });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "ColDenial",  HeaderText = "Denial Reason", Width = 200, ReadOnly = true });

        // Stretch last column
        _grid.Columns["ColDenial"]!.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
    }

    // ── Data loading ──────────────────────────────────────────────────────────

    private int SelectedDays()
    {
        int idx = _cbDays.SelectedIndex;
        return idx switch { 0 => 7, 1 => 14, 2 => 30, 3 => 60, 4 => 90, _ => 30 };
    }

    private async Task LoadAsync()
    {
        _grid.Rows.Clear();
        _lblCount.Text = "Loading…";

        try
        {
            var records = await _sqlite.GetAgeVerificationAuditAsync(SelectedDays());

            _grid.SuspendLayout();
            foreach (var r in records)
            {
                int row = _grid.Rows.Add();
                var cells = _grid.Rows[row].Cells;
                cells["ColId"].Value      = r.Id;
                cells["ColDate"].Value    = r.VerifiedAt.ToLocalTime().ToString("MM/dd/yyyy HH:mm");
                cells["ColCashier"].Value = r.CashierName;
                cells["ColMethod"].Value  = r.VerificationMethod;
                cells["ColResult"].Value  = r.VerificationResult;
                cells["ColPassed"].Value  = r.CustomerIs21OrOver ? "✔" : "✕";
                cells["ColIdType"].Value  = r.IdType ?? "";
                cells["ColIdToken"].Value = r.IdLast4OrToken ?? "";
                cells["ColDenial"].Value  = r.DenialReason ?? "";

                // Color-code rows: green = passed, red = denied
                _grid.Rows[row].DefaultCellStyle.BackColor =
                    r.CustomerIs21OrOver
                        ? Color.FromArgb(20, 22, 163, 74)
                        : Color.FromArgb(20, 220, 38, 38);
                _grid.Rows[row].DefaultCellStyle.ForeColor = AppColors.TextPrimary;
            }
            _grid.ResumeLayout();

            _lblCount.Text = $"{records.Count} record(s) in the last {SelectedDays()} days";
        }
        catch (Exception ex)
        {
            _lblCount.Text = $"Error loading records: {ex.Message}";
        }
    }

    // ── Grid theme ────────────────────────────────────────────────────────────

    private static void ApplyGridTheme(DataGridView dg)
    {
        dg.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = AppColors.NavyDark,
            ForeColor = AppColors.TextWhite,
            Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            Padding   = new Padding(6, 0, 0, 0),
        };
        dg.ColumnHeadersHeight       = 36;
        dg.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

        dg.DefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = AppColors.SurfaceCard,
            ForeColor = AppColors.TextPrimary,
            Font      = new Font("Segoe UI", 9.5F),
            Padding   = new Padding(6, 0, 0, 0),
        };

        dg.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = AppColors.BackgroundPrimary,
        };

        dg.RowTemplate.Height = 34;

        dg.EnableHeadersVisualStyles = false;

        dg.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
    }
}
