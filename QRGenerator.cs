using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
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

            // Read the body from the HttpRequest
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            // Manually deserialize the JSON body into your custom model
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

            var bitData = new BitData(config.EncodingMode, lengthBits, QRrequest.Message, dataCodewordssInfo);

            var matrix = new BaseMatrix(config.Version, config.ErrorCorrectionLevel, 3);
            await matrix.DrawFixedPatterns();

            return new OkObjectResult(
                $"Length: {QRrequest.Message.Length}, \n" +
                $"Mode: {config.EncodingMode}, \n" +
                $"Version: {config.Version}, \n" +
                $"ECL: {config.ErrorCorrectionLevel}\n" +
                $"LengthBits: {lengthBits}\n" +
                $"TotalDataCodewordss: {dataCodewordssInfo.TotalDataCodewords}\n" +
                /*$"bitData: \n{bitData}\n" +*/
                $"matrix: \n{matrix}\n");
        }
    }
}
