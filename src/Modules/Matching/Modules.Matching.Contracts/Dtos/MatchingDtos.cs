namespace FSH.Modules.Matching.Contracts.Dtos;

public sealed record ProfileCategoryDto(
    Guid Id,
    string Code,
    string Name,
    int AnalysisTypeId,
    bool IsReference,
    bool Mitochondrial,
    DateTime CreatedAtUtc);

public sealed record MatchingRuleDto(
    Guid Id,
    Guid SourceCategoryId,
    int Type,
    string CategoryRelated,
    string MinimumStringency,
    bool FailOnMatch,
    bool ForwardToUpper,
    string MatchingAlgorithm,
    int MinLocusMatch,
    int MismatchsAllowed,
    bool ConsiderForN,
    bool Mitochondrial,
    DateTime CreatedAtUtc);

public sealed record ProfileMatchingConfigurationDto(
    Guid GeneticProfileId,
    Guid CategoryId,
    bool Matchable,
    Guid? VictimProfileId,
    DateTime UpdatedAtUtc);

public sealed record AutosomalMatchResultDto(
    Guid Id,
    Guid MatchSearchId,
    Guid CandidateProfileId,
    int Rank,
    string RawOverall,
    int RawMismatches,
    int SharedMarkers,
    double LeftPonderation,
    double RightPonderation,
    int RuleMismatches,
    int RuleQualifiedLoci,
    bool RuleQualified,
    string DetailedJson);

public sealed record MatchHitDto(
    Guid Id,
    Guid MatchSearchId,
    Guid MatchResultId,
    Guid QueryProfileId,
    Guid CandidateProfileId,
    string Status,
    string? ReviewNote,
    Guid? ReviewedByUserId,
    DateTime CreatedAtUtc,
    DateTime? ReviewedAtUtc);

public sealed record AutosomalMatchSearchDto(
    Guid Id,
    Guid QueryProfileId,
    Guid MatchingRuleId,
    Guid AnalysisRunId,
    int CandidateCount,
    int QualifiedCount,
    bool Mixture,
    DateTime CreatedAtUtc,
    IReadOnlyList<AutosomalMatchResultDto> Results,
    IReadOnlyList<MatchHitDto> Hits);
