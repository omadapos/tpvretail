namespace OmadaPOS.Infrastructure;

/// <summary>
/// Raised by <see cref="RetryingHandler"/> when all retry attempts are exhausted
/// due to a network-level error (no internet, server unreachable, timeout).
///
/// Consumers (frmHome) subscribe to display a user-friendly offline banner
/// instead of letting a raw exception propagate to the cashier screen.
///
/// The event fires at most once per 60 seconds to avoid flooding the UI.
/// </summary>
public static class OfflineNotifier
{
    private static long _lastFireTick = 0;

    /// <summary>Fired when a network failure persists after all retries.</summary>
    public static event EventHandler? ConnectivityLost;

    internal static void Raise()
    {
        long now = Environment.TickCount64;
        long last = Interlocked.Read(ref _lastFireTick);

        // Throttle: only fire once per 60 s to avoid banner spam.
        if (now - last < 60_000) return;

        Interlocked.Exchange(ref _lastFireTick, now);
        ConnectivityLost?.Invoke(null, EventArgs.Empty);
    }
}
