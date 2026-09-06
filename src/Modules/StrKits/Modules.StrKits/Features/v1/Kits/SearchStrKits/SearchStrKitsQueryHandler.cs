using FSH.Framework.Shared.Persistence;
using FSH.Modules.StrKits.Contracts.Dtos;
using FSH.Modules.StrKits.Contracts.v1.Kits;
using FSH.Modules.StrKits.Data;
using FSH.Modules.StrKits.Domain;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.StrKits.Features.v1.Kits.SearchStrKits;

public sealed class SearchStrKitsQueryHandler(StrKitsDbContext dbContext)
    : IQueryHandler<SearchStrKitsQuery, PagedResponse<StrKitSummaryDto>>
{
    public async ValueTask<PagedResponse<StrKitSummaryDto>> Handle(
        SearchStrKitsQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        int page = query.PageNumber < 1 ? 1 : query.PageNumber;
        int size = query.PageSize is < 1 or > 200 ? 20 : query.PageSize;
        IQueryable<StrKit> kits = dbContext.StrKits.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            string term = query.Search.Trim();
            kits = kits.Where(x =>
                EF.Functions.ILike(x.KitCode, $"%{term}%") ||
                EF.Functions.ILike(x.Name, $"%{term}%"));
        }

        kits = kits.OrderBy(x => x.KitCode).ThenByDescending(x => x.VersionNumber);
        long total = await kits.LongCountAsync(cancellationToken).ConfigureAwait(false);
        List<StrKit> rows = await kits.Skip((page - 1) * size).Take(size)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
        Guid[] ids = rows.Select(x => x.Id).ToArray();
        Dictionary<Guid, int> counts = await dbContext.StrKitLoci.AsNoTracking()
            .Where(x => ids.Contains(x.StrKitId))
            .GroupBy(x => x.StrKitId)
            .Select(g => new { Id = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Id, x => x.Count, cancellationToken)
            .ConfigureAwait(false);

        return new PagedResponse<StrKitSummaryDto>
        {
            Items = rows.Select(x => new StrKitSummaryDto(
                x.Id,
                x.KitCode,
                x.Name,
                x.AnalysisTypeId,
                x.VersionNumber,
                counts.GetValueOrDefault(x.Id),
                x.CreatedAtUtc)).ToList(),
            PageNumber = page,
            PageSize = size,
            TotalCount = total,
            TotalPages = (int)Math.Ceiling(total / (double)size),
        };
    }
}
