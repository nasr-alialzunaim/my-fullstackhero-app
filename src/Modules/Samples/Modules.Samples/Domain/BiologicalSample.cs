using FSH.Framework.Core.Domain;

namespace FSH.Modules.Samples.Domain;

public sealed class BiologicalSample : AggregateRoot<Guid>
{
    public Guid EvidenceItemId { get; private set; }
    public Guid? ParentSampleId { get; private set; }
    public string? ExternalSampleCode { get; private set; }
    public DateTime? CollectedAtUtc { get; private set; }
    public string? CollectionNote { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private BiologicalSample()
    {
    }

    public static BiologicalSample Create(
        Guid evidenceItemId,
        Guid? parentSampleId,
        string? externalSampleCode,
        DateTime? collectedAtUtc,
        string? collectionNote)
    {
        if (evidenceItemId == Guid.Empty)
        {
            throw new ArgumentException(
                "Evidence identity cannot be empty.",
                nameof(evidenceItemId));
        }

        if (parentSampleId == Guid.Empty)
        {
            throw new ArgumentException(
                "Parent sample identity cannot be empty.",
                nameof(parentSampleId));
        }

        return new BiologicalSample
        {
            Id = Guid.CreateVersion7(),
            EvidenceItemId = evidenceItemId,
            ParentSampleId = parentSampleId,
            ExternalSampleCode = Normalize(externalSampleCode),
            CollectedAtUtc = collectedAtUtc,
            CollectionNote = Normalize(collectionNote),
            CreatedAtUtc = DateTime.UtcNow,
        };
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
