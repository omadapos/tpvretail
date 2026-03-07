namespace OmadaPOS.Models;

/// <summary>
/// Immutable data record for a single shopping-cart line.
/// Contains only raw product data; all price calculations are
/// performed by <see cref="OmadaPOS.Domain.Services.PricingEngine"/>.
///
/// Subtotal, TaxAmount and Total are populated by PricingEngine.ApplyPricing()
/// and should never be set manually from outside the domain layer.
/// </summary>
public class CartItem
{
    // ─── Product data ─────────────────────────────────────────────────────────

    public int     Number      { get; set; }
    public int     ProductId   { get; set; }
    public string  ProductName { get; set; } = string.Empty;
    public double  Quantity    { get; set; }
    public double  Weight      { get; set; }
    public decimal UnitPrice   { get; set; }
    /// <summary>Tax rate as a percentage (e.g. 7 means 7%).</summary>
    public double  Tax         { get; set; }
    public DateTime Date       { get; set; } = DateTime.Now;

    // ─── Promotion data ───────────────────────────────────────────────────────

    /// <summary>Promotion type identifier. Currently only "Price" is supported.</summary>
    public string?  PromotionName  { get; set; }
    /// <summary>Quantity threshold for the promotion (e.g. 2 in "2 for $5").</summary>
    public double   PromotionValue { get; set; }
    /// <summary>Promotion price (e.g. 5 in "2 for $5").</summary>
    public decimal  PromotionLimit { get; set; }

    // ─── Extra attributes ────────────────────────────────────────────────────

    public bool    IsEBT  { get; set; }
    public string? UPC    { get; set; }
    public string? Image  { get; set; }
    /// <summary>Weight in kg for scale-sold items. When set and > 0, overrides Quantity in price calculations.</summary>
    public double? Peso   { get; set; }

    // ─── Computed pricing — set by PricingEngine, never inline ───────────────

    /// <summary>Line subtotal before tax, with any promotions applied.</summary>
    public decimal Subtotal  { get; set; }

    /// <summary>Tax amount for this line (Subtotal × Tax%).</summary>
    public decimal TaxAmount { get; set; }

    /// <summary>Line total including tax (Subtotal + TaxAmount).</summary>
    public decimal Total     { get; set; }

    // ─── Utility ─────────────────────────────────────────────────────────────

    public CartItem Clone() => new()
    {
        Number         = Number,
        ProductId      = ProductId,
        ProductName    = ProductName,
        Quantity       = Quantity,
        Weight         = Weight,
        UnitPrice      = UnitPrice,
        Tax            = Tax,
        Date           = Date,
        PromotionName  = PromotionName,
        PromotionValue = PromotionValue,
        PromotionLimit = PromotionLimit,
        IsEBT          = IsEBT,
        UPC            = UPC,
        Image          = Image,
        Peso           = Peso,
        Subtotal       = Subtotal,
        TaxAmount      = TaxAmount,
        Total          = Total
    };
}
