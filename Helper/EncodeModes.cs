using QR_Generator.Constants;
using QR_Generator.Constants.RegexPatterns;

namespace QR_Generator.Helper;

public class EncodeModes
{
    public static EncodingMode GetEncodingMode(string input)
    {
        return input switch
        {
            null => throw new NullReferenceException(),
            _ when RegexPatterns.NumericRegex().IsMatch(input) => EncodingMode.Numeric,
            _ when RegexPatterns.AlphanumericRegex().IsMatch(input) => EncodingMode.Alphanumeric,
            _ when RegexPatterns.Latin1Regex().IsMatch(input) => EncodingMode.Byte,
            _ when RegexPatterns.KanjiRegex().IsMatch(input) => EncodingMode.Kanji,
            _ => EncodingMode.ECI // Default mode for unmatched input
        };
    }
}
