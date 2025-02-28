using QR_Generator.Constants.Enums;

namespace QR_Generator.Extensions;

public static class ByteListExtension
{
    public static void AddNumber(this List<byte> list, int padding, int value)
    {
        for (int i = padding - 1; i >= 0; i--)
        {
            list.Add((byte)(value >> i & 1));
        }
    }

    public static void AddByteArray(this List<byte> list, byte[] bytes)
    {
        foreach (var codeword in bytes)
        {
            list.AddNumber(8, codeword);
        }
    }

    public static void AddEncodingMode(this List<byte> list, EncodingMode encodingMode)
    {
        for (int i = 4 - 1; i >= 0; i--)
        {
            list.Add((byte)((int)encodingMode >> i & 1));
        }
    }

    public static List<byte> GetByteList(this List<byte> list)
    {
        if (list == null)
        {
            throw new ArgumentNullException(nameof(list));
        }

        if (list.Count % 8 != 0)
        {
            throw new ArgumentException("The bit list length must be a multiple of 8.", nameof(list));
        }

        var byteList = new List<byte>();

        for (int i = 0; i < list.Count; i += 8)
        {
            byte currentByte = 0;
            for (int j = 0; j < 8; j++)
            {
                currentByte = (byte)((currentByte << 1) | list[i + j]);
            }
            byteList.Add(currentByte);
        }

        return byteList;
    }
}
