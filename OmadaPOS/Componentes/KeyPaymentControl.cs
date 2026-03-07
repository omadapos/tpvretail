

namespace OmadaPOS.Componentes;

public partial class KeyPaymentControl : UserControl
{
    public delegate void KeyPaymentClickedEventHandler(object sender, string tag);

    public event KeyPaymentClickedEventHandler? KeyPaymentClicked;

    public KeyPaymentControl()
    {
        InitializeComponent();

        ApplyStyles();
    }

    private void buttonKeyScan_Click(object sender, EventArgs e)
    {

        Button btn = (Button)sender;
        string btnTag = btn.Tag.ToString();

        KeyPaymentClicked?.Invoke(this, btnTag);
    }

    private void ApplyStyles()
    {
        // Fondo claro — coincide con la columna de pagos
        var payBg = Color.FromArgb(248, 249, 252);
        this.BackColor                  = payBg;
        tableLayoutPanelMoney.BackColor = payBg;
        tableLayoutPanelMoney.Padding   = new Padding(4);

        // ── Dígitos — Navy (teclado numérico) ────────────────────────────
        var digits = new[] { button1, button2, button3, button4,
                              button5, button6, button7, button8,
                              button9, button0, button00 };
        foreach (var btn in digits)
            ElegantButtonStyles.Style(btn, ElegantButtonStyles.Keypad, fontSize: 30f);

        // ── Billetes — Verde (dinero en efectivo), columna derecha ───────
        ElegantButtonStyles.Style(button10,  ElegantButtonStyles.CashGreen, fontSize: 26f);
        ElegantButtonStyles.Style(button20,  ElegantButtonStyles.CashGreen, fontSize: 26f);
        ElegantButtonStyles.Style(button50,  ElegantButtonStyles.CashGreen, fontSize: 26f);
        ElegantButtonStyles.Style(button100, ElegantButtonStyles.CashGreen, fontSize: 26f);

        // ── Clear — Rojo (acción destructiva: borrar monto ingresado) ─────
        ElegantButtonStyles.Style(buttonClear, ElegantButtonStyles.AlertRed, fontSize: 26f);
        buttonClear.Text = "⌫  C";
    }


}
