namespace FSH.Modules.Evidence.Contracts;

public readonly record struct EvidenceItemId
{
    public EvidenceItemId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Evidence identity cannot be empty.", nameof(value));
        }

        Value = value;
    }

    public Guid Value { get; }

    public static EvidenceItemId New() => new(Guid.CreateVersion7());

    public override string ToString() =>
        Value.ToString("D", System.Globalization.CultureInfo.InvariantCulture);
}
