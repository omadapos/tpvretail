using Microsoft.Extensions.DependencyInjection;
using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Services;
using OmadaPOS.Libreria.Utils;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace OmadaPOS.Views;

public partial class frmSignIn
{
    private readonly IUserService userService;
    private bool isPhoneFocus = true;

    public frmSignIn()
    {
        InitializeComponent();
        userService = Program.GetService<IUserService>();
    }

    private void frmSignIn_Load(object sender, EventArgs e)
    {
        this.StartPosition = FormStartPosition.CenterScreen;

        textBoxPhone.GotFocus += (s, ev) => isPhoneFocus = true;
        textBoxPhone.Leave    += (s, ev) => textBoxPhone.Focus();

        string? windowsId = WindowsIdProvider.GetMachineGuid();
        if (!string.IsNullOrEmpty(windowsId))
            labelId.Text = windowsId;

        AplicarDisenoLogin();
    }

    // ─────────────────────────────────────────────────────────────────
    //  DISEÑO PREMIUM MARKET — POS Supermarket Login
    // ─────────────────────────────────────────────────────────────────
    private void AplicarDisenoLogin()
    {
        // ── Fondo general (30% Navy) ──────────────────────────────
        this.BackColor = AppColors.NavyDark;
        tableLayoutPanelMain.BackColor = Color.Transparent;

        // ── Branding lateral izquierdo ────────────────────────────
        AgregarPanelBranding();

        // ── Card central del keypad ───────────────────────────────
        EstilizarCardKeypad();

        // ── Display PIN ───────────────────────────────────────────
        EstilizarDisplayPin();

        // ── Botones numéricos ─────────────────────────────────────
        var numButtons = new[] { button1, button2, button3, button4,
                                  button5, button6, button7, button8, button9, button0 };
        foreach (var btn in numButtons)
            ElegantButtonStyles.Style(btn, AppColors.NavyBase, AppColors.TextWhite, radius: 10, fontSize: 34f);

        // Clear — Ámbar (advertencia)
        ElegantButtonStyles.Style(buttonClear, AppColors.Warning, AppColors.TextWhite, radius: 10, fontSize: 22f);
        buttonClear.Text = "⌫  CLEAR";

        // Login — Verde acento (acción principal)
        ElegantButtonStyles.Style(buttonLogin, AppColors.AccentGreen, AppColors.TextWhite, radius: 10, fontSize: 22f);
        buttonLogin.Text = "LOGIN  ▶";

        // ── Label ID (terminal) ───────────────────────────────────
        labelId.ForeColor = AppColors.TextMuted;
        labelId.Font = new Font("Consolas", 11F, FontStyle.Regular);
        labelId.BackColor = Color.Transparent;
    }

    private void AgregarPanelBranding()
    {
        // Panel contenedor del branding en columna 0
        var panelBrand = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            Margin = new Padding(20, 0, 10, 0)
        };

        // Logo / nombre del sistema
        var lblNombre = new Label
        {
            Text = "Omada",
            Font = new Font("Montserrat", 48F, FontStyle.Bold),
            ForeColor = AppColors.TextWhite,
            AutoSize = false,
            Dock = DockStyle.None,
            TextAlign = ContentAlignment.BottomLeft,
            BackColor = Color.Transparent,
            Location = new Point(0, 0)
        };

        var lblPos = new Label
        {
            Text = "POS",
            Font = new Font("Montserrat", 48F, FontStyle.Bold),
            ForeColor = AppColors.AccentGreen,
            AutoSize = false,
            Dock = DockStyle.None,
            TextAlign = ContentAlignment.TopLeft,
            BackColor = Color.Transparent,
            Location = new Point(0, 60)
        };

        var lblTagline = new Label
        {
            Text = "Point of Sale System",
            Font = new Font("Segoe UI", 14F, FontStyle.Regular),
            ForeColor = Color.FromArgb(160, 200, 220),
            AutoSize = false,
            Dock = DockStyle.None,
            TextAlign = ContentAlignment.TopLeft,
            BackColor = Color.Transparent,
            Location = new Point(4, 130),
            Size = new Size(350, 30)
        };

        // Línea decorativa verde
        var lblLinea = new Label
        {
            Text = "",
            BackColor = AppColors.AccentGreen,
            AutoSize = false,
            Dock = DockStyle.None,
            Location = new Point(0, 170),
            Size = new Size(80, 4)
        };

