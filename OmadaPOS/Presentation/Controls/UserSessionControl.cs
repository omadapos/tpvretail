using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Utils;

namespace OmadaPOS.Presentation.Controls;

public class UserSessionControl : UserControl
{
    // Cached static font — avoids allocation inside UserButton_Paint on every repaint
    private static readonly Font _chevronFont = new("Segoe UI", 10F);

    private readonly Form _ownerForm;
    private readonly Control _dismissSurface;
    private readonly Button _userButton;
    private Panel? _menuPanel;
    private bool _menuVisible;

    public event EventHandler? SettingsRequested;
    public event EventHandler? DailyCloseRequested;
    public event EventHandler? LogoutRequested;

    private UserSessionControl(Form ownerForm, Control dismissSurface, Button userButton)
    {
        _ownerForm = ownerForm ?? throw new ArgumentNullException(nameof(ownerForm));
        _dismissSurface = dismissSurface ?? throw new ArgumentNullException(nameof(dismissSurface));
        _userButton = userButton ?? throw new ArgumentNullException(nameof(userButton));

        Dock = DockStyle.Fill;
        Margin = _userButton.Margin;
        Padding = new Padding(0);
        BackColor = Color.Transparent;

        _userButton.Dock = DockStyle.Fill;
        _userButton.Margin = new Padding(4, 4, 4, 4);
        _userButton.Padding = new Padding(0);
        _userButton.Text = string.Empty;
        _userButton.BackColor = Color.Transparent;
        _userButton.Paint += UserButton_Paint;

        Controls.Add(_userButton);
        BuildUserMenu();

        _ownerForm.MouseClick += (_, _) => HideMenu();
        _dismissSurface.MouseClick += (_, _) => HideMenu();
    }

    // Column 4 = user session zone in the 6-column header layout.
    // Fixed column avoids dependency on Designer-assigned position.
    public static UserSessionControl Attach(Form ownerForm, Control dismissSurface, TableLayoutPanel headerLayout, Button userButton, int column = 4)
    {
        ArgumentNullException.ThrowIfNull(ownerForm);
        ArgumentNullException.ThrowIfNull(dismissSurface);
        ArgumentNullException.ThrowIfNull(headerLayout);
        ArgumentNullException.ThrowIfNull(userButton);

        userButton.Parent?.Controls.Remove(userButton);

        var control = new UserSessionControl(ownerForm, dismissSurface, userButton);
        headerLayout.Controls.Add(control, column, 0);

        return control;
    }

    public void ToggleMenu()
    {
        if (_menuVisible)
            HideMenu();
        else
            ShowMenu();
    }

    public void InvalidateSession() => _userButton.Invalidate();

    public void HideMenu()
    {
        if (_menuPanel == null) return;
        _menuPanel.Visible = false;
        _menuVisible = false;
    }

    private void ShowMenu()
    {
        if (_menuPanel == null) return;

        var lbl = _menuPanel.Controls.OfType<Label>().FirstOrDefault(x => x.Name == "lblFlyoutName");
        if (lbl != null)
            lbl.Text = $"👤  {SessionManager.Name}";

        var btnPos = _userButton.PointToScreen(Point.Empty);
        var formPos = _ownerForm.PointToClient(btnPos);

        _menuPanel.Location = new Point(
            formPos.X + _userButton.Width - _menuPanel.Width,
            formPos.Y + _userButton.Height + 2);

        _menuPanel.BringToFront();
        _menuPanel.Visible = true;
        _menuVisible = true;
    }

    private void BuildUserMenu()
    {
        _menuPanel = new Panel
        {
            Size = new Size(260, 190),
            BackColor = AppColors.NavyBase,
            Visible = false,
        };
        _menuPanel.Paint += MenuPanel_Paint;

        var lblNombre = new Label
        {
            Name      = "lblFlyoutName",
            Text      = $"👤  {SessionManager.Name}",
            Font      = AppTypography.PaymentLabel,
            ForeColor = AppColors.TextWhite,
            BackColor = Color.Transparent,
            Dock      = DockStyle.Top,
            Height    = 48,
            TextAlign = ContentAlignment.MiddleCenter,
            Padding   = new Padding(0, 6, 0, 0),
        };

        var sep = new Panel
        {
            Height    = 1,
            Dock      = DockStyle.Top,
            BackColor = AppBorders.SeparatorOnDark,
            Margin    = AppSpacing.None,
        };

        var btnSettings = CreateFlyoutButton("⚙  Settings", AppColors.NavyBase);
        btnSettings.Click += (s, e) =>
        {
            HideMenu();
            SettingsRequested?.Invoke(this, EventArgs.Empty);
        };

        var btnDailyClose = CreateFlyoutButton("📋  Daily Close", AppColors.NavyBase);
        btnDailyClose.Click += (s, e) =>
        {
            HideMenu();
            DailyCloseRequested?.Invoke(this, EventArgs.Empty);
        };

        var btnLogout = CreateFlyoutButton("⏻  Logout", AppColors.Danger);
        btnLogout.Click += (s, e) =>
        {
            HideMenu();
            LogoutRequested?.Invoke(this, EventArgs.Empty);
        };

        _menuPanel.Controls.Add(btnLogout);
        _menuPanel.Controls.Add(btnDailyClose);
        _menuPanel.Controls.Add(btnSettings);
        _menuPanel.Controls.Add(sep);
        _menuPanel.Controls.Add(lblNombre);

        _ownerForm.Controls.Add(_menuPanel);
        _menuPanel.BringToFront();
    }

