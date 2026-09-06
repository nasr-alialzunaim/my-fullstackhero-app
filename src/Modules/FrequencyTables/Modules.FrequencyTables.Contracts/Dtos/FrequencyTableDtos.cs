namespace FSH.Modules.FrequencyTables.Contracts.Dtos;

public sealed record FrequencyEntryDto(
    Guid Id,
    string Marker,
    string Allele,
    double Frequency);

public sealed record FrequencyTableDto(
    Guid Id,
    string Name,
    string Model,
    double Theta,
    int VersionNumber,
    Guid? SupersedesTableId,
    bool IsActive,
    bool IsDefault,
    IReadOnlyList<FrequencyEntryDto> Entries,
    DateTime CreatedAtUtc);

public sealed record FrequencyTableSummaryDto(
    Guid Id,
    string Name,
    string Model,
    double Theta,
    int VersionNumber,
    bool IsActive,
    bool IsDefault,
    int MarkerCount,
    int EntryCount,
    DateTime CreatedAtUtc);
