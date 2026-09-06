using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.ScientificAnalysis.Contracts.Authorization;
using FSH.Modules.ScientificAnalysis.Engine;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.ScientificAnalysis.Features.v1.Genis;

public static class GenisMetadataEndpoints
{
    internal static void MapGenisMetadataEndpoints(this IEndpointRouteBuilder group)
    {
        group.MapGet("/health", async (
                HttpContext context,
                GenisScientificEngineClient client,
                CancellationToken ct) =>
            {
                try
                {
                    GenisEngineResponse response = await client.GetAsync("/health", ct)
                        .ConfigureAwait(false);
                    await GenisEndpointWriter.WriteEngineResponseAsync(context, response, ct)
                        .ConfigureAwait(false);
                }
                catch (Exception ex) when (
                    ex is HttpRequestException or InvalidOperationException or TaskCanceledException)
                {
                    await GenisEndpointWriter.WriteAdapterFailureAsync(context, ex, ct)
                        .ConfigureAwait(false);
                }
            })
            .WithName("GenisHealth")
            .WithSummary("Proxy the validated GENis health endpoint")
            .RequirePermission(ScientificAnalysisPermissions.View);

        group.MapGet("/version", async (
                HttpContext context,
                GenisScientificEngineClient client,
                CancellationToken ct) =>
            {
                try
                {
                    GenisEngineVersion metadata = await client.GetValidatedVersionAsync(ct)
                        .ConfigureAwait(false);
                    await Results.Json(new
                    {
                        service = metadata.Service,
                        version = metadata.Version,
                        genisUpstreamCommit = metadata.GenisUpstreamCommit,
                    }).ExecuteAsync(context).ConfigureAwait(false);
                }
                catch (Exception ex) when (
                    ex is HttpRequestException or InvalidOperationException or
                    System.Text.Json.JsonException or TaskCanceledException)
                {
                    await GenisEndpointWriter.WriteAdapterFailureAsync(context, ex, ct)
                        .ConfigureAwait(false);
                }
            })
            .WithName("GenisVersion")
            .WithSummary("Return validated GENis engine provenance")
            .RequirePermission(ScientificAnalysisPermissions.View);

        group.MapGet("/algorithms", async (
                HttpContext context,
                GenisScientificEngineClient client,
                CancellationToken ct) =>
            {
                try
                {
                    _ = await client.GetValidatedVersionAsync(ct).ConfigureAwait(false);
                    GenisEngineResponse response = await client.GetAsync("/v1/algorithms", ct)
                        .ConfigureAwait(false);
                    await GenisEndpointWriter.WriteEngineResponseAsync(context, response, ct)
                        .ConfigureAwait(false);
                }
                catch (Exception ex) when (
                    ex is HttpRequestException or InvalidOperationException or
                    System.Text.Json.JsonException or TaskCanceledException)
                {
                    await GenisEndpointWriter.WriteAdapterFailureAsync(context, ex, ct)
                        .ConfigureAwait(false);
                }
            })
            .WithName("GenisAlgorithms")
            .WithSummary("Proxy the validated GENis algorithm catalogue")
            .RequirePermission(ScientificAnalysisPermissions.View);
    }
}
