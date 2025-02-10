using QR_Generator.Constants.Enums;

namespace QR_Generator.Helper;

public class LengthBitsHelper
{
    private static readonly Dictionary<EncodingMode, int[]> LengthBits = new()
    {
    {EncodingMode.Numeric, [10, 12, 14]},
    {EncodingMode.Alphanumeric,  [9, 11, 13]},
    {EncodingMode.Byte,  [8, 16, 16]},
    {EncodingMode.Kanji,  [8, 10, 12]}
    };

    public static int GetLengthBits(EncodingMode mode, int version)
    {
        int bitsIndex = version > 26 ? 2 : version > 9 ? 1 : 0;

        return LengthBits[mode][bitsIndex];
    }
}
