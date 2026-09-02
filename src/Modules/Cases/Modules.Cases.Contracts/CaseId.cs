namespace FSH.Modules.Cases.Contracts;

/// <summary>
/// Installation-local, immutable identity of a forensic case.
/// External case numbers remain separate from this identity.
/// </summary>
public readonly record struct CaseId
{
    public CaseId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Case identity cannot be empty.", nameof(value));
        }

        Value = value;
    }

    public Guid Value { get; }

    public static CaseId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString("D", System.Globalization.CultureInfo.InvariantCulture);
}
