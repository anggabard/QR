using QR_Generator.Extensions;
using QR_Generator.Helper;
using QR_Generator.Services.Startup;

namespace QR_Generator.Services;

public class VersionInformationPatternService : StartupService
{
    private static Dictionary<int, List<byte>> InformationPatterns = [];

    public override void Initialize()
    {
        InformationPatterns = JsonHelper.LoadFromAssembly<Dictionary<int, string>>("QR_Generator.Config.Version.VersionInformationPattern.json")
                                        .ToDictionary(
                                            kvp => kvp.Key,
                                            kvp => kvp.Value.GetBytesFromString()
                                        );
    }

    public static List<byte>? Get(int version)
    {
        if (version < 7 || version > 40)
        {
            return null;
        }

        return InformationPatterns[version];
    }

    public static Dictionary<int, List<(int, int)>> GetPatternPositions(int matrixSize)
    {
        var last = matrixSize - 1;
        var result = new Dictionary<int, List<(int, int)>>();
        for (var i = 0; i <= 5; i++)
        {
            result[i] = [(8, i), (last - i, 8)];
        }

        result[6] = [(8, 7), (last - 6, 8)];
        result[7] = [(8, 8), (8, last - 7)];
        result[8] = [(7, 8), (8, last - 6)];

        for (var i = 5; i >= 0; i--)
        {
            result[14 - i] = [(i, 8), (8, last - i)];
        }

        return result;
    }
}
