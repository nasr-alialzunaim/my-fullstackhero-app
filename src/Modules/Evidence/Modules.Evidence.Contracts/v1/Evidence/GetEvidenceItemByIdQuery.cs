using FSH.Modules.Evidence.Contracts.Dtos;
using Mediator;

namespace FSH.Modules.Evidence.Contracts.v1.Evidence;

public sealed record GetEvidenceItemByIdQuery(Guid EvidenceItemId)
    : IQuery<EvidenceItemDto>;
