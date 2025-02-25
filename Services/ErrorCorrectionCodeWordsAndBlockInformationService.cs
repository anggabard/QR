﻿using QR_Generator.Constants.Enums;
using QR_Generator.Helper;
using QR_Generator.Models;
using QR_Generator.Services.Base;

namespace QR_Generator.Services;

public class ErrorCorrectionCodeWordsAndBlockInformationService : StartupService
{
    private Dictionary<int, Dictionary<ErrorCorrectionLevel, DataCodewords>> VersionDataCodewords { get; set; }

    public override void Initialize()
    {
        VersionDataCodewords = JsonHelper.LoadFromAssembly<Dictionary<int, Dictionary<ErrorCorrectionLevel, DataCodewords>>>("QR_Generator.Config.CodeWordsBlockInformation.ErrorCorrectionCodeWordsAndBlockInformation.json");
    }

    public int GetTotalDataCodewordss(int version, ErrorCorrectionLevel errorCorrectionLevel)
    {
        if(version < 1 || version > 40)
        {
            throw new ArgumentException("Version must be between 1 and 40");
        }

        return VersionDataCodewords[version][errorCorrectionLevel].TotalDataCodewords;
    }
}
