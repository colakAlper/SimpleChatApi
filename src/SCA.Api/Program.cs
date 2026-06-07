using Microsoft.Extensions.Diagnostics.HealthChecks;
using SCA.Api.Extensions;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    // Program.cs should only show the startup flow.
    builder.UseProjectSerilog();
    builder.Services.AddProjectLayers(builder.Configuration);

    WebApplication app = builder.Build();

    // The API should not start if Redis or the Ollama model is missing.
    if (!app.Environment.IsEnvironment("Testing"))
    {
        HealthCheckService healthCheckService = app.Services.GetRequiredService<HealthCheckService>();
        HealthReport report = await healthCheckService.CheckHealthAsync();

        if (report.Status != HealthStatus.Healthy)
        {
            foreach (KeyValuePair<string, HealthReportEntry> entry in report.Entries)
            {
                if (entry.Value.Status == HealthStatus.Unhealthy)
                {
                    Log.Fatal("Health check '{Name}' failed: {Description}", entry.Key, entry.Value.Description);
                }
            }

            throw new InvalidOperationException("One or more dependency health checks failed. API will not start.");
        }
    }

    app.UseApiLayer();
    await app.RunAsync();
}
catch (Exception exception)
{
    Log.Fatal(exception, "Application could not start.");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

namespace SCA.Api
{
    public partial class Program;
}