using System.Net;
using System.Text.Json;
using FSH.Framework.Core.Context;
using FSH.Framework.Core.Exceptions;
using FSH.Modules.Genetics.Contracts.Dtos;
using FSH.Modules.Genetics.Contracts.v1.Profiles;
using FSH.Modules.Matching.Contracts.Dtos;
using FSH.Modules.Matching.Contracts.v1.Matching;
using FSH.Modules.Matching.Data;
using FSH.Modules.Matching.Domain;
using FSH.Modules.ScientificAnalysis.Contracts;
using FSH.Modules.StrKits.Contracts.Dtos;
using FSH.Modules.StrKits.Contracts.v1.Kits;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Matching.Features.v1.Matching;

public sealed class CreateProfileCategoryCommandHandler(MatchingDbContext dbContext)
    : ICommandHandler<CreateProfileCategoryCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        CreateProfileCategoryCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        string code = command.Code.Trim().ToUpperInvariant();
        if (await dbContext.ProfileCategories.AnyAsync(
            x => x.Code == code,
            cancellationToken).ConfigureAwait(false))
        {
            throw new CustomException(
                $"Profile category '{code}' already exists.",
                (IEnumerable<string>?)null,
                HttpStatusCode.Conflict);
        }

        ProfileCategory category = ProfileCategory.Create(
            code,
            command.Name,
            command.AnalysisTypeId,
            command.IsReference,
            command.Mitochondrial);
        dbContext.ProfileCategories.Add(category);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return category.Id;
    }
}

public sealed class CreateMatchingRuleCommandHandler(MatchingDbContext dbContext)
    : ICommandHandler<CreateMatchingRuleCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        CreateMatchingRuleCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        _ = await dbContext.ProfileCategories.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == command.SourceCategoryId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException($"Source category {command.SourceCategoryId} not found.");

        string relatedCode = command.CategoryRelated.Trim().ToUpperInvariant();
        bool relatedExists = await dbContext.ProfileCategories.AsNoTracking()
            .AnyAsync(x => x.Code == relatedCode, cancellationToken)
            .ConfigureAwait(false);

        if (!relatedExists)
        {
            throw new NotFoundException($"Related category '{relatedCode}' not found.");
        }

        MatchingRule rule = MatchingRule.Create(
            command.SourceCategoryId,
            command.Type,
            relatedCode,
            command.MinimumStringency,
            command.FailOnMatch,
            command.ForwardToUpper,
            command.MatchingAlgorithm,
            command.MinLocusMatch,
            command.MismatchsAllowed,
            command.ConsiderForN,
            command.Mitochondrial);

        dbContext.MatchingRules.Add(rule);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return rule.Id;
    }
}

public sealed class ConfigureProfileMatchingCommandHandler(
    MatchingDbContext dbContext,
    IMediator mediator)
    : ICommandHandler<ConfigureProfileMatchingCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        ConfigureProfileMatchingCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        _ = await mediator.Send(
            new GetGeneticProfileByIdQuery(command.GeneticProfileId),
            cancellationToken).ConfigureAwait(false);

        _ = await dbContext.ProfileCategories.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == command.CategoryId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException($"Profile category {command.CategoryId} not found.");

        if (command.VictimProfileId.HasValue)
        {
            _ = await mediator.Send(
                new GetGeneticProfileByIdQuery(command.VictimProfileId.Value),
                cancellationToken).ConfigureAwait(false);
        }

        ProfileMatchingConfiguration? configuration = await dbContext.ProfileConfigurations
            .FirstOrDefaultAsync(
                x => x.GeneticProfileId == command.GeneticProfileId,
                cancellationToken)
            .ConfigureAwait(false);

        if (configuration is null)
        {
            configuration = ProfileMatchingConfiguration.Create(
                command.GeneticProfileId,
                command.CategoryId,
                command.Matchable,
                command.VictimProfileId);
            dbContext.ProfileConfigurations.Add(configuration);
        }
        else
        {
            configuration.Update(command.CategoryId, command.Matchable, command.VictimProfileId);
        }

        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return command.GeneticProfileId;
    }
}

