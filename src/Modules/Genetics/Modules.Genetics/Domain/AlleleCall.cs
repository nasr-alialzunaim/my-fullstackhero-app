namespace FSH.Modules.Genetics.Domain;

public sealed class AlleleCall
{
    public Guid Id { get; private set; }
    public Guid ProfileLocusId { get; private set; }
    public string Value { get; private set; } = default!;
    public int SortOrder { get; private set; }

    private AlleleCall()
    {
    }

    public static AlleleCall Create(
        Guid locusId,
        string value,
        int sortOrder)
    {
        if (locusId == Guid.Empty)
        {
            throw new ArgumentException("Locus identity cannot be empty.", nameof(locusId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        return new AlleleCall
        {
            Id = Guid.CreateVersion7(),
            ProfileLocusId = locusId,
            Value = value.Trim(),
            SortOrder = sortOrder,
        };
    }
}
