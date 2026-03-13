using OposScale_CCO;
using OposScanner_CCO;
using System.ServiceProcess;

namespace OmadaPOS.Services;

public class ZebraScannerService
{
    private OPOSScannerClass? scanner;
    private OPOSScaleClass?   scale;

    public event Action<string>? OnBarcodeDataReceived;

    public event Action<string, double>? OnWeightUpdated;

    /// <summary>True when the OPOS scanner is claimed and receiving data events.</summary>
    public bool IsConnected => scanner?.Claimed == true;

    /// <summary>True when the OPOS scale is claimed and enabled.</summary>
    public bool IsScaleConnected => scale?.Claimed == true && scale?.DeviceEnabled == true;

    // COM objects created here, on the UI thread (STA).
    // Initialize() must also be called from the UI thread so Open/ClaimDevice/events
    // all live in the same STA apartment as the objects.
    public ZebraScannerService()
    {
        scanner = new OPOSScannerClass();
        scale   = new OPOSScaleClass();
    }

    /// <summary>
    /// Re-opens and re-claims the OPOS scanner without restarting the application.
    /// MUST be called from an STA thread — creates a new COM object.
    /// </summary>
    public bool TryReconnectScanner()
    {
        if (scanner?.Claimed == true) return true;

        try
        {
            if (scanner != null)
            {
                scanner.DataEvent -= Scanner_DataEvent;
                try { scanner.Close(); } catch { }
            }

            scanner = new OPOSScannerClass();
            scanner.Open("ZEBRA_SCANNER");
            scanner.ClaimDevice(2000);

            if (scanner.Claimed)
            {
                scanner.DataEvent       += Scanner_DataEvent;
                scanner.DeviceEnabled    = true;
                scanner.DataEventEnabled = true;
                scanner.DecodeData       = true;
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Initializes the OPOS scanner and scale.
    /// Must be called from the UI thread (STA) — the same thread where COM objects were created.
    /// Returns an empty string on full success, or a status message describing partial/full failure.
    /// </summary>
    public string Initialize()
    {
        // Ensure CoreScanner service is running before OPOS tries to open devices.
        // Without it running, the MP7000 won't be visible to the OPOS layer regardless
        // of timeout — this is why the device only works when 123Scan is open simultaneously.
        EnsureCoreScannerService();

        var issues = new List<string>();

        // ── Scanner ─────────────────────────────────────────────────────────
        try
        {
            scanner?.Open("ZEBRA_SCANNER");
            int scannerOpenRC = scanner?.ResultCode ?? -1;
            System.Diagnostics.Debug.WriteLine($"[OPOS] Scanner.Open RC={scannerOpenRC}");

            scanner?.ClaimDevice(2000);
            System.Diagnostics.Debug.WriteLine($"[OPOS] Scanner.Claimed={scanner?.Claimed}, RC={scanner?.ResultCode}");

            if (scanner?.Claimed == true)
            {
                scanner.DataEvent       += Scanner_DataEvent;
                scanner.DeviceEnabled    = true;
                scanner.DataEventEnabled = true;
                scanner.DecodeData       = true;
            }
            else
            {
                issues.Add($"Scanner no conectado (Open RC={scannerOpenRC}, Claim RC={scanner?.ResultCode})");
            }
        }
        catch (Exception ex)
        {
            issues.Add($"Scanner error: {ex.Message}");
        }

        // ── Scale ────────────────────────────────────────────────────────────
        try
        {
            scale?.Open("ZEBRA_SCALE");
            int scaleOpenRC = scale?.ResultCode ?? -1;
            System.Diagnostics.Debug.WriteLine($"[OPOS] Scale.Open RC={scaleOpenRC}");

            scale?.ClaimDevice(5000);
            System.Diagnostics.Debug.WriteLine($"[OPOS] Scale.Claimed={scale?.Claimed}, RC={scale?.ResultCode}, CapStatusUpdate={scale?.CapStatusUpdate}");

            if (scale?.Claimed != true)
            {
                issues.Add($"Báscula no conectada (Open RC={scaleOpenRC}, Claim RC={scale?.ResultCode})");
            }
            else if (scale.CapStatusUpdate)
            {
                scale.StatusNotify = (int)OPOSScaleConstants.SCAL_SN_ENABLED;

                if (scale.ResultCode == (int)OPOSConstants.OPOS_SUCCESS)
                {
                    scale.StatusUpdateEvent += StatusUpdateEvent;
                    scale.DeviceEnabled      = true;

                    if (!scale.DeviceEnabled)
                        issues.Add($"Báscula: fallo al habilitar (código {scale.ResultCode})");
                }
                else
                {
                    issues.Add($"Báscula: StatusNotify falló (código {scale.ResultCode})");
                }
            }
            else
            {
                issues.Add("Báscula: CapStatusUpdate=false — device en modo incorrecto (verifica USB mode = IBM OPOS en 123Scan)");
            }
        }
        catch (Exception ex)
        {
            issues.Add($"Báscula error: {ex.Message}");
        }

        return string.Join(" | ", issues);
    }

    /// <summary>
    /// Ensures the Zebra CoreScanner Windows service is running.
    /// The MP7000 is only visible to the OPOS layer when this service is active.
    /// If the service is stopped, starts it and waits up to 10 seconds for enumeration.
    /// Requires admin privileges to start; silently ignores permission errors.
    /// </summary>
    private static void EnsureCoreScannerService()
    {
        // Known service names used by Zebra CoreScanner across driver versions
        string[] candidates = ["CoreScanner", "ZebraCoreScannerService", "Zebra.Iot.Suite.Scanner.Service"];

        foreach (var name in candidates)
        {
            try
            {
                using var sc = new ServiceController(name);
                _ = sc.Status; // throws if service doesn't exist

                if (sc.Status != ServiceControllerStatus.Running)
                {
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                    // Allow the service time to enumerate connected USB devices before OPOS Open()
                    Thread.Sleep(2500);
                }
                return; // found and handled
            }
            catch { /* service not found or no permission — try next name */ }
        }
    }

    /// <summary>
    /// Re-opens and re-claims the OPOS scale without restarting the application.
    /// Call this from a manual "Reconectar báscula" button after fixing the hardware.
    /// </summary>
    public bool TryReconnectScale()
    {
        if (scale?.Claimed == true && scale.DeviceEnabled) return true;

        try
        {
            EnsureCoreScannerService();

            if (scale != null)
            {
                scale.StatusUpdateEvent -= StatusUpdateEvent;
                try { scale.ReleaseDevice(); } catch { }
                try { scale.Close();         } catch { }
            }

            scale = new OPOSScaleClass();
            scale.Open("ZEBRA_SCALE");
            scale.ClaimDevice(5000);

            if (scale.Claimed && scale.CapStatusUpdate)
            {
                scale.StatusNotify = (int)OPOSScaleConstants.SCAL_SN_ENABLED;
                if (scale.ResultCode == (int)OPOSConstants.OPOS_SUCCESS)
                {
                    scale.StatusUpdateEvent += StatusUpdateEvent;
                    scale.DeviceEnabled      = true;
                    return scale.DeviceEnabled;
                }
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    private void Scanner_DataEvent(int Status)
    {
        scanner.DataEventEnabled = true;
        string label = scanner.ScanDataLabel ?? string.Empty;
        OnBarcodeDataReceived?.Invoke(label);
    }

    private void StatusUpdateEvent(int value)
    {
        //int status = (int)scale.ResultCode;
        string weightStatus = string.Empty;

        switch (value)
        {
            case (int)OPOSScaleConstants.SCAL_SUE_STABLE_WEIGHT:
                weightStatus = WeightFormat(scale.ScaleLiveWeight);
                break;

            case (int)OPOSScaleConstants.SCAL_SUE_WEIGHT_UNSTABLE:
                weightStatus = ".......................";
                break;

            case (int)OPOSScaleConstants.SCAL_SUE_WEIGHT_ZERO:
                weightStatus = WeightFormat(scale.ScaleLiveWeight);
                break;

            case (int)OPOSScaleConstants.SCAL_SUE_WEIGHT_OVERWEIGHT:
                weightStatus = "Weight limit exceeded.";
                break;

            case (int)OPOSScaleConstants.SCAL_SUE_NOT_READY:
                weightStatus = "Scale not ready.";
                break;

            case (int)OPOSScaleConstants.SCAL_SUE_WEIGHT_UNDER_ZERO:
                weightStatus = "Scale under zero weight.";
                break;

            default:
                weightStatus = $"Unknown status [{value}]";
                break;
        }

        OnWeightUpdated?.Invoke(weightStatus, scale.ScaleLiveWeight);
    }

    private string WeightFormat(int weight)
    {
        string units = UnitAbbreviation(scale.WeightUnits);
        return string.IsNullOrEmpty(units) ? "Unknown weight unit" : $"{0.001 * weight:0.000} {units}";
    }

    private string UnitAbbreviation(int units)
    {
        switch(units)
        {
            case (int)OPOSScaleConstants.SCAL_WU_GRAM:
                return "gr.";
            case (int)OPOSScaleConstants.SCAL_WU_KILOGRAM:
                return "Kg.";
            case (int)OPOSScaleConstants.SCAL_WU_OUNCE:
                return "oz.";
            case (int)OPOSScaleConstants.SCAL_WU_POUND:
                return "lb.";
            default:
                return string.Empty;
        }
    }

    public void Close()
    {
        if (scanner != null)
        {
            try
            {
                scanner.DataEvent        -= Scanner_DataEvent;
                scanner.DataEventEnabled  = false;
                if (scanner.Claimed) scanner.ReleaseDevice();
                scanner.Close();
            }
            catch { /* ignore on close */ }
        }

        if (scale != null)
        {
            try
            {
                scale.StatusUpdateEvent -= StatusUpdateEvent;
                scale.DataEventEnabled   = false;
                if (scale.Claimed) scale.ReleaseDevice();
                scale.Close();
            }
            catch { /* ignore on close */ }
        }
    }
}