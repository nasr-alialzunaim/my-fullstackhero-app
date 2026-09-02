using FSH.Framework.Core.Domain;

namespace FSH.Modules.ScientificAnalysis.Domain;

public sealed class AnalysisRun : AggregateRoot<Guid>
{
    public string AlgorithmId { get; private set; } = default!;
    public string EngineName { get; private set; } = default!;
    public string EngineVersion { get; private set; } = default!;
    public string UpstreamCommit { get; private set; } = default!;
    public string RequestJson { get; private set; } = default!;
    public string? ResponseJson { get; private set; }
    public string RequestSha256 { get; private set; } = default!;
    public string? ResponseSha256 { get; private set; }
    public int? EngineHttpStatusCode { get; private set; }
    public string Outcome { get; private set; } = default!;
    public Guid InitiatedByUserId { get; private set; }
    public DateTime StartedAtUtc { get; private set; }
    public DateTime CompletedAtUtc { get; private set; }

    private AnalysisRun()
    {
    }

    public static AnalysisRun Completed(
        Guid id,
        string algorithmId,
        string engineVersion,
        string upstreamCommit,
        string requestJson,
        string responseJson,
        string requestSha256,
        string responseSha256,
        int engineHttpStatusCode,
        Guid initiatedByUserId,
        DateTime startedAtUtc,
        DateTime completedAtUtc)
    {
        return new AnalysisRun
        {
            Id = id,
            AlgorithmId = algorithmId,
            EngineName = "genis-scientific-engine",
            EngineVersion = engineVersion,
            UpstreamCommit = upstreamCommit,
            RequestJson = requestJson,
            ResponseJson = responseJson,
            RequestSha256 = requestSha256,
            ResponseSha256 = responseSha256,
            EngineHttpStatusCode = engineHttpStatusCode,
            Outcome = engineHttpStatusCode is >= 200 and < 300 ? "Succeeded" : "EngineRejected",
            InitiatedByUserId = initiatedByUserId,
            StartedAtUtc = startedAtUtc,
            CompletedAtUtc = completedAtUtc,
        };
    }

    public static AnalysisRun Failed(
        Guid id,
        string algorithmId,
        string engineVersion,
        string upstreamCommit,
        string requestJson,
        string requestSha256,
        string failureJson,
        Guid initiatedByUserId,
        DateTime startedAtUtc,
        DateTime completedAtUtc)
    {
        return new AnalysisRun
        {
            Id = id,
            AlgorithmId = algorithmId,
            EngineName = "genis-scientific-engine",
            EngineVersion = engineVersion,
            UpstreamCommit = upstreamCommit,
            RequestJson = requestJson,
            ResponseJson = failureJson,
            RequestSha256 = requestSha256,
            ResponseSha256 = null,
            EngineHttpStatusCode = null,
            Outcome = "AdapterFailed",
            InitiatedByUserId = initiatedByUserId,
            StartedAtUtc = startedAtUtc,
            CompletedAtUtc = completedAtUtc,
        };
    }
}
