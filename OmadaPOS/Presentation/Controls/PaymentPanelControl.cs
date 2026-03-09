using OmadaPOS.Componentes;

namespace OmadaPOS.Presentation.Controls;

/// <summary>
/// Self-contained payment column for frmHome.
/// Builds its own UI entirely in code — no Designer dependencies.
///
/// Layout (top → bottom):
///   Zone 1 (~46%) — NumericPad (MoneyWithBills)
///   Zone 2  (46px) — Tools strip: Quick Sale | Lookup UPC
///   Zone 3 (~14%) — Tender / Change summary card
///   Zone 4 (~16%) — Scale display (weight + product image)
///   Zone 5 (~24%) — Payment buttons  4 × 2 grid
///
/// Usage:
///   _paymentPanelControl = PaymentPanelControl.Attach(tableLayoutPanelMain, tableLayoutPanelPayment);
///   _paymentPanelControl.CashPayClicked += buttonPayCash_Click;
///   // ... wire remaining events
/// </summary>
public sealed class PaymentPanelControl : UserControl
{
    // ── Public events — wire these to the existing frmHome click handlers ─────
    public event EventHandler? CashPayClicked;
    public event EventHandler? CreditPayClicked;
    public event EventHandler? DebitPayClicked;
    public event EventHandler? EBTFoodClicked;
    public event EventHandler? EBTBalanceClicked;
    public event EventHandler? SplitPayClicked;
    public event EventHandler? GiftCardClicked;
    public event EventHandler? OpenDrawerClicked;
    public event EventHandler? QuickSaleClicked;
    public event EventHandler? UPCLookupClicked;
    public event EventHandler? NumpadValueChanged;
    public event EventHandler? ScalePictureClicked;

    // ── Scale state — read by frmHome.pictureBoxPesado_Click logic ────────────
    public string ScaleProductDisplayText { get; private set; } = string.Empty;
    public string ScaleProductId          { get; private set; } = string.Empty;

    // ── Numpad access ─────────────────────────────────────────────────────────
    public decimal ValueDecimal => _numPad.ValueDecimal;
    public long    ValueCents   => _numPad.ValueCents;

    // ── Private controls ──────────────────────────────────────────────────────
    private NumericPadControl _numPad      = null!;
    private Label _lblTenderAmt            = null!;
    private Label _lblChangeAmt            = null!;
    private Label _lblWeight               = null!;
    private Label _lblProduct              = null!;
    private Label _lblScaleStatus          = null!;
    private PictureBox _pbScale            = null!;

    // ── Factory ───────────────────────────────────────────────────────────────
    /// <summary>
    /// Replaces <paramref name="oldPaymentPanel"/> in <paramref name="mainLayout"/>
    /// with a new self-contained PaymentPanelControl.
    /// The old panel is removed and disposed.
    /// </summary>
    public static PaymentPanelControl Attach(
        TableLayoutPanel mainLayout,
        TableLayoutPanel oldPaymentPanel)
    {
        ArgumentNullException.ThrowIfNull(mainLayout);
        ArgumentNullException.ThrowIfNull(oldPaymentPanel);

        var pos = mainLayout.GetPositionFromControl(oldPaymentPanel);
        mainLayout.Controls.Remove(oldPaymentPanel);
        oldPaymentPanel.Dispose();

        var ctrl = new PaymentPanelControl { Dock = DockStyle.Fill };
        mainLayout.Controls.Add(ctrl, pos.Column, pos.Row);
        return ctrl;
    }

    // ── Constructor ───────────────────────────────────────────────────────────
    private PaymentPanelControl()
    {
        DoubleBuffered = true;
        BackColor      = AppColors.BackgroundPrimary;
        Padding        = AppSpacing.PaymentColumn;
        Margin         = new Padding(2);
        BuildUI();
    }

