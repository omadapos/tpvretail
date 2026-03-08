namespace OmadaPOS.Services;

/// <summary>
/// Holds the supervisor override PIN used to exit / close the POS application.
/// The PIN can be changed at runtime (e.g. from the Settings screen) and persists
/// for the lifetime of the process. Default: "1234".
/// </summary>
public static class SupervisorConfig
{
    public static string Pin { get; set; } = "1234";
}
