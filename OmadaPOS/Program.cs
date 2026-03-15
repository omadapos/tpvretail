using OmadaPOS.Domain.Services;
using OmadaPOS.Infrastructure;
using OmadaPOS.Services;
using OmadaPOS.Services.Navigation;
using OmadaPOS.Services.POS;
using OmadaPOS.Views;
using OmadaPOS.Libreria.Services;
using OmadaPOS.Libreria.Models;
using OmadaPOS.Libreria.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace OmadaPOS;

internal static class Program
{
    // Internal — only read, never replaced after startup.
    internal static IServiceProvider ServiceProvider { get; private set; } = null!;

    internal static IConfiguration Configuration { get; private set; } = null!;

    static void ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddLogging(b => b.AddSerilog(dispose: true));

        // ── Infrastructure — Singletons (shared across the whole app lifetime) ─────
        services.AddSingleton<ISqliteManager, SqliteManager>();
        services.AddSingleton<IPaymentTerminalService, PaymentTerminalService>();
        services.AddSingleton<IWindowService, WindowService>();
        services.AddSingleton<IHomeInteractionService, HomeInteractionService>();
        services.AddSingleton<IPricingEngine, PricingEngine>();
        // Handler chain (outer → inner):
        //   RetryingHandler → retries transient 5xx / network errors (up to 2 retries)
        //   TokenExpiryHandler → intercepts 401 and forces re-login
        //   HttpClientHandler → actual TCP connection
        services.AddSingleton<HttpClient>(_ =>
        {
            var inner   = new TokenExpiryHandler();          // already wraps HttpClientHandler
            var outer   = new RetryingHandler { InnerHandler = inner };
            return new HttpClient(outer)
            {
                // 15 s per attempt; with 2 retries + backoff total worst case ≈ 47 s
                Timeout = TimeSpan.FromSeconds(15)
            };
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
        services.AddTransient<frmAgeVerificationLog>();
        services.AddTransient<frmDiagnostics>();
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
        // ── Load external configuration ───────────────────────────────────────
        Configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .Build();

        // Override Constants defaults with values from appsettings.json
        Constants.BaseUrl               = Configuration["Api:BaseUrl"]               ?? Constants.BaseUrl;
        Constants.URL_STORAGE           = Configuration["Api:StorageUrl"]             ?? Constants.URL_STORAGE;
        Constants.IP                    = Configuration["Terminal:DefaultIp"]          ?? Constants.IP;
        Constants.PORT                  = int.TryParse(Configuration["Terminal:DefaultPort"],    out int port)    ? port    : Constants.PORT;
        Constants.TIMEOUT               = int.TryParse(Configuration["Terminal:TimeoutMs"],      out int timeout) ? timeout : Constants.TIMEOUT;
        Constants.CUSTOMERID            = int.TryParse(Configuration["Business:DefaultCustomerId"],        out int cid)  ? cid  : Constants.CUSTOMERID;
        Constants.CustomProduct         = int.TryParse(Configuration["Business:CustomProductId"],          out int cp1)  ? cp1  : Constants.CustomProduct;
        Constants.CustomProductTax      = int.TryParse(Configuration["Business:CustomProductTaxId"],       out int cp2)  ? cp2  : Constants.CustomProductTax;
        Constants.CustomProductWeight   = int.TryParse(Configuration["Business:CustomProductWeightId"],    out int cp3)  ? cp3  : Constants.CustomProductWeight;

        // ── Serilog — file sink (rotates daily, keeps 30 files) ───────────────
        string logDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "OmadaPOS", "logs");
        Directory.CreateDirectory(logDir);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Is(Enum.TryParse<Serilog.Events.LogEventLevel>(
                Configuration["Logging:MinimumLevel"], ignoreCase: true, out var lvl)
                    ? lvl : Serilog.Events.LogEventLevel.Information)
            .Enrich.WithProperty("App", "OmadaPOS")
            .WriteTo.File(
                path:                   Path.Combine(logDir, "omadapos-.log"),
                rollingInterval:        Serilog.RollingInterval.Day,
                retainedFileCountLimit: 30,
                outputTemplate:         "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        Log.Information("OmadaPOS starting. BaseUrl={BaseUrl}", Constants.BaseUrl);

        // ── Global crash logging (also write to Serilog) ──────────────────────
        var logPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "OmadaPOS_crash.txt");

        Application.ThreadException += (_, e) =>
        { Log.Fatal(e.Exception, "Unhandled UI thread exception"); LogCrash(logPath, e.Exception); };
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            var ex = e.ExceptionObject as Exception ?? new Exception(e.ExceptionObject?.ToString());
            Log.Fatal(ex, "Unhandled AppDomain exception");
            LogCrash(logPath, ex);
        };
        TaskScheduler.UnobservedTaskException += (_, e) =>
        { Log.Warning(e.Exception, "Unobserved task exception"); LogCrash(logPath, e.Exception); e.SetObserved(); };

        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Load persisted supervisor PIN (overrides the hardcoded default "1234").
        LocalPinStore.Load();

        ConfigureServices();

        // ── 401 handler: when token expires, clear session and force re-login ──
        SessionExpiredNotifier.SessionExpired += (_, _) =>
        {
            // Must marshal to the STA UI thread
            var signIn = ServiceProvider.GetRequiredService<frmSignIn>();
            signIn.BeginInvoke(() =>
            {
                Log.Warning("Session token expired — forcing re-login");
                SessionManager.Clear();
                SessionExpiredNotifier.Reset();

                // Close all open forms except frmSignIn
                foreach (Form f in Application.OpenForms.Cast<Form>().ToArray())
                    if (f is not frmSignIn) f.Close();

                MessageBox.Show(
                    "Your session has expired. Please sign in again.",
                    "Session Expired", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                signIn.Show();
            });
        };

        // ── Offline / connectivity-lost handler ───────────────────────────────
        OfflineNotifier.ConnectivityLost += (_, _) =>
        {
            // Show a friendly toast on the currently active form.
            var home = Application.OpenForms.OfType<frmHome>().FirstOrDefault();
            if (home is { IsDisposed: false })
            {
                home.BeginInvoke(() =>
                {
                    home.ShowOfflineToast();
                });
            }
        };

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

        Log.Information("OmadaPOS shutting down");
        Log.CloseAndFlush();
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