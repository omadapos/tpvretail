using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Services;
using OmadaPOS.Libreria.Utils;
using OmadaPOS.Services.Navigation;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace OmadaPOS.Views;

public partial class frmSignIn
{
    // ─────────────────────────────────────────────────────────────────
    //  State
    // ─────────────────────────────────────────────────────────────────
    private readonly IUserService   _userService;
    private readonly IWindowService _windowService;
    private string _pin = string.Empty;
    private System.Windows.Forms.Timer? _clock;
    private bool _loginInProgress;

    // Overlay
    private Panel?  _pnlOverlay;
    private Label?  _lblOverlayStatus;
    private System.Windows.Forms.Timer? _spinnerTimer;

    // Static brushes for PIN dots — emerald filled, dark slate empty
    private static readonly SolidBrush _dotFilledBrush = new(Color.FromArgb(52, 211, 153));   // AccentGreenLight
    private static readonly SolidBrush _dotEmptyBrush  = new(Color.FromArgb(51, 65, 85));     // NavyLight

    // ─────────────────────────────────────────────────────────────────
    //  Constructor
    // ─────────────────────────────────────────────────────────────────
    public frmSignIn(IUserService userService, IWindowService windowService)
    {
        DoubleBuffered = true;
        InitializeComponent();
        _userService   = userService;
        _windowService = windowService;
        BuildOverlay();
    }

    // ─────────────────────────────────────────────────────────────────
    //  Load
    // ─────────────────────────────────────────────────────────────────
    private void FrmSignIn_Load(object sender, EventArgs e)
    {
        // Terminal ID in footer
        string? guid = WindowsIdProvider.GetMachineGuid();
        labelId.Text = string.IsNullOrEmpty(guid) ? "Terminal ID: N/A" : $"Terminal: {guid}";

        // Clock
        _clock = new System.Windows.Forms.Timer { Interval = 1000 };
        _clock.Tick += (_, _) => UpdateClock();
        _clock.Start();
        UpdateClock();

        // Rounded card corners — 14 px radius
        ApplyCardRegion();

        // Subtle dot-grid on background
        pnlBackground.Paint += PnlBackground_GridPaint;

        // Center card
        CenterCard();
    }

    private void ApplyCardRegion()
    {
        const int R = 14;
        int w = pnlCard.Width, h = pnlCard.Height;
        var path = new System.Drawing.Drawing2D.GraphicsPath();
        path.AddArc(0,     0,     R * 2, R * 2, 180, 90);
        path.AddArc(w - R * 2, 0,     R * 2, R * 2, 270, 90);
        path.AddArc(w - R * 2, h - R * 2, R * 2, R * 2,   0, 90);
        path.AddArc(0,     h - R * 2, R * 2, R * 2,  90, 90);
        path.CloseAllFigures();
        pnlCard.Region = new Region(path);
    }

    private void PnlBackground_GridPaint(object? sender, PaintEventArgs e)
    {
        const int Spacing = 28;
        using var dot = new SolidBrush(Color.FromArgb(45, 255, 255, 255)); // slightly more visible on dark navy
        for (int x = Spacing; x < pnlBackground.Width; x += Spacing)
            for (int y = Spacing; y < pnlBackground.Height; y += Spacing)
                e.Graphics.FillEllipse(dot, x - 1f, y - 1f, 2f, 2f);
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _clock?.Stop();
        _clock?.Dispose();
        _spinnerTimer?.Stop();
        _spinnerTimer?.Dispose();
        _errorCts?.Cancel();
        _errorCts?.Dispose();
        _errorCts = null;
        base.OnFormClosed(e);
    }

    // ─────────────────────────────────────────────────────────────────
    //  Clock
    // ─────────────────────────────────────────────────────────────────
    private void UpdateClock()
    {
        if (lblClock.IsHandleCreated)
            lblClock.Text = DateTime.Now.ToString("hh:mm tt");
    }

