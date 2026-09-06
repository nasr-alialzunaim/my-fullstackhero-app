using FSH.Framework.Shared.Persistence;
using FSH.Modules.Cases.Contracts.Dtos;
using FSH.Modules.Cases.Contracts.v1.Cases;
using FSH.Modules.Cases.Data;
using FSH.Modules.Cases.Domain;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Cases.Features.v1.Cases.SearchCases;

public sealed class SearchCasesQueryHandler(CasesDbContext dbContext)
    : IQueryHandler<SearchCasesQuery, PagedResponse<CaseDto>>
{
    public async ValueTask<PagedResponse<CaseDto>> Handle(
        SearchCasesQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        int page = query.PageNumber < 1 ? 1 : query.PageNumber;
        int size = query.PageSize is < 1 or > 200 ? 20 : query.PageSize;

        IQueryable<ForensicCase> cases = dbContext.Cases.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            string term = query.Search.Trim();
            cases = cases.Where(x =>
                EF.Functions.ILike(x.Number, $"%{term}%") ||
                EF.Functions.ILike(x.Title, $"%{term}%") ||
                (x.Description != null && EF.Functions.ILike(x.Description, $"%{term}%")));
        }

        cases = ApplySort(cases, query.SortBy, query.SortDir);

        long total = await cases.LongCountAsync(cancellationToken).ConfigureAwait(false);
        List<ForensicCase> rows = await cases
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return new PagedResponse<CaseDto>
        {
            Items = rows.Select(x => new CaseDto(
                x.Id,
                x.Number,
                x.Title,
                x.Description,
                x.CreatedAtUtc,
                x.UpdatedAtUtc)).ToList(),
            PageNumber = page,
            PageSize = size,
            TotalCount = total,
            TotalPages = (int)Math.Ceiling(total / (double)size),
        };
    }

    private static IQueryable<ForensicCase> ApplySort(
        IQueryable<ForensicCase> query,
        string? sortBy,
        string? sortDir)
    {
        bool descending = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);

        return sortBy?.ToUpperInvariant() switch
        {
            "NUMBER" => descending ? query.OrderByDescending(x => x.Number) : query.OrderBy(x => x.Number),
            "TITLE" => descending ? query.OrderByDescending(x => x.Title) : query.OrderBy(x => x.Title),
            "CREATEDATUTC" or "CREATED" => descending
                ? query.OrderByDescending(x => x.CreatedAtUtc)
                : query.OrderBy(x => x.CreatedAtUtc),
            _ => query.OrderByDescending(x => x.CreatedAtUtc),
        };
    }
}