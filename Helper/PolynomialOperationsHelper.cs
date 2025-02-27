namespace QR_Generator.Helper;

public class PolynomialOperationsHelper
{
    public static byte[] PolyMul(byte[] poly1, byte[] poly2)
    {
        byte[] coeffs = new byte[poly1.Length + poly2.Length - 1];

        for (int index = 0; index < coeffs.Length; index++)
        {
            byte coeff = 0;
            for (int p1index = 0; p1index <= index; p1index++)
            {
                int p2index = index - p1index;
                coeff ^= GF256Helper.Mul(GetValueOrDefault(poly1, p1index), GetValueOrDefault(poly2, p2index));
            }
            coeffs[index] = coeff;
        }
        return coeffs;
    }

    public static byte[] PolyRest(byte[] dividend, byte[] divisor)
    {
        int quotientLength = dividend.Length - divisor.Length + 1;
        byte[] rest = (byte[])dividend.Clone();

        for (int count = 0; count < quotientLength; count++)
        {
            if (rest[0] != 0)
            {
                byte factor = GF256Helper.Div(rest[0], divisor[0]);
                byte[] subtr = new byte[rest.Length];
                Array.Copy(PolyMul(divisor, new byte[] { factor }), 0, subtr, 0, divisor.Length);
                rest = rest.Select((value, index) => (byte)(value ^ subtr[index])).ToArray().Skip(1).ToArray();
            }
            else
            {
                rest = rest.Skip(1).ToArray();
            }
        }

        return rest;
    }

    public static byte[] GetGeneratorPoly(int degree)
    {
        byte[] lastPoly = [1];

        for (byte index = 0; index < degree; index++)
        {
            byte[] secondPoly = [1, GF256Helper.GetExp(index)];
            lastPoly = PolyMul(lastPoly, secondPoly);
        }

        return lastPoly;
    }

    private static byte GetValueOrDefault(byte[] array, int index)
    {
        if (index >= 0 && index < array.Length)
        {
            return array[index];
        }
        return 0;
    }
}
