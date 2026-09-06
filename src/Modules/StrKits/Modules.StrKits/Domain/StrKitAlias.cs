namespace FSH.Modules.StrKits.Domain;

public sealed class StrKitAlias
{
    public Guid Id { get; private set; }
    public Guid StrKitId { get; private set; }
    public string Alias { get; private set; } = default!;

    private StrKitAlias() { }

    public static StrKitAlias Create(Guid strKitId, string alias)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(alias);
        return new StrKitAlias
        {
            Id = Guid.CreateVersion7(),
            StrKitId = strKitId,
            Alias = alias.Trim(),
        };
    }
}
