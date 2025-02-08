namespace QR_Generator.Constants;

[Flags]
public enum EncodingMode
{
    Numeric         = 0b0001,   // 1
    Alphanumeric    = 0b0010,   // 2
    Byte            = 0b0100,   // 4
    Kanji           = 0b1000,   // 8
    ECI             = 0b0111    // 7
}
