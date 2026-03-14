using System.Security.Cryptography;
using System.Text;

namespace OmadaPOS.Services;

/// <summary>
/// Holds the supervisor override PIN used to exit / close the POS / close day.
/// The PIN is stored as a SHA-256 hash so it is never kept in plain text at runtime.
/// Default hash corresponds to PIN "1234".
/// </summary>
public static class SupervisorConfig
{
    // Default: SHA-256 of "1234"
    public static string PinHash { get; set; } = HashPin("1234");

    /// <summary>Returns true if <paramref name="rawPin"/> matches the stored hash.</summary>
    public static bool Verify(string rawPin) => HashPin(rawPin) == PinHash;

    /// <summary>Hashes a raw PIN string with SHA-256 and returns a Base64 string.</summary>
    public static string HashPin(string rawPin)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawPin));
        return Convert.ToBase64String(bytes);
    }
}

/// <summary>
/// Persists the supervisor PIN hash to a local file so it survives application restarts
/// without requiring a backend API change.
/// File: %LocalAppData%\OmadaPOS\supervisor.pin
/// </summary>
public static class LocalPinStore
{
    private static readonly string _path = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "OmadaPOS", "supervisor.pin");

    /// <summary>
    /// Loads the stored PIN hash into <see cref="SupervisorConfig.PinHash"/>.
    /// If the file does not exist, the default "1234" hash is kept.
    /// </summary>
    public static void Load()
    {
        try
        {
            if (!File.Exists(_path)) return;
            string hash = File.ReadAllText(_path).Trim();
            if (!string.IsNullOrWhiteSpace(hash))
                SupervisorConfig.PinHash = hash;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[LocalPinStore] Load failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Hashes <paramref name="rawPin"/> and saves it to disk.
    /// Also updates <see cref="SupervisorConfig.PinHash"/> in memory.
    /// </summary>
    public static void Save(string rawPin)
    {
        try
        {
            string hash = SupervisorConfig.HashPin(rawPin);
            Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
            File.WriteAllText(_path, hash);
            SupervisorConfig.PinHash = hash;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[LocalPinStore] Save failed: {ex.Message}");
            throw;
        }
    }
}
