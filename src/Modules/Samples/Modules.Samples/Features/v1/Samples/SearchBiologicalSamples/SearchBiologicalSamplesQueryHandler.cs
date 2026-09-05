using FSH.Framework.Shared.Persistence;
using FSH.Modules.Samples.Contracts.Dtos;
using FSH.Modules.Samples.Contracts.v1.Samples;
using FSH.Modules.Samples.Data;
using FSH.Modules.Samples.Domain;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Samples.Features.v1.Samples.SearchBiologicalSamples;

public sealed class SearchBiologicalSamplesQueryHandler(SamplesDbContext dbContext) : IQueryHandler<SearchBiologicalSamplesQuery, PagedResponse<BiologicalSampleDto>>
{
    public async ValueTask<PagedResponse<BiologicalSampleDto>> Handle(SearchBiologicalSamplesQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);
        int page = query.PageNumber; int size = query.PageSize;
        IQueryable<BiologicalSample> samples = dbContext.BiologicalSamples.AsNoTracking();

        if (query.CaseId.HasValue) samples = samples.Where(x => x.CaseId == query.CaseId.Value);
        if (query.SubjectId.HasValue) samples = samples.Where(x => x.SubjectId == query.SubjectId.Value);
        if (!string.IsNullOrWhiteSpace(query.SampleContext) && Enum.TryParse(query.SampleContext, true, out SampleContext context)) samples = samples.Where(x => x.SampleContext == context);
        if (!string.IsNullOrWhiteSpace(query.Status) && Enum.TryParse(query.Status, true, out SampleStatus status)) samples = samples.Where(x => x.Status == status);
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            string term = query.Search.Trim();
            samples = samples.Where(x => EF.Functions.ILike(x.SampleCode, $"%{term}%") || (x.ExternalSampleCode != null && EF.Functions.ILike(x.ExternalSampleCode, $"%{term}%")) || (x.CollectionNote != null && EF.Functions.ILike(x.CollectionNote, $"%{term}%")));
        }

        long total = await samples.LongCountAsync(cancellationToken).ConfigureAwait(false);
        List<BiologicalSample> rows = await samples.OrderByDescending(x => x.CreatedAtUtc).Skip((page - 1) * size).Take(size).ToListAsync(cancellationToken).ConfigureAwait(false);
        return new PagedResponse<BiologicalSampleDto>
        {
            Items = rows.Select(Map).ToList(), PageNumber = page, PageSize = size, TotalCount = total, TotalPages = (int)Math.Ceiling(total / (double)size),
        };
    }

    private static BiologicalSampleDto Map(BiologicalSample x) => new(x.Id, x.SampleCode, x.ExternalSampleCode, x.SampleContext.ToString(), x.CaseId, x.SubjectId, x.ParentSampleId, x.SampleType, x.Matrix, x.CollectionLocation, x.CollectedAtUtc, x.CollectionNote, x.ContainerCode, x.SealNumber, x.Status.ToString(), x.CreatedByUserId, x.CreatedAtUtc, x.UpdatedAtUtc);
}