    // ── UI construction ───────────────────────────────────────────────────────
    private void BuildUI()
    {
        var root = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 1,
            RowCount    = 5,
            BackColor   = Color.Transparent,
            Padding     = new Padding(0),
            Margin      = new Padding(0),
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Percent,  46));  // Zone 1: NumPad
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));  // Zone 2: Tools strip
        root.RowStyles.Add(new RowStyle(SizeType.Percent,  14));  // Zone 3: Tender/Change
        root.RowStyles.Add(new RowStyle(SizeType.Percent,  16));  // Zone 4: Scale
        root.RowStyles.Add(new RowStyle(SizeType.Percent,  24));  // Zone 5: Buttons

        root.Controls.Add(BuildNumpad(),      0, 0);
        root.Controls.Add(BuildToolsStrip(),  0, 1);
        root.Controls.Add(BuildSummaryCard(), 0, 2);
        root.Controls.Add(BuildScale(),       0, 3);
        root.Controls.Add(BuildButtons(),     0, 4);

        Controls.Add(root);
    }

    // ── Zone 1 — NumericPad ───────────────────────────────────────────────────
    private Control BuildNumpad()
    {
        _numPad = new NumericPadControl(NumericPadControl.PadMode.MoneyWithBills)
        {
            Dock      = DockStyle.Fill,
            BackColor = AppColors.BackgroundPrimary,
        };
        _numPad.ValueChanged += (s, e) => NumpadValueChanged?.Invoke(s, e);
        return _numPad;
    }

    // ── Zone 2 — Tools strip ─────────────────────────────────────────────────
    private Control BuildToolsStrip()
    {
        var panel = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 2,
            RowCount    = 1,
            BackColor   = AppColors.BackgroundPrimary,
            Padding     = new Padding(0, 4, 0, 4),
            Margin      = new Padding(0),
        };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var btnQsale  = MakeButton("QUICK SALE",  ElegantButtonStyles.CashGreen, 13f, 0, 3);
        var btnLookup = MakeButton("LOOKUP UPC",  ElegantButtonStyles.Keypad,    13f, 3, 0);

        btnQsale.Click  += (s, e) => QuickSaleClicked?.Invoke(s, e);
        btnLookup.Click += (s, e) => UPCLookupClicked?.Invoke(s, e);

        panel.Controls.Add(btnQsale,  0, 0);
        panel.Controls.Add(btnLookup, 1, 0);
        return panel;
    }

    // ── Zone 3 — Tender / Change card ────────────────────────────────────────
    private Control BuildSummaryCard()
    {
        var card = new RoundedPanel
        {
            Dock            = DockStyle.Fill,
            BackgroundStart = AppColors.BackgroundSecondary,
            BackgroundEnd   = AppColors.SurfaceMuted,
            BorderColor     = AppBorders.PanelLight,
            ShadowColor     = AppShadows.Subtle,
            CornerRadius    = AppRadii.Panel,
            Padding         = AppSpacing.SummaryInner,
            Margin          = AppSpacing.PanelMargin,
        };

        var grid = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 2,
            RowCount    = 2,
            BackColor   = Color.Transparent,
            Padding     = new Padding(0),
            Margin      = new Padding(0),
        };
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
        grid.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        grid.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

        _lblTenderAmt = AmountLabel("0.00", AppColors.TextPrimary);
        _lblChangeAmt = AmountLabel("0.00", AppColors.AccentGreen);

        grid.Controls.Add(SummaryLabel("Tender"), 0, 0);
        grid.Controls.Add(_lblTenderAmt,           1, 0);
        grid.Controls.Add(SummaryLabel("Change"),  0, 1);
        grid.Controls.Add(_lblChangeAmt,           1, 1);

        card.Controls.Add(grid);
        return card;
    }

    // ── Zone 4 — Scale display ────────────────────────────────────────────────
    private Control BuildScale()
    {
        var outer = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 2,
            RowCount    = 1,
            BackColor   = AppColors.NavyDark,
            Padding     = AppSpacing.ScaleSection,
            Margin      = new Padding(0, 2, 0, 2),
        };
        outer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));
        outer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
        outer.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        // Left: weight + product + status labels
        var left = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 1,
            RowCount    = 3,
            BackColor   = Color.Transparent,
            Padding     = new Padding(0),
            Margin      = new Padding(0),
        };
        left.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        left.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
        left.RowStyles.Add(new RowStyle(SizeType.Percent, 35));
        left.RowStyles.Add(new RowStyle(SizeType.Percent, 25));

        _lblWeight = new Label
        {
            Dock      = DockStyle.Fill,
            Font      = AppTypography.WeightDisplay,
            ForeColor = AppColors.Warning,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleLeft,
        };
        _lblProduct = new Label
        {
            Dock      = DockStyle.Fill,
            Font      = AppTypography.Body,
            ForeColor = AppColors.TextWhite,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleLeft,
        };
        _lblScaleStatus = new Label
        {
            Dock      = DockStyle.Fill,
            Font      = AppTypography.Caption,
            ForeColor = AppColors.TextMuted,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleLeft,
        };

        left.Controls.Add(_lblWeight,      0, 0);
        left.Controls.Add(_lblProduct,     0, 1);
        left.Controls.Add(_lblScaleStatus, 0, 2);

        // Right: product image (tappable to add weighted item)
        _pbScale = new PictureBox
        {
            Dock     = DockStyle.Fill,
            BackColor = AppColors.NavyDark,
            SizeMode = PictureBoxSizeMode.Zoom,
            Cursor   = Cursors.Hand,
        };
        _pbScale.Click += (s, e) => ScalePictureClicked?.Invoke(s, e);

        outer.Controls.Add(left,     0, 0);
        outer.Controls.Add(_pbScale, 1, 0);
        return outer;
    }

    // ── Zone 5 — Payment buttons 4 × 2 ──────────────────────────────────────
    private Control BuildButtons()
    {
        var grid = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 4,
            RowCount    = 2,
            BackColor   = AppColors.BackgroundPrimary,
            Padding     = AppSpacing.ButtonGroup,
            Margin      = new Padding(0),
        };
        // 4 × 25% = 100% (previously 4 × 20% = 80% — fixed)
        for (int i = 0; i < 4; i++)
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        grid.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        grid.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

        // Row 0 — primary payment methods
        var btnCredit = PayBtn("CREDIT", AppColors.PaymentCredit,       22f);
        var btnDebit  = PayBtn("DEBIT",  ElegantButtonStyles.DebitGray, 18f);
        var btnSplit  = PayBtn("SPLIT",  AppColors.PaymentSplit,        18f);
        var btnCash   = PayBtn("CASH",   AppColors.AccentGreen,         22f);

        btnCredit.Click += (s, e) => CreditPayClicked?.Invoke(s, e);
        btnDebit.Click  += (s, e) => DebitPayClicked?.Invoke(s, e);
        btnSplit.Click  += (s, e) => SplitPayClicked?.Invoke(s, e);
        btnCash.Click   += (s, e) => CashPayClicked?.Invoke(s, e);

        grid.Controls.Add(btnCredit, 0, 0);
        grid.Controls.Add(btnDebit,  1, 0);
        grid.Controls.Add(btnSplit,  2, 0);
        grid.Controls.Add(btnCash,   3, 0);

        // Row 1 — secondary methods
        var btnEBTBal = PayBtn("EBT BAL",  AppColors.Warning,         16f);
        var btnEBTFd  = PayBtn("EBT FOOD", AppColors.PaymentEBT,      16f);
        var btnDrawer = PayBtn("DRAWER",   AppColors.NavyLight,       16f);
        var btnGift   = PayBtn("GIFT",     AppColors.PaymentGiftCard, 16f);

        btnEBTBal.Click += (s, e) => EBTBalanceClicked?.Invoke(s, e);
        btnEBTFd.Click  += (s, e) => EBTFoodClicked?.Invoke(s, e);
        btnDrawer.Click += (s, e) => OpenDrawerClicked?.Invoke(s, e);
        btnGift.Click   += (s, e) => GiftCardClicked?.Invoke(s, e);

        grid.Controls.Add(btnEBTBal, 0, 1);
        grid.Controls.Add(btnEBTFd,  1, 1);
        grid.Controls.Add(btnDrawer, 2, 1);
        grid.Controls.Add(btnGift,   3, 1);

        return grid;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────
    private static Button MakeButton(string text, Color bg, float size, int marginL, int marginR)
    {
        var btn = new Button
        {
            Text   = text,
            Dock   = DockStyle.Fill,
            Margin = new Padding(marginL, 0, marginR, 0),
        };
        ElegantButtonStyles.Style(btn, bg, AppColors.TextWhite, fontSize: size);
        return btn;
    }

    private static Button PayBtn(string text, Color bg, float size)
    {
        var btn = new Button { Text = text, Dock = DockStyle.Fill, Margin = new Padding(2) };
        ElegantButtonStyles.Style(btn, bg, AppColors.TextWhite, fontSize: size);
        return btn;
    }

    private static Label SummaryLabel(string text) => new()
    {
        Text      = text,
        Font      = AppTypography.PaymentLabel,
        ForeColor = AppColors.SlateBlue,
        BackColor = Color.Transparent,
        Dock      = DockStyle.Fill,
        TextAlign = ContentAlignment.MiddleLeft,
    };

    private static Label AmountLabel(string text, Color color) => new()
    {
        Text      = text,
        Font      = AppTypography.AmountDisplay,
        ForeColor = color,
        BackColor = Color.Transparent,
        Dock      = DockStyle.Fill,
        TextAlign = ContentAlignment.MiddleRight,
    };

    // ── Public update API ─────────────────────────────────────────────────────
    public void UpdatePaymentValues(decimal tendered, decimal change)
    {
        _lblTenderAmt.Text = tendered.ToString("N2");
        _lblChangeAmt.Text = change.ToString("N2");
    }

    public void ResetPaymentValues()
    {
        _lblTenderAmt.Text = "0.00";
        _lblChangeAmt.Text = "0.00";
    }

    public void Reset()
    {
        _numPad.Reset();
        ResetPaymentValues();
    }

    /// <summary>Sets scale status text — safe to call from any thread.</summary>
    public void SetScaleStatus(string status)
    {
        if (_lblScaleStatus.InvokeRequired)
            _lblScaleStatus.Invoke(() => _lblScaleStatus.Text = status);
        else
            _lblScaleStatus.Text = status;
    }

    /// <summary>Sets scale weight — safe to call from any thread (Zebra scanner callback).</summary>
    public void SetScaleWeight(string weightStatus)
    {
        if (_lblWeight.InvokeRequired)
            _lblWeight.Invoke(() => _lblWeight.Text = weightStatus);
        else
            _lblWeight.Text = weightStatus;
    }

    /// <summary>Sets the product awaiting weight confirmation.</summary>
    public void SetScaleProduct(string displayText, string productId, string imageLocation)
    {
        ScaleProductDisplayText  = displayText;
        ScaleProductId           = productId;
        _lblProduct.Text         = displayText;
        _pbScale.ImageLocation   = imageLocation;
    }

    /// <summary>Clears scale product state after it has been added to cart.</summary>
    public void ClearScaleProduct()
    {
        ScaleProductDisplayText = string.Empty;
        ScaleProductId          = string.Empty;
        _lblProduct.Text        = string.Empty;
        _lblWeight.Text         = string.Empty;
        _pbScale.Image          = null;
    }

    /// <summary>Apply overall background theme. Called by frmHome.AplicarEstiloVisual.</summary>
    public void ApplyTheme()
    {
        BackColor = AppColors.BackgroundPrimary;
    }
}
