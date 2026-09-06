using FSH.Framework.Core.Domain;

namespace FSH.Modules.Matching.Domain;

public sealed class AutosomalMatchSearch : AggregateRoot<Guid>
{
    public Guid QueryProfileId { get; private set; }
    public Guid MatchingRuleId { get; private set; }
    public Guid AnalysisRunId { get; private set; }
    public int CandidateCount { get; private set; }
    public int QualifiedCount { get; private set; }
    public bool Mixture { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private AutosomalMatchSearch() { }

    public static AutosomalMatchSearch Create(
        Guid queryProfileId,
        Guid matchingRuleId,
        Guid analysisRunId,
        int candidateCount,
        int qualifiedCount,
        bool mixture)
    {
        return new AutosomalMatchSearch
        {
            Id = Guid.CreateVersion7(),
            QueryProfileId = queryProfileId,
            MatchingRuleId = matchingRuleId,
            AnalysisRunId = analysisRunId,
            CandidateCount = candidateCount,
            QualifiedCount = qualifiedCount,
            Mixture = mixture,
            CreatedAtUtc = DateTime.UtcNow,
        };
    }
}
