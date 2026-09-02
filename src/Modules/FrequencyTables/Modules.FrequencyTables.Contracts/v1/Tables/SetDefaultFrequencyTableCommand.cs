using Mediator;

namespace FSH.Modules.FrequencyTables.Contracts.v1.Tables;

public sealed record SetDefaultFrequencyTableCommand(Guid TableId) : ICommand<Guid>;
