using OmadaPOS.Domain.Services;
using OmadaPOS.Services;
using OmadaPOS.Services.Navigation;
using OmadaPOS.Services.POS;
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

        // Motor de cálculo de precios — fuente única de verdad para subtotal, tax y total
        services.AddSingleton<IPricingEngine, PricingEngine>();

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
        services.AddScoped<IHomeInitializationService, HomeInitializationService>();
        services.AddScoped<IBarcodeSaleService, BarcodeSaleService>();
        services.AddScoped<IPaymentCoordinatorService, PaymentCoordinatorService>();
        services.AddScoped<IHomeHoldCartService, HoldCartService>();
        services.AddScoped<ICashDrawerService, CashDrawerService>();
        services.AddSingleton<IWindowService, WindowService>();
        services.AddSingleton<IHomeInteractionService, HomeInteractionService>();

        services.AddSingleton<HttpClient>();
         
        services.AddTransient<frmHome>();
        services.AddTransient<frmSignIn>();
        services.AddTransient<frmSplit>();
        services.AddTransient<frmHold>();
        services.AddTransient<frmCheckPrice>();
        services.AddTransient<frmProductNew>();
        services.AddSingleton<frmCustomerScreen>(); // Singleton — one instance per session, opened by frmHome
        services.AddTransient<frmSetting>();
        services.AddTransient<frmCierreDiario>();
        services.AddTransient<frmPopupQuantity>();
        services.AddTransient<frmPopupCashPayment>();
        services.AddTransient<frmGiftCard>();
        services.AddTransient<frmPaymentStatus>();
        services.AddTransient<frmPrintInvoice>();
        services.AddTransient<frmProductNoExist>();
        services.AddTransient<frmKeyLookup>();
        services.AddTransient<frmError>();

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
        // ── Global crash logging ──────────────────────────────────────────────
        var logPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "OmadaPOS_crash.txt");

        Application.ThreadException += (_, e) => LogCrash(logPath, e.Exception);
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            LogCrash(logPath, e.ExceptionObject as Exception ?? new Exception(e.ExceptionObject?.ToString()));
        TaskScheduler.UnobservedTaskException += (_, e) =>
        { LogCrash(logPath, e.Exception); e.SetObserved(); };

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

        Application.Run(ServiceProvider.GetRequiredService<frmSignIn>());
    }

    private static void LogCrash(string path, Exception? ex)
    {
        try
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"=== CRASH {DateTime.Now:yyyy-MM-dd HH:mm:ss} ===");
            sb.AppendLine($"Type   : {ex?.GetType().FullName}");
            sb.AppendLine($"Message: {ex?.Message}");
            sb.AppendLine($"Stack  :\n{ex?.StackTrace}");
            if (ex?.InnerException != null)
            {
                sb.AppendLine($"Inner  : {ex.InnerException.GetType().FullName}: {ex.InnerException.Message}");
                sb.AppendLine(ex.InnerException.StackTrace);
            }
            sb.AppendLine();
            File.AppendAllText(path, sb.ToString());
        }
        catch { /* never crash the crash handler */ }
    }
}