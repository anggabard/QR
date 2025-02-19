using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QR_Generator.Services.Base;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

var startupTypes = StartupService.GetChildTypes();
foreach(var startupType in startupTypes)
{
    builder.Services.AddSingleton(startupType);
}

var host = builder.Build();

// Resolve MyService and force initialization
foreach (var serviceType in startupTypes)
{
    var service = (StartupService)host.Services.GetRequiredService(serviceType);
    await service.InitializeAsync(); // Initialize the service
}

host.Run();
