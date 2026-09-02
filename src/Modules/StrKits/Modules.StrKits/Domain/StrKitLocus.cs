namespace FSH.Modules.StrKits.Domain;

public sealed class StrKitLocus
{
    public Guid Id { get; private set; }
    public Guid StrKitId { get; private set; }
    public string Marker { get; private set; } = default!;
    public string? Chromosome { get; private set; }
    public int MinimumAllelesQty { get; private set; }
    public int MaximumAllelesQty { get; private set; }
    public string? Fluorophore { get; private set; }
    public int Order { get; private set; }
    public bool Required { get; private set; }
    public double? AlleleRangeMin { get; private set; }
    public double? AlleleRangeMax { get; private set; }

    private StrKitLocus() { }

    public static StrKitLocus Create(
        Guid strKitId,
        string marker,
        string? chromosome,
        int minimumAllelesQty,
        int maximumAllelesQty,
        string? fluorophore,
        int order,
        bool required,
        double? alleleRangeMin,
        double? alleleRangeMax)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(marker);

        return new StrKitLocus
        {
            Id = Guid.CreateVersion7(),
            StrKitId = strKitId,
            Marker = marker.Trim(),
            Chromosome = Normalize(chromosome),
            MinimumAllelesQty = minimumAllelesQty,
            MaximumAllelesQty = maximumAllelesQty,
            Fluorophore = Normalize(fluorophore),
            Order = order,
            Required = required,
            AlleleRangeMin = alleleRangeMin,
            AlleleRangeMax = alleleRangeMax,
        };
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
