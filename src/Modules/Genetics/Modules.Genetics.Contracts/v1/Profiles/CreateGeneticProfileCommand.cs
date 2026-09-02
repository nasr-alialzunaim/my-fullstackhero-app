using Mediator;

namespace FSH.Modules.Genetics.Contracts.v1.Profiles;

public sealed record PeakObservationInput(
    string? AlleleValue = null,
    double? HeightRfu = null,
    double? SizeBp = null,
    string? Channel = null);

public sealed record ProfileLocusInput(
    string Marker,
    IReadOnlyList<string> Alleles,
    IReadOnlyList<PeakObservationInput> Peaks);

public sealed record CreateGeneticProfileCommand(
    Guid SampleId,
    string? ExternalProfileCode,
    int? AnalysisTypeId,
    bool IsReference,
    Guid? SupersedesProfileId,
    IReadOnlyList<ProfileLocusInput> Loci) : ICommand<Guid>;
