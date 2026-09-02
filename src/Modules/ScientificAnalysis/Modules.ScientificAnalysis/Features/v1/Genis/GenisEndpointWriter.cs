using FSH.Modules.ScientificAnalysis.Engine;
using Microsoft.AspNetCore.Http;

namespace FSH.Modules.ScientificAnalysis.Features.v1.Genis;

internal static class GenisEndpointWriter
{
    internal static async Task WriteEngineResponseAsync(
        HttpContext context,
        GenisEngineResponse response,
        CancellationToken cancellationToken)
    {
        context.Response.StatusCode = response.StatusCode;
        context.Response.ContentType = response.ContentType;
        await context.Response.WriteAsync(response.Body, cancellationToken)
            .ConfigureAwait(false);
    }

    internal static async Task WriteProxyResponseAsync(
        HttpContext context,
        GenisProxyResult result,
        CancellationToken cancellationToken)
    {
        context.Response.Headers["X-Analysis-Run-Id"] = result.AnalysisRunId.ToString();
        context.Response.Headers["X-Genis-Version"] = result.EngineVersion;
        context.Response.Headers["X-Genis-Upstream-Commit"] = result.UpstreamCommit;
        await WriteEngineResponseAsync(context, result.Response, cancellationToken)
            .ConfigureAwait(false);
    }

    internal static async Task WriteAdapterFailureAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
        context.Response.ContentType = "application/json";
        string body = System.Text.Json.JsonSerializer.Serialize(new
        {
            error = "GENIS_ADAPTER_UNAVAILABLE",
            message = exception.Message,
        });
        await context.Response.WriteAsync(body, cancellationToken).ConfigureAwait(false);
    }
}
