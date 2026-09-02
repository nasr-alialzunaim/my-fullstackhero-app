using FSH.Framework.Core.Exceptions;
using FSH.Modules.Cases.Contracts.Dtos;
using FSH.Modules.Cases.Contracts.v1.Cases;
using FSH.Modules.Cases.Data;
using FSH.Modules.Cases.Domain;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Cases.Features.v1.Cases.GetCaseById;

public sealed class GetCaseByIdQueryHandler(CasesDbContext dbContext)
    : IQueryHandler<GetCaseByIdQuery, CaseDto>
{
    public async ValueTask<CaseDto> Handle(GetCaseByIdQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        ForensicCase entity = await dbContext.Cases
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == query.CaseId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException($"Case {query.CaseId} not found.");

        return new CaseDto(
            entity.Id,
            entity.Number,
            entity.Title,
            entity.Description,
            entity.CreatedAtUtc,
            entity.UpdatedAtUtc);
    }
}