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
        // Usa un estilo táctil profesional definido en tu clase ElegantButtonStyles
        Button[] botones = {
            button0, button1, button2, button3, button4,
            button5, button6, button7, button8, button9,
            buttonC, buttonLeft
        };

        foreach (var btn in botones)
        {
            ElegantButtonStyles.Style(btn);
            btn.Font = new Font("Roboto Mono," +
                "", 18, FontStyle.Bold); // Tamaño táctil
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;
        }

        labelDisplay.Font = new Font("Roboto Mono,", 20, FontStyle.Bold);
        labelDisplay.ForeColor = Color.Black;
        labelDisplay.BackColor = Color.White;
        labelDisplay.TextAlign = ContentAlignment.MiddleRight;
    }

    private void btnNumber_Click(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.Tag != null)
        {
            string btnTag = btn.Tag.ToString();

            switch (btnTag)
            {
                case "0":
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                    labelDisplay.Text += btnTag;
                    break;

                case "C":
                    labelDisplay.Text = "";
                    break;

                case "<-":
                    if (!string.IsNullOrEmpty(labelDisplay.Text))
                    {
                        labelDisplay.Text = labelDisplay.Text.Substring(0, labelDisplay.Text.Length - 1);
                    }
                    break;
            }
        }
    }

    public string Getdata()
    {
        return labelDisplay.Text;
    }

    public void Setdata(string value)
    {
        labelDisplay.Text = value;
    }
}
