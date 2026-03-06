namespace OmadaPOS.Libreria.Models;

public class AdminSetting
{
    public int Id { get; set; }

    public int? BranchId { get; set; }

    public string? WindowsId { get; set; }

    public string? IP { get; set; } 

    public int? Port { get; set; } = 0;

    public string? Terminal { get; set; }

    public string? PrinterName { get; set; }
}
