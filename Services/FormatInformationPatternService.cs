using QR_Generator.Constants.Enums;
using QR_Generator.Extensions;
using QR_Generator.Helper;
using QR_Generator.Services.Startup;

namespace QR_Generator.Services;

public class FormatInformationPatternService : StartupService
{
    private static Dictionary<ErrorCorrectionLevel, Dictionary<Masks, string>> FormatInformations = [];

    public override void Initialize()
    {
        FormatInformations = JsonHelper.LoadFromAssembly<Dictionary<ErrorCorrectionLevel, Dictionary<Masks, string>>>("QR_Generator.Config.Format.FormatInformationPattern.json");
    }

    public static List<byte> Get(ErrorCorrectionLevel ecl, Masks mask)
    {
        return FormatInformations[ecl][mask].GetBytesFromString();
    }
}
