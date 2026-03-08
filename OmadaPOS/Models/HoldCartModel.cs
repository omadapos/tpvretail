namespace OmadaPOS.Models;

public class HoldCartModel
{
    public string   HoldId       { get; set; } = string.Empty;
    public DateTime LastModified { get; set; }
    public int      ItemCount    { get; set; }

    public override string ToString() =>
        $"Cart: {HoldId}  ·  {ItemCount} item{(ItemCount != 1 ? "s" : "")}  ·  {LastModified:hh:mm tt}";
}