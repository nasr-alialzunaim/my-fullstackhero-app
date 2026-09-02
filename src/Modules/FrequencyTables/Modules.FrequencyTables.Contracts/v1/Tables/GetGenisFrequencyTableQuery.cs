using Mediator;

namespace FSH.Modules.FrequencyTables.Contracts.v1.Tables;

public sealed record GetGenisFrequencyTableQuery(Guid TableId)
    : IQuery<Dictionary<string, Dictionary<string, double>>>;
