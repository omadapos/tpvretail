using OmadaPOS.Impresora;
using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Services;

namespace OmadaPOS.Views
{
    public partial class frmCierreDiario : Form
    {
        private IOrderService orderService;
        private IBranchService branchService;

        public frmCierreDiario()
        {
            InitializeComponent();

            orderService  = Program.GetService<IOrderService>();
            branchService = Program.GetService<IBranchService>();

            this.Load += frmCierreDiario_Load;
        }

        private void frmCierreDiario_Load(object sender, EventArgs e)
        {
            AplicarEstiloVisual();
        }

        private void AplicarEstiloVisual()
        {
            ConfigurarTamano(anchoPorc: 0.38, altoPorc: 0.55);

            this.BackColor = AppColors.BackgroundPrimary;

            // ── Header contextual — Rojo (acción destructiva / fin de jornada) ──
            AgregarHeader();

            // ── Botón "Close Day" — Danger (confirmar cierre) ──
            ElegantButtonStyles.Style(buttonClose, AppColors.Danger, AppColors.TextWhite, fontSize: 22f);
            buttonClose.Text = "⏻  CLOSE DAY";

            // ── Estilo del DateTimePicker ──
            dateTimePickerFecha.Font      = new Font("Segoe UI", 16F, FontStyle.Regular);
            dateTimePickerFecha.CalendarForeColor = AppColors.TextPrimary;
            dateTimePickerFecha.BackColor = AppColors.BackgroundSecondary;
        }

        private void AgregarHeader()
        {
            var panel = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 70,
                BackColor = AppColors.Danger,
            };

            var lblIcono = new Label
            {
                Text      = "⏻",
                Font      = new Font("Segoe UI", 22F, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 255, 255, 255),
                AutoSize  = true,
                Location  = new Point(16, 14),
                BackColor = Color.Transparent
            };

            var lblTitulo = new Label
            {
                Text      = "Close Day",
                Font      = new Font("Montserrat", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize  = true,
                Location  = new Point(58, 10),
                BackColor = Color.Transparent
            };

            var lblSub = new Label
            {
                Text      = "Select date and confirm daily closing",
                Font      = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = Color.FromArgb(200, 255, 255, 255),
                AutoSize  = true,
                Location  = new Point(59, 38),
                BackColor = Color.Transparent
            };

            panel.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(80, 0, 0, 0), 2);
                e.Graphics.DrawLine(pen, 0, panel.Height - 2, panel.Width, panel.Height - 2);
            };

            panel.Controls.Add(lblIcono);
            panel.Controls.Add(lblTitulo);
            panel.Controls.Add(lblSub);
            this.Controls.Add(panel);
            panel.BringToFront();
        }

        private void ConfigurarTamano(double anchoPorc, double altoPorc)
        {
            var screen    = Screen.PrimaryScreen!.WorkingArea;
            this.Width    = (int)(screen.Width  * anchoPorc);
            this.Height   = (int)(screen.Height * altoPorc);
            this.Location = new Point(
                screen.X + (screen.Width  - this.Width)  / 2,
                screen.Y + (screen.Height - this.Height) / 2);
        }

        private async void buttonClose_Click(object sender, EventArgs e)
        {
            var date  = dateTimePickerFecha.Value;
            var sDate = date.ToString("yyyyMMdd");

            var cierre = await orderService.CierreDiario(sDate, SessionManager.UserName);
            if (cierre != null)
            {
                var branchInfo = await branchService.LoadBranch(SessionManager.BranchId ?? 0);
                if (branchInfo != null)
                {
                    var ticket = new TicketCierre(cierre, branchInfo.Address, branchInfo.Name);
                    ticket.print();
                    this.Close();
                }
            }
        }
    }
}
