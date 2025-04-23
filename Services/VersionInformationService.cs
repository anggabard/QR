using QR_Generator.Helper;
using QR_Generator.Services.Base;

namespace QR_Generator.Services;

public class VersionInformationService : StartupService
{
    private static Dictionary<int, List<byte>> VersionInformationCode;

    public override void Initialize()
    {
        VersionInformationCode = JsonHelper.LoadFromAssembly<Dictionary<int, string>>("QR_Generator.Config.VersionInformation.VersionInformation.json")
                                        .ToDictionary(
                                            kvp => kvp.Key,
                                            kvp => GetValueFromString(kvp.Value)
                                        );
    }

    public static List<byte>? Get(int version)
    {
        if (version < 7 || version > 40)
        {
            return null;
        }

        return VersionInformationCode[version];
    }

    private static List<byte> GetValueFromString(string binaryString)
    {
        return binaryString.Select(bitChar =>
        {
            if (bitChar == '0') return (byte)0;
            if (bitChar == '1') return (byte)1;
            throw new ArgumentException($"Encountered invalid character: {bitChar}.");
        }).ToList();
    }
}
