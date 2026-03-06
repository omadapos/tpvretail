namespace OmadaPOS.Libreria.Models;

public static class SessionManager
{
    public static string? Token { get; set; }
    public static int? BranchId { get; set; }
    public static string? UserName { get; set; }
    public static string? Name { get; set; }
    public static int? AdminId { get; set; }
    public static string? Phone { get; set; }
}
