namespace FSH.Modules.ScientificAnalysis.Engine;

public sealed record GenisEngineVersion(
    string Service,
    string Version,
    string GenisUpstreamCommit);

public sealed record GenisEngineResponse(
    int StatusCode,
    string ContentType,
    string Body);

public sealed record GenisProxyResult(
    Guid AnalysisRunId,
    string EngineVersion,
    string UpstreamCommit,
    GenisEngineResponse Response);
