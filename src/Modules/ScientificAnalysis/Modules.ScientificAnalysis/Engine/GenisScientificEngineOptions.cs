namespace FSH.Modules.ScientificAnalysis.Engine;

public sealed class GenisScientificEngineOptions
{
    public const string SectionName = "GenisScientificEngine";

    public bool Enabled { get; set; }
    public string BaseUrl { get; set; } = "http://127.0.0.1:8088";
    public int TimeoutSeconds { get; set; } = 120;
    public string ExpectedService { get; set; } = "genis-scientific-engine";
    public string ExpectedVersion { get; set; } = "1.1.0";
    public string ExpectedUpstreamCommit { get; set; } =
        "1ba3e3cd618ecbad2f42a9d526f20ec83a93eede";
}
