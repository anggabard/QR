namespace QR_Generator.Models;

public class DataCodewordsInfo
{
    public int ECCodewordsPerBlock { get; set; }
    public Group Group1 { get; set; }
    public Group? Group2 { get; set; }
    public int TotalDataCodewords
    {
        get
        {
            var sum = Group1.NumberOfBlocks * Group1.NumberOfDataCodewordsPerBlock;
            if (Group2 != null)
            {
                sum += Group2.NumberOfBlocks * Group2.NumberOfDataCodewordsPerBlock;
            }

            return sum;
        }
    }
}

public class Group
{
    public int NumberOfBlocks { get; set; }

    public int NumberOfDataCodewordsPerBlock { get; set; }
}