        // Subtítulo instrucción
        var lblInstruccion = new Label
        {
            Text = "Use your PIN or\nemployee ID to sign in.",
            Font = new Font("Segoe UI", 13F, FontStyle.Regular),
            ForeColor = Color.FromArgb(140, 180, 210),
            AutoSize = false,
            Dock = DockStyle.None,
            TextAlign = ContentAlignment.TopLeft,
            BackColor = Color.Transparent,
            Location = new Point(0, 190),
            Size = new Size(350, 70)
        };

        panelBrand.Controls.Add(lblNombre);
        panelBrand.Controls.Add(lblPos);
        panelBrand.Controls.Add(lblTagline);
        panelBrand.Controls.Add(lblLinea);
        panelBrand.Controls.Add(lblInstruccion);

        // Agregar a columna 0, fila 1 (misma fila que el keypad)
        tableLayoutPanelMain.Controls.Add(panelBrand, 0, 1);
    }

    private void EstilizarCardKeypad()
    {
        tableLayoutPanel1.BackColor = AppColors.BackgroundSecondary;
        tableLayoutPanel1.Padding   = new Padding(14);
        tableLayoutPanel1.Margin    = new Padding(10, 8, 10, 8);

        // Línea de acento verde en el borde superior de la card
        tableLayoutPanel1.Paint += (s, e) =>
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using var pen = new Pen(AppColors.AccentGreen, 4);
            g.DrawLine(pen, 0, 0, tableLayoutPanel1.Width, 0);
        };
    }

    private void EstilizarDisplayPin()
    {
        // Panel contenedor del PIN para darle fondo oscuro y aspecto de display
        var panelPin = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppColors.NavyDark,
            Margin = new Padding(0),
            Padding = new Padding(8, 4, 8, 4)
        };

        // Reubicar el textbox dentro del panel de PIN
        tableLayoutPanelMain.Controls.Remove(textBoxPhone);
        panelPin.Controls.Add(textBoxPhone);

        textBoxPhone.Dock = DockStyle.Fill;
        textBoxPhone.BackColor = AppColors.NavyDark;
        textBoxPhone.ForeColor = AppColors.AccentGreen;
        textBoxPhone.Font = new Font("Consolas", 40F, FontStyle.Bold);
        textBoxPhone.BorderStyle = BorderStyle.None;
        textBoxPhone.TextAlign = HorizontalAlignment.Center;

        // Línea verde inferior como cursor visual
        panelPin.Paint += (s, e) =>
        {
            using var pen = new Pen(AppColors.AccentGreen, 3);
            e.Graphics.DrawLine(pen, 12, panelPin.Height - 4, panelPin.Width - 12, panelPin.Height - 4);
        };

        tableLayoutPanelMain.Controls.Add(panelPin, 1, 0);
    }

    // ─────────────────────────────────────────────────────────────────
    //  EVENTOS — lógica sin cambios
    // ─────────────────────────────────────────────────────────────────
    private void buttonKey_Click(object sender, EventArgs e)
    {
        if (sender is Button btn && int.TryParse(btn.Tag?.ToString(), out int _))
        {
            if (isPhoneFocus)
                textBoxPhone.Text += btn.Tag;
        }
    }

    private async void buttonLogin_Click(object sender, EventArgs e)
    {
        string username = textBoxPhone.Text;
        // TODO: El servidor actualmente valida con una clave fija de sistema.
        // Reemplazar por autenticación real (PIN individual por empleado) en la API.
        const string password = "12345678";

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            MessageBox.Show("Please enter values in all fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        try
        {
            Cursor = Cursors.WaitCursor;

            var loginRequest = new LoginRequest
            {
                Email    = username,
                Password = password,
                WindowsId = labelId.Text,
            };

            LoginResponse response = await userService.Login(loginRequest);

            if (!string.IsNullOrEmpty(response.Token))
            {
                SessionManager.Token    = response.Token;
                SessionManager.BranchId = response.BranchId;
                SessionManager.UserName = username;
                SessionManager.Name     = response.Name;
                SessionManager.AdminId  = response.AdminId;
                SessionManager.Phone    = response.Phone;

                Hide();

                var homeForm = Program.ServiceProvider?.GetService<frmHome>();
                homeForm?.Show();
            }
            else
            {
                MessageBox.Show("Login error!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Login error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Cursor = Cursors.Arrow;
        }
    }

    private void buttonClear_Click(object sender, EventArgs e)
    {
        if (isPhoneFocus)
            textBoxPhone.Clear();
    }
}
