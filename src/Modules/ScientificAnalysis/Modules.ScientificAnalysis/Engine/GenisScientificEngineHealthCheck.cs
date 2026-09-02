using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FSH.Modules.ScientificAnalysis.Engine;

public sealed class GenisScientificEngineHealthCheck(
    GenisScientificEngineClient client) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            GenisEngineVersion version = await client.GetValidatedVersionAsync(cancellationToken)
                .ConfigureAwait(false);

            return HealthCheckResult.Healthy(
                "Validated GENis scientific engine is available.",
                new Dictionary<string, object>
                {
                    ["service"] = version.Service,
                    ["version"] = version.Version,
                    ["upstreamCommit"] = version.GenisUpstreamCommit,
                });
        }
        catch (Exception ex) when (
            ex is HttpRequestException or
            InvalidOperationException or
            JsonException or
            TaskCanceledException)
        {
            return HealthCheckResult.Unhealthy(
                "Validated GENis scientific engine is unavailable or provenance mismatched.",
                ex);
        }
    }
}
