using OmadaPOS.Domain;
using OmadaPOS.Models;

namespace OmadaPOS.Services;

// ── Audit record ───────────────────────────────────────────────────────────────

/// <summary>
/// Audit record saved to SQLite for every age-verification event.
/// Privacy rules: no full ID number, no image, no address.
/// </summary>
public class AgeVerificationAuditRecord
{
    public int      Id                  { get; set; }
    public string   SessionId           { get; init; } = string.Empty;
    public string   CashierName         { get; init; } = string.Empty;
    public DateTime VerifiedAt          { get; init; } = DateTime.Now;
    public string   VerificationMethod  { get; init; } = string.Empty;
    public string   VerificationResult  { get; init; } = string.Empty;
    public bool     CustomerIs21OrOver  { get; init; }
    public string?  IdType              { get; init; }
    public string?  IdLast4OrToken      { get; init; }
    public string?  DenialReason        { get; init; }
}

// ── Interface ──────────────────────────────────────────────────────────────────

public interface IAgeVerificationService
{
    /// <summary>Returns true when at least one cart item requires age verification.</summary>
    bool RequiresVerification(IReadOnlyList<CartItem> items);

    /// <summary>
    /// Calculates the customer's age from <paramref name="dob"/> and returns an
    /// <see cref="AgeVerificationResult"/> reflecting Approved or Denied status.
    /// </summary>
    AgeVerificationResult VerifyAge(
        DateOnly              dob,
        AgeVerificationMethod method,
        string?               idType,
        string?               idLast4);

    /// <summary>Persists the audit record to SQLite (fire-and-log, never throws).</summary>
    Task SaveAuditAsync(AgeVerificationAuditRecord record);
}

// ── Implementation ─────────────────────────────────────────────────────────────

public sealed class AgeVerificationService : IAgeVerificationService
{
    private readonly ISqliteManager _db;

    public AgeVerificationService(ISqliteManager db)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public bool RequiresVerification(IReadOnlyList<CartItem> items)
        => items.Any(i => i.RequiresAgeVerification);

    public AgeVerificationResult VerifyAge(
        DateOnly              dob,
        AgeVerificationMethod method,
        string?               idType,
        string?               idLast4)
    {
        int  age      = AgeCalculator.Calculate(dob);
        bool approved = age >= AgeVerificationPolicy.MinimumAge;

        return new AgeVerificationResult(
            Status:         approved ? AgeVerificationStatus.Approved : AgeVerificationStatus.Denied,
            Method:         method,
            CustomerAge:    age,
            IdType:         idType,
            IdLast4OrToken: idLast4,
            DenialReason:   approved
                                ? null
                                : $"Customer is {age} years old (minimum {AgeVerificationPolicy.MinimumAge})");
    }

    public async Task SaveAuditAsync(AgeVerificationAuditRecord record)
    {
        try
        {
            await _db.SaveAgeVerificationAuditAsync(record).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[AgeVerificationService] Audit save failed: {ex.Message}");
        }
    }
}
