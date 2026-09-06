using FSH.Framework.Core.Exceptions;
using FSH.Modules.Matching.Contracts.Dtos;
using FSH.Modules.Matching.Contracts.v1.Matching;
using FSH.Modules.Matching.Data;
using FSH.Modules.Matching.Domain;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Matching.Features.v1.Matching;

public sealed class ListProfileCategoriesQueryHandler(MatchingDbContext dbContext)
    : IQueryHandler<ListProfileCategoriesQuery, IReadOnlyList<ProfileCategoryDto>>
{
    public async ValueTask<IReadOnlyList<ProfileCategoryDto>> Handle(
        ListProfileCategoriesQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        return await dbContext.ProfileCategories.AsNoTracking()
            .OrderBy(x => x.Code)
            .Select(x => new ProfileCategoryDto(
                x.Id,
                x.Code,
                x.Name,
                x.AnalysisTypeId,
                x.IsReference,
                x.Mitochondrial,
                x.CreatedAtUtc))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}

public sealed class ListMatchingRulesQueryHandler(MatchingDbContext dbContext)
    : IQueryHandler<ListMatchingRulesQuery, IReadOnlyList<MatchingRuleDto>>
{
    public async ValueTask<IReadOnlyList<MatchingRuleDto>> Handle(
        ListMatchingRulesQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        IQueryable<MatchingRule> rules = dbContext.MatchingRules.AsNoTracking();
        if (query.SourceCategoryId.HasValue)
        {
            rules = rules.Where(x => x.SourceCategoryId == query.SourceCategoryId.Value);
        }

        return await rules.OrderBy(x => x.SourceCategoryId)
            .ThenBy(x => x.CategoryRelated)
            .Select(x => new MatchingRuleDto(
                x.Id,
                x.SourceCategoryId,
                x.Type,
                x.CategoryRelated,
                x.MinimumStringency,
                x.FailOnMatch,
                x.ForwardToUpper,
                x.MatchingAlgorithm,
                x.MinLocusMatch,
                x.MismatchsAllowed,
                x.ConsiderForN,
                x.Mitochondrial,
                x.CreatedAtUtc))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}

public sealed class GetProfileMatchingConfigurationQueryHandler(MatchingDbContext dbContext)
    : IQueryHandler<GetProfileMatchingConfigurationQuery, ProfileMatchingConfigurationDto?>
{
    public async ValueTask<ProfileMatchingConfigurationDto?> Handle(
        GetProfileMatchingConfigurationQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        ProfileMatchingConfiguration? config = await dbContext.ProfileConfigurations.AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.GeneticProfileId == query.GeneticProfileId,
                cancellationToken)
            .ConfigureAwait(false);

        return config is null
            ? null
            : new ProfileMatchingConfigurationDto(
                config.GeneticProfileId,
                config.CategoryId,
                config.Matchable,
                config.VictimProfileId,
                config.UpdatedAtUtc);
    }
}

public sealed class GetAutosomalMatchSearchQueryHandler(MatchingDbContext dbContext)
    : IQueryHandler<GetAutosomalMatchSearchQuery, AutosomalMatchSearchDto>
{
    public async ValueTask<AutosomalMatchSearchDto> Handle(
        GetAutosomalMatchSearchQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        AutosomalMatchSearch search = await dbContext.AutosomalMatchSearches.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == query.SearchId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException($"Autosomal match search {query.SearchId} not found.");

        List<AutosomalMatchResult> results = await dbContext.AutosomalMatchResults.AsNoTracking()
            .Where(x => x.MatchSearchId == search.Id)
            .OrderBy(x => x.Rank)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        List<MatchHit> hits = await dbContext.MatchHits.AsNoTracking()
            .Where(x => x.MatchSearchId == search.Id)
            .OrderBy(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return MatchingMapper.ToSearchDto(search, results, hits);
    }
}

internal static class MatchingMapper
{
    internal static AutosomalMatchSearchDto ToSearchDto(
        AutosomalMatchSearch search,
        IReadOnlyList<AutosomalMatchResult> results,
        IReadOnlyList<MatchHit> hits)
    {
        ArgumentNullException.ThrowIfNull(search);
        ArgumentNullException.ThrowIfNull(results);
        ArgumentNullException.ThrowIfNull(hits);

        return new AutosomalMatchSearchDto(
            search.Id,
            search.QueryProfileId,
            search.MatchingRuleId,
            search.AnalysisRunId,
            search.CandidateCount,
            search.QualifiedCount,
            search.Mixture,
            search.CreatedAtUtc,
            results.Select(x => new AutosomalMatchResultDto(
                x.Id,
                x.MatchSearchId,
                x.CandidateProfileId,
                x.Rank,
                x.RawOverall,
                x.RawMismatches,
                x.SharedMarkers,
                x.LeftPonderation,
                x.RightPonderation,
                x.RuleMismatches,
                x.RuleQualifiedLoci,
                x.RuleQualified,
                x.DetailedJson)).ToList(),
            hits.Select(x => new MatchHitDto(
                x.Id,
                x.MatchSearchId,
                x.MatchResultId,
                x.QueryProfileId,
                x.CandidateProfileId,
                x.Status,
                x.ReviewNote,
                x.ReviewedByUserId,
                x.CreatedAtUtc,
                x.ReviewedAtUtc)).ToList());
    }
}
