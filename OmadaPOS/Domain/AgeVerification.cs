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
/// Parses AAMVA-compliant PDF417 barcodes produced by US driver's licenses.
///
/// AAMVA format overview
/// ─────────────────────
/// The raw barcode starts with the header "@\n\x1e\rANSI " followed by an
/// IIN/file-header section.  Data elements are three-character identifiers
/// immediately followed by the value and terminated by a newline or CR.
///
/// Fields used here
/// ────────────────
///   DBB  Date of Birth (MMDDYYYY in AAMVA 2000–2016; YYYYMMDD in some states)
///   DAQ  Customer ID number / DL number
/// </summary>
public static class AamvaParser
{
    // Every AAMVA-compliant barcode contains this header prefix.
    private const string AamvaHeader = "ANSI ";

    /// <summary>
    /// Attempts to extract the date of birth from an AAMVA PDF417 barcode string.
    /// </summary>
    /// <param name="rawBarcode">The raw string received from the scanner.</param>
    /// <param name="dob">The parsed date of birth when the method returns true.</param>
    /// <returns>True if parsing succeeded and <paramref name="dob"/> is populated.</returns>
    public static bool TryParseBirthDate(string rawBarcode, out DateOnly dob)
    {
        dob = default;

        if (string.IsNullOrWhiteSpace(rawBarcode))
            return false;

        // Must contain the AAMVA header to be a valid driver's-license barcode
        if (!rawBarcode.Contains(AamvaHeader, StringComparison.Ordinal))
            return false;

        string? dobRaw = ExtractField(rawBarcode, "DBB");
        if (dobRaw == null)
            return false;

        // Try MMDDYYYY (most common for US AAMVA 2000–2016)
        if (dobRaw.Length == 8
            && int.TryParse(dobRaw[..2], out int mm)
            && int.TryParse(dobRaw[2..4], out int dd)
            && int.TryParse(dobRaw[4..],  out int yyyy)
            && IsValidDate(yyyy, mm, dd))
        {
            dob = new DateOnly(yyyy, mm, dd);
            return true;
        }

        // Fallback: YYYYMMDD (used by some AAMVA 2016 issuers and some states)
        if (dobRaw.Length == 8
            && int.TryParse(dobRaw[..4], out yyyy)
            && int.TryParse(dobRaw[4..6], out mm)
            && int.TryParse(dobRaw[6..],  out dd)
            && IsValidDate(yyyy, mm, dd))
        {
            dob = new DateOnly(yyyy, mm, dd);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Attempts to extract the last 4 characters of the customer's ID number (DAQ field).
    /// </summary>
    /// <param name="rawBarcode">The raw string received from the scanner.</param>
    /// <param name="last4">The last 4 characters of the ID number when the method returns true.</param>
    /// <returns>True if the DAQ field was found and at least 4 characters long.</returns>
    public static bool TryParseLast4(string rawBarcode, out string last4)
    {
        last4 = string.Empty;

        if (string.IsNullOrWhiteSpace(rawBarcode))
            return false;

        string? idNumber = ExtractField(rawBarcode, "DAQ");
        if (idNumber == null || idNumber.Length < 4)
            return false;

        last4 = idNumber[^4..];
        return true;
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    /// <summary>
    /// Extracts the value immediately following a 3-character AAMVA field identifier.
    /// Returns null when the field is not present.
    /// </summary>
    private static string? ExtractField(string barcode, string fieldId)
    {
        int idx = barcode.IndexOf(fieldId, StringComparison.Ordinal);
        if (idx < 0) return null;

        int valueStart = idx + fieldId.Length;
        if (valueStart >= barcode.Length) return null;

        // Value ends at next newline, CR, or end-of-string
        int valueEnd = barcode.IndexOfAny(['\n', '\r'], valueStart);
        return valueEnd < 0
            ? barcode[valueStart..]
            : barcode[valueStart..valueEnd];
    }

    private static bool IsValidDate(int year, int month, int day)
    {
        if (year < 1900 || year > DateTime.Today.Year) return false;
        if (month < 1   || month > 12)                 return false;
        if (day < 1     || day > DateTime.DaysInMonth(year, month)) return false;
        return true;
    }
}