    private static Button CreateFlyoutButton(string text, Color bgColor)
    {
        var btn = new Button
        {
            Text      = text,
            Font      = AppTypography.Body,
            ForeColor = AppColors.TextWhite,
            BackColor = bgColor,
            FlatStyle = FlatStyle.Flat,
            Dock      = DockStyle.Top,
            Height    = 44,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding   = new Padding(18, 0, 0, 0),
            Cursor    = Cursors.Hand,
            TabStop   = false,
        };
        btn.FlatAppearance.BorderSize = 0;
        btn.FlatAppearance.MouseOverBackColor = Color.Transparent;
        return btn;
    }

    private void MenuPanel_Paint(object? sender, PaintEventArgs e)
    {
        if (sender is not Panel panel) return;
        var g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        var bounds = new Rectangle(0, 0, panel.Width - 1, panel.Height - 1);
        using var path = ElegantButtonStyles.RoundedPath(bounds, 10);
        using var bg = new SolidBrush(AppColors.NavyBase);
        g.FillPath(bg, path);

        using var accent = new SolidBrush(AppColors.AccentGreen);
        g.FillRectangle(accent, bounds.X + 12, bounds.Y, bounds.Width - 24, 3);

        using var border = new Pen(AppBorders.SeparatorOnDark, AppBorders.Thin);
        g.DrawPath(border, path);
    }

    private void UserButton_Paint(object? sender, PaintEventArgs e)
    {
        if (sender is not Button btn) return;

        var g = e.Graphics;
        g.SmoothingMode     = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        var bounds = new Rectangle(2, 2, btn.Width - 4, btn.Height - 4);
        using var bgBrush = new SolidBrush(AppColors.NavyBase);
        using var bgPath  = ElegantButtonStyles.RoundedPath(bounds, bounds.Height / 2);
        g.FillPath(bgBrush, bgPath);

        using var borderPen = new Pen(AppBorders.SeparatorOnDark, AppBorders.Thin);
        g.DrawPath(borderPen, bgPath);

        int avatarSize = bounds.Height - 8;
        var avatarRect = new Rectangle(bounds.X + 8, bounds.Y + 4, avatarSize, avatarSize);
        using var avatarBrush = new SolidBrush(AppColors.AccentGreen);
        g.FillEllipse(avatarBrush, avatarRect);

        string inicial = (SessionManager.Name?.Length > 0) ? SessionManager.Name[0].ToString().ToUpper() : "?";
        var       inicialFont  = AppTypography.AppHeader;     // static shared — do NOT dispose
        using var inicialBrush = new SolidBrush(AppColors.TextWhite);
        using var sfCenter     = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        g.DrawString(inicial, inicialFont, inicialBrush, avatarRect, sfCenter);

        string nombre = SessionManager.Name?.Split(' ')[0] ?? "User";
        if (nombre.Length > 10) nombre = nombre[..10];
        var nameRect = new Rectangle(avatarRect.Right + 6, bounds.Y, bounds.Width - avatarRect.Right - 22, bounds.Height);
        var       nameFont  = AppTypography.BodySmall;        // static shared — do NOT dispose
        using var nameBrush = new SolidBrush(AppColors.TextWhite);
        using var sfLeft    = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
        g.DrawString(nombre, nameFont, nameBrush, nameRect, sfLeft);

        var chevronRect = new Rectangle(bounds.Right - 20, bounds.Y, 18, bounds.Height);
        using var chevronBrush = new SolidBrush(AppColors.TextMuted);
        g.DrawString("▾", _chevronFont, chevronBrush, chevronRect, sfCenter);
    }
}
