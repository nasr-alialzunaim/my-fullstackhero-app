using FSH.Framework.Shared.Persistence;
using FSH.Modules.Cases.Contracts.Dtos;
using Mediator;

namespace FSH.Modules.Cases.Contracts.v1.Cases;

public sealed record SearchCasesQuery(
    string? Search = null,
    int PageNumber = 1,
    int PageSize = 20,
    string? SortBy = null,
    string? SortDir = null) : IQuery<PagedResponse<CaseDto>>;