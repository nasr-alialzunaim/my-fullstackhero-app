using Mediator;

namespace FSH.Modules.FrequencyTables.Contracts.v1.Tables;

public sealed record ToggleFrequencyTableActiveCommand(Guid TableId) : ICommand<Guid>;