    // ─────────────────────────────────────────────────────────────────
    //  Loading overlay
    // ─────────────────────────────────────────────────────────────────
    private void BuildOverlay()
    {
        const int W    = 380;
        const int H    = 240;
        const int SpinD = 60;
        float spinAngle = 0f;

        // ── Brand ────────────────────────────────────────────────────────
        var lblBrand = new Label
        {
            Text      = AppConstants.AppName,
            Font      = new Font("Segoe UI", 24F, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            AutoSize  = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Bounds    = new Rectangle(0, 10, W, 56),
        };

        // Emerald "Point of Sale" subtitle
        var lblSub = new Label
        {
            Text      = AppConstants.AppTagline,
            Font      = new Font("Segoe UI", 10F, FontStyle.Regular),
            ForeColor = AppColors.AccentGreen,
            BackColor = Color.Transparent,
            AutoSize  = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Bounds    = new Rectangle(0, 62, W, 22),
        };

        // ── GDI+ animated spinner (arc) ──────────────────────────────────
        var spinPanel = new Panel
        {
            BackColor = Color.Transparent,
            Bounds    = new Rectangle((W - SpinD) / 2, 94, SpinD, SpinD),
        };
        spinPanel.Paint += (_, e) =>
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var r = new RectangleF(4, 4, SpinD - 8, SpinD - 8);

            // Faint background ring
            using var bgPen = new Pen(Color.FromArgb(35, 16, 185, 129), 5f)
                { StartCap = LineCap.Round, EndCap = LineCap.Round };
            g.DrawEllipse(bgPen, r);

            // Rotating emerald arc
            using var fgPen = new Pen(AppColors.AccentGreen, 5f)
                { StartCap = LineCap.Round, EndCap = LineCap.Round };
            g.DrawArc(fgPen, r, spinAngle, 100f);
        };

        // ── Status message ───────────────────────────────────────────────
        _lblOverlayStatus = new Label
        {
            Text      = string.Empty,
            Font      = new Font("Segoe UI", 10F, FontStyle.Regular),
            ForeColor = Color.FromArgb(148, 163, 184),   // #94A3B8 muted slate
            BackColor = Color.Transparent,
            AutoSize  = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Bounds    = new Rectangle(0, 164, W, 32),
        };

        // Version / footer hint
        var lblFooter = new Label
        {
            Text      = "Iniciando sistema…",
            Font      = new Font("Segoe UI", 8F, FontStyle.Italic),
            ForeColor = Color.FromArgb(71, 85, 105),   // #475569 very muted
            BackColor = Color.Transparent,
            AutoSize  = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Bounds    = new Rectangle(0, 200, W, 22),
        };

        // ── Center card panel ────────────────────────────────────────────
        var pnlCenter = new Panel
        {
            BackColor = Color.Transparent,
            Size      = new Size(W, H),
        };
        pnlCenter.Controls.AddRange([lblBrand, lblSub, spinPanel, _lblOverlayStatus, lblFooter]);

        // ── Full-screen dark overlay ─────────────────────────────────────
        _pnlOverlay = new Panel
        {
            BackColor = Color.FromArgb(10, 17, 32),   // deep navy — always dark regardless of theme
            Visible   = false,
        };
        // Paint subtle vignette border
        _pnlOverlay.Paint += (_, e) =>
        {
            using var pen = new Pen(AppColors.AccentGreen, 2f);
            e.Graphics.DrawLine(pen, 0, 0, _pnlOverlay.Width, 0);         // top accent
            e.Graphics.DrawLine(pen, 0, _pnlOverlay.Height - 2,           // bottom accent
                _pnlOverlay.Width, _pnlOverlay.Height - 2);
        };

        _pnlOverlay.Controls.Add(pnlCenter);
        _pnlOverlay.Resize += (_, _) => pnlCenter.Location = new Point(
            (_pnlOverlay.Width  - W) / 2,
            (_pnlOverlay.Height - H) / 2);

        Controls.Add(_pnlOverlay);

        // 40 ms = 25 fps, 6° per tick
        _spinnerTimer = new System.Windows.Forms.Timer { Interval = 40 };
        _spinnerTimer.Tick += (_, _) =>
        {
            spinAngle = (spinAngle + 6f) % 360f;
            if (spinPanel.IsHandleCreated) spinPanel.Invalidate();
        };
    }

    private void ShowOverlay(string message)
    {
        if (_pnlOverlay is null) return;
        _pnlOverlay.Bounds = new Rectangle(0, 0, ClientSize.Width, ClientSize.Height);
        if (_lblOverlayStatus is not null) _lblOverlayStatus.Text = message;
        _pnlOverlay.Visible = true;
        _pnlOverlay.BringToFront();
        _spinnerTimer?.Start();
    }

    private void SetOverlayMessage(string message)
    {
        if (_lblOverlayStatus is not null) _lblOverlayStatus.Text = message;
    }

    private void HideOverlay()
    {
        _spinnerTimer?.Stop();
        if (_pnlOverlay is not null) _pnlOverlay.Visible = false;
    }

    // ─────────────────────────────────────────────────────────────────
    //  Card centering
    // ─────────────────────────────────────────────────────────────────
    private void PnlBackground_Resize(object? sender, EventArgs e) => CenterCard();

    private void CenterCard()
    {
        pnlCard.Location = new Point(
            (pnlBackground.Width  - pnlCard.Width)  / 2,
            (pnlBackground.Height - pnlCard.Height) / 2);
    }

    // ─────────────────────────────────────────────────────────────────
    //  PIN management
    // ─────────────────────────────────────────────────────────────────
    internal void AppendDigit(string digit)
    {
        if (_pin.Length >= 6 || _loginInProgress) return;
        _pin += digit;
        pnlPinDots.Invalidate();
        HideError();

        // Auto-submit when the 6th digit is entered
        if (_pin.Length == 6)
            ButtonLogin_Click(this, EventArgs.Empty);
    }

