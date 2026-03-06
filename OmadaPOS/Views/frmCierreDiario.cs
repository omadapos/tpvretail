using OmadaPOS.Impresora;
using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Services;

namespace OmadaPOS.Views
{
    public partial class frmCierreDiario: Form
    {
        private IOrderService orderService;
        private IBranchService branchService;

        public frmCierreDiario()
        {
            InitializeComponent();
            //EstilosPOS.AplicarEstilosFormulario(this, true);
            orderService = Program.GetService<IOrderService>();
            branchService = Program.GetService<IBranchService>();
        }

        private async void buttonClose_Click(object sender, EventArgs e)
        {
            var date = dateTimePickerFecha.Value;
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
