using QR_Generator.Constants.Enums.Matrix;

namespace QR_Generator.Extensions;

public static class ColorExtensions
{
    public static byte? ToNullableByte(this Color color)
    {
        if (Enum.IsDefined(typeof(Color), color)){
            return (byte)color;
        }

        return null;
    }
}
