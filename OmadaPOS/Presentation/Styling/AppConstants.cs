namespace OmadaPOS.Presentation.Styling;

/// <summary>
/// Application-wide string constants — single source of truth for branding
/// and other repeated literals to prevent typo-based bugs.
/// </summary>
public static class AppConstants
{
    /// <summary>Displayed application name across all screens and receipts.</summary>
    public const string AppName    = "OMADA POS";

    /// <summary>Marketing tagline shown on the login overlay.</summary>
    public const string AppTagline = "Point of Sale";

    /// <summary>User-Agent header sent to external APIs (e.g. Open Food Facts).</summary>
    public const string UserAgent  = "OmadaPOS/1.0 (internal-use)";
}

/// <summary>
/// Payment method identifiers — used across split payment, receipt printing,
/// and waiting overlay. Single definition prevents silent typo bugs.
/// </summary>
public static class PaymentMethod
{
    public const string Cash    = "CASH";
    public const string Credit  = "CREDIT";
    public const string Debit   = "DEBIT";
    public const string Ebt     = "EBT";
    public const string Split   = "SPLIT";
    public const string GiftCard = "GiftCard";
}
