namespace QR_Generator.Extensions;

public static class StringExtension
{
    public static List<byte> GetBytesFromString(this string binaryString)
    {
        return binaryString.Select(bitChar =>
        {
            if (bitChar == '0') return (byte)0;
            if (bitChar == '1') return (byte)1;
            throw new ArgumentException($"Encountered invalid character: {bitChar}.");
        }).ToList();
    }
}
