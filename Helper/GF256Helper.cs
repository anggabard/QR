using QR_Generator.Services.Startup;

namespace QR_Generator.Helper;

public class GF256Helper : StartupService
{
    private readonly static byte[] LOG = new byte[256];
    private readonly static byte[] EXP = new byte[256];

    public override void Initialize()
    {
        int value = 1;
        EXP[0] = 1;
        for (int exponent = 1; exponent < 256; exponent++)
        {
            value = value > 127 ? ((value << 1) ^ 285) : value << 1;
            LOG[value] = (byte)(exponent % 255);
            EXP[exponent % 255] = (byte)value;
        }
    }

    public static byte GetExp(byte value) { return EXP[value]; }
    public static byte GetLog(byte value) { return LOG[value]; }

    public static byte Mul(byte a, byte b)
    {
        if (a == 0 || b == 0)
        {
            return 0;
        }

        int logA = LOG[a];
        int logB = LOG[b];
        int logResult = (logA + logB) % 255;
        return EXP[logResult];
    }

    public static byte Div(byte a, byte b)
    {
        if (b == 0)
        {
            throw new DivideByZeroException("Division by zero in Galois field.");
        }
        if (a == 0)
        {
            return 0;
        }

        return EXP[(LOG[a] + LOG[b] * 254) % 255];
    }
}
