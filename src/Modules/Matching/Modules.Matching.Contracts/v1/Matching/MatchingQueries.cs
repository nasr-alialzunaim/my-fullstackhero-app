using FSH.Modules.Matching.Contracts.Dtos;
using Mediator;

namespace FSH.Modules.Matching.Contracts.v1.Matching;

public sealed record ListProfileCategoriesQuery()
    : IQuery<IReadOnlyList<ProfileCategoryDto>>;

public sealed record ListMatchingRulesQuery(Guid? SourceCategoryId = null)
    : IQuery<IReadOnlyList<MatchingRuleDto>>;

public sealed record GetProfileMatchingConfigurationQuery(Guid GeneticProfileId)
    : IQuery<ProfileMatchingConfigurationDto?>;

public sealed record GetAutosomalMatchSearchQuery(Guid SearchId)
    : IQuery<AutosomalMatchSearchDto>;
