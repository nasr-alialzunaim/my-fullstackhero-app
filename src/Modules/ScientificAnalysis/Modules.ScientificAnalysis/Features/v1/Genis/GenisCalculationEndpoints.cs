using System.Text;
using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.ScientificAnalysis.Contracts.Authorization;
using FSH.Modules.ScientificAnalysis.Engine;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.ScientificAnalysis.Features.v1.Genis;

public static class GenisCalculationEndpoints
{
    private static readonly (string Route, string AlgorithmId, string EnginePath)[] Routes =
    [
        ("/lr-mix", "lr-mix", "/v1/lr-mix"),
        ("/lr-mix-mix", "lr-mix-mix-new", "/v1/lr-mix-mix"),
        ("/lr-mix-mix/legacy", "lr-mix-mix-legacy", "/v1/lr-mix-mix/legacy"),
        ("/rmp", "random-match-probability", "/v1/rmp"),
        ("/match/autosomal", "autosomal-matching", "/v1/match/autosomal"),
        ("/match/autosomal/rank", "autosomal-matching-rank", "/v1/match/autosomal/rank"),
        ("/match/mtdna", "mtdna-matching", "/v1/match/mtdna"),
        ("/match/mtdna/rank", "mtdna-matching-rank", "/v1/match/mtdna/rank"),
        ("/pedigree/lr", "bayesian-pedigree-lr", "/v1/pedigree/lr"),
        ("/pedigree/consistency", "lange-goradia-consistency", "/v1/pedigree/consistency"),
    ];

    internal static void MapGenisCalculationEndpoints(this IEndpointRouteBuilder group)
    {
        foreach ((string route, string algorithmId, string enginePath) in Routes)
        {
            string localRoute = route;
            string localAlgorithmId = algorithmId;
            string localEnginePath = enginePath;

            group.MapPost(localRoute, async (
                    HttpContext context,
                    GenisAnalysisProxy proxy,
                    CancellationToken ct) =>
                {
                    using var reader = new StreamReader(
                        context.Request.Body,
                        Encoding.UTF8,
                        detectEncodingFromByteOrderMarks: false,
                        leaveOpen: true);
                    string requestJson = await reader.ReadToEndAsync(ct).ConfigureAwait(false);

                    try
                    {
                        GenisProxyResult result = await proxy.RunAsync(
                            localAlgorithmId,
                            localEnginePath,
                            requestJson,
                            ct).ConfigureAwait(false);

                        await GenisEndpointWriter.WriteProxyResponseAsync(context, result, ct)
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
                .WithName("Genis_" + localAlgorithmId.Replace('-', '_'))
                .WithSummary($"Run validated GENis algorithm: {localAlgorithmId}")
                .RequirePermission(ScientificAnalysisPermissions.Run);
        }
    }
}
