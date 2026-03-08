using System.Text;

namespace OmadaPOS.Impresora;

/// <summary>
/// Fluent builder for ESC/POS byte streams compatible with Epson TM, Star,
/// Bixolon and all standard 80 mm thermal receipt printers.
/// 
/// Usage:
///   byte[] bytes = new EscPosBuilder()
///       .Init()
///       .Center().Bold(true).DoubleSize(true).Line("MY STORE")
///       .NormalStyle().Left()
///       .TwoCol("Total:", "$12.50")
///       .Cut()
///       .Build();
///   RawPrinterHelper.SendBytesToPrinter(printerName, bytes);
/// </summary>
public sealed class EscPosBuilder
{
    // ── Encoding ──────────────────────────────────────────────────────────────
    // Latin-1 (ISO-8859-1) — always available in .NET, accepted by all ESC/POS printers
    private static readonly Encoding Cp437 = Encoding.Latin1;

    // ── Paper geometry ────────────────────────────────────────────────────────
    /// <summary>Characters per line at Font A (12-dot) on 80 mm paper.</summary>
    public const int W = 48;

    // ── Internal buffer ───────────────────────────────────────────────────────
    private readonly List<byte> _buf = new(512);

    // ══════════════════════════════════════════════════════════════════════════
    //  LIFECYCLE
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>Initialize printer — always call first.</summary>
    public EscPosBuilder Init()
    {
        Emit(0x1B, 0x40);   // ESC @
        // Select code page 437
        Emit(0x1B, 0x74, 0x00);
        return this;
    }

    /// <summary>Feed n lines and partial-cut.</summary>
    public EscPosBuilder Cut(int feedLines = 5)
    {
        Feed(feedLines);
        Emit(0x1D, 0x56, 0x42, 0x00);  // GS V B — partial cut
        return this;
    }

