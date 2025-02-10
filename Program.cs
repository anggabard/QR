using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QR_Generator.Services;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

builder.Services.AddSingleton<CharacterCapacitiesService>();

var host = builder.Build();

// Resolve MyService and force initialization
var characterCapacitiesService = host.Services.GetRequiredService<CharacterCapacitiesService>();
characterCapacitiesService.Initialize(); // Initialize the singleton during startup

host.Run();
