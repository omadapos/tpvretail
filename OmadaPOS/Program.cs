using OmadaPOS.Services;
using OmadaPOS.Views;
using OmadaPOS.Libreria.Services;
using Microsoft.Extensions.DependencyInjection;

namespace OmadaPOS;

internal static class Program
{
    public static IServiceProvider? ServiceProvider { get; set; }

    static void ConfigureServices()
    {
        var services = new ServiceCollection();

        // Configuración básica de logging
        services.AddLogging();

        // Registrar SqliteManager como Singleton
        services.AddSingleton<ISqliteManager, SqliteManager>();
        services.AddScoped<IShoppingCart, ShoppingCart>();
        
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IBannerService, BannerService>();
        services.AddScoped<IGiftCardService, GiftCardService>();

        services.AddSingleton<IPaymentTerminalService, PaymentTerminalService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IPaymentSplitService, PaymentSplitService>();

        services.AddScoped<IBranchService, BranchService>();
        services.AddScoped<IAdminSettingService, AdminSettingService>();
        services.AddScoped<IHoldService, HoldService>();

        services.AddScoped<IOrderApplicationService, OrderApplicationService>();
        services.AddScoped<IProductApplicationService, ProductApplicationService>();

        services.AddSingleton<HttpClient>();
         
        services.AddTransient<frmHome>();

        services.AddSingleton<ZebraScannerService>();

        ServiceProvider = services.BuildServiceProvider();
    }

    public static T GetService<T>() where T : class
    {
        return (T)ServiceProvider.GetService(typeof(T));
    }

    [STAThread]
    static async Task Main()
    {
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        ConfigureServices();

        // Inicializar la base de datos al inicio de la aplicación
        try
        {
            var sqliteManager = ServiceProvider.GetRequiredService<ISqliteManager>();

          //   await sqliteManager.DropTablesAsync();

            await sqliteManager.InitializeDatabaseAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al inicializar la base de datos: {ex.Message}", 
                "Error de Inicialización", 
                MessageBoxButtons.OK, 
                MessageBoxIcon.Error);
            return;
        }

        Application.Run(new frmSignIn());
    }
}