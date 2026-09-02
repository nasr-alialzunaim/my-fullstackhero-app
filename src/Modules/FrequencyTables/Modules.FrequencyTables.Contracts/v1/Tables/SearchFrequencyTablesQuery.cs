using FSH.Framework.Shared.Persistence;
using FSH.Modules.FrequencyTables.Contracts.Dtos;
using Mediator;

namespace FSH.Modules.FrequencyTables.Contracts.v1.Tables;

public sealed record SearchFrequencyTablesQuery(
    string? Search = null,
    int PageNumber = 1,
    int PageSize = 20) : IQuery<PagedResponse<FrequencyTableSummaryDto>>;
