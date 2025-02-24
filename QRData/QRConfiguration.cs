using QR_Generator.Constants.Enums;
using System.ComponentModel.DataAnnotations;

namespace QR_Generator.QRData;

public class QRConfiguration
{
    [Range(1, 40, ErrorMessage = "Version must be between 1 and 40.")]
    public int Version { get; set; }

    public ErrorCorrectionLevel ErrorCorrectionLevel { get; set; }

    public EncodingMode EncodingMode { get; set; }
}
