using FSH.Modules.DNA.Contracts.v1.Cases;
using FSH.Modules.DNA.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.DNA.Features.v1.Cases.ListCases;

public sealed class ListCasesQueryHandler(DnaDbContext dbContext)
    : IQueryHandler<ListCasesQuery, IReadOnlyList<CaseListItem>>
{
    public async ValueTask<IReadOnlyList<CaseListItem>> Handle(
        ListCasesQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        return await dbContext.Cases
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new CaseListItem(
                x.Id,
                x.CaseNumber,
                x.Title,
                x.Status.ToString(),
                x.CreatedAtUtc))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
