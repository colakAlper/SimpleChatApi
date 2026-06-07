using CB.Api.Extensions;
using CB.Infrastructure.Startup;
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

    // TODO: Buna gerek yok, direkt olarak yoksa hata versin.
    if (!app.Environment.IsEnvironment("Testing"))
    {
        // The API should not start if Redis or the Ollama model is missing.
        await app.Services.ValidateInfrastructureAsync();
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

public partial class Program;
