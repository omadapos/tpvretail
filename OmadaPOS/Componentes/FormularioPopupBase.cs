using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace OmadaPOS.Componentes
{
    /// <summary>
    /// Base para todos los formularios popup del sistema.
    /// Aplica el PremiumMarket Theme (60-30-10).
    /// </summary>
    public partial class FormularioPopupBase : Form
    {
        private Label lblTitulo;
        private Button btnCerrar;

        public FormularioPopupBase()
        {
            InitializeComponent();
            AplicarTheme();
        }

        /// <summary>
        /// Título que se muestra en el header del popup.
        /// Las subclases pueden asignarlo en su constructor o Load.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string TituloPopup
        {
            get => lblTitulo?.Text ?? string.Empty;
            set { if (lblTitulo != null) lblTitulo.Text = value; }
        }

        private void AplicarTheme()
        {
            // ── Header (30% Navy) ──────────────────────────
            panelHeader.BackColor = AppColors.NavyDark;
            panelHeader.Height = 54;

            lblTitulo = new Label
            {
                Text = this.Text,
                ForeColor = AppColors.TextWhite,
                Font = new Font("Montserrat", 13F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(18, 14),
                BackColor = Color.Transparent
            };

            btnCerrar = new Button
            {
                Text = "✕",
                ForeColor = AppColors.TextWhite,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                BackColor = AppColors.Danger,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(54, 54),
                Dock = DockStyle.Right,
                Cursor = Cursors.Hand,
                TabStop = false
            };
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.FlatAppearance.MouseOverBackColor = ElegantButtonStyles.Darken(AppColors.Danger, 0.15f);
            btnCerrar.Click += (s, e) => this.Close();

            panelHeader.Controls.Add(lblTitulo);
            panelHeader.Controls.Add(btnCerrar);

            // ── Cuerpo (60% Background) ────────────────────
            this.BackColor = AppColors.BackgroundPrimary;

            // ── Borde inferior del header ──────────────────
            panelHeader.Paint += (s, e) =>
            {
                using var pen = new Pen(AppColors.AccentGreen, 3);
                e.Graphics.DrawLine(pen, 0, panelHeader.Height - 3,
                                         panelHeader.Width, panelHeader.Height - 3);
            };
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            if (lblTitulo != null) lblTitulo.Text = this.Text;
        }
    }
}
