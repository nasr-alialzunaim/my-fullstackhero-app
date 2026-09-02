using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FSH.Framework.Core.Context;
using FSH.Modules.ScientificAnalysis.Contracts;
using FSH.Modules.ScientificAnalysis.Data;
using FSH.Modules.ScientificAnalysis.Domain;

namespace FSH.Modules.ScientificAnalysis.Engine;

public sealed class GenisAnalysisProxy(
    GenisScientificEngineClient client,
    ScientificAnalysisDbContext dbContext,
    ICurrentUser currentUser) : IScientificEngineGateway
{
    public async Task<GenisProxyResult> RunAsync(
        string algorithmId,
        string enginePath,
        string requestJson,
        CancellationToken cancellationToken)
    {
        Guid runId = Guid.CreateVersion7();
        Guid userId = currentUser.GetUserId();
        DateTime startedAtUtc = DateTime.UtcNow;
        string requestHash = Sha256(requestJson);

        string version = "unavailable";
        string commit = "unavailable";

        try
        {
            GenisEngineVersion metadata = await client
                .GetValidatedVersionAsync(cancellationToken)
                .ConfigureAwait(false);

            version = metadata.Version;
            commit = metadata.GenisUpstreamCommit;

            GenisEngineResponse response = await client
                .PostJsonAsync(enginePath, requestJson, cancellationToken)
                .ConfigureAwait(false);

            DateTime completedAtUtc = DateTime.UtcNow;
            string responseHash = Sha256(response.Body);

            AnalysisRun run = AnalysisRun.Completed(
                runId,
                algorithmId,
                version,
                commit,
                requestJson,
                response.Body,
                requestHash,
                responseHash,
                response.StatusCode,
                userId,
                startedAtUtc,
                completedAtUtc);

            dbContext.AnalysisRuns.Add(run);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new GenisProxyResult(runId, version, commit, response);
        }
        catch (Exception ex) when (
            ex is HttpRequestException or
            InvalidOperationException or
            JsonException or
            TaskCanceledException)
        {
            DateTime completedAtUtc = DateTime.UtcNow;
            string failureJson = JsonSerializer.Serialize(new
            {
                error = "GENIS_ADAPTER_FAILURE",
                message = ex.Message,
            });

            AnalysisRun run = AnalysisRun.Failed(
                runId,
                algorithmId,
                version,
                commit,
                requestJson,
                requestHash,
                failureJson,
                userId,
                startedAtUtc,
                completedAtUtc);

            dbContext.AnalysisRuns.Add(run);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            throw;
        }
    }

    public async Task<ScientificEngineCallResult> RunGenisAsync(
        string algorithmId,
        string enginePath,
        string requestJson,
        CancellationToken cancellationToken)
    {
        GenisProxyResult result = await RunAsync(
            algorithmId,
            enginePath,
            requestJson,
            cancellationToken).ConfigureAwait(false);

        return new ScientificEngineCallResult(
            result.AnalysisRunId,
            "genis-scientific-engine",
            result.EngineVersion,
            result.UpstreamCommit,
            result.Response.StatusCode,
            result.Response.ContentType,
            result.Response.Body);
    }

    private static string Sha256(string value)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes);
    }
}