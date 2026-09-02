namespace FSH.Modules.Genetics.Contracts;

public readonly record struct GeneticProfileId
{
    public GeneticProfileId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Genetic profile identity cannot be empty.", nameof(value));
        }

        Value = value;
    }

    public Guid Value { get; }

    public static GeneticProfileId New() => new(Guid.CreateVersion7());

    public override string ToString() =>
        Value.ToString("D", System.Globalization.CultureInfo.InvariantCulture);
}
