namespace QR_Generator.Models;

public class DataCodewords
{
    public int ECCodewordsPerBlock { get; set; }
    public int NumberOfBlocksInGroup1 { get; set; }
    public int? NumberOfBlocksInGroup2 { get; set; }
    public int NumberOfDataCodewordsInEachOfGroup1Blocks { get; set; }
    public int? NumberOfDataCodewordsInEachOfGroup2Blocks { get; set; }
    public int TotalDataCodewords
    {
        get
        {
            var sum = NumberOfBlocksInGroup1 * NumberOfDataCodewordsInEachOfGroup1Blocks;
            if (NumberOfBlocksInGroup2 != null)
            {
                sum += (int)NumberOfBlocksInGroup2 * (int)NumberOfDataCodewordsInEachOfGroup2Blocks;//this should always have a value if NumberOfBlocksInGroup2 != null
            }

            return sum;
        }
    }
}
