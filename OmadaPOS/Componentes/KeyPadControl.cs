using System.Drawing.Drawing2D;

namespace OmadaPOS.Componentes;

public partial class KeyPadControl : UserControl
{
    public KeyPadControl()
    {
        InitializeComponent();
        AplicarEstilosBotones();
    }

    private void KeyPadControl_Load(object sender, EventArgs e)
    {
        labelDisplay.Text = "";
    }

    private void AplicarEstilosBotones()
    {
        // ── Fondo del control ─────────────────────────────────────
        this.BackColor              = AppColors.BackgroundPrimary;
        tableLayoutPanelMain.BackColor  = AppColors.BackgroundPrimary;
        tableLayoutPanelButton.BackColor = AppColors.BackgroundPrimary;

        // ── Botones numéricos (30% Navy) ──────────────────────────
        Button[] numeros = { button0, button1, button2, button3,
                              button4, button5, button6, button7, button8, button9 };

        foreach (var btn in numeros)
            ElegantButtonStyles.Style(btn, AppColors.NavyBase, AppColors.TextWhite, radius: 10, fontSize: 28f);

        // ── Backspace — Ámbar (advertencia) ──────────────────────
        ElegantButtonStyles.Style(buttonLeft, AppColors.Warning, AppColors.TextWhite, radius: 10, fontSize: 22f);
        buttonLeft.Text = "⌫";

        // ── Clear — Rojo (acción destructiva) ────────────────────
        ElegantButtonStyles.Style(buttonC, AppColors.Danger, AppColors.TextWhite, radius: 10, fontSize: 22f);
        buttonC.Text = "C";

        // ── Display de entrada ────────────────────────────────────
        EstilizarDisplay();
    }

    private void EstilizarDisplay()
    {
        labelDisplay.BackColor  = AppColors.NavyDark;
        labelDisplay.ForeColor  = AppColors.AccentGreen;
        labelDisplay.Font       = new Font("Consolas", 30F, FontStyle.Bold);
        labelDisplay.BorderStyle = BorderStyle.None;
        labelDisplay.TextAlign  = ContentAlignment.MiddleRight;
        labelDisplay.Padding    = new Padding(0, 0, 12, 0);

        // Línea verde inferior como cursor visual
        labelDisplay.Paint += (s, e) =>
        {
            using var pen = new Pen(AppColors.AccentGreen, 3);
            e.Graphics.DrawLine(pen, 8, labelDisplay.Height - 4,
                                     labelDisplay.Width - 8, labelDisplay.Height - 4);
        };
    }

    // ─────────────────────────────────────────────────────────────
    //  EVENTOS — lógica sin cambios
    // ─────────────────────────────────────────────────────────────
    private void btnNumber_Click(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.Tag != null)
        {
            switch (btn.Tag.ToString())
            {
                case "0": case "1": case "2": case "3": case "4":
                case "5": case "6": case "7": case "8": case "9":
                    labelDisplay.Text += btn.Tag;
                    break;

                case "C":
                    labelDisplay.Text = "";
                    break;

                case "<-":
                    if (!string.IsNullOrEmpty(labelDisplay.Text))
                        labelDisplay.Text = labelDisplay.Text[..^1];
                    break;
            }
        }
    }

    public string Getdata()  => labelDisplay.Text;
    public void   Setdata(string value) => labelDisplay.Text = value;
}
