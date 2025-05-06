namespace QR_Generator.Models;

public class QRRequest
{
    public required string Message { get; set; }
    public bool AsBinary { get; set; }
}
