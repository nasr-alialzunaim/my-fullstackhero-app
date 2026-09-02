namespace FSH.Modules.Matching.Domain;

public sealed class AutosomalMatchResult
{
    public Guid Id { get; private set; }
    public Guid MatchSearchId { get; private set; }
    public Guid CandidateProfileId { get; private set; }
    public int Rank { get; private set; }
    public string RawOverall { get; private set; } = default!;
    public int RawMismatches { get; private set; }
    public int SharedMarkers { get; private set; }
    public double LeftPonderation { get; private set; }
    public double RightPonderation { get; private set; }
    public int RuleMismatches { get; private set; }
    public int RuleQualifiedLoci { get; private set; }
    public bool RuleQualified { get; private set; }
    public string DetailedJson { get; private set; } = default!;

    private AutosomalMatchResult() { }

    public static AutosomalMatchResult Create(
        Guid matchSearchId,
        Guid candidateProfileId,
        int rank,
        string rawOverall,
        int rawMismatches,
        int sharedMarkers,
        double leftPonderation,
        double rightPonderation,
        int ruleMismatches,
        int ruleQualifiedLoci,
        bool ruleQualified,
        string detailedJson)
    {
        return new AutosomalMatchResult
        {
            Id = Guid.CreateVersion7(),
            MatchSearchId = matchSearchId,
            CandidateProfileId = candidateProfileId,
            Rank = rank,
            RawOverall = rawOverall,
            RawMismatches = rawMismatches,
            SharedMarkers = sharedMarkers,
            LeftPonderation = leftPonderation,
            RightPonderation = rightPonderation,
            RuleMismatches = ruleMismatches,
            RuleQualifiedLoci = ruleQualifiedLoci,
            RuleQualified = ruleQualified,
            DetailedJson = detailedJson,
        };
    }
}
