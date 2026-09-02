using FSH.Framework.Shared.Persistence;
using FSH.Modules.FrequencyTables.Contracts.Dtos;
using FSH.Modules.FrequencyTables.Contracts.v1.Tables;
using FSH.Modules.FrequencyTables.Data;
using FSH.Modules.FrequencyTables.Domain;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.FrequencyTables.Features.v1.Tables.SearchFrequencyTables;

public sealed class SearchFrequencyTablesQueryHandler(FrequencyTablesDbContext dbContext)
    : IQueryHandler<SearchFrequencyTablesQuery, PagedResponse<FrequencyTableSummaryDto>>
{
    public async ValueTask<PagedResponse<FrequencyTableSummaryDto>> Handle(
        SearchFrequencyTablesQuery query,
        CancellationToken cancellationToken)
    {
        int page = query.PageNumber < 1 ? 1 : query.PageNumber;
        int size = query.PageSize is < 1 or > 200 ? 20 : query.PageSize;
        IQueryable<FrequencyTable> tables = dbContext.FrequencyTables.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            string term = query.Search.Trim();
            tables = tables.Where(x => EF.Functions.ILike(x.Name, $"%{term}%"));
        }

        tables = tables.OrderByDescending(x => x.IsDefault)
            .ThenByDescending(x => x.IsActive)
            .ThenBy(x => x.Name)
            .ThenByDescending(x => x.VersionNumber);

        long total = await tables.LongCountAsync(cancellationToken).ConfigureAwait(false);
        List<FrequencyTable> rows = await tables.Skip((page - 1) * size).Take(size)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
        Guid[] ids = rows.Select(x => x.Id).ToArray();

        var stats = await dbContext.FrequencyEntries.AsNoTracking()
            .Where(x => ids.Contains(x.FrequencyTableId))
            .GroupBy(x => x.FrequencyTableId)
            .Select(g => new
            {
                Id = g.Key,
                EntryCount = g.Count(),
                MarkerCount = g.Select(x => x.Marker).Distinct().Count(),
            })
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        return new PagedResponse<FrequencyTableSummaryDto>
        {
            Items = rows.Select(x =>
            {
                var stat = stats.FirstOrDefault(s => s.Id == x.Id);
                return new FrequencyTableSummaryDto(
                    x.Id, x.Name, x.Model, x.Theta, x.VersionNumber, x.IsActive, x.IsDefault,
                    stat?.MarkerCount ?? 0, stat?.EntryCount ?? 0, x.CreatedAtUtc);
            }).ToList(),
            PageNumber = page,
            PageSize = size,
            TotalCount = total,
            TotalPages = (int)Math.Ceiling(total / (double)size),
        };
    }
}
