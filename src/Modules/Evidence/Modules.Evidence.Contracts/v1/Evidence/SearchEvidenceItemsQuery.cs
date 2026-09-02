using FSH.Framework.Shared.Persistence;
using FSH.Modules.Evidence.Contracts.Dtos;
using Mediator;

namespace FSH.Modules.Evidence.Contracts.v1.Evidence;

public sealed record SearchEvidenceItemsQuery(
    Guid? CaseId = null,
    string? Search = null,
    int PageNumber = 1,
    int PageSize = 20) : IQuery<PagedResponse<EvidenceItemDto>>;
