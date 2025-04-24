using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QR_Generator.Services.Startup;

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

foreach (var serviceType in startupTypes)
{
    var service = (StartupService)host.Services.GetRequiredService(serviceType);
    await service.InitializeAsync();
}

host.Run();
