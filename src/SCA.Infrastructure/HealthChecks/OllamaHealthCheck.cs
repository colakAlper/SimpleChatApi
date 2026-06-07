using Microsoft.Extensions.Diagnostics.HealthChecks;
using SCA.Application.Abstractions;
using SCA.Infrastructure.Ollama;

namespace SCA.Infrastructure.HealthChecks;

public sealed class OllamaHealthCheck(IOllamaClient ollama, OllamaOptions options) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        bool ready = await ollama.IsModelReadyAsync(cancellationToken);
        if (!ready)
        {
            string command = $"docker compose exec ollama ollama pull {options.Model}";
            return HealthCheckResult.Unhealthy(
                $"Ollama model '{options.Model}' is missing. Run: {command}");
        }

        return HealthCheckResult.Healthy($"Ollama model '{options.Model}' is ready.");
    }
}
