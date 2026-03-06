using OmadaPOS.Impresora;
using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Services;

namespace OmadaPOS.Views
{
    public partial class frmPopupCashPayment: Form
    {
        private readonly IOrderService orderService;
        private readonly IBranchService branchService;

        private readonly int orderId;
        private readonly int consecutivo;
        private readonly decimal devuelta;

        public frmPopupCashPayment(int orderId, int consecutivo, decimal devuelta)
        {
            this.orderId = orderId;
            this.consecutivo = consecutivo;
            this.devuelta = devuelta;

            InitializeComponent();
            ElegantButtonStyles.Style(buttonClose, Color.FromArgb(156, 163, 175), Color.White);     // #22C55E
            ElegantButtonStyles.Style(buttonPrint, Color.FromArgb(34, 197, 94), Color.White); // #EF4444

            // Usar inyección de dependencia si es posible o obtener el servicio de manera centralizada
            orderService = Program.GetService<IOrderService>();
            branchService = Program.GetService<IBranchService>();

            UpdateUI();

            // Configuración de los eventos de los botones
            // Asegúrate de que los botones están correctamente referenciados antes de agregar eventos
            // SetupEventHandlers();
        }

        private void UpdateUI()
        {
            labelDevuelta.Text = devuelta.ToString("N2");
            labelInvoice.Text = $"Invoice # {consecutivo}";
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void buttonPrint_Click(object sender, EventArgs e)
        {
            var order = await orderService.GetOrderById(orderId);

            var orderDetails = await orderService.GetOrderDetailsByOrderId(orderId);

            // Asegurar que ambos, order y orderDetails, no son null antes de proceder
            if (order == null || orderDetails == null)
            {
                MessageBox.Show("No se pudo obtener la información del pedido.");
                return;
            }

            var imagePrint = Properties.Resources.print_resize;

            var branchInfo = await branchService.LoadBranch(SessionManager.BranchId ?? 0);

            if (branchInfo != null)
            {
                var ticket = new Ticket(orderId, order.Consecutivo, order, orderDetails, SessionManager.Name,
                    null, branchInfo.Address, branchInfo.FooterMsg);

                ticket.Print();

                this.Close();
            }
        }

    }
}
