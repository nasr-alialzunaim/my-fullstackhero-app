using FSH.Framework.Core.Domain;

namespace FSH.Modules.Evidence.Domain;

public sealed class EvidenceItem : AggregateRoot<Guid>
{
    public Guid CaseId { get; private set; }
    public string? ExternalReference { get; private set; }
    public string? Description { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private EvidenceItem()
    {
    }

    public static EvidenceItem Create(
        Guid caseId,
        string? externalReference,
        string? description)
    {
        if (caseId == Guid.Empty)
        {
            throw new ArgumentException("Case identity cannot be empty.", nameof(caseId));
        }

        return new EvidenceItem
        {
            Id = Guid.CreateVersion7(),
            CaseId = caseId,
            ExternalReference = Normalize(externalReference),
            Description = Normalize(description),
            CreatedAtUtc = DateTime.UtcNow,
        };
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
