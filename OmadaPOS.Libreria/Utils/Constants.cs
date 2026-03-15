namespace OmadaPOS.Libreria.Utils;

/// <summary>
/// Application-wide constants with settable defaults.
/// Override values by calling <c>Constants.Apply(config)</c> at startup
/// (done in Program.cs from appsettings.json).
/// </summary>
public class Constants
{
    // ── API ────────────────────────────────────────────────────────────────────
    public static string BaseUrl     { get; set; } = "https://efoodapi.azurewebsites.net";
    public static string URL_STORAGE { get; set; } = "https://efood.blob.core.windows.net/product/";

    // ── Terminal defaults (overridden per-terminal in AdminSetting) ────────────
    public static string IP      { get; set; } = "192.168.1.100";
    public static int    PORT    { get; set; } = 10009;
    public static int    TIMEOUT { get; set; } = 180000;

    // ── Business ───────────────────────────────────────────────────────────────
    /// <summary>Default customer ID when no customer lookup is used.</summary>
    public static int CUSTOMERID { get; set; } = 54;

    // ── Custom product PLUs (backend-assigned IDs) ─────────────────────────────
    public static int CustomProduct       { get; set; } = 65634;
    public static int CustomProductTax    { get; set; } = 65635;
    public static int CustomProductWeight { get; set; } = 69271;

    public static Dictionary<string, string> LISTERRORS =
                  new Dictionary<string, string>
                  {
                       { "000000", "payment Ok" },
                       { "000001", "The transaction has been approved offline/locally by the application" },
                       { "000002", "Transaction is partially approved." },
                       { "000003", "The transaction was approved, but surcharge fee was not applied." },
                       { "000029", "Batch closed some transactions successfully, but some SAF transactions have declined. Check the Failed SAF database." },
                       { "000100", "Host error" },
                       { "000200", "Payment Transaction was not processed, only the track data for the nonpayment card was returned. (Card was not processed for any form of payment)." },
                       { "000300", "Batch Close successful but the batch is out of balance. The closed batch needs to be reconciled by the host." },
                       { "000301", "atch has already been closed once but there are only offline failed records in the database." },
                       { "100001", "Timeout" },
                       { "100002", "User aborted." },
                       { "100003", "Error reading chip" },
                       { "100008", "Send message to host error." },
                       { "100009", "Receive message error." },
                       { "100015", "CVV Error." },
                       { "100023", "Error. unknown" }
                  };
}

public enum PaymentType
{
    Credit,
    Debit,
    EBT,
    EBTBalance,
    CreditReturn,
    DebitReturn
}