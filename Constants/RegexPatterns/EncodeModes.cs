using System.Text.RegularExpressions;

namespace QR_Generator.Constants.RegexPatterns;

public static partial class RegexPatterns
{
    [GeneratedRegex(@"^\d*$", RegexOptions.CultureInvariant | RegexOptions.Compiled)]
    public static partial Regex NumericRegex();

    [GeneratedRegex(@"^[\dA-Z $%*+\-./:]*$", RegexOptions.CultureInvariant | RegexOptions.Compiled)]
    public static partial Regex AlphanumericRegex();

    [GeneratedRegex(@"^[\x00-\xff]*$", RegexOptions.CultureInvariant | RegexOptions.Compiled)]
    public static partial Regex Latin1Regex();

    [GeneratedRegex(@"\p{IsCJKUnifiedIdeographs}|\p{IsHiragana}|\p{IsKatakana}*$", RegexOptions.CultureInvariant | RegexOptions.Compiled)]
    public static partial Regex KanjiRegex();
}
