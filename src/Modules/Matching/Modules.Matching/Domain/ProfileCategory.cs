using FSH.Framework.Core.Domain;

namespace FSH.Modules.Matching.Domain;

public sealed class ProfileCategory : AggregateRoot<Guid>
{
    public string Code { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public int AnalysisTypeId { get; private set; }
    public bool IsReference { get; private set; }
    public bool Mitochondrial { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private ProfileCategory() { }

    public static ProfileCategory Create(
        string code,
        string name,
        int analysisTypeId,
        bool isReference,
        bool mitochondrial)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new ProfileCategory
        {
            Id = Guid.CreateVersion7(),
            Code = code.Trim(),
            Name = name.Trim(),
            AnalysisTypeId = analysisTypeId,
            IsReference = isReference,
            Mitochondrial = mitochondrial,
            CreatedAtUtc = DateTime.UtcNow,
        };
    }
}
