using Mediator;

namespace FSH.Modules.StrKits.Contracts.v1.Kits;

public sealed record StrKitLocusInput(
    string Marker,
    string? Chromosome,
    int MinimumAllelesQty,
    int MaximumAllelesQty,
    string? Fluorophore,
    int Order,
    bool Required,
    double? AlleleRangeMin = null,
    double? AlleleRangeMax = null);

public sealed record CreateStrKitCommand(
    string KitCode,
    string Name,
    int AnalysisTypeId,
    int RepresentativeParameter,
    Guid? SupersedesKitId,
    IReadOnlyList<string> Aliases,
    IReadOnlyList<StrKitLocusInput> Loci) : ICommand<Guid>;
