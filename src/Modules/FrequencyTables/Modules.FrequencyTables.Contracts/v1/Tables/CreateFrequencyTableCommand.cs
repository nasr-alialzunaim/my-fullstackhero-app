using Mediator;

namespace FSH.Modules.FrequencyTables.Contracts.v1.Tables;

public sealed record FrequencyEntryInput(
    string Marker,
    string Allele,
    double Frequency);

public sealed record CreateFrequencyTableCommand(
    string Name,
    string Model,
    double Theta,
    Guid? SupersedesTableId,
    IReadOnlyList<FrequencyEntryInput> Entries) : ICommand<Guid>;
