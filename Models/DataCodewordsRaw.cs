namespace QR_Generator.Models;

public class DataCodewordsRaw
{
    public int ECCodewordsPerBlock { get; set; }
    public int NumberOfBlocksInGroup1 { get; set; }
    public int? NumberOfBlocksInGroup2 { get; set; }
    public int NumberOfDataCodewordsInEachOfGroup1Blocks { get; set; }
    public int? NumberOfDataCodewordsInEachOfGroup2Blocks { get; set; }
}
