namespace FSH.Modules.FrequencyTables.Domain;

public sealed class FrequencyEntry
{
    public Guid Id { get; private set; }
    public Guid FrequencyTableId { get; private set; }
    public string Marker { get; private set; } = default!;
    public string Allele { get; private set; } = default!;
    public double Frequency { get; private set; }

    private FrequencyEntry() { }

    public static FrequencyEntry Create(
        Guid tableId,
        string marker,
        string allele,
        double frequency)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(marker);
        ArgumentException.ThrowIfNullOrWhiteSpace(allele);

        return new FrequencyEntry
        {
            Id = Guid.CreateVersion7(),
            FrequencyTableId = tableId,
            Marker = marker.Trim(),
            Allele = NormalizeAllele(allele),
            Frequency = frequency,
        };
    }

    private static string NormalizeAllele(string allele)
    {
        string normalized = allele.Trim().Replace(',', '.');
        return normalized == "-1.0" ? "-1" : normalized;
    }
}
