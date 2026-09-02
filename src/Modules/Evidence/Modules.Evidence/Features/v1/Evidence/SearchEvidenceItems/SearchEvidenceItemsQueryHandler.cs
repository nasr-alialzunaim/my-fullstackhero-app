using FSH.Framework.Shared.Persistence;
using FSH.Modules.Evidence.Contracts.Dtos;
using FSH.Modules.Evidence.Contracts.v1.Evidence;
using FSH.Modules.Evidence.Data;
using FSH.Modules.Evidence.Domain;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Evidence.Features.v1.Evidence.SearchEvidenceItems;

public sealed class SearchEvidenceItemsQueryHandler(EvidenceDbContext dbContext)
    : IQueryHandler<SearchEvidenceItemsQuery, PagedResponse<EvidenceItemDto>>
{
    public async ValueTask<PagedResponse<EvidenceItemDto>> Handle(
        SearchEvidenceItemsQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        int page = query.PageNumber < 1 ? 1 : query.PageNumber;
        int size = query.PageSize is < 1 or > 200 ? 20 : query.PageSize;

        IQueryable<EvidenceItem> items = dbContext.EvidenceItems.AsNoTracking();

        if (query.CaseId.HasValue)
        {
            items = items.Where(x => x.CaseId == query.CaseId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            string term = query.Search.Trim();
            items = items.Where(x =>
                (x.ExternalReference != null &&
                    EF.Functions.ILike(x.ExternalReference, $"%{term}%")) ||
                (x.Description != null &&
                    EF.Functions.ILike(x.Description, $"%{term}%")));
        }

        items = items.OrderByDescending(x => x.CreatedAtUtc);

        long total = await items.LongCountAsync(cancellationToken).ConfigureAwait(false);
        List<EvidenceItem> rows = await items
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return new PagedResponse<EvidenceItemDto>
        {
            Items = rows.Select(x => new EvidenceItemDto(
                x.Id,
                x.CaseId,
                x.ExternalReference,
                x.Description,
                x.CreatedAtUtc)).ToList(),
            PageNumber = page,
            PageSize = size,
            TotalCount = total,
            TotalPages = (int)Math.Ceiling(total / (double)size),
        };
    }
}
