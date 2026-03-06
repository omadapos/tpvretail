namespace OmadaPOS.Componentes;

public partial class KeyPadMoneyControl : UserControl
{
    int inputValue = 0;

    public KeyPadMoneyControl()
    {
        InitializeComponent();

        ApplyStyles();
    }

    public decimal GetValue()
    {
        return (decimal)(inputValue / 100.0);
    }

    public void SetValue(int value)
    {
        inputValue = value;

        displayTotales();
    }

    public void ClearValue()
    {
        inputValue = 0;

        displayTotales();
    }

    private void buttonKey_Click(object sender, EventArgs e)
    {
        Button btn = (Button)sender;
        string btnTag = btn.Tag.ToString();

        switch (btnTag)
        {
            case "0":
            case "00":
            case "1":
            case "2":
            case "3":
            case "4":
            case "5":
            case "6":
            case "7":
            case "8":
            case "9":
                try
                {
                    string numTemp = inputValue.ToString();
                    numTemp += btnTag;
                    int valNum = int.Parse(numTemp);
                    inputValue = valNum;
                }
                catch { }

                displayTotales();
                break;


            case "1000":
            case "2000":
            case "5000":
            case "10000":
                try
                {
                    int valNum = int.Parse(btnTag);
                    inputValue = valNum;
                }
                catch { }

                displayTotales();
                break;

            default: // C
                inputValue = 0;
                displayTotales();
                break;

        }
    }

    private void displayTotales()
    {
        labelInput.Text = "$" + (inputValue / 100.0).ToString("n2");
    }

    private void KeyPadMoneyControl_Load(object sender, EventArgs e)
    {
        displayTotales();
    }

    private void ApplyStyles()
    {
        ElegantButtonStyles.Style(button1, fontSize: 40f);
        ElegantButtonStyles.Style(button2, fontSize: 40f);
        ElegantButtonStyles.Style(button3, fontSize: 40f);
        ElegantButtonStyles.Style(button4, fontSize: 40f);
        ElegantButtonStyles.Style(button5, fontSize: 40f);
        ElegantButtonStyles.Style(button6, fontSize: 40f);
        ElegantButtonStyles.Style(button7, fontSize: 40f);
        ElegantButtonStyles.Style(button8, fontSize: 40f);
        ElegantButtonStyles.Style(button9, fontSize: 40f);
        ElegantButtonStyles.Style(button0, fontSize: 40f);
        ElegantButtonStyles.Style(button00, fontSize: 40f);
        ElegantButtonStyles.Style(buttonC, fontSize: 40f);
     //   ElegantButtonStyles.Style(button10, ElegantButtonStyles.Keypad);
     //   ElegantButtonStyles.Style(button20, ElegantButtonStyles.Keypad);
    //    ElegantButtonStyles.Style(button50, ElegantButtonStyles.Keypad);
    //    ElegantButtonStyles.Style(button100, ElegantButtonStyles.Keypad);
    }

}