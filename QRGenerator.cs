using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace QR_Generator
{
    public class QRGenerator
    {
        private readonly ILogger<QRGenerator> _logger;

        public QRGenerator(ILogger<QRGenerator> logger)
        {
            _logger = logger;
        }

        [Function("QRG")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
