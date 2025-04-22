using QR_Generator.Constants.Dictionaries;
using QR_Generator.Constants.Enums;
using QR_Generator.Extensions;
using QR_Generator.Models;
using System.Text;

namespace QR_Generator.QRData;

public class BitData
{
    private readonly List<byte> TempData = [];
    private readonly List<byte> Data = [];

    public BitData(EncodingMode encodingMode, int lengthBits, string message, DataCodewordsInfo dataCodewordsInfo)
    {
        TempData.AddEncodingMode(encodingMode);
        TempData.AddNumber(lengthBits, message.Length);//AddMessageLenght
        AddMesage(encodingMode, message);
        AddTerminationBlock(dataCodewordsInfo.TotalDataCodewords);
        FillRemainingCodewords(dataCodewordsInfo.TotalDataCodewords);

        AddErrorCorrection(dataCodewordsInfo);
    }

    public List<byte> GetData() { return Data; }

    private void FillRemainingCodewords(int totalDataCodewords)
    {
        byte[] n_236 = [1, 1, 1, 0, 1, 1, 0, 0];
        byte[] n_17 = [0, 0, 0, 1, 0, 0, 0, 1];

        var remainingCodewords = totalDataCodewords - TempData.Count / 8;
        if (remainingCodewords == 0)
            return;

        for (int i = 1; i <= remainingCodewords; i++)
        {
            if (i % 2 == 0)
            {
                TempData.AddRange(n_17);
            }
            else
            {
                TempData.AddRange(n_236);
            }
        }
    }

    private void AddTerminationBlock(int totalDataCodewords)
    {
        var remainingBits = totalDataCodewords * 8 - TempData.Count;
        var count = remainingBits > 4 ? 4 : remainingBits;
        if (count == 0)
            return;

        for (int i = 0; i < count; i++)
        {
            TempData.Add(0);
        }

        if (totalDataCodewords * 8 - TempData.Count > 0 && TempData.Count % 8 != 0)
        {
            var remainingZeros = 8 - TempData.Count % 8;
            for (int i = 0; i < remainingZeros; i++)
            {
                TempData.Add(0);
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
                TempData.AddNumber(10, group);
            }
            else if (group < 10)
            {
                TempData.AddNumber(4, group);
            }
            else
            {
                TempData.AddNumber(7, group);
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

            TempData.AddNumber(padding, value);
        }
    }

    private void AddByteMessage(string message)
    {
        foreach (char c in message)
        {
            string binaryString = Convert.ToString(c, 2).PadLeft(8, '0');

            foreach (char bit in binaryString)
            {
                TempData.Add((byte)(bit - '0'));
            }
        }
    }

    public List<byte> XorBitLists(List<byte> mask)
    {
        List<byte> result = [];

        for (int i = 0; i < mask.Count; i++)
        {
            result.Add((byte)(TempData[i] ^ mask[i]));
        }

        return result;
    }

    private void AddErrorCorrection(DataCodewordsInfo dataCodewordsInfo)
    {
        var dataBlocks = SplitIntoBlocks(dataCodewordsInfo);
        var errorCorrectionBlocks = dataBlocks.Select(block => block.EDC(dataCodewordsInfo.ECCodewordsPerBlock));
        MergeInterleaved(dataBlocks, errorCorrectionBlocks);
    }

    private List<byte[]> SplitIntoBlocks(DataCodewordsInfo dataCodewordsInfo)
    {
        var data = TempData.GetByteList().ToArray();
        List<byte[]> blocks = [];

        var skip = 0;
        var take = dataCodewordsInfo.Group1.NumberOfDataCodewordsPerBlock;
        for (int i = 0; i < dataCodewordsInfo.Group1.NumberOfBlocks; i++)
        {
            blocks.Add(data.Skip(skip).Take(take).ToArray());
            skip += take;
        }

        if (dataCodewordsInfo.Group2 != null)
        {
            take = dataCodewordsInfo.Group2.NumberOfDataCodewordsPerBlock;
            for (int i = 0; i < dataCodewordsInfo.Group2.NumberOfBlocks; i++)
            {
                blocks.Add(data.Skip(skip).Take(take).ToArray());
                skip += take;
            }
        }

        return blocks;
    }

    private void MergeInterleaved(List<byte[]> dataBlocks, IEnumerable<byte[]> errorCorrectionBlocks)
    {
        List<byte> mergedData = [];

        int maxDataBlockSize = dataBlocks.Max(b => b.Length);
        for (int i = 0; i < maxDataBlockSize; i++)
        {
            foreach (var block in dataBlocks)
            {
                if (i < block.Length)
                    mergedData.Add(block[i]);
            }
        }

        int maxECBlockSize = errorCorrectionBlocks.Max(b => b.Length);
        for (int i = 0; i < maxECBlockSize; i++)
        {
            foreach (var block in errorCorrectionBlocks)
            {
                if (i < block.Length)
                    mergedData.Add(block[i]);
            }
        }

        Data.AddByteArray([.. mergedData]);
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
