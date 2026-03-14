using OmadaPOS.Domain;
using OmadaPOS.Presentation.Styling;
using OmadaPOS.Services;

namespace OmadaPOS.Views;

/// <summary>
/// Age-verification dialog shown before payment when the cart contains a
/// product that requires age verification (alcohol, tobacco, etc.).
///
/// Two verification paths:
///   1. Scan  — The form captures keyboard-wedge input (USB-HID mode) via an
///              invisible TextBox, and OPOS scanner input via ZebraScannerService.
///              Both run the scanned string through AamvaParser.
///   2. Manual — Cashier enters Month / Day / Year in three TextBoxes.
///
/// One verification is sufficient per transaction regardless of how many
/// age-restricted products are in the cart.
/// </summary>
public sealed class frmAgeVerification : Form
{
    // ── Dependencies ──────────────────────────────────────────────────────────
    private readonly IAgeVerificationService _svc;
    private readonly ZebraScannerService?    _scanner;

    // ── Result ────────────────────────────────────────────────────────────────
    public AgeVerificationResult? VerificationResult { get; private set; }

    // ── Scan capture (keyboard-wedge) ─────────────────────────────────────────
    private TextBox _txtScanCapture = null!;

    // ── Manual entry ─────────────────────────────────────────────────────────
    private TextBox _txtMonth  = null!;
    private TextBox _txtDay    = null!;
    private TextBox _txtYear   = null!;

    // ── Result display ────────────────────────────────────────────────────────
    private Panel _resultPanel   = null!;
    private Label _resultLabel   = null!;

    // ── Footer buttons ────────────────────────────────────────────────────────
    private Button _btnCancel    = null!;
    private Button _btnConfirm   = null!;   // green — shown when Approved
    private Button _btnAcknowledge = null!; // red   — shown when Denied

    // ── KeyDown attachment state ───────────────────────────────────────────────
    private bool _scanHandlerAttached = true;

    // ── Keyboard-wedge scan timer ─────────────────────────────────────────────
    // PDF417 barcodes (driver licences) contain multiple \r characters (one per
    // field). Processing on the first Enter would give us only a fragment. Instead
    // we accumulate raw keystrokes — including \r — in a StringBuilder and fire
    // a 250 ms idle timer; when no more keystrokes arrive the full barcode is ready.
    private readonly System.Text.StringBuilder _kwBuffer = new();
    private readonly System.Windows.Forms.Timer _kwTimer  = new() { Interval = 250 };

    // ── Colors ────────────────────────────────────────────────────────────────
    private static readonly Color _pendingBg  = AppColors.SlateBlue;       // #475569 slate
    private static readonly Color _approvedBg = AppColors.AccentGreenDark; // #15803D
    private static readonly Color _deniedBg   = AppColors.Danger;          // #DC2626

    public frmAgeVerification(IAgeVerificationService svc, ZebraScannerService? scanner = null)
    {
        _svc     = svc     ?? throw new ArgumentNullException(nameof(svc));
        _scanner = scanner;

        DoubleBuffered = true;
        InitForm();

        if (_scanner != null)
            _scanner.OnBarcodeDataReceived += OnOposBarcode;

        _kwTimer.Tick += (_, _) =>
        {
            _kwTimer.Stop();
            string data = _kwBuffer.ToString();
            _kwBuffer.Clear();
            if (!string.IsNullOrWhiteSpace(data))
                ProcessBarcodeString(data);
        };

        FormClosed += (_, _) =>
        {
            _kwTimer.Stop();
            _kwTimer.Dispose();
            if (_scanner != null)
                _scanner.OnBarcodeDataReceived -= OnOposBarcode;
        };

        Shown += (_, _) =>
        {
            ThemeManager.ApplyAll(this);
            _txtScanCapture.Focus();
        };
    }

    // ── Form setup ─────────────────────────────────────────────────────────────