    private void ClearPin()
    {
        _pin = string.Empty;
        pnlPinDots.Invalidate();
    }

    // ─────────────────────────────────────────────────────────────────
    //  Button events
    // ─────────────────────────────────────────────────────────────────
    private void ButtonClear_Click(object sender, EventArgs e)
    {
        if (_loginInProgress) return;

        if (_pin.Length > 0)
        {
            // Single press: delete last digit; hold Clear clears all (handled below)
            _pin = _pin[..^1];
            pnlPinDots.Invalidate();
        }

        HideError();
    }

    private async void ButtonLogin_Click(object sender, EventArgs e)
    {
        if (_loginInProgress) return;
        await DoLoginAsync();
    }

    // ─────────────────────────────────────────────────────────────────
    //  Login logic
    // ─────────────────────────────────────────────────────────────────
    private async Task DoLoginAsync()
    {
        if (string.IsNullOrWhiteSpace(_pin))
        {
            ShowError("Please enter your PIN or Employee ID.");
            return;
        }

        _loginInProgress    = true;
        buttonLogin.Enabled = false;
        ShowOverlay("Verificando credenciales…");

        try
        {
            var request = new LoginRequest
            {
                Email     = _pin,
                Password  = "12345678",
                WindowsId = labelId.Text,
            };

            LoginResponse response = await _userService.Login(request);

            if (!string.IsNullOrEmpty(response.Token))
            {
                SessionManager.Token    = response.Token;
                SessionManager.BranchId = response.BranchId;
                SessionManager.UserName = _pin;
                SessionManager.Name     = response.Name;
                SessionManager.AdminId  = response.AdminId;
                SessionManager.Phone    = response.Phone;

                SetOverlayMessage("Iniciando sistema…");
                _windowService.OpenHome();
                HideOverlay();
                Hide();
            }
            else
            {
                HideOverlay();
                ShowError("Invalid PIN. Please try again.");
                ClearPin();
            }
        }
        catch (Exception ex)
        {
            HideOverlay();
            Show();
            ShowError($"Error: {ex.Message}");
            ClearPin();
        }
        finally
        {
            _loginInProgress    = false;
            buttonLogin.Enabled = true;
        }
    }

    // ─────────────────────────────────────────────────────────────────
    //  Inline error feedback
    // ─────────────────────────────────────────────────────────────────
    private CancellationTokenSource? _errorCts;

    private async void ShowError(string message)
    {
        _errorCts?.Cancel();
        _errorCts = new CancellationTokenSource();
        var token = _errorCts.Token;

        lblError.Text    = message;
        lblError.Visible = true;

        try
        {
            await Task.Delay(3500, token);
            if (!token.IsCancellationRequested)
                lblError.Visible = false;
        }
        catch (TaskCanceledException) { }
    }

    private void HideError()
    {
        _errorCts?.Cancel();
        lblError.Visible = false;
    }

    // ─────────────────────────────────────────────────────────────────
    //  Custom Paint — Header accent line
    // ─────────────────────────────────────────────────────────────────
    private void PnlHeader_Paint(object? sender, PaintEventArgs e)
    {
        using var pen = new Pen(Color.FromArgb(16, 185, 129), 2f);
        e.Graphics.DrawLine(pen, 0, pnlHeader.Height - 1, pnlHeader.Width, pnlHeader.Height - 1);
    }

    // ─────────────────────────────────────────────────────────────────
    //  Custom Paint — Card top emerald border (3 px, respects rounded Region)
    // ─────────────────────────────────────────────────────────────────
    private void PnlCard_Paint(object? sender, PaintEventArgs e)
    {
        using var pen = new Pen(Color.FromArgb(5, 150, 105), 3f);
        e.Graphics.DrawLine(pen, 14, 1, pnlCard.Width - 14, 1);
    }

    // ─────────────────────────────────────────────────────────────────
    //  Custom Paint — PIN dots
    //  Uses static brushes — zero allocation per keypress repaint
    // ─────────────────────────────────────────────────────────────────
    private void PnlPinDots_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        const int   MaxDots = 6;
        const float DotSize = 18f;
        const float Gap     = 22f;

        float totalW = MaxDots * DotSize + (MaxDots - 1) * Gap;
        float startX = (pnlPinDots.Width  - totalW) / 2f;
        float y      = (pnlPinDots.Height - DotSize) / 2f;

        for (int i = 0; i < MaxDots; i++)
        {
            float x     = startX + i * (DotSize + Gap);
            var   brush = i < _pin.Length ? _dotFilledBrush : _dotEmptyBrush;
            g.FillEllipse(brush, x, y, DotSize, DotSize);
        }
    }
}
