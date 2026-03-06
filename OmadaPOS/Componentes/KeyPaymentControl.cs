

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
        ElegantButtonStyles.Style(button1, ElegantButtonStyles.Keypad);
        ElegantButtonStyles.Style(button2, ElegantButtonStyles.Keypad);
        ElegantButtonStyles.Style(button3, ElegantButtonStyles.Keypad);
        ElegantButtonStyles.Style(button4, ElegantButtonStyles.Keypad);
        ElegantButtonStyles.Style(button5, ElegantButtonStyles.Keypad);
        ElegantButtonStyles.Style(button6, ElegantButtonStyles.Keypad);
        ElegantButtonStyles.Style(button7, ElegantButtonStyles.Keypad);
        ElegantButtonStyles.Style(button8, ElegantButtonStyles.Keypad);
        ElegantButtonStyles.Style(button9, ElegantButtonStyles.Keypad);
        ElegantButtonStyles.Style(button10, ElegantButtonStyles.CashGreen);
        ElegantButtonStyles.Style(button20, ElegantButtonStyles.CashGreen);
        ElegantButtonStyles.Style(button50, ElegantButtonStyles.CashGreen);
        ElegantButtonStyles.Style(button100, ElegantButtonStyles.CashGreen);
        ElegantButtonStyles.Style(buttonClear, ElegantButtonStyles.Keypad);
        ElegantButtonStyles.Style(button0, ElegantButtonStyles.Keypad);
        ElegantButtonStyles.Style(button00, ElegantButtonStyles.Keypad);
    }


}
