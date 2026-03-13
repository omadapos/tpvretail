namespace OmadaPOS.Domain;

// ── Enumerations ──────────────────────────────────────────────────────────────

public enum AgeVerificationStatus { Pending, Approved, Denied }
public enum AgeVerificationMethod { Manual, Scan }

// ── Value objects ─────────────────────────────────────────────────────────────

/// <summary>
/// Immutable result produced by a single age-verification event.
/// Stored in frmHome per transaction; cleared on cancel / completion.
/// </summary>
public record AgeVerificationResult(
    AgeVerificationStatus Status,
    AgeVerificationMethod Method,
    int                   CustomerAge,
    string?               IdType,
    string?               IdLast4OrToken,
    string?               DenialReason);

// ── Policy ────────────────────────────────────────────────────────────────────

public static class AgeVerificationPolicy
{
    /// <summary>Minimum legal purchase age for age-restricted products (US federal / state standard).</summary>
    public const int MinimumAge = 21;
}

// ── Age calculator ────────────────────────────────────────────────────────────

public static class AgeCalculator
{
    /// <summary>
    /// Returns the number of full years between <paramref name="dob"/> and today.
    /// Correctly handles leap-year birthdays (Feb 29 customers).
    /// </summary>
    public static int Calculate(DateOnly dob)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        int age   = today.Year - dob.Year;

        // Subtract 1 if the birthday has not occurred yet this calendar year
        if (today < dob.AddYears(age))
            age--;

        return age;
    }

    /// <summary>Returns true when the customer meets the minimum legal age requirement.</summary>
    public static bool IsOfLegalAge(DateOnly dob)
        => Calculate(dob) >= AgeVerificationPolicy.MinimumAge;
}

// ── AAMVA PDF417 driver's-license barcode parser ──────────────────────────────

/// <summary>
/// Parses AAMVA-compliant PDF417 barcodes produced by US/Canada driver's licenses.
///
/// AAMVA format overview
/// ─────────────────────
/// The raw barcode starts with:
///   @ (0x40)  +  LF (0x0A)  +  RS (0x1E)  +  CR (0x0D)  +  "ANSI " + IIN...
///
/// Some scanners / OPOS implementations strip leading control characters, so
/// we accept "ANSI " appearing anywhere in the string.
///
/// Fields used
/// ───────────
///   DBB  Date of Birth  —  MMDDYYYY (AAMVA ≤ 2016) or YYYYMMDD (AAMVA 2016 some states)
///   DAQ  DL / ID number (last 4 stored for audit, never the full number)
///
/// Field terminator
/// ────────────────
/// AAMVA fields end with LF (0x0A), CR (0x0D), or FS (0x1C — file separator).
/// All three are handled here.
/// </summary>
public static class AamvaParser
{
    // Field terminators used across AAMVA versions:
    //   LF  (\n 0x0A) — most states
    //   CR  (\r 0x0D) — some older issuers
    //   FS  (\x1c 0x1C) — file separator used by some AAMVA 2016 states
    private static readonly char[] FieldTerminators = ['\n', '\r', '\x1c'];

    /// <summary>
    /// Returns true if <paramref name="rawBarcode"/> looks like a valid AAMVA
    /// driver's-license barcode.
    ///
    /// Detection strategy:
    ///   The spec mandates "ANSI " (with space) but real Connecticut barcodes
    ///   arrive as "ANSI636006..." (no space — IIN follows immediately).
    ///   We therefore look for "ANSI" anywhere in the string AND require the
    ///   "DBB" field (date of birth) to be present, which rules out false
    ///   positives from product barcodes that happen to contain "ANSI".
    /// </summary>
    public static bool IsAamvaBarcode(string rawBarcode)
        => !string.IsNullOrWhiteSpace(rawBarcode)
        && rawBarcode.Contains("ANSI", StringComparison.Ordinal)
        && rawBarcode.Contains("DBB",  StringComparison.Ordinal);

    /// <summary>
    /// Attempts to extract the date of birth from an AAMVA PDF417 barcode string.
    /// </summary>
    public static bool TryParseBirthDate(string rawBarcode, out DateOnly dob)
    {
        dob = default;
        if (!IsAamvaBarcode(rawBarcode)) return false;

        string? dobRaw = ExtractField(rawBarcode, "DBB");

        // Connecticut and most AAMVA states pad to exactly 8 digits
        if (string.IsNullOrWhiteSpace(dobRaw) || dobRaw.Length < 8)
            return false;

        // Take exactly 8 digits, ignore any trailing noise
        dobRaw = dobRaw[..8];

        // Format 1: MMDDYYYY  (AAMVA 2000–2016, most US states)
        if (TryParseMMDDYYYY(dobRaw, out dob)) return true;

        // Format 2: YYYYMMDD  (AAMVA 2016 revision, some state issuers)
        if (TryParseYYYYMMDD(dobRaw, out dob)) return true;

        return false;
    }

    /// <summary>
    /// Attempts to extract the last 4 characters of the DL number (DAQ field).
    /// </summary>
    public static bool TryParseLast4(string rawBarcode, out string last4)
    {
        last4 = string.Empty;
        if (!IsAamvaBarcode(rawBarcode)) return false;

        string? idNumber = ExtractField(rawBarcode, "DAQ");
        if (string.IsNullOrWhiteSpace(idNumber) || idNumber.Length < 4)
            return false;

        last4 = idNumber.Trim()[^4..];
        return true;
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    /// <summary>
    /// Extracts the value following a 3-character AAMVA field ID.
    /// Handles all standard AAMVA field terminators: LF, CR, FS (0x1C).
    /// Trims whitespace from the extracted value.
    /// </summary>
    private static string? ExtractField(string barcode, string fieldId)
    {
        int idx = barcode.IndexOf(fieldId, StringComparison.Ordinal);
        if (idx < 0) return null;

        int valueStart = idx + fieldId.Length;
        if (valueStart >= barcode.Length) return null;

        int valueEnd = barcode.IndexOfAny(FieldTerminators, valueStart);
        var raw = valueEnd < 0
            ? barcode[valueStart..]
            : barcode[valueStart..valueEnd];

        return raw.Trim();
    }

    private static bool TryParseMMDDYYYY(string s, out DateOnly d)
    {
        d = default;
        if (s.Length != 8) return false;
        if (!int.TryParse(s[..2], out int mm))  return false;
        if (!int.TryParse(s[2..4], out int dd)) return false;
        if (!int.TryParse(s[4..],  out int yy)) return false;
        if (!IsValidDate(yy, mm, dd)) return false;
        d = new DateOnly(yy, mm, dd);
        return true;
    }

    private static bool TryParseYYYYMMDD(string s, out DateOnly d)
    {
        d = default;
        if (s.Length != 8) return false;
        if (!int.TryParse(s[..4],  out int yy)) return false;
        if (!int.TryParse(s[4..6], out int mm)) return false;
        if (!int.TryParse(s[6..],  out int dd)) return false;
        if (!IsValidDate(yy, mm, dd)) return false;
        d = new DateOnly(yy, mm, dd);
        return true;
    }

    private static bool IsValidDate(int year, int month, int day)
    {
        if (year  < 1900 || year  > DateTime.Today.Year)             return false;
        if (month < 1    || month > 12)                               return false;
        if (day   < 1    || day   > DateTime.DaysInMonth(year, month)) return false;
        return true;
    }
}
