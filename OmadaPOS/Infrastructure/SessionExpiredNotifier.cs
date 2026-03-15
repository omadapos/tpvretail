namespace OmadaPOS.Infrastructure;

/// <summary>
/// Decoupled channel for notifying the UI layer that the API returned 401
/// (token expired or invalidated server-side).
///
/// The <see cref="TokenExpiryHandler"/> raises <see cref="SessionExpired"/>
/// from a background thread; subscribers must marshal to the UI thread themselves.
/// </summary>
public static class SessionExpiredNotifier
{
    private static int _fired = 0;   // ensures we show only one dialog per expiry

    /// <summary>
    /// Raised once per expiry event (subsequent 401s within the same session are suppressed).
    /// Reset by calling <see cref="Reset"/> after a successful re-login.
    /// </summary>
    public static event EventHandler? SessionExpired;

    internal static void Raise()
    {
        // Only fire once — multiple concurrent API calls could all return 401
        if (Interlocked.CompareExchange(ref _fired, 1, 0) == 0)
            SessionExpired?.Invoke(null, EventArgs.Empty);
    }

    /// <summary>Call after a successful login to allow future expiry events.</summary>
    public static void Reset() => Interlocked.Exchange(ref _fired, 0);
}
