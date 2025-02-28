using QR_Generator.Helper;

namespace QR_Generator.Extensions;

public static class ByteArrayExtension
{
    public static byte[] EDC(this byte[] data, int ECCodewordsPerBlock)
    {
        byte[] messagePoly = new byte[data.Length + ECCodewordsPerBlock];
        Array.Copy(data, messagePoly, data.Length);

        return PolynomialOperationsHelper.PolyRest(messagePoly, PolynomialOperationsHelper.GetGeneratorPoly(ECCodewordsPerBlock));
    }
}
