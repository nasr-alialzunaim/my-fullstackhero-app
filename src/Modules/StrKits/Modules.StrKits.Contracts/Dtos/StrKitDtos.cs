namespace FSH.Modules.StrKits.Contracts.Dtos;

public sealed record StrKitLocusDto(
    Guid Id,
    string Marker,
    string? Chromosome,
    int MinimumAllelesQty,
    int MaximumAllelesQty,
    string? Fluorophore,
    int Order,
    bool Required,
    double? AlleleRangeMin,
    double? AlleleRangeMax);

public sealed record StrKitDto(
    Guid Id,
    string KitCode,
    string Name,
    int AnalysisTypeId,
    int RepresentativeParameter,
    int VersionNumber,
    Guid? SupersedesKitId,
    IReadOnlyList<string> Aliases,
    IReadOnlyList<StrKitLocusDto> Loci,
    DateTime CreatedAtUtc);

public sealed record StrKitSummaryDto(
    Guid Id,
    string KitCode,
    string Name,
    int AnalysisTypeId,
    int VersionNumber,
    int LocusCount,
    DateTime CreatedAtUtc);
