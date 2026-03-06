namespace OmadaPOS.Domain;

/// <summary>
/// Parses GS1-128 weight-embedded barcodes used by retail scales.
/// Barcodes starting with "20" encode the PLU code and the price in cents.
/// Previously embedded as inline string operations inside frmHome.SearchProduct().
/// </summary>
public static class WeightBarcodeParser
{
    /// <summary>
    /// Returns true when the UPC represents a scale/weight-embedded barcode.
    /// These barcodes begin with the prefix "20".
    /// </summary>
    public static bool IsWeightBarcode(string upc) =>
        upc.Length > 2 && upc.Substring(0, 2) == "20";

    /// <summary>
    /// Extracts the PLU code (digits 1–5) and the price in dollars (digits 7–10, in cents)
    /// from a weight-embedded barcode.
    /// Returns false if the barcode is shorter than 12 characters.
    /// </summary>
    public static bool TryParse(string upc, out string pluCode, out decimal price)
    {
        pluCode = string.Empty;
        price = 0m;

        if (upc.Length <= 11)
            return false;

        string sPrice = upc.Substring(7, 4);
        price = decimal.Parse(sPrice) / 100;
        pluCode = upc.Substring(1, 5);
        return true;
    }
}
