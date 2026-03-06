using OmadaPOS.Estilos;
using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Services;
using OmadaPOS.Libreria.Utils;

namespace OmadaPOS.Views
{
    public partial class frmSetting : Form
    {
        private IAdminSettingService adminSettingService;

        public frmSetting()
        {
            InitializeComponent();
            ElegantButtonStyles.Style(buttonSave, ElegantButtonStyles.Buttonok, fontSize: 18f);
            ElegantButtonStyles.Style(buttonCancel, ElegantButtonStyles.ButtonCacnel, fontSize: 18f);
            adminSettingService = Program.GetService<IAdminSettingService>();
        }

        private async void buttonSave_Click(object sender, EventArgs e)
        {
            await adminSettingService.UpdateSetting(new AdminSetting() {
                WindowsId = labelWindowsId.Text,
                IP = textBoxIP.Text,
                Port = int.TryParse(textBoxPort.Text, out int port) ? port : 0,
                Terminal = textBoxTerminal.Text,
                PrinterName = textBoxPrinterName.Text
            });



            this.Close();
        }

        private void frmSetting_Load(object sender, EventArgs e)
        {

            Cursor = Cursors.WaitCursor;

            LoadSettings();

            Cursor = Cursors.Arrow;
        }

        private async void LoadSettings()
        {
            try
            {
                string? windowsId = WindowsIdProvider.GetMachineGuid();
                if (string.IsNullOrEmpty(windowsId))
                    return;

                var setting = await adminSettingService.LoadSettingById(windowsId);

                if (setting != null)
                {
                    labelWindowsId.Text = setting.WindowsId ?? string.Empty;
                    textBoxIP.Text = setting.IP ?? string.Empty;
                    textBoxPort.Text = setting.Port?.ToString() ?? string.Empty;
                    textBoxTerminal.Text = setting.Terminal ?? string.Empty;
                    textBoxPrinterName.Text = setting.PrinterName ?? string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la configuración: " + ex.Message);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {

            this.Close();
        }
    }
}
