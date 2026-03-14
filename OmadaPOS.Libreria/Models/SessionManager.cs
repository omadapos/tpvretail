namespace OmadaPOS.Libreria.Models;

public static class SessionManager
{
    public static string? Token { get; set; }
    public static int? BranchId { get; set; }
    public static string? UserName { get; set; }
    public static string? Name { get; set; }
    public static int? AdminId { get; set; }
    public static string? Phone { get; set; }

    /// <summary>
    /// True when the branch uses a Cash Discount Program —
    /// credit card payments carry a 3.8% service fee.
    /// Loaded from branch data at frmHome startup.
    /// </summary>
    public static bool CashDiscountEnabled { get; set; }

    /// <summary>Clears all session state. Call on logout to prevent data leaking to the next session.</summary>
    public static void Clear()
    {
        Token               = null;
        BranchId            = null;
        UserName            = null;
        Name                = null;
        AdminId             = null;
        Phone               = null;
        CashDiscountEnabled = false;
    }
}