    /// <summary>Feed n lines.</summary>
    public EscPosBuilder Feed(int lines = 1)
    {
        Emit(0x1B, 0x64, (byte)Math.Clamp(lines, 1, 255));
        return this;
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  JUSTIFICATION
    // ══════════════════════════════════════════════════════════════════════════

    public EscPosBuilder Left()   { Emit(0x1B, 0x61, 0x00); return this; }
    public EscPosBuilder Center() { Emit(0x1B, 0x61, 0x01); return this; }
    public EscPosBuilder Right()  { Emit(0x1B, 0x61, 0x02); return this; }

    // ══════════════════════════════════════════════════════════════════════════
    //  TEXT STYLE
    // ══════════════════════════════════════════════════════════════════════════

    public EscPosBuilder Bold(bool on)
    {
        Emit(0x1B, 0x45, on ? (byte)1 : (byte)0);  // ESC E
        return this;
    }

    /// <summary>
    /// Character magnification.
    /// <paramref name="w"/>: 1–8 × horizontal, <paramref name="h"/>: 1–8 × vertical.
    /// </summary>
    public EscPosBuilder CharSize(int w = 1, int h = 1)
    {
        byte n = (byte)(((Math.Clamp(w, 1, 8) - 1) << 4) | (Math.Clamp(h, 1, 8) - 1));
        Emit(0x1D, 0x21, n);   // GS !
        return this;
    }

    /// <summary>Reset all text styles to default.</summary>
    public EscPosBuilder NormalStyle()
    {
        Bold(false);
        Emit(0x1D, 0x21, 0x00);
        return this;
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  TEXT OUTPUT
    // ══════════════════════════════════════════════════════════════════════════

    public EscPosBuilder Text(string text)
    {
        _buf.AddRange(Cp437.GetBytes(Sanitize(text)));
        return this;
    }

    /// <summary>Write text followed by LF.</summary>
    public EscPosBuilder Line(string text = "")
    {
        Text(text);
        Emit(0x0A);
        return this;
    }

    /// <summary>Line feed only.</summary>
    public EscPosBuilder NewLine() { Emit(0x0A); return this; }

    // ══════════════════════════════════════════════════════════════════════════
    //  FORMATTED LINES
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>Full-width separator line.</summary>
    public EscPosBuilder Separator(char ch = '-')
        => Line(new string(ch, W));

    /// <summary>
    /// Two-column line: left text + right-aligned right text padded to <see cref="W"/>.
    /// </summary>
    public EscPosBuilder TwoCol(string left, string right, int width = W)
    {
        left  = Truncate(left,  width - 2);
        right = Truncate(right, width - 2);
        int spaces = Math.Max(1, width - left.Length - right.Length);
        return Line(left + new string(' ', spaces) + right);
    }

    /// <summary>
    /// Three-column line: left | center (padded) | right-aligned.
    /// </summary>
    public EscPosBuilder ThreeCol(string left, string mid, string right, int width = W)
    {
        left  = Truncate(left,  14);
        right = Truncate(right, 10);
        int remaining = width - left.Length - right.Length;
        string midPad = Truncate(mid, remaining - 2).PadLeft(remaining / 2 + mid.Length / 2)
                                                     .PadRight(remaining);
        return Line(left + midPad + right);
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  QR CODE  (ESC/POS model 2 — Epson/Star/Bixolon compatible)
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Print a QR code using native ESC/POS GS ( k commands.
    /// <paramref name="moduleSize"/>: 3–8 (dots per module; 5 = ~2 cm at 203 DPI).
    /// Error correction level M (recovers 15 % damage).
    /// </summary>
    public EscPosBuilder QrCode(string data, int moduleSize = 5)
    {
        byte[] payload = Encoding.ASCII.GetBytes(data);
        int    storeLen = payload.Length + 3;
        byte   pL = (byte)(storeLen & 0xFF);
        byte   pH = (byte)((storeLen >> 8) & 0xFF);

        // Select model 2
        Emit(0x1D, 0x28, 0x6B, 0x04, 0x00, 0x31, 0x41, 0x32, 0x00);
        // Module size
        Emit(0x1D, 0x28, 0x6B, 0x03, 0x00, 0x31, 0x43, (byte)Math.Clamp(moduleSize, 1, 8));
        // Error correction: M (0x31)
        Emit(0x1D, 0x28, 0x6B, 0x03, 0x00, 0x31, 0x45, 0x31);
        // Store data
        Emit(0x1D, 0x28, 0x6B, pL, pH, 0x31, 0x50, 0x30);
        _buf.AddRange(payload);
        // Print
        Emit(0x1D, 0x28, 0x6B, 0x03, 0x00, 0x31, 0x51, 0x30);
        return this;
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  BARCODE — CODE 128
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Print a CODE 128 barcode.
    /// <paramref name="height"/>: bar height in dots (default 64 ≈ 8 mm).
    /// <paramref name="width"/>: bar module width 2–6 (default 3).
    /// HRI (human-readable) printed below.
    /// </summary>
    public EscPosBuilder Barcode128(string data, int height = 60, int width = 2)
    {
        byte[] payload = Encoding.ASCII.GetBytes(data);

        Emit(0x1D, 0x68, (byte)Math.Clamp(height, 1, 255));    // GS h — height
        Emit(0x1D, 0x77, (byte)Math.Clamp(width,  2, 6));      // GS w — width
        Emit(0x1D, 0x48, 0x02);                                 // GS H — HRI below
        Emit(0x1D, 0x66, 0x01);                                 // GS f — HRI font B (smaller)
        // GS k 73 n [data]  — CODE128 (function code 73 = 0x49)
        Emit(0x1D, 0x6B, 0x49, (byte)payload.Length);
        _buf.AddRange(payload);
        Emit(0x0A);
        return this;
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  BUILD
    // ══════════════════════════════════════════════════════════════════════════

    public byte[] Build() => [.. _buf];

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void Emit(params byte[] bytes) => _buf.AddRange(bytes);

    /// <summary>Replace characters outside CP437 to avoid garbled output.</summary>
    private static string Sanitize(string s)
        => string.Concat(s.Select(c => c < 128 || (c >= 160 && c <= 255) ? c : '?'));

    private static string Truncate(string s, int max)
        => s.Length > max ? s[..max] : s;
}
