using QR_Generator.Constants.Dictionaries;
using QR_Generator.Constants.Enums;
using QR_Generator.Helper;
using System.Text;

namespace QR_Generator.QRData;

public class BitData
{
    private readonly List<byte> Data = [];
    private readonly byte[] n_236 = [1, 1, 1, 0, 1, 1, 0, 0];
    private readonly byte[] n_17 = [0, 0, 0, 1, 0, 0, 0, 1];

    public BitData(EncodingMode encodingMode, int lengthBits, string message, int totalDataCodewords, int ECCodewordsPerBlock)
    {
        AddEncodingMode(encodingMode);
        AddNumber(lengthBits, message.Length);//AddMessageLenght
        AddMesage(encodingMode, message);
        AddTerminationBlock(totalDataCodewords);
        FillRemainingCodewords(totalDataCodewords);
        AddEDC(ECCodewordsPerBlock);
    }

    private void FillRemainingCodewords(int totalDataCodewords)
    {
        var remainingCodewords = totalDataCodewords - Data.Count / 8;
        if (remainingCodewords == 0)
            return;

        for (int i = 1; i <= remainingCodewords; i++)
        {
            if (i % 2 == 0)
            {
                Data.AddRange(n_17);
            }
            else
            {
                Data.AddRange(n_236);
            }
        }
    }

    private void AddTerminationBlock(int totalDataCodewords)
    {
        var remainingBits = totalDataCodewords * 8 - Data.Count;
        var count = remainingBits > 4 ? 4 : remainingBits;
        if (count == 0)
            return;

        for (int i = 0; i < count; i++)
        {
            Data.Add(0);
        }

        if (totalDataCodewords * 8 - Data.Count > 0 && Data.Count % 8 != 0)
        {
            var remainingZeros = 8 - Data.Count % 8;
            for (int i = 0; i < remainingZeros; i++)
            {
                Data.Add(0);
            }
        }
    }

    private void AddMesage(EncodingMode encodingMode, string message)
    {
        switch (encodingMode)
        {
            case EncodingMode.Numeric: AddNumericMessage(message); break;
            case EncodingMode.Alphanumeric: AddAlphanumericMessage(message); break;
            case EncodingMode.Byte: AddByteMessage(message); break;
            case EncodingMode.Kanji: throw new NotImplementedException();
            default: throw new NotImplementedException();
        }
    }

    private void AddNumericMessage(string message)
    {
        for (int i = 0; i < message.Length; i += 3)
        {
            var group = int.Parse(message.Substring(i, Math.Min(3, message.Length - i)));
            if (group > 99)
            {
                AddNumber(10, group);
            }
            else if (group < 10)
            {
                AddNumber(4, group);
            }
            else
            {
                AddNumber(7, group);
            }
        }
    }

    private void AddAlphanumericMessage(string message)
    {
        for (int i = 0; i < message.Length; i += 2)
        {
            var group = message.Substring(i, Math.Min(2, message.Length - i));
            int value, padding;

            if (group.Length == 2)
            {
                value = AlphanumericMaping.Convert[group[0]] * 45 + AlphanumericMaping.Convert[group[1]];
                padding = 11;
            }
            else
            {
                value = AlphanumericMaping.Convert[group[0]];
                padding = 6;
            }

            AddNumber(padding, value);
        }
    }

    private void AddByteMessage(string message)
    {
        foreach (char c in message)
        {
            string binaryString = Convert.ToString(c, 2).PadLeft(8, '0');

            foreach (char bit in binaryString)
            {
                Data.Add((byte)(bit - '0'));
            }
        }
    }

    private void AddNumber(int padding, int value)
    {
        for (int i = padding - 1; i >= 0; i--)
        {
            Data.Add((byte)(value >> i & 1));
        }
    }

    private void AddEncodingMode(EncodingMode encodingMode)
    {
        for (int i = 4 - 1; i >= 0; i--)
        {
            Data.Add((byte)((int)encodingMode >> i & 1));
        }
    }

    public List<byte> XorBitLists(List<byte> mask)
    {
        List<byte> result = [];

        for (int i = 0; i < mask.Count; i++)
        {
            result.Add((byte)(Data[i] ^ mask[i]));
        }

        return result;
    }

    public List<byte> GetByteList()
    {
        if (Data == null)
        {
            throw new ArgumentNullException(nameof(Data));
        }

        if (Data.Count % 8 != 0)
        {
            throw new ArgumentException("The bit list length must be a multiple of 8.", nameof(Data));
        }

        var byteList = new List<byte>();

        for (int i = 0; i < Data.Count; i += 8)
        {
            byte currentByte = 0;
            for (int j = 0; j < 8; j++)
            {
                currentByte = (byte)((currentByte << 1) | Data[i + j]);
            }
            byteList.Add(currentByte);
        }

        return byteList;
    }

    public void AddEDC(int ECCodewordsPerBlock)
    {
        var polyCoef = GetByteList().ToArray();
        byte[] messagePoly = new byte[Data.Count / 8 + ECCodewordsPerBlock];
        Array.Copy(polyCoef, messagePoly, polyCoef.Length);

        var EDC = PolynomialOperationsHelper.PolyRest(messagePoly, PolynomialOperationsHelper.GetGeneratorPoly(ECCodewordsPerBlock));
        foreach (var codeword in EDC)
        {
            AddNumber(8, codeword);
        }
    }
    public override string ToString()
    {
        StringBuilder sb = new();

        for (int i = 0; i < Data.Count; i++)
        {
            sb.Append(Data[i]);

            if ((i + 1) % 8 == 0 && i != Data.Count - 1)
            {
                sb.Append(' ');
            }

            if ((i + 1) % 40 == 0 && i != Data.Count - 1)
            {
                sb.Append('\n');
            }

        }

        return sb.ToString();
    }

}
