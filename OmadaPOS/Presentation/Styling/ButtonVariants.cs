using System.Windows.Forms;

namespace OmadaPOS.Presentation.Styling;

/// <summary>
/// Semantic button presets for the POS Design System.
///
/// Each method applies a complete, consistent visual style to a button using
/// <see cref="ElegantButtonStyles.Style"/> as the rendering engine.
///
/// Usage:
///   ButtonVariants.Primary(btnConfirm);
///   ButtonVariants.Danger(btnCancel);
///   ButtonVariants.Payment(btnCash, AppColors.PaymentCash);
///
/// Replaces direct calls to ElegantButtonStyles.Style() with semantically
/// named methods that encode intent rather than raw color values.
/// </summary>
public static class ButtonVariants
{
    // ─── Action buttons ───────────────────────────────────────────────────────

    /// <summary>Primary positive action — confirm, save, OK. Emerald green.</summary>
    public static void Primary(Button button, float fontSize = 18f) =>
        ElegantButtonStyles.Style(button, AppColors.AccentGreen, AppColors.TextWhite, AppRadii.Medium, fontSize);

    /// <summary>Destructive / cancel action — delete, close, reject. Red.</summary>
    public static void Danger(Button button, float fontSize = 18f) =>
        ElegantButtonStyles.Style(button, AppColors.Danger, AppColors.TextWhite, AppRadii.Medium, fontSize);

    /// <summary>Caution / warning action — EBT, amber. </summary>
    public static void Warning(Button button, float fontSize = 18f) =>
        ElegantButtonStyles.Style(button, AppColors.Warning, AppColors.TextWhite, AppRadii.Medium, fontSize);

    /// <summary>Informational / secondary positive — debit, neutral slate.</summary>
    public static void Secondary(Button button, float fontSize = 18f) =>
        ElegantButtonStyles.Style(button, AppColors.SlateBlue, AppColors.TextWhite, AppRadii.Medium, fontSize);

    /// <summary>Structural / navigation — navy, header-aligned.</summary>
    public static void NavyGhost(Button button, float fontSize = 18f) =>
        ElegantButtonStyles.Style(button, AppColors.NavyBase, AppColors.TextWhite, AppRadii.Medium, fontSize);

    // ─── Payment method buttons ───────────────────────────────────────────────

    /// <summary>Cash payment button — emerald green.</summary>
    public static void PaymentCash(Button button, float fontSize = 24f) =>
        ElegantButtonStyles.Style(button, AppColors.PaymentCash, AppColors.TextWhite, AppRadii.Medium, fontSize);

    /// <summary>Credit card payment button — blue.</summary>
    public static void PaymentCredit(Button button, float fontSize = 18f) =>
        ElegantButtonStyles.Style(button, AppColors.PaymentCredit, AppColors.TextWhite, AppRadii.Medium, fontSize);

    /// <summary>Debit card payment button — slate gray.</summary>
    public static void PaymentDebit(Button button, float fontSize = 18f) =>
        ElegantButtonStyles.Style(button, AppColors.PaymentDebit, AppColors.TextWhite, AppRadii.Medium, fontSize);

    /// <summary>EBT payment button — amber-brown.</summary>
    public static void PaymentEBT(Button button, float fontSize = 18f) =>
        ElegantButtonStyles.Style(button, AppColors.PaymentEBT, AppColors.TextWhite, AppRadii.Medium, fontSize);

    /// <summary>Gift card payment button — violet.</summary>
    public static void PaymentGiftCard(Button button, float fontSize = 18f) =>
        ElegantButtonStyles.Style(button, AppColors.PaymentGiftCard, AppColors.TextWhite, AppRadii.Medium, fontSize);

    /// <summary>Split payment button — cyan.</summary>
    public static void PaymentSplit(Button button, float fontSize = 18f) =>
        ElegantButtonStyles.Style(button, AppColors.PaymentSplit, AppColors.TextWhite, AppRadii.Medium, fontSize);

    // ─── Keypad / numeric ─────────────────────────────────────────────────────

    /// <summary>Standard numeric keypad key — navy base, large font for touch use.</summary>
    public static void Keypad(Button button, float fontSize = 30f) =>
        ElegantButtonStyles.Style(button, AppColors.NavyBase, AppColors.TextWhite, AppRadii.Medium, fontSize);

    /// <summary>Keypad action key (delete / clear) — slightly distinct slate tone.</summary>
    public static void KeypadAction(Button button, float fontSize = 18f) =>
        ElegantButtonStyles.Style(button, AppColors.NavyLight, AppColors.TextWhite, AppRadii.Medium, fontSize);

    // ─── Alphabet / search bar ────────────────────────────────────────────────

    /// <summary>
    /// Letter-filter button in the product search alphabet bar.
    /// Slate-gray default, emerald on hover/active — styled via FlatStyle.Flat
    /// (no Paint override) for performance over a full-width row.
    /// Delegates to <see cref="ModernAlphabetButtonStyle.Apply"/> so there
    /// is a single source of truth for alphabet button appearance.
    /// </summary>
    public static void Alphabet(Button button) =>
        ModernAlphabetButtonStyle.Apply(button);

    // ─── Utility ─────────────────────────────────────────────────────────────

    /// <summary>EBT balance enquiry — amber warning tone.</summary>
    public static void EBTBalance(Button button, float fontSize = 18f) =>
        ElegantButtonStyles.Style(button, AppColors.Warning, AppColors.TextWhite, AppRadii.Medium, fontSize);

    /// <summary>
    /// Applies a payment method button by semantic name string.
    /// Matches: "cash", "credit", "debit", "ebt", "giftcard"/"gift", "split".
    /// Falls back to <see cref="NavyGhost"/> for unknown names.
    /// </summary>
    public static void ApplyByMethodName(Button button, string methodName, float fontSize = 18f)
    {
        switch (methodName.ToLowerInvariant())
        {
            case "cash":                PaymentCash(button, fontSize);     break;
            case "credit":              PaymentCredit(button, fontSize);   break;
            case "debit":               PaymentDebit(button, fontSize);    break;
            case "ebt":                 PaymentEBT(button, fontSize);      break;
            case "giftcard":
            case "gift":                PaymentGiftCard(button, fontSize); break;
            case "split":               PaymentSplit(button, fontSize);    break;
            default:                    NavyGhost(button, fontSize);       break;
        }
    }
}
