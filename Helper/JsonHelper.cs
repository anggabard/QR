using System.Reflection;
using System.Text.Json;

namespace QR_Generator.Helper;

public class JsonHelper
{
    public static T LoadFromAssembly<T>(string resourceName)
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
        var result = JsonSerializer.Deserialize<T>(jsonContent);
        if (result == null)
        {
            throw new FileNotFoundException($"Assembly not found: {resourceName}");
        }

        return result;
    }
}
