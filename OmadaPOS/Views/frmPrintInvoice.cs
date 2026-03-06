using OmadaPOS.Impresora;
using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Services;
using System.Numerics;

namespace OmadaPOS.Views
{
    public partial class frmPrintInvoice : Form
    {
        private IOrderService orderService;
        private IBranchService branchService;

        public frmPrintInvoice()
        {
            InitializeComponent();
            ElegantButtonStyles.Style(buttonSearch, ElegantButtonStyles.MoneyButtonGreen, fontSize: 18f);
            orderService = Program.GetService<IOrderService>();
            branchService = Program.GetService<IBranchService>();
        }

        private async void frmPrintInvoice_Load(object sender, EventArgs e)
        {
            InitListViewInvoices();
            InitListViewIProducts();

            var list = await orderService.GetOrderTop();

            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    var lvi = new ListViewItem(item.Id.ToString());
                    lvi.SubItems.Add(item.Consecutivo.ToString());
                    lvi.SubItems.Add(item.Order_Amount.ToString("N2"));
                    lvi.SubItems.Add(item.Created_At.ToString("yyyy-MM-dd HH:mm"));
                    lvi.SubItems.Add(item.Devuelta.ToString("N2"));
                    lvi.SubItems.Add("Print");
                    lvi.Tag = item.Id; // OrderId
                    listViewInvoices.Items.Add(lvi);
                }
            }
        }

        private void InitListViewInvoices()
        {
            // Limpia columnas y items existentes
            listViewInvoices.Clear();
            listViewInvoices.Items.Clear();

            // Configura columnas
            listViewInvoices.View = View.Details;
            listViewInvoices.FullRowSelect = true;
            listViewInvoices.GridLines = true;
            listViewInvoices.MultiSelect = false;

            listViewInvoices.Columns.Add("Id", 100, HorizontalAlignment.Center);
            listViewInvoices.Columns.Add("Consecutive", 100, HorizontalAlignment.Center);
            listViewInvoices.Columns.Add("Order Amount", 150, HorizontalAlignment.Center);
            listViewInvoices.Columns.Add("Created At", 200, HorizontalAlignment.Center);
            listViewInvoices.Columns.Add("Change", 120, HorizontalAlignment.Center);
            listViewInvoices.Columns.Add("", 100, HorizontalAlignment.Center); // Columna Print

            // Estilo visual
            listViewInvoices.Font = new Font("Montserrat", 14F, FontStyle.Regular);
            listViewInvoices.Dock = DockStyle.Fill;
            listViewInvoices.BackColor = Color.White;

            // Ajusta el ancho de las columnas al tamaño del ListView
            listViewInvoices.Resize += (s, ev) =>
            {
                int totalWidth = listViewInvoices.ClientSize.Width;
                int columnCount = listViewInvoices.Columns.Count;
                if (columnCount == 0) return;

                int columnWidth = totalWidth / columnCount;
                int lastColumnWidth = totalWidth - (columnWidth * (columnCount - 1));

                for (int i = 0; i < columnCount; i++)
                {
                    listViewInvoices.Columns[i].Width = (i == columnCount - 1) ? lastColumnWidth : columnWidth;
                }
            };
            listViewInvoices.MouseClick += listViewInvoices_MouseClick;
        }

        private void InitListViewIProducts()
        {
            // Limpia columnas y items existentes
            listViewProducts.Clear();
            listViewProducts.Items.Clear();

            // Configura columnas
            listViewProducts.View = View.Details;
            listViewProducts.FullRowSelect = true;
            listViewProducts.GridLines = true;
            listViewProducts.MultiSelect = false;

            listViewProducts.Columns.Add("Id", 100, HorizontalAlignment.Center);
            listViewProducts.Columns.Add("Name", 150, HorizontalAlignment.Center);
            listViewProducts.Columns.Add("Price", 200, HorizontalAlignment.Center);

            // Estilo visual
            listViewProducts.Font = new Font("Montserrat", 14F, FontStyle.Regular);
            listViewProducts.Dock = DockStyle.Fill;
            listViewProducts.BackColor = Color.White;

            // Ajusta el ancho de las columnas al tamaño del ListView
            listViewProducts.Resize += (s, ev) =>
            {
                int totalWidth = listViewProducts.ClientSize.Width;
                int columnCount = listViewProducts.Columns.Count;
                if (columnCount == 0) return;

                int columnWidth = totalWidth / columnCount;
                int lastColumnWidth = totalWidth - (columnWidth * (columnCount - 1));

                for (int i = 0; i < columnCount; i++)
                {
                    listViewProducts.Columns[i].Width = (i == columnCount - 1) ? lastColumnWidth : columnWidth;
                }
            };
        }

        private void listViewInvoices_MouseClick(object sender, MouseEventArgs e)
        {
            var info = listViewInvoices.HitTest(e.Location);
            if (info.Item != null && info.SubItem != null)
            {
                int printColumnIndex = 4; // Índice de la columna "Print"
                if (info.Item.SubItems.IndexOf(info.SubItem) == printColumnIndex)
                {
                    int orderId = int.Parse(info.Item.Text);
                    PrintInvoice(orderId);
                }
            }
        }

        private async void PrintInvoice(int orderId)
        {
            var order = await orderService.GetOrderById(orderId);
            if (order == null)
            {
                MessageBox.Show("Order no exists");
                return;
            }

            var branchInfo = await branchService.LoadBranch(SessionManager.BranchId ?? 0);

            if (branchInfo != null)
            {
                var orderDetails = await orderService.GetOrderDetailsByOrderId(orderId);

                var ticket = new Ticket(orderId, order.Consecutivo, order, orderDetails, SessionManager.Name,
                    null, branchInfo.Address, branchInfo.Name);

                ticket.Print();
            }
        }

        private async void buttonSearch_Click(object sender, EventArgs e)
        {
            listViewInvoices.Items.Clear();

            string fecha1 = dateTimePicker1.Value.ToString("yyyyMMdd");
            string fecha2 = dateTimePicker2.Value.ToString("yyyyMMdd");

            Cursor = Cursors.WaitCursor;
            var list = await orderService.GetOrderTop(fecha1, fecha2);
            Cursor = Cursors.Default;

            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    var lvi = new ListViewItem(item.Id.ToString());
                    lvi.SubItems.Add(item.Consecutivo.ToString());
                    lvi.SubItems.Add(item.Order_Amount.ToString("N2"));
                    lvi.SubItems.Add(item.Created_At.ToString("yyyy-MM-dd HH:mm"));
                    lvi.SubItems.Add(item.Devuelta.ToString("N2"));
                    lvi.SubItems.Add("Print");
                    lvi.Tag = item.Id; // OrderId
                    listViewInvoices.Items.Add(lvi);
                }
            }
        }

        private async void listViewInvoices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewInvoices.SelectedItems.Count > 0)
            {
                var selectedItem = listViewInvoices.SelectedItems[0];
                int orderId = int.Parse(selectedItem.Tag.ToString());

                listViewProducts.Items.Clear();

                Cursor = Cursors.WaitCursor;
                var list = await orderService.GetOrderDetailsByOrderId(orderId);
                Cursor = Cursors.Default;

                if (list != null && list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        var lvi = new ListViewItem(item.Product_Id.ToString());
                        lvi.SubItems.Add(item.Product_Name.ToString());
                        lvi.SubItems.Add(item.Price.ToString("N2"));
                        listViewProducts.Items.Add(lvi);
                    }
                }
            }
        }
    }
}
