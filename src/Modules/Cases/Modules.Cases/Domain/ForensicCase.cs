using FSH.Framework.Core.Domain;

namespace FSH.Modules.Cases.Domain;

/// <summary>
/// Authoritative case record for the installation. Human-readable case numbers are
/// external references and never replace the immutable internal identifier.
/// </summary>
public sealed class ForensicCase : AggregateRoot<Guid>
{
    public string Number { get; private set; } = default!;
    public string Title { get; private set; } = default!;
    public string? Description { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    private ForensicCase()
    {
    }

    public static ForensicCase Create(string number, string title, string? description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(number);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        return new ForensicCase
        {
            Id = Guid.CreateVersion7(),
            Number = number.Trim(),
            Title = title.Trim(),
            Description = NormalizeOptional(description),
            CreatedAtUtc = DateTime.UtcNow,
        };
    }

    public void Update(string number, string title, string? description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(number);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        Number = number.Trim();
        Title = title.Trim();
        Description = NormalizeOptional(description);
        UpdatedAtUtc = DateTime.UtcNow;
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}