using Microsoft.Extensions.DependencyInjection;
using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Services;
using OmadaPOS.Libreria.Utils;

namespace OmadaPOS.Views;

public partial class frmSignIn
{
    private readonly IUserService userService;
    private bool isPhoneFocus = true;

    public frmSignIn()
    {
        InitializeComponent();
        var marcaAgua = new Controles.WatermarkOmadaPOS();
        this.Controls.Add(marcaAgua);
        marcaAgua.SendToBack(); // Colócalo detrás de todo


        ElegantButtonStyles.Style(buttonLogin, fontSize: 40);
        ElegantButtonStyles.Style(buttonClear, fontSize: 40f);
        ElegantButtonStyles.Style(button0, fontSize: 40f);
        ElegantButtonStyles.Style(button9, fontSize: 40f);
        ElegantButtonStyles.Style(button8, fontSize: 40f);
        ElegantButtonStyles.Style(button7, fontSize: 40f);
        ElegantButtonStyles.Style(button6, fontSize: 40f);
        ElegantButtonStyles.Style(button5, fontSize: 40f);
        ElegantButtonStyles.Style(button4, fontSize: 40f);
        ElegantButtonStyles.Style(button3, fontSize: 40f);
        ElegantButtonStyles.Style(button2, fontSize: 40f);
        ElegantButtonStyles.Style(button1, fontSize: 40f);
        userService = Program.GetService<IUserService>();
    }

    private void frmSignIn_Load(object sender, EventArgs e)
    {
        // Set the size and position of the form
        this.Size = new Size(800, 600); // Set the desired size
        this.StartPosition = FormStartPosition.CenterScreen; // Center the form on the screen

        // Ensure the textbox is always focused
        textBoxPhone.GotFocus += (s, ev) => isPhoneFocus = true;
        textBoxPhone.Leave += (s, ev) => textBoxPhone.Focus();

        string? windowsId = WindowsIdProvider.GetMachineGuid();
        if (!string.IsNullOrEmpty(windowsId))
        {
            labelId.Text = windowsId;
        }
    }

    private void buttonKey_Click(object sender, EventArgs e)
    {
        if (sender is Button btn && int.TryParse(btn.Tag.ToString(), out int _))
        {
            if (isPhoneFocus)
            {
                textBoxPhone.Text += btn.Tag;
            }
        }
    }

    private async void buttonLogin_Click(object sender, EventArgs e)
    {
        string username = textBoxPhone.Text;
        string password = "12345678";

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            MessageBox.Show("Please enter values in all fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        try
        {
            Cursor = Cursors.WaitCursor;

            var loginRequest = new LoginRequest
            {
                Email = username,
                Password = password,
                WindowsId = labelId.Text,
            };

            LoginResponse response = await userService.Login(loginRequest);

            if (!string.IsNullOrEmpty(response.Token))
            {
                SessionManager.Token = response.Token;
                SessionManager.BranchId = response.BranchId;
                SessionManager.UserName = username;
                SessionManager.Name = response.Name;
                SessionManager.AdminId = response.AdminId;
                SessionManager.Phone = response.Phone;

                Hide();

                var homeForm = Program.ServiceProvider?.GetService<frmHome>();
                homeForm.Show();
            }
            else
            {
                MessageBox.Show("Login error!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Login error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Cursor = Cursors.Arrow;
        }
    }

    private void buttonClear_Click(object sender, EventArgs e)
    {
        if (isPhoneFocus)
        {
            textBoxPhone.Clear();
        }
    }

 
}