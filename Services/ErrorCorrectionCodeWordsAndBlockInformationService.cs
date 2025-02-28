using QR_Generator.Constants.Enums;
using QR_Generator.Helper;
using QR_Generator.Models;
using QR_Generator.Services.Base;

namespace QR_Generator.Services;

public class ErrorCorrectionCodeWordsAndBlockInformationService : StartupService
{
    private Dictionary<int, Dictionary<ErrorCorrectionLevel, DataCodewordsRaw>> VersionDataCodewords { get; set; }

    public override void Initialize()
    {
        VersionDataCodewords = JsonHelper.LoadFromAssembly<Dictionary<int, Dictionary<ErrorCorrectionLevel, DataCodewordsRaw>>>("QR_Generator.Config.CodeWordsBlockInformation.ErrorCorrectionCodeWordsAndBlockInformation.json");
    }

    private static void CheckVersion(int version)
    {
        if (version < 1 || version > 40)
        {
            throw new ArgumentException("Version must be between 1 and 40");
        }
    }

    private DataCodewordsInfo ConvertToInfo(DataCodewordsRaw dataCodewordsRaw)
    {
        var info = new DataCodewordsInfo
        {
            ECCodewordsPerBlock = dataCodewordsRaw.ECCodewordsPerBlock,
            Group1 = new Group
            {
                NumberOfBlocks = dataCodewordsRaw.NumberOfBlocksInGroup1,
                NumberOfDataCodewordsPerBlock = dataCodewordsRaw.NumberOfDataCodewordsInEachOfGroup1Blocks,
            },
        };

        if (dataCodewordsRaw.NumberOfBlocksInGroup2 != null && dataCodewordsRaw.NumberOfDataCodewordsInEachOfGroup2Blocks != null)
        {
            info.Group2 = new Group
            {
                NumberOfBlocks = (int)dataCodewordsRaw.NumberOfBlocksInGroup2,
                NumberOfDataCodewordsPerBlock = (int)dataCodewordsRaw.NumberOfDataCodewordsInEachOfGroup2Blocks
            };
        }

        return info;
    }

    public DataCodewordsInfo Get(int version, ErrorCorrectionLevel errorCorrectionLevel)
    {
        CheckVersion(version);
        return ConvertToInfo(VersionDataCodewords[version][errorCorrectionLevel]);
    }
}
