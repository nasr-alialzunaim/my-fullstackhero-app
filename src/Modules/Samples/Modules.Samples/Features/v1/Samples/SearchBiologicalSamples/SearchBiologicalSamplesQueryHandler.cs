using FSH.Framework.Shared.Persistence;
using FSH.Modules.Samples.Contracts.Dtos;
using FSH.Modules.Samples.Contracts.v1.Samples;
using FSH.Modules.Samples.Data;
using FSH.Modules.Samples.Domain;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Samples.Features.v1.Samples.SearchBiologicalSamples;

public sealed class SearchBiologicalSamplesQueryHandler(SamplesDbContext dbContext)
    : IQueryHandler<SearchBiologicalSamplesQuery, PagedResponse<BiologicalSampleDto>>
{
    public async ValueTask<PagedResponse<BiologicalSampleDto>> Handle(
        SearchBiologicalSamplesQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        int page = query.PageNumber < 1 ? 1 : query.PageNumber;
        int size = query.PageSize is < 1 or > 200 ? 20 : query.PageSize;

        IQueryable<BiologicalSample> samples = dbContext.BiologicalSamples.AsNoTracking();

        if (query.EvidenceItemId.HasValue)
        {
            samples = samples.Where(x => x.EvidenceItemId == query.EvidenceItemId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            string term = query.Search.Trim();
            samples = samples.Where(x =>
                (x.ExternalSampleCode != null &&
                    EF.Functions.ILike(x.ExternalSampleCode, $"%{term}%")) ||
                (x.CollectionNote != null &&
                    EF.Functions.ILike(x.CollectionNote, $"%{term}%")));
        }

        samples = samples.OrderByDescending(x => x.CreatedAtUtc);

        long total = await samples.LongCountAsync(cancellationToken).ConfigureAwait(false);
        List<BiologicalSample> rows = await samples
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return new PagedResponse<BiologicalSampleDto>
        {
            Items = rows.Select(x => new BiologicalSampleDto(
                x.Id,
                x.EvidenceItemId,
                x.ParentSampleId,
                x.ExternalSampleCode,
                x.CollectedAtUtc,
                x.CollectionNote,
                x.CreatedAtUtc)).ToList(),
            PageNumber = page,
            PageSize = size,
            TotalCount = total,
            TotalPages = (int)Math.Ceiling(total / (double)size),
        };
    }
}
