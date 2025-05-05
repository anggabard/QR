using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using QR_Generator.Extensions;
using QR_Generator.Helper;
using QR_Generator.Matrix;
using QR_Generator.Models;
using QR_Generator.QRData;
using QR_Generator.Services;
using System.Text.Json;

namespace QR_Generator
{
    public class QRGenerator(
        ILogger<QRGenerator> logger, 
        CharacterCapacitiesService characterCapacitiesService, 
        ErrorCorrectionCodeWordsAndBlockInformationService codeWordsAndBlockInformationService)
    {
        [Function("QRG")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            logger.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var QRrequest = JsonSerializer.Deserialize<QRRequest>(requestBody);

            if (QRrequest == null || string.IsNullOrEmpty(QRrequest.Message))
            {
                return new BadRequestObjectResult("Request body is missing or invalid.");
            }

            var config = new QRConfiguration
            {
                EncodingMode = EncodeModeHelper.GetEncodingMode(QRrequest.Message)
            };

            (config.Version, config.ErrorCorrectionLevel) = characterCapacitiesService.GetMinVersionAndMaxErrorCorrection(config.EncodingMode, QRrequest.Message.Length);

            var lengthBits = LengthBitsHelper.GetLengthBits(config.EncodingMode, config.Version);
            var dataCodewordssInfo = codeWordsAndBlockInformationService.Get(config.Version, config.ErrorCorrectionLevel);

            var bitData = await Task.Run(() => new BitData(config.EncodingMode, lengthBits, QRrequest.Message, dataCodewordssInfo));

            var patternsMatrix = new PatternsMatrix(config.Version);
            await patternsMatrix.DrawFixedPatterns();

            var dataMatrix = patternsMatrix.ToDataMatrix();
            await dataMatrix.SetData(bitData.GetData());

            var maskService = new MaskService(patternsMatrix.GetMatrix(), dataMatrix.GetMatrix(), dataMatrix.MatrixSize);
            await maskService.GenerateMasks(config.ErrorCorrectionLevel);

            var QR = maskService.GetFinalQR();

            return new OkObjectResult(
                $"Length: {QRrequest.Message.Length}, \n" +
                $"Mode: {config.EncodingMode}, \n" +
                $"Version: {config.Version}, \n" +
                $"ECL: {config.ErrorCorrectionLevel}\n" +
                $"LengthBits: {lengthBits}\n" +
                $"TotalDataCodewordss: {dataCodewordssInfo.TotalDataCodewords}\n" +
                //$"bitData: \n{bitData}\n" +
                $"matrix: \n{QR.ToQR()}\n");
        }
    }
}
