namespace OmadaPOS.Componentes
{
    public partial class AbecedarioControl : UserControl
    {
        public delegate void LetraClickedEventHandler(object sender, string letra);
        public event LetraClickedEventHandler? LetraClicked;

        private Button[] allButtons = Array.Empty<Button>();
        private Button?  buttonAll;
        private Button?  buttonActive; // último botón presionado (resaltado)

        public AbecedarioControl()
        {
            InitializeComponent();
            InicializarBotones();
            AplicarEstilo();
            AgregarBotonAll();
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

        private void AplicarEstilo()
        {
            // Contenedor — navy oscuro como barra de búsqueda
            tableLayoutPanelButton.BackColor       = AppColors.NavyDark;
            tableLayoutPanelButton.Padding         = new Padding(4, 3, 4, 3);
            tableLayoutPanelButton.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
            this.BackColor = AppColors.NavyDark;

            foreach (var btn in allButtons)
            {
                // ❌ NO re-suscribimos btn.Click aquí.
                // El Designer ya registra buttonKey_Click en cada botón.
                // Hacerlo de nuevo causaría que cada click dispare el evento DOS veces.
                ModernAlphabetButtonStyle.Apply(btn);
                btn.Margin = new Padding(2, 1, 2, 1);
            }
        }

        // ── Botón "ALL" — limpiar filtro de letra ────────────────────────
        private void AgregarBotonAll()
        {
            // Añadir columna 14 al TableLayoutPanel existente
            tableLayoutPanelButton.ColumnCount = 14;
            tableLayoutPanelButton.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 7.69F));

            buttonAll = new Button
            {
                Text                  = "✕\nALL",
                Tag                   = "",
                Dock                  = DockStyle.Fill,
                Cursor                = Cursors.Hand,
                FlatStyle             = FlatStyle.Flat,
                BackColor             = AppColors.Danger,
                ForeColor             = AppColors.TextWhite,
                Font                  = new Font("Segoe UI", 9F, FontStyle.Bold),
                Margin                = new Padding(2, 1, 2, 1),
                UseVisualStyleBackColor = false,
            };
            buttonAll.FlatAppearance.BorderSize         = 0;
            buttonAll.FlatAppearance.MouseOverBackColor = Color.FromArgb(170, 40, 40);
            buttonAll.FlatAppearance.MouseDownBackColor = Color.FromArgb(140, 30, 30);

            // Ocupa las 2 filas del TableLayoutPanel
            tableLayoutPanelButton.Controls.Add(buttonAll, 13, 0);
            tableLayoutPanelButton.SetRowSpan(buttonAll, 2);

            buttonAll.Click += (s, e) =>
            {
                // Quitar resaltado del botón activo anterior
                ResetearBotonActivo();
                LetraClicked?.Invoke(this, "");
            };
        }

        // ── Resaltado del botón activo ────────────────────────────────────
        private void ResetearBotonActivo()
        {
            if (buttonActive != null)
            {
                buttonActive.BackColor = AppColors.NavyBase;
                buttonActive.ForeColor = AppColors.TextWhite;
                buttonActive = null;
            }
        }

        private void MarcarBotonActivo(Button btn)
        {
            ResetearBotonActivo();
            btn.BackColor = AppColors.AccentGreenDark;
            btn.ForeColor = AppColors.TextWhite;
            buttonActive  = btn;
        }

        // ── Handler de click — registrado desde el Designer ───────────────
        private void buttonKey_Click(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is string letra)
            {
                MarcarBotonActivo(btn);
                LetraClicked?.Invoke(this, letra);
            }
        }
    }
}
