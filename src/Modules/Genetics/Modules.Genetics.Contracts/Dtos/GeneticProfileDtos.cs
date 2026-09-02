namespace FSH.Modules.Genetics.Contracts.Dtos;

public sealed record PeakObservationDto(
    Guid Id,
    string? AlleleValue,
    double? HeightRfu,
    double? SizeBp,
    string? Channel,
    int SortOrder);

public sealed record AlleleCallDto(
    Guid Id,
    string Value,
    int SortOrder);

public sealed record ProfileLocusDto(
    Guid Id,
    string Marker,
    IReadOnlyList<AlleleCallDto> Alleles,
    IReadOnlyList<PeakObservationDto> Peaks);

public sealed record GeneticProfileDto(
    Guid Id,
    Guid SampleId,
    string? ExternalProfileCode,
    Guid? StrKitId,
    int? Contributors,
    int VersionNumber,
    Guid? SupersedesProfileId,
    int? AnalysisTypeId,
    bool IsReference,
    DateTime CreatedAtUtc,
    IReadOnlyList<ProfileLocusDto> Loci);

public sealed record GeneticProfileSummaryDto(
    Guid Id,
    Guid SampleId,
    string? ExternalProfileCode,
    Guid? StrKitId,
    int? Contributors,
    int VersionNumber,
    Guid? SupersedesProfileId,
    int? AnalysisTypeId,
    bool IsReference,
    int LocusCount,
    DateTime CreatedAtUtc);