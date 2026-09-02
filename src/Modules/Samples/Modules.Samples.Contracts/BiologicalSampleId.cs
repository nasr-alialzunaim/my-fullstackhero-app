namespace FSH.Modules.Samples.Contracts;

public readonly record struct BiologicalSampleId
{
    public BiologicalSampleId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Sample identity cannot be empty.", nameof(value));
        }

        Value = value;
    }

    public Guid Value { get; }

    public static BiologicalSampleId New() => new(Guid.CreateVersion7());

    public override string ToString() =>
        Value.ToString("D", System.Globalization.CultureInfo.InvariantCulture);
}
