using FSH.Framework.Shared.Persistence;
using FSH.Modules.Genetics.Contracts.Dtos;
using FSH.Modules.Genetics.Contracts.v1.Profiles;
using FSH.Modules.Genetics.Data;
using FSH.Modules.Genetics.Domain;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Genetics.Features.v1.Profiles.SearchGeneticProfiles;

public sealed class SearchGeneticProfilesQueryHandler(GeneticsDbContext dbContext)
    : IQueryHandler<SearchGeneticProfilesQuery, PagedResponse<GeneticProfileSummaryDto>>
{
    public async ValueTask<PagedResponse<GeneticProfileSummaryDto>> Handle(
        SearchGeneticProfilesQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        int page = query.PageNumber < 1 ? 1 : query.PageNumber;
        int size = query.PageSize is < 1 or > 200 ? 20 : query.PageSize;

        IQueryable<GeneticProfile> profiles = dbContext.GeneticProfiles.AsNoTracking();

        if (query.SampleId.HasValue)
        {
            profiles = profiles.Where(x => x.SampleId == query.SampleId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            string term = query.Search.Trim();
            profiles = profiles.Where(x =>
                x.ExternalProfileCode != null &&
                EF.Functions.ILike(x.ExternalProfileCode, $"%{term}%"));
        }

        profiles = profiles.OrderByDescending(x => x.CreatedAtUtc);

        long total = await profiles.LongCountAsync(cancellationToken).ConfigureAwait(false);
        List<GeneticProfile> rows = await profiles
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        Guid[] profileIds = rows.Select(x => x.Id).ToArray();

        Dictionary<Guid, int> locusCounts = await dbContext.ProfileLoci
            .AsNoTracking()
            .Where(x => profileIds.Contains(x.GeneticProfileId))
            .GroupBy(x => x.GeneticProfileId)
            .Select(g => new { ProfileId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.ProfileId, x => x.Count, cancellationToken)
            .ConfigureAwait(false);

        return new PagedResponse<GeneticProfileSummaryDto>
        {
            Items = rows.Select(x => new GeneticProfileSummaryDto(
                x.Id,
                x.SampleId,
                x.ExternalProfileCode,
                x.StrKitId,
                x.Contributors,
                x.VersionNumber,
                x.SupersedesProfileId,
                x.AnalysisTypeId,
                x.IsReference,
                locusCounts.GetValueOrDefault(x.Id),
                x.CreatedAtUtc)).ToList(),
            PageNumber = page,
            PageSize = size,
            TotalCount = total,
            TotalPages = (int)Math.Ceiling(total / (double)size),
        };
    }
}