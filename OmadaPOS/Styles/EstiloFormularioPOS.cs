using System;
using System.Drawing;
using System.Windows.Forms;

namespace OmadaPOS.Estilos
{
    public class EstiloFormularioPOS : Form
    {
        private System.Windows.Forms.Timer fadeOutTimer;


        public EstiloFormularioPOS()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.BackColor = AppColors.BackgroundPrimary;
            this.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            this.KeyPreview = true;
            this.Opacity = 1.0;

            ConfigurarTamañoYPosicion();
            EstablecerCabecera();
        }

        private void ConfigurarTamañoYPosicion()
        {
            var pantalla = Screen.PrimaryScreen.WorkingArea;

            this.Width = (int)(pantalla.Width * 0.7);
            this.Height = (int)(pantalla.Height * 0.7);

            this.Location = new Point(
                (pantalla.Width - this.Width) / 2,
                (pantalla.Height - this.Height) / 2
            );
        }

        private void EstablecerCabecera()
        {
            var barra = new Panel
            {
                Height = 54,
                Dock = DockStyle.Top,
                BackColor = AppColors.NavyDark
            };

            var lblTitulo = new Label
            {
                Text = "OmadaPOS",
                ForeColor = AppColors.TextWhite,
                Font = new Font("Montserrat", 13F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(18, 14),
                BackColor = Color.Transparent
            };

            var btnCerrar = new Button
            {
                Text = "✕",
                ForeColor = AppColors.TextWhite,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                BackColor = AppColors.Danger,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(54, 54),
                Dock = DockStyle.Right,
                Cursor = Cursors.Hand
            };
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.FlatAppearance.MouseOverBackColor = ElegantButtonStyles.Darken(AppColors.Danger, 0.15f);
            btnCerrar.Click += (s, e) => IniciarFadeOut();

            barra.Controls.Add(lblTitulo);
            barra.Controls.Add(btnCerrar);

            barra.Paint += (s, e) =>
            {
                using var pen = new Pen(AppColors.AccentGreen, 3);
                e.Graphics.DrawLine(pen, 0, barra.Height - 3, barra.Width, barra.Height - 3);
            };

            this.Controls.Add(barra);
        }

        private void IniciarFadeOut()
        {
            fadeOutTimer = new System.Windows.Forms.Timer();

            fadeOutTimer.Interval = 25; // velocidad del fade
            fadeOutTimer.Tick += (s, e) =>
            {
                if (this.Opacity > 0)
                {
                    this.Opacity -= 0.05;
                }
                else
                {
                    fadeOutTimer.Stop();
                    this.Close();
                }
            };
            fadeOutTimer.Start();
        }
    }
}
