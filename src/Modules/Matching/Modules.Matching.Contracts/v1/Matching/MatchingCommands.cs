using FSH.Modules.Matching.Contracts.Dtos;
using Mediator;

namespace FSH.Modules.Matching.Contracts.v1.Matching;

public sealed record CreateProfileCategoryCommand(
    string Code,
    string Name,
    int AnalysisTypeId,
    bool IsReference,
    bool Mitochondrial) : ICommand<Guid>;

public sealed record CreateMatchingRuleCommand(
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
    bool Mitochondrial) : ICommand<Guid>;

public sealed record ConfigureProfileMatchingCommand(
    Guid GeneticProfileId,
    Guid CategoryId,
    bool Matchable,
    Guid? VictimProfileId = null) : ICommand<Guid>;

public sealed record RunAutosomalDatabaseSearchCommand(
    Guid QueryProfileId,
    Guid MatchingRuleId) : ICommand<AutosomalMatchSearchDto>;

public sealed record ReviewMatchHitCommand(
    Guid HitId,
    string Status,
    string? ReviewNote = null) : ICommand<Guid>;
