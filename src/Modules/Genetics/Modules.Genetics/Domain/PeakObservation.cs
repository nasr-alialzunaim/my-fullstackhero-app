namespace FSH.Modules.Genetics.Domain;

public sealed class PeakObservation
{
    public Guid Id { get; private set; }
    public Guid ProfileLocusId { get; private set; }
    public string? AlleleValue { get; private set; }
    public double? HeightRfu { get; private set; }
    public double? SizeBp { get; private set; }
    public string? Channel { get; private set; }
    public int SortOrder { get; private set; }

    private PeakObservation()
    {
    }

    public static PeakObservation Create(
        Guid locusId,
        string? alleleValue,
        double? heightRfu,
        double? sizeBp,
        string? channel,
        int sortOrder)
    {
        if (locusId == Guid.Empty)
        {
            throw new ArgumentException("Locus identity cannot be empty.", nameof(locusId));
        }

        return new PeakObservation
        {
            Id = Guid.CreateVersion7(),
            ProfileLocusId = locusId,
            AlleleValue = Normalize(alleleValue),
            HeightRfu = heightRfu,
            SizeBp = sizeBp,
            Channel = Normalize(channel),
            SortOrder = sortOrder,
        };
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
