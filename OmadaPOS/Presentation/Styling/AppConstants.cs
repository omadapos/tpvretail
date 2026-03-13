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

    /// <summary>User-Agent header sent to external HTTP requests.</summary>
    public const string UserAgent = "OmadaPOS/1.0 (internal-use)";

    // ── External Product Catalog API ───────────────────────────────────────────
    /// <summary>Base URL for the OmadaPOS global product catalog API.</summary>
    public const string ExternalProductApiBase = "https://api.omadapos.com/v1/products/upc/";

    /// <summary>
    /// API key for the OmadaPOS product catalog.
    /// For production, move this to encrypted app settings or a secrets manager.
    /// </summary>
    public const string ExternalProductApiKey  = "ext_c29e8323471fbfe00527f6cbb01535d8e01e42563fa6097109f265be";
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