public sealed class RunAutosomalDatabaseSearchCommandHandler(
    MatchingDbContext dbContext,
    IMediator mediator,
    IScientificEngineGateway scientificGateway)
    : ICommandHandler<RunAutosomalDatabaseSearchCommand, AutosomalMatchSearchDto>
{
    public async ValueTask<AutosomalMatchSearchDto> Handle(
        RunAutosomalDatabaseSearchCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        MatchingRule rule = await dbContext.MatchingRules.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == command.MatchingRuleId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException($"Matching rule {command.MatchingRuleId} not found.");

        if (rule.Mitochondrial)
        {
            throw new CustomException(
                "A mitochondrial rule cannot be used for autosomal database search.",
                (IEnumerable<string>?)null,
                HttpStatusCode.BadRequest);
        }

        ProfileMatchingConfiguration queryConfiguration = await dbContext.ProfileConfigurations
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.GeneticProfileId == command.QueryProfileId,
                cancellationToken)
            .ConfigureAwait(false)
            ?? throw new CustomException(
                "The query profile has no matching configuration.",
                (IEnumerable<string>?)null,
                HttpStatusCode.BadRequest);

        if (!queryConfiguration.Matchable)
        {
            throw new CustomException(
                "The query profile is not enabled for matching.",
                (IEnumerable<string>?)null,
                HttpStatusCode.BadRequest);
        }

        if (queryConfiguration.CategoryId != rule.SourceCategoryId)
        {
            throw new CustomException(
                "The selected rule does not belong to the query profile category.",
                (IEnumerable<string>?)null,
                HttpStatusCode.BadRequest);
        }

        ProfileCategory relatedCategory = await dbContext.ProfileCategories.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Code == rule.CategoryRelated, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException($"Related category '{rule.CategoryRelated}' not found.");

        List<ProfileMatchingConfiguration> candidateConfigurations =
            await dbContext.ProfileConfigurations.AsNoTracking()
                .Where(x =>
                    x.Matchable &&
                    x.CategoryId == relatedCategory.Id &&
                    x.GeneticProfileId != command.QueryProfileId)
                .OrderBy(x => x.GeneticProfileId)
                .Take(1000)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

        if (candidateConfigurations.Count == 0)
        {
            throw new CustomException(
                "No matchable candidate profiles exist for the related category.",
                (IEnumerable<string>?)null,
                HttpStatusCode.BadRequest);
        }

        GeneticProfileDto queryProfile = await mediator.Send(
            new GetGeneticProfileByIdQuery(command.QueryProfileId),
            cancellationToken).ConfigureAwait(false);

        IReadOnlyList<GeneticProfileDto> candidateProfiles = await mediator.Send(
            new GetGeneticProfilesByIdsQuery(
                candidateConfigurations.Select(x => x.GeneticProfileId).ToArray()),
            cancellationToken).ConfigureAwait(false);

        Guid[] allVictimIds = candidateConfigurations
            .Select(x => x.VictimProfileId)
            .Append(queryConfiguration.VictimProfileId)
            .Where(x => x.HasValue)
            .Select(x => x!.Value)
            .Distinct()
            .ToArray();

        IReadOnlyList<GeneticProfileDto> victimProfiles = allVictimIds.Length == 0
            ? Array.Empty<GeneticProfileDto>()
            : await mediator.Send(
                new GetGeneticProfilesByIdsQuery(allVictimIds),
                cancellationToken).ConfigureAwait(false);

        Dictionary<Guid, GeneticProfileDto> victimById = victimProfiles.ToDictionary(x => x.Id);

        object? alleleRanges = null;
        if (queryProfile.StrKitId.HasValue)
        {
            StrKitDto kit = await mediator.Send(
                new GetStrKitByIdQuery(queryProfile.StrKitId.Value),
                cancellationToken).ConfigureAwait(false);

            Dictionary<string, object> ranges = kit.Loci
                .Where(x => x.AlleleRangeMin.HasValue && x.AlleleRangeMax.HasValue)
                .ToDictionary(
                    x => x.Marker,
                    x => (object)new
                    {
                        min = x.AlleleRangeMin!.Value,
                        max = x.AlleleRangeMax!.Value,
                    },
                    StringComparer.Ordinal);

            if (ranges.Count > 0)
            {
                alleleRanges = ranges;
            }
        }

        Dictionary<string, string[]> queryGenotype = ToGenotypification(queryProfile);
        Dictionary<string, string[]>? queryVictim = queryConfiguration.VictimProfileId.HasValue
            ? ToGenotypification(victimById[queryConfiguration.VictimProfileId.Value])
            : null;

        var candidatePayload = candidateProfiles.Select(profile =>
        {
            ProfileMatchingConfiguration config = candidateConfigurations
                .First(x => x.GeneticProfileId == profile.Id);

            return new
            {
                id = profile.Id.ToString("D", System.Globalization.CultureInfo.InvariantCulture),
                profile = ToGenotypification(profile),
                victim = config.VictimProfileId.HasValue
                    ? ToGenotypification(victimById[config.VictimProfileId.Value])
                    : null,
            };
        }).ToArray();

        bool mixture =
            string.Equals(rule.MatchingAlgorithm, "GENIS_MM", StringComparison.Ordinal) ||
            (queryProfile.Contributors ?? 1) > 1 ||
            queryVictim is not null ||
            candidateProfiles.Any(x => (x.Contributors ?? 1) > 1) ||
            candidateConfigurations.Any(x => x.VictimProfileId.HasValue);

        string requestJson = JsonSerializer.Serialize(new
        {
            query = queryGenotype,
            queryVictim,
            candidates = candidatePayload,
            alleleRanges,
            mixture,
        });

        ScientificEngineCallResult call = await scientificGateway.RunGenisAsync(
            "autosomal-matching-rank",
            "/v1/match/autosomal/rank",
            requestJson,
            cancellationToken).ConfigureAwait(false);

        if (call.StatusCode is < 200 or >= 300)
        {
            throw new CustomException(
                $"GENis autosomal rank rejected the request with HTTP {call.StatusCode}.",
                (IEnumerable<string>?)null,
                HttpStatusCode.UnprocessableEntity);
        }

        using JsonDocument response = JsonDocument.Parse(call.Body);
        JsonElement ranked = response.RootElement.GetProperty("ranked");

        List<ParsedAutosomalResult> parsed = new(ranked.GetArrayLength());
        foreach (JsonElement item in ranked.EnumerateArray())
        {
            Guid candidateId = Guid.Parse(item.GetProperty("candidateId").GetString()!);
            string overall = item.GetProperty("overall").GetString()!;
            int rawMismatches = item.GetProperty("mismatches").GetInt32();
            int sharedMarkers = item.GetProperty("sharedMarkers").GetInt32();
            double leftPonderation = item.GetProperty("leftPonderation").GetDouble();
            double rightPonderation = item.GetProperty("rightPonderation").GetDouble();

            JsonElement detailedElement = item.GetProperty("detailed");
            Dictionary<string, string> detailed = detailedElement.EnumerateObject()
                .ToDictionary(
                    property => property.Name,
                    property => property.Value.GetString()!,
                    StringComparer.Ordinal);

            (int ruleMismatches, int qualifiedLoci, bool qualified) =
                GenisMatchingRuleEvaluator.Evaluate(
                    detailed,
                    rule.MinimumStringency,
                    rule.MismatchsAllowed,
                    rule.MinLocusMatch);

            parsed.Add(new ParsedAutosomalResult(
                candidateId,
                item.GetProperty("rank").GetInt32(),
                overall,
                rawMismatches,
                sharedMarkers,
                leftPonderation,
                rightPonderation,
                ruleMismatches,
                qualifiedLoci,
                qualified,
                detailedElement.GetRawText()));
        }

        int qualifiedCount = parsed.Count(x => x.RuleQualified);
        AutosomalMatchSearch search = AutosomalMatchSearch.Create(
            command.QueryProfileId,
            rule.Id,
            call.AnalysisRunId,
            candidateProfiles.Count,
            qualifiedCount,
            mixture);

        dbContext.AutosomalMatchSearches.Add(search);

        List<AutosomalMatchResult> results = new(parsed.Count);
        List<MatchHit> hits = [];

        foreach (ParsedAutosomalResult item in parsed)
        {
            AutosomalMatchResult result = AutosomalMatchResult.Create(
                search.Id,
                item.CandidateProfileId,
                item.Rank,
                item.RawOverall,
                item.RawMismatches,
                item.SharedMarkers,
                item.LeftPonderation,
                item.RightPonderation,
                item.RuleMismatches,
                item.RuleQualifiedLoci,
                item.RuleQualified,
                item.DetailedJson);

            results.Add(result);
            dbContext.AutosomalMatchResults.Add(result);

            if (item.RuleQualified)
            {
                MatchHit hit = MatchHit.Create(
                    search.Id,
                    result.Id,
                    command.QueryProfileId,
                    item.CandidateProfileId);
                hits.Add(hit);
                dbContext.MatchHits.Add(hit);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return MatchingMapper.ToSearchDto(search, results, hits);
    }

    private static Dictionary<string, string[]> ToGenotypification(GeneticProfileDto profile)
    {
        Dictionary<string, string[]> genotype = profile.Loci
            .Where(x => x.Alleles.Count > 0)
            .ToDictionary(
                x => x.Marker,
                x => x.Alleles.OrderBy(a => a.SortOrder).Select(a => a.Value).ToArray(),
                StringComparer.Ordinal);

        if (genotype.Count == 0)
        {
            throw new InvalidOperationException(
                $"Genetic profile {profile.Id} contains no allele calls.");
        }

        return genotype;
    }

    private sealed record ParsedAutosomalResult(
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
}

public sealed class ReviewMatchHitCommandHandler(
    MatchingDbContext dbContext,
    ICurrentUser currentUser)
    : ICommandHandler<ReviewMatchHitCommand, Guid>
{
    private static readonly HashSet<string> AllowedStatuses =
        new(StringComparer.Ordinal)
        {
            "PendingReview",
            "Confirmed",
            "Dismissed",
            "Inconclusive",
        };

    public async ValueTask<Guid> Handle(
        ReviewMatchHitCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (!AllowedStatuses.Contains(command.Status))
        {
            throw new CustomException(
                "Hit status must be PendingReview, Confirmed, Dismissed, or Inconclusive.",
                (IEnumerable<string>?)null,
                HttpStatusCode.BadRequest);
        }

        MatchHit hit = await dbContext.MatchHits
            .FirstOrDefaultAsync(x => x.Id == command.HitId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException($"Match hit {command.HitId} not found.");

        hit.Review(command.Status, command.ReviewNote, currentUser.GetUserId());
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return hit.Id;
    }
}
