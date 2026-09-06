using FSH.Framework.Core.Exceptions;
using FSH.Modules.Evidence.Contracts.Dtos;
using FSH.Modules.Evidence.Contracts.v1.Evidence;
using FSH.Modules.Evidence.Data;
using FSH.Modules.Evidence.Domain;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Evidence.Features.v1.Evidence.GetEvidenceItemById;

public sealed class GetEvidenceItemByIdQueryHandler(EvidenceDbContext dbContext)
    : IQueryHandler<GetEvidenceItemByIdQuery, EvidenceItemDto>
{
    public async ValueTask<EvidenceItemDto> Handle(
        GetEvidenceItemByIdQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        EvidenceItem entity = await dbContext.EvidenceItems
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == query.EvidenceItemId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException($"Evidence item {query.EvidenceItemId} not found.");

        return new EvidenceItemDto(
            entity.Id,
            entity.CaseId,
            entity.ExternalReference,
            entity.Description,
            entity.CreatedAtUtc);
    }
}
