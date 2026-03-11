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

    // Static brushes for PIN dots — reused on every keypress repaint (avoids 12 allocs per call)
    private static readonly SolidBrush _dotFilledBrush = new(AppColors.NavyDark);
    private static readonly SolidBrush _dotEmptyBrush  = new(AppColors.TextOnDarkSecondary);

    // ─────────────────────────────────────────────────────────────────
    //  Constructor
    // ─────────────────────────────────────────────────────────────────
    public frmSignIn(IUserService userService, IWindowService windowService)
    {
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

        // Center card
        CenterCard();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _clock?.Stop();
        _clock?.Dispose();
        _spinnerTimer?.Stop();
        _spinnerTimer?.Dispose();
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
            Text      = "OMADA  POS",
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
            Text      = "Point of Sale",
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
        if (_pin.Length >= 20 || _loginInProgress) return;
        _pin += digit;
        pnlPinDots.Invalidate();
        HideError();
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
        using var pen = new Pen(AppColors.AccentGreenDark, 2f);
        e.Graphics.DrawLine(pen, 0, pnlHeader.Height - 1, pnlHeader.Width, pnlHeader.Height - 1);
    }

    // ─────────────────────────────────────────────────────────────────
    //  Custom Paint — Card top emerald border
    // ─────────────────────────────────────────────────────────────────
    private void PnlCard_Paint(object? sender, PaintEventArgs e)
    {
        using var pen = new Pen(AppColors.AccentGreenDark, 3f);
        e.Graphics.DrawLine(pen, 0, 0, pnlCard.Width, 0);
    }

    // ─────────────────────────────────────────────────────────────────
    //  Custom Paint — PIN dots
    //  Uses static brushes — zero allocation per keypress repaint
    // ─────────────────────────────────────────────────────────────────
    private void PnlPinDots_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode     = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        const int   MaxDots = 12;
        const float DotSize = 13f;
        const float Gap     = 14f;

        float totalW = MaxDots * DotSize + (MaxDots - 1) * Gap;
        float startX = (pnlPinDots.Width  - totalW) / 2f;
        float y      = (pnlPinDots.Height - DotSize) / 2f - 4f;

        for (int i = 0; i < MaxDots; i++)
        {
            float x      = startX + i * (DotSize + Gap);
            var   brush  = i < _pin.Length ? _dotFilledBrush : _dotEmptyBrush;
            g.FillEllipse(brush, x, y, DotSize, DotSize);
        }

        // Underline accent
        float lineY = y + DotSize + 10f;
        using var linePen = new Pen(AppColors.AccentGreenDark, 2f);
        g.DrawLine(linePen, startX - 4, lineY, startX + totalW + 4, lineY);
    }
}
