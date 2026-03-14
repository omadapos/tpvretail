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
    // Internal — only read, never replaced after startup.
    internal static IServiceProvider ServiceProvider { get; private set; } = null!;

    static void ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddLogging();

        // ── Infrastructure — Singletons (shared across the whole app lifetime) ─────
        services.AddSingleton<ISqliteManager, SqliteManager>();
        services.AddSingleton<IPaymentTerminalService, PaymentTerminalService>();
        services.AddSingleton<IWindowService, WindowService>();
        services.AddSingleton<IHomeInteractionService, HomeInteractionService>();
        services.AddSingleton<IPricingEngine, PricingEngine>();
        services.AddSingleton<HttpClient>(_ => new HttpClient
        {
            // Default is 100 s — too long for a POS terminal. With 4 sequential startup
            // calls, a hung backend would freeze the UI for up to 400 s without this limit.
            Timeout = TimeSpan.FromSeconds(15)
        });
        services.AddSingleton<ZebraScannerService>();

        // ── Session state — Singleton (one cart per POS session) ─────────────────
        // ShoppingCart is shared between frmHome and frmCustomerScreen so
        // both see the same items in real time.
        services.AddSingleton<IShoppingCart, ShoppingCart>();

        // ── Stateless API / DB services — Transient (no shared mutable state) ────
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<ICategoryService, CategoryService>();
        services.AddTransient<IOrderService, OrderService>();
        services.AddTransient<IBannerService, BannerService>();
        services.AddTransient<IGiftCardService, GiftCardService>();
        services.AddTransient<IPaymentService, PaymentService>();
        services.AddTransient<IPaymentSplitService, PaymentSplitService>();
        services.AddTransient<IBranchService, BranchService>();
        services.AddTransient<IAdminSettingService, AdminSettingService>();
        services.AddTransient<IHoldService, HoldService>();

        // ── Age verification — Singleton config cache + Transient service ─────────
        services.AddSingleton<IAgeRestrictionConfigService, AgeRestrictionConfigService>();
        services.AddTransient<IAgeVerificationService, AgeVerificationService>();
        services.AddSingleton<IExternalProductService, ExternalProductService>();

        // ── Application services — Transient ──────────────────────────────────────
        services.AddTransient<IOrderApplicationService, OrderApplicationService>();
        services.AddTransient<IProductApplicationService, ProductApplicationService>();
        services.AddTransient<IHomeInitializationService, HomeInitializationService>();
        services.AddTransient<IBarcodeSaleService, BarcodeSaleService>();
        services.AddTransient<IPaymentCoordinatorService, PaymentCoordinatorService>();
        services.AddTransient<IHomeHoldCartService, HoldCartService>();
        services.AddTransient<ICashDrawerService, CashDrawerService>();

        // ── Forms ─────────────────────────────────────────────────────────────────
        // frmSignIn: Singleton so the same instance is shown/hidden across sessions.
        // frmCustomerScreen: Singleton — one display per machine, lifecycle tied to frmHome.
        // All other forms: Transient — fresh instance each time they are opened.
        services.AddSingleton<frmSignIn>();
        services.AddSingleton<frmCustomerScreen>();
        services.AddTransient<frmHome>();
        services.AddTransient<frmSplit>();
        services.AddTransient<frmHold>();
        services.AddTransient<frmCheckPrice>();
        services.AddTransient<frmProductNew>();
        services.AddTransient<frmSetting>();
        services.AddTransient<frmCierreDiario>();
        services.AddTransient<frmPopupQuantity>();
        services.AddTransient<frmPopupCashPayment>();
        services.AddTransient<frmGiftCard>();
        services.AddTransient<frmPaymentStatus>();
        services.AddTransient<frmPrintInvoice>();
        services.AddTransient<frmProductNoExist>();
        services.AddTransient<frmRefund>();
        services.AddTransient<frmKeyLookup>();
        services.AddTransient<frmError>();

        ServiceProvider = services.BuildServiceProvider();
    }

    // Kept for legacy call-sites that use the service locator pattern.
    // Prefer constructor injection for new code.
    internal static T GetService<T>() where T : class
        => ServiceProvider.GetRequiredService<T>();

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

        // Load persisted supervisor PIN (overrides the hardcoded default "1234").
        LocalPinStore.Load();

        ConfigureServices();

        // Inicializar la base de datos al inicio de la aplicación
        try
        {
            var sqliteManager = ServiceProvider.GetRequiredService<ISqliteManager>();

          //   await sqliteManager.DropTablesAsync();

            await sqliteManager.InitializeDatabaseAsync();

            // Load age-restriction whitelist into memory after DB is ready
            var ageConfig = ServiceProvider.GetRequiredService<IAgeRestrictionConfigService>();
            await ageConfig.ReloadAsync();
            // Dev testing only — uncomment to flag a UPC as age-restricted without using the backend:
            // await ageConfig.AddUpcAsync("YOUR_TEST_UPC", "TEST");
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