using FSH.Framework.Core.Domain;

namespace FSH.Modules.Matching.Domain;

/// <summary>
/// Compatibility-preserving representation of GENis configdata.MatchingRule.
/// Field spelling MismatchsAllowed intentionally follows upstream GENis.
/// </summary>
public sealed class MatchingRule : AggregateRoot<Guid>
{
    public Guid SourceCategoryId { get; private set; }
    public int Type { get; private set; }
    public string CategoryRelated { get; private set; } = default!;
    public string MinimumStringency { get; private set; } = default!;
    public bool FailOnMatch { get; private set; }
    public bool ForwardToUpper { get; private set; }
    public string MatchingAlgorithm { get; private set; } = default!;
    public int MinLocusMatch { get; private set; }
    public int MismatchsAllowed { get; private set; }
    public bool ConsiderForN { get; private set; }
    public bool Mitochondrial { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private MatchingRule()
    {
    }

    public static MatchingRule Create(
        Guid sourceCategoryId,
        int type,
        string categoryRelated,
        string minimumStringency,
        bool failOnMatch,
        bool forwardToUpper,
        string matchingAlgorithm,
        int minLocusMatch,
        int mismatchsAllowed,
        bool considerForN,
        bool mitochondrial)
    {
        if (sourceCategoryId == Guid.Empty)
        {
            throw new ArgumentException("Source category identity cannot be empty.", nameof(sourceCategoryId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(categoryRelated);
        ArgumentException.ThrowIfNullOrWhiteSpace(minimumStringency);
        ArgumentException.ThrowIfNullOrWhiteSpace(matchingAlgorithm);
        ArgumentOutOfRangeException.ThrowIfNegative(minLocusMatch);
        ArgumentOutOfRangeException.ThrowIfNegative(mismatchsAllowed);

        return new MatchingRule
        {
            Id = Guid.CreateVersion7(),
            SourceCategoryId = sourceCategoryId,
            Type = type,
            CategoryRelated = categoryRelated.Trim().ToUpperInvariant(),
            MinimumStringency = minimumStringency.Trim(),
            FailOnMatch = failOnMatch,
            ForwardToUpper = forwardToUpper,
            MatchingAlgorithm = matchingAlgorithm.Trim(),
            MinLocusMatch = minLocusMatch,
            MismatchsAllowed = mismatchsAllowed,
            ConsiderForN = considerForN,
            Mitochondrial = mitochondrial,
            CreatedAtUtc = DateTime.UtcNow,
        };
    }
}
