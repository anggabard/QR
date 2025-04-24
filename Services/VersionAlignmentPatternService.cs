using QR_Generator.Helper;
using QR_Generator.Services.Startup;

namespace QR_Generator.Services;

public class VersionAlignmentPatternService : StartupService
{
    private static Dictionary<int, List<int>> AlignmentPatternsCoordonates = [];
    private static readonly Dictionary<int, List<(int, int)>> AlignmentPatternsPositions = [];

    public override void Initialize()
    {
        AlignmentPatternsCoordonates = JsonHelper.LoadFromAssembly<Dictionary<int, List<int>>>("QR_Generator.Config.Version.VersionAlignmentPattern.json");

        AlignmentPatternsPositions[1] = [];//No Alignment patterns for V1
        foreach (var kvp in AlignmentPatternsCoordonates)
        {
            AlignmentPatternsPositions[kvp.Key] = [];
            foreach (int centerRow in kvp.Value)
            {
                foreach (int centerCol in kvp.Value)
                {
                    // Skip Finder Pattern areas
                    if (!((centerRow == 6 && centerCol == 6) ||
                          (centerRow == 6 && centerCol == kvp.Value[^1]) ||
                          (centerRow == kvp.Value[^1] && centerCol == 6)))
                    {
                        AlignmentPatternsPositions[kvp.Key].Add((centerRow, centerCol));
                    }
                }
            }
        }
    }

    public static List<(int, int)>? Get(int version)
    {
        if (version < 1 || version > 40)
        {
            return null;
        }

        return AlignmentPatternsPositions[version];
    }
}
