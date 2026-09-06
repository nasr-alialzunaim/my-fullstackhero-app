using FSH.Framework.Shared.Persistence;
using FSH.Modules.StrKits.Contracts.Dtos;
using Mediator;

namespace FSH.Modules.StrKits.Contracts.v1.Kits;

public sealed record SearchStrKitsQuery(
    string? Search = null,
    int PageNumber = 1,
    int PageSize = 20) : IQuery<PagedResponse<StrKitSummaryDto>>;
