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
    private readonly IUserService  _userService;
    private readonly IWindowService _windowService;
    private string _pin = string.Empty;
    private System.Windows.Forms.Timer? _clock;
    private bool _loginInProgress;

    // ─────────────────────────────────────────────────────────────────
    //  Constructor
    // ─────────────────────────────────────────────────────────────────
    public frmSignIn(IUserService userService, IWindowService windowService)
    {
        InitializeComponent();
        _userService   = userService;
        _windowService = windowService;
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

        _loginInProgress = true;
        buttonLogin.Enabled = false;
        Cursor = Cursors.WaitCursor;

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

                Hide();
                _windowService.OpenHome();
            }
            else
            {
                ShowError("Invalid PIN. Please try again.");
                ClearPin();
            }
        }
        catch (Exception ex)
        {
            ShowError($"Connection error: {ex.Message}");
            ClearPin();
        }
        finally
        {
            _loginInProgress    = false;
            buttonLogin.Enabled = true;
            Cursor = Cursors.Default;
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
        var g = e.Graphics;
        using var pen = new Pen(Color.FromArgb(5, 150, 105), 2f);
        g.DrawLine(pen, 0, pnlHeader.Height - 1, pnlHeader.Width, pnlHeader.Height - 1);
    }

    // ─────────────────────────────────────────────────────────────────
    //  Custom Paint — Card top emerald border
    // ─────────────────────────────────────────────────────────────────
    private void PnlCard_Paint(object? sender, PaintEventArgs e)
    {
        using var pen = new Pen(Color.FromArgb(5, 150, 105), 3f);
        e.Graphics.DrawLine(pen, 0, 0, pnlCard.Width, 0);
    }

    // ─────────────────────────────────────────────────────────────────
    //  Custom Paint — PIN dots
    // ─────────────────────────────────────────────────────────────────
    private void PnlPinDots_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode     = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        const int MaxDots   = 12;
        const float DotSize = 13f;
        const float Gap     = 14f;

        float totalW = MaxDots * DotSize + (MaxDots - 1) * Gap;
        float startX = (pnlPinDots.Width - totalW) / 2f;
        float y      = (pnlPinDots.Height - DotSize) / 2f - 4f;

        for (int i = 0; i < MaxDots; i++)
        {
            float x = startX + i * (DotSize + Gap);
            bool  filled = i < _pin.Length;

            Color dotColor = filled
                ? Color.FromArgb(15, 23, 42)
                : Color.FromArgb(203, 213, 225);

            using var brush = new SolidBrush(dotColor);
            g.FillEllipse(brush, x, y, DotSize, DotSize);
        }

        // Underline accent
        float lineY = y + DotSize + 10f;
        using var linePen = new Pen(Color.FromArgb(5, 150, 105), 2f);
        g.DrawLine(linePen, startX - 4, lineY, startX + totalW + 4, lineY);
    }
}
