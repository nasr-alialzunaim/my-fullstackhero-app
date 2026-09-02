using FSH.Modules.FrequencyTables.Contracts.Dtos;
using Mediator;

namespace FSH.Modules.FrequencyTables.Contracts.v1.Tables;

public sealed record GetFrequencyTableByIdQuery(Guid TableId) : IQuery<FrequencyTableDto>;
