namespace OmadaPOS.Libreria.Models;

public class LoginResponse
{
    public string? Token { get; set; }
    public string? Name { get; set; }
    public int? AdminId { get; set; }
    public string? Phone { get; set; }
    public int? BranchId { get; set; } = 31;
}
