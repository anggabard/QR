using QR_Generator.Constants.Enums;
using QR_Generator.Helper;
using QR_Generator.Models;
using QR_Generator.Services.Startup;

namespace QR_Generator.Services;

public class CharacterCapacitiesService : StartupService
{
    private Dictionary<EncodingMode, EncodingModeCapatity> CharacterCapacities = [];

    public override void Initialize()
    {
        CharacterCapacities = new Dictionary<EncodingMode, EncodingModeCapatity>
        {
            { EncodingMode.Numeric, JsonHelper.LoadFromAssembly<EncodingModeCapatity>("QR_Generator.Config.CharacterCapacities.Numeric.json") },
            { EncodingMode.Alphanumeric, JsonHelper.LoadFromAssembly<EncodingModeCapatity>("QR_Generator.Config.CharacterCapacities.Alphanumeric.json") },
            { EncodingMode.Byte, JsonHelper.LoadFromAssembly<EncodingModeCapatity>("QR_Generator.Config.CharacterCapacities.Byte.json") },
            { EncodingMode.Kanji, JsonHelper.LoadFromAssembly<EncodingModeCapatity>("QR_Generator.Config.CharacterCapacities.Kanji.json") }
        };
    }

    public (int version, ErrorCorrectionLevel ecl) GetMinVersionAndMaxErrorCorrection(EncodingMode encodingMode, int messageLength)
    {
        var version = 0;
        foreach (var value in CharacterCapacities[encodingMode].L)
        {
            if (messageLength < value)
                break;
            version++;
        }

        var ecl = ErrorCorrectionLevel.L;
        var capacities = CharacterCapacities[encodingMode];

        foreach (var (level, capacity) in new[] {
                  (ErrorCorrectionLevel.M, capacities.M),
                  (ErrorCorrectionLevel.Q, capacities.Q),
                  (ErrorCorrectionLevel.H, capacities.H)})
        {
            if (messageLength < capacity[version])
            {
                ecl = level;
            }
            else
            {
                break;
            }
        }

        return (version, ecl);
    }
}
