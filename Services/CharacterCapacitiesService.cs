using QR_Generator.Constants.Enums;
using QR_Generator.Models;
using System.Reflection;
using System.Text.Json;

namespace QR_Generator.Services;

public class CharacterCapacitiesService
{
    private Dictionary<EncodingMode, EncodingModeCapatity> CharacterCapacities;

    public void Initialize()
    {
        CharacterCapacities = new Dictionary<EncodingMode, EncodingModeCapatity>
        {
            { EncodingMode.Numeric, LoadJson("QR_Generator.Config.CharacterCapacities.Numeric.json") },
            { EncodingMode.Alphanumeric, LoadJson("QR_Generator.Config.CharacterCapacities.Alphanumeric.json") },
            { EncodingMode.Byte, LoadJson("QR_Generator.Config.CharacterCapacities.Byte.json") },
            { EncodingMode.Kanji, LoadJson("QR_Generator.Config.CharacterCapacities.Kanji.json") }
        };
    }

    private EncodingModeCapatity LoadJson(string resourceName)
    {
        // Get the current assembly
        var assembly = Assembly.GetExecutingAssembly();

        // Load the embedded resource
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");
        }

        using var reader = new StreamReader(stream);
        var jsonContent = reader.ReadToEnd();

        // Deserialize the JSON content into the specified type
        var result = JsonSerializer.Deserialize<EncodingModeCapatity>(jsonContent);
        if (result == null)
        {
            throw new FileNotFoundException($"Assembly not found: {resourceName}");
        }

        return result;
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
