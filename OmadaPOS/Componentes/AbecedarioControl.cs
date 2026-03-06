using System.Drawing.Drawing2D;

namespace OmadaPOS.Componentes
{
    public partial class AbecedarioControl : UserControl
    {
        public delegate void LetraClickedEventHandler(object sender, string letra);
        public event LetraClickedEventHandler? LetraClicked;

        private Button[] allButtons;

        public AbecedarioControl()
        {
            InitializeComponent();
            InicializarBotones();
            ApplyStyles();
        }

        private void InicializarBotones()
        {
            allButtons = new Button[]
            {
                buttonA, buttonB, buttonC, buttonD, buttonE, buttonF,
                buttonG, buttonH, buttonI, buttonJ, buttonK, buttonL,
                buttonM, buttonN, buttonO, buttonP, buttonQ, buttonR,
                buttonS, buttonT, buttonU, buttonV, buttonW, buttonX,
                buttonY, buttonZ
            };
        }

        private void ApplyStyles()
        {
            foreach (var btn in allButtons)
            {
                ModernAlphabetButtonStyle.Apply(btn);
                btn.Tag = btn.Text; // Asegura que la letra esté en Tag
                btn.Click += buttonKey_Click;
            }
        }

        private void buttonKey_Click(object sender, EventArgs e)
        {
            if (sender is Button cb && cb.Tag is string letra)
            {
                LetraClicked?.Invoke(this, letra);
            }
        }
    }
}
