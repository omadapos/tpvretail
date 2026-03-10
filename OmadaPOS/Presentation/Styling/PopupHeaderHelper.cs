namespace OmadaPOS.Presentation.Styling;

/// <summary>
/// Centralises the contextual header pattern used by popup forms.
/// Previously each popup duplicated ~50 lines of Panel / Label creation code.
///
/// Usage:
///   PopupHeaderHelper.AddHeader(this, AppColors.Info, "⌕", "Product Lookup", "Scan or enter barcode");
///   PopupHeaderHelper.ConfigureSize(this, anchoPorc: 0.42, altoPorc: 0.72);
/// </summary>
public static class PopupHeaderHelper
{
    private const int HeaderHeight  = 70;
    private const int AccentLineH   = 3;
    private const int IconX         = 16;
    private const int IconY         = 14;
    private const int TextX         = 58;
    private const int TitleY        = 10;
    private const int SubtitleY     = 38;

    /// <summary>
    /// Creates and attaches a coloured header panel at the top of <paramref name="target"/>.
    /// The panel contains an icon label, a title and an optional subtitle,
    /// plus an emerald accent line along the bottom edge.
    /// </summary>
    public static Panel AddHeader(
        Form   target,
        Color  color,
        string icon,
        string title,
        string subtitle = "")
    {
        var panel = new Panel
        {
            Dock      = DockStyle.Top,
            Height    = HeaderHeight,
            BackColor = color,
            Padding   = new Padding(16, 0, 16, 0)
        };

        var lblIcon = new Label
        {
            Text      = icon,
            Font      = AppTypography.AppHeader,
            ForeColor = AppColors.OverlayLight,
            AutoSize  = true,
            Location  = new Point(IconX, IconY),
            BackColor = Color.Transparent
        };

        var lblTitle = new Label
        {
            Text      = title,
            Font      = AppTypography.PopupTitle,
            ForeColor = AppColors.TextWhite,
            AutoSize  = true,
            Location  = new Point(TextX, TitleY),
            BackColor = Color.Transparent
        };

        panel.Controls.Add(lblIcon);
        panel.Controls.Add(lblTitle);

        if (!string.IsNullOrWhiteSpace(subtitle))
        {
            var lblSub = new Label
            {
                Text      = subtitle,
                Font      = AppTypography.ChevronIcon,   // Segoe UI 10 Regular — compact hint
                ForeColor = AppColors.OverlayLight,
                AutoSize  = true,
                Location  = new Point(TextX, SubtitleY),
                BackColor = Color.Transparent
            };
            panel.Controls.Add(lblSub);
        }

        panel.Paint += (_, e) =>
        {
            using var pen = new Pen(AppBorders.AccentLine, AccentLineH);
            e.Graphics.DrawLine(pen, 0, panel.Height - AccentLineH, panel.Width, panel.Height - AccentLineH);
        };

        target.Controls.Add(panel);
        panel.BringToFront();

        return panel;
    }

    /// <summary>
    /// Resizes <paramref name="target"/> to a percentage of the primary screen's working area
    /// and centres it on screen.
    /// </summary>
    public static void ConfigureSize(Form target, double widthPct, double heightPct)
    {
        var screen  = Screen.PrimaryScreen!.WorkingArea;
        target.Width    = (int)(screen.Width  * widthPct);
        target.Height   = (int)(screen.Height * heightPct);
        target.Location = new Point(
            screen.X + (screen.Width  - target.Width)  / 2,
            screen.Y + (screen.Height - target.Height) / 2);
    }
}