    private void InitForm()
    {
        Text            = "Age Verification Required";
        FormBorderStyle = FormBorderStyle.None;
        StartPosition   = FormStartPosition.CenterParent;
        Size            = new Size(900, 660);
        MinimumSize     = new Size(820, 580);
        BackColor       = AppColors.NavyBase;
        KeyPreview      = true;
        KeyDown        += Form_KeyDown;

        // ── Root layout: header / body / result / footer ──────────────────────
        var root = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 1,
            RowCount    = 4,
            BackColor   = Color.Transparent,
            Padding     = Padding.Empty,
            Margin      = Padding.Empty,
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 90F));   // header
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));   // body
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 90F));   // result panel
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));   // footer

        root.Controls.Add(BuildHeader(),       0, 0);
        root.Controls.Add(BuildBody(),         0, 1);
        root.Controls.Add(BuildResultPanel(),  0, 2);
        root.Controls.Add(BuildFooter(),       0, 3);

        Controls.Add(root);

        // Invisible 1×1 capture box for keyboard-wedge mode
        _txtScanCapture = new TextBox
        {
            Location  = new Point(-100, -100),
            Size      = new Size(1, 1),
            TabStop   = false,
            MaxLength = 4096,
        };
        Controls.Add(_txtScanCapture);
    }

    // ── Header ────────────────────────────────────────────────────────────────

    private Panel BuildHeader()
    {
        var header = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.NavyDark,
            Padding   = new Padding(24, 0, 24, 0),
        };
        // Emerald accent line at bottom
        header.Paint += (_, e) =>
        {
            using var pen = new Pen(AppColors.AccentGreen, 2f);
            e.Graphics.DrawLine(pen, 0, header.Height - 2, header.Width, header.Height - 2);
        };

        var lblTitle = new Label
        {
            Text      = "⚠  AGE VERIFICATION REQUIRED",
            Font      = new Font("Segoe UI", 18F, FontStyle.Bold),
            ForeColor = AppColors.TextWhite,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
        };
        var lblSub = new Label
        {
            Text      = "Customer must be 21 or older to purchase this item.",
            Font      = new Font("Segoe UI", 11F, FontStyle.Regular),
            ForeColor = AppColors.TextOnDarkMuted,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Bottom,
            Height    = 28,
            TextAlign = ContentAlignment.BottomLeft,
            Padding   = new Padding(0, 0, 0, 6),
        };

        header.Controls.Add(lblTitle);
        header.Controls.Add(lblSub);
        return header;
    }

    // ── Body: scan column + manual column ─────────────────────────────────────

    private Control BuildBody()
    {
        var body = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 2,
            RowCount    = 1,
            BackColor   = Color.Transparent,
            Padding     = new Padding(20, 16, 20, 8),
        };
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42F));
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58F));
        body.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        body.Controls.Add(BuildScanColumn(), 0, 0);
        body.Controls.Add(BuildManualColumn(), 1, 0);
        return body;
    }

    // ── Scan column ───────────────────────────────────────────────────────────

    private Control BuildScanColumn()
    {
        var card = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.NavyBase,
            Margin    = new Padding(0, 0, 10, 0),
        };
        card.Paint += (_, e) =>
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using var pen = new Pen(AppColors.NavyLight, 1f);
            e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
        };

        var inner = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 1,
            RowCount    = 4,
            BackColor   = Color.Transparent,
            Padding     = new Padding(20, 20, 20, 16),
        };
        inner.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));  // icon + title
        inner.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));  // instructions
        inner.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));  // OPOS badge
        inner.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));  // status hint

        // Title
        var lblTitle = new Label
        {
            Text      = "🪪  Scan Driver's License",
            Font      = new Font("Segoe UI", 14F, FontStyle.Bold),
            ForeColor = AppColors.TextWhite,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
        };
        inner.Controls.Add(lblTitle, 0, 0);

        // Instructions
        var lblInstr = new Label
        {
            Text = "Present the back of the customer's\ndriver's license or state ID\nto the scanner.\n\n" +
                   "The PDF417 barcode will be read\nautomatically.",
            Font      = new Font("Segoe UI", 11F),
            ForeColor = AppColors.TextOnDarkMuted,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
        };
        inner.Controls.Add(lblInstr, 0, 1);

        // OPOS badge
        var lblOpos = new Label
        {
            Text      = _scanner != null
                            ? "✔  Scanner active (OPOS + USB-HID)"
                            : "ℹ  USB-HID scanner only",
            Font      = new Font("Segoe UI", 9F, FontStyle.Italic),
            ForeColor = _scanner != null ? AppColors.AccentGreen : AppColors.TextMuted,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
        };
        inner.Controls.Add(lblOpos, 0, 2);

        // Waiting hint
        var lblWait = new Label
        {
            Text      = "Waiting for scan…",
            Font      = new Font("Segoe UI", 10F, FontStyle.Italic),
            ForeColor = AppColors.TextMuted,
            BackColor = AppColors.NavyDark,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Padding   = new Padding(0, 8, 0, 8),
        };
        inner.Controls.Add(lblWait, 0, 3);

        card.Controls.Add(inner);
        return card;
    }

    // ── Manual column ─────────────────────────────────────────────────────────

    private Control BuildManualColumn()
    {
        var card = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.NavyBase,
            Margin    = new Padding(10, 0, 0, 0),
        };
        card.Paint += (_, e) =>
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using var pen = new Pen(AppColors.NavyLight, 1f);
            e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
        };

        var inner = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 1,
            RowCount    = 4,
            BackColor   = Color.Transparent,
            Padding     = new Padding(24, 20, 24, 16),
        };
        inner.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));   // title
        inner.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));   // label "Date of Birth"
        inner.RowStyles.Add(new RowStyle(SizeType.Absolute, 72F));   // MM / DD / YYYY inputs
        inner.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));   // verify button area

        // Title
        var lblTitle = new Label
        {
            Text      = "✏  Enter Date of Birth Manually",
            Font      = new Font("Segoe UI", 14F, FontStyle.Bold),
            ForeColor = AppColors.TextWhite,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
        };
        inner.Controls.Add(lblTitle, 0, 0);

        // DOB label
        var lblDob = new Label
        {
            Text      = "DATE OF BIRTH",
            Font      = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = AppColors.TextOnDarkMuted,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.BottomLeft,
        };
        inner.Controls.Add(lblDob, 0, 1);

        // MM / DD / YYYY row
        var dateRow = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 5,
            RowCount    = 1,
            BackColor   = Color.Transparent,
            Padding     = Padding.Empty,
        };
        dateRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));  // MM
        dateRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 24F));  // /
        dateRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));  // DD
        dateRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 24F));  // /
        dateRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F)); // YYYY
        dateRow.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        _txtMonth = CreateDateBox("MM", 2);
        _txtDay   = CreateDateBox("DD", 2);
        _txtYear  = CreateDateBox("YYYY", 4);

        // Auto-advance: MM→DD→YYYY on full length
        _txtMonth.TextChanged += (_, _) => { if (_txtMonth.Text.Length == 2) _txtDay.Focus(); };
        _txtDay.TextChanged   += (_, _) => { if (_txtDay.Text.Length == 2)   _txtYear.Focus(); };

        // When any manual field gains focus, detach the scan KeyDown handler
        // so Enter in the date fields triggers the verify button, not scan processing
        _txtMonth.Enter += DetachScanHandler;
        _txtDay.Enter   += DetachScanHandler;
        _txtYear.Enter  += DetachScanHandler;
        _txtMonth.Leave += ReattachScanHandler;
        _txtDay.Leave   += ReattachScanHandler;
        _txtYear.Leave  += ReattachScanHandler;

        // Allow Enter key inside the year field to trigger verify
        _txtYear.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; VerifyManual(); }
        };

        dateRow.Controls.Add(_txtMonth, 0, 0);
        dateRow.Controls.Add(MakeSeparatorLabel("/"), 1, 0);
        dateRow.Controls.Add(_txtDay, 2, 0);
        dateRow.Controls.Add(MakeSeparatorLabel("/"), 3, 0);
        dateRow.Controls.Add(_txtYear, 4, 0);

        inner.Controls.Add(dateRow, 0, 2);

        // Verify button area
        var btnArea = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = Color.Transparent,
            Padding   = new Padding(0, 16, 0, 0),
        };
        var btnVerify = new Button
        {
            Text      = "VERIFY DATE OF BIRTH",
            Dock      = DockStyle.Top,
            Height    = 52,
            Font      = new Font("Segoe UI", 13F, FontStyle.Bold),
            ForeColor = AppColors.TextWhite,
            BackColor = AppColors.Info,
            FlatStyle = FlatStyle.Flat,
            FlatAppearance = { BorderSize = 0, MouseOverBackColor = Color.Transparent, MouseDownBackColor = Color.Transparent },
        };
        btnVerify.Click += (_, _) => VerifyManual();
        btnArea.Controls.Add(btnVerify);
        inner.Controls.Add(btnArea, 0, 3);

        card.Controls.Add(inner);
        return card;
    }

    private static TextBox CreateDateBox(string placeholder, int maxLength) => new()
    {
        Font            = new Font("Consolas", 18F, FontStyle.Bold),
        ForeColor       = AppColors.TextPrimary,
        BackColor       = Color.White,
        BorderStyle     = BorderStyle.FixedSingle,
        TextAlign       = HorizontalAlignment.Center,
        MaxLength       = maxLength,
        PlaceholderText = placeholder,
        Dock            = DockStyle.Fill,
    };

    private static Label MakeSeparatorLabel(string text) => new()
    {
        Text      = text,
        Font      = new Font("Segoe UI", 20F, FontStyle.Bold),
        ForeColor = AppColors.TextOnDarkMuted,
        BackColor = Color.Transparent,
        TextAlign = ContentAlignment.BottomCenter,
        Dock      = DockStyle.Fill,
        Padding   = new Padding(0, 0, 0, 4),
    };

    // ── Result panel ──────────────────────────────────────────────────────────

    private Panel BuildResultPanel()
    {
        _resultPanel = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = _pendingBg,
            Padding   = new Padding(24, 0, 24, 0),
        };

        _resultLabel = new Label
        {
            Text      = "⏳  Pending — waiting for ID verification",
            Font      = new Font("Segoe UI", 14F, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
        };

        _resultPanel.Controls.Add(_resultLabel);
        return _resultPanel;
    }

    // ── Footer ────────────────────────────────────────────────────────────────

    private Panel BuildFooter()
    {
        var footer = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.NavyDark,
            Padding   = new Padding(20, 10, 20, 10),
        };

        _btnCancel = new Button
        {
            Text      = "✕  CANCEL SALE",
            Width     = 220,
            Dock      = DockStyle.Left,
            Font      = new Font("Segoe UI", 13F, FontStyle.Bold),
            ForeColor = AppColors.TextWhite,
            BackColor = AppColors.Danger,
            FlatStyle = FlatStyle.Flat,
            FlatAppearance = { BorderSize = 0, MouseOverBackColor = Color.Transparent, MouseDownBackColor = Color.Transparent },
        };
        _btnCancel.Click += (_, _) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };

        _btnConfirm = new Button
        {
            Text      = "✔  CONFIRM & CONTINUE",
            Width     = 260,
            Dock      = DockStyle.Right,
            Font      = new Font("Segoe UI", 13F, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = AppColors.AccentGreenDark,
            FlatStyle = FlatStyle.Flat,
            Visible   = false,
            FlatAppearance = { BorderSize = 0, MouseOverBackColor = Color.Transparent, MouseDownBackColor = Color.Transparent },
        };
        _btnConfirm.Click += (_, _) =>
        {
            DialogResult = DialogResult.OK;
            Close();
        };

        _btnAcknowledge = new Button
        {
            Text      = "✔  ACKNOWLEDGE & CLOSE",
            Width     = 280,
            Dock      = DockStyle.Right,
            Font      = new Font("Segoe UI", 13F, FontStyle.Bold),
            ForeColor = AppColors.TextWhite,
            BackColor = AppColors.Danger,
            FlatStyle = FlatStyle.Flat,
            Visible   = false,
            FlatAppearance = { BorderSize = 0, MouseOverBackColor = Color.Transparent, MouseDownBackColor = Color.Transparent },
        };
        _btnAcknowledge.Click += (_, _) =>
        {
            DialogResult = DialogResult.OK;
            Close();
        };

        // Add right buttons first so DockStyle.Right stacks correctly
        footer.Controls.Add(_btnAcknowledge);
        footer.Controls.Add(_btnConfirm);
        footer.Controls.Add(_btnCancel);
        return footer;
    }

    // ── Scan processing (keyboard-wedge + OPOS) ───────────────────────────────

    private void Form_KeyDown(object? sender, KeyEventArgs e)
    {
        if (!_scanHandlerAttached) return;

        // Accumulate every keystroke — including Enter (\r) — into the buffer.
        // PDF417 / AAMVA barcodes use \r as a field terminator, so we must NOT
        // stop on the first Enter. The 250 ms idle timer fires when the full
        // barcode burst has been received.
        char ch = e.KeyCode switch
        {
            Keys.Enter  => '\r',
            Keys.Tab    => '\t',
            _           => (char)0
        };

        if (ch != 0)
        {
            e.SuppressKeyPress = true;
            _kwBuffer.Append(ch);
        }
        else if (e.KeyValue >= 32 && e.KeyValue <= 126 && !e.Control && !e.Alt)
        {
            // Printable ASCII — let it fall through to _txtScanCapture naturally;
            // also mirror it into the buffer so the timer path gets the full string.
            _kwBuffer.Append((char)e.KeyValue);
        }

        // Reset idle timer on every keystroke.
        _kwTimer.Stop();
        _kwTimer.Start();
    }

    // Called by OPOS scanner event (arrives on background thread → invoke to UI)
    private void OnOposBarcode(string barcode)
    {
        if (InvokeRequired) { Invoke(() => OnOposBarcode(barcode)); return; }
        // OPOS delivers the complete barcode in one shot — no buffering needed.
        _kwTimer.Stop();
        _kwBuffer.Clear();
        ProcessBarcodeString(barcode);
    }

    private void ProcessScanCapture()
    {
        string data = _txtScanCapture.Text;
        _txtScanCapture.Clear();

        if (string.IsNullOrWhiteSpace(data)) return;
        ProcessBarcodeString(data);
    }

    private void ProcessBarcodeString(string rawBarcode)
    {
        if (!AamvaParser.TryParseBirthDate(rawBarcode, out DateOnly dob))
        {
            ShowScanError("Could not read ID — please try again or enter the date of birth manually.");
            return;
        }

        AamvaParser.TryParseLast4(rawBarcode, out string last4);
        ApplyVerification(dob, AgeVerificationMethod.Scan, "DL", last4.Length > 0 ? last4 : null);
    }

    // ── Manual entry ──────────────────────────────────────────────────────────

    private void VerifyManual()
    {
        if (!int.TryParse(_txtMonth.Text, out int month) || month < 1 || month > 12)
        {
            ShowManualError("Please enter a valid month (01–12).");
            _txtMonth.Focus();
            return;
        }
        if (!int.TryParse(_txtDay.Text, out int day) || day < 1 || day > 31)
        {
            ShowManualError("Please enter a valid day (01–31).");
            _txtDay.Focus();
            return;
        }
        if (!int.TryParse(_txtYear.Text, out int year) || year < 1900 || year > DateTime.Today.Year)
        {
            ShowManualError($"Please enter a valid year (1900–{DateTime.Today.Year}).");
            _txtYear.Focus();
            return;
        }

        DateOnly dob;
        try
        {
            dob = new DateOnly(year, month, day);
        }
        catch (ArgumentOutOfRangeException)
        {
            ShowManualError("The date you entered is not valid. Please check month and day.");
            return;
        }

        ApplyVerification(dob, AgeVerificationMethod.Manual, "Manual", null);
    }

    // ── Shared verification logic ──────────────────────────────────────────────

    private void ApplyVerification(DateOnly dob, AgeVerificationMethod method, string? idType, string? idLast4)
    {
        VerificationResult = _svc.VerifyAge(dob, method, idType, idLast4);
        UpdateResultPanel();
    }

    private void UpdateResultPanel()
    {
        if (VerificationResult == null) return;

        if (VerificationResult.Status == AgeVerificationStatus.Approved)
        {
            _resultPanel.BackColor = _approvedBg;
            _resultLabel.Text      = $"✅  Age Verified — Customer is {VerificationResult.CustomerAge} years old (21+)";
            _btnConfirm.Visible    = true;
            _btnAcknowledge.Visible = false;
        }
        else
        {
            _resultPanel.BackColor  = _deniedBg;
            _resultLabel.Text       = $"❌  Sale Denied — Customer is {VerificationResult.CustomerAge} years old (under 21)";
            _btnConfirm.Visible     = false;
            _btnAcknowledge.Visible = true;
        }

        _resultPanel.Invalidate();
    }

    private void ShowScanError(string message)
    {
        _resultPanel.BackColor = _pendingBg;
        _resultLabel.Text      = $"⚠  {message}";
        _resultPanel.Invalidate();
        _txtScanCapture.Focus();
    }

    private void ShowManualError(string message)
    {
        _resultPanel.BackColor = _pendingBg;
        _resultLabel.Text      = $"⚠  {message}";
        _resultPanel.Invalidate();
    }

    // ── Scan handler attach / detach ──────────────────────────────────────────

    private void DetachScanHandler(object? sender, EventArgs e)
        => _scanHandlerAttached = false;

    private void ReattachScanHandler(object? sender, EventArgs e)
    {
        _scanHandlerAttached = true;
        _txtScanCapture.Focus();
    }
}
