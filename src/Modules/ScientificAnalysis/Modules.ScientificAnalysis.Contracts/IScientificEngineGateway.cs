namespace FSH.Modules.ScientificAnalysis.Contracts;

/// <summary>
/// Stable contracts-only gateway used by forensic workflow modules to invoke
/// the validated local GENis scientific engine without depending on its runtime adapter.
/// </summary>
public interface IScientificEngineGateway
{
    Task<ScientificEngineCallResult> RunGenisAsync(
        string algorithmId,
        string enginePath,
        string requestJson,
        CancellationToken cancellationToken);
}

public sealed record ScientificEngineCallResult(
    Guid AnalysisRunId,
    string EngineName,
    string EngineVersion,
    string UpstreamCommit,
    int StatusCode,
    string ContentType,
    string Body);
