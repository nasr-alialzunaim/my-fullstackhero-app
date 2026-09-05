using FSH.Framework.Core.Domain;

namespace FSH.Modules.Samples.Domain;

public enum SampleContext
{
    CaseSample = 1,
    KnownReference = 2,
    Unknown = 3,
}

public enum SampleStatus
{
    Registered = 1,
    InProcessing = 2,
    Stored = 3,
    Consumed = 4,
    Released = 5,
    Disposed = 6,
}

public sealed class BiologicalSample : AggregateRoot<Guid>
{
    public string SampleCode { get; private set; } = default!;
    public string? ExternalSampleCode { get; private set; }
    public SampleContext SampleContext { get; private set; }
    public Guid? CaseId { get; private set; }
    public Guid? SubjectId { get; private set; }
    public Guid? ParentSampleId { get; private set; }
    public string? SampleType { get; private set; }
    public string? Matrix { get; private set; }
    public string? CollectionLocation { get; private set; }
    public DateTime? CollectedAtUtc { get; private set; }
    public string? CollectionNote { get; private set; }
    public string? ContainerCode { get; private set; }
    public string? SealNumber { get; private set; }
    public SampleStatus Status { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    private BiologicalSample()
    {
    }

    public static BiologicalSample CreateCaseSample(
        string sampleCode,
        Guid caseId,
        Guid? parentSampleId,
        string? externalSampleCode,
        string? sampleType,
        string? matrix,
        string? collectionLocation,
        DateTime? collectedAtUtc,
        string? collectionNote,
        string? containerCode,
        string? sealNumber,
        Guid createdByUserId)
    {
        if (caseId == Guid.Empty)
        {
            throw new ArgumentException("Case identity cannot be empty.", nameof(caseId));
        }

        return CreateCore(
            sampleCode,
            SampleContext.CaseSample,
            caseId,
            null,
            parentSampleId,
            externalSampleCode,
            sampleType,
            matrix,
            collectionLocation,
            collectedAtUtc,
            collectionNote,
            containerCode,
            sealNumber,
            createdByUserId);
    }

    public static BiologicalSample CreateKnownReference(
        string sampleCode,
        Guid subjectId,
        Guid? parentSampleId,
        string? externalSampleCode,
        string? sampleType,
        string? matrix,
        string? collectionLocation,
        DateTime? collectedAtUtc,
        string? collectionNote,
        string? containerCode,
        string? sealNumber,
        Guid createdByUserId)
    {
        if (subjectId == Guid.Empty)
        {
            throw new ArgumentException("Subject identity cannot be empty.", nameof(subjectId));
        }

        return CreateCore(
            sampleCode,
            SampleContext.KnownReference,
            null,
            subjectId,
            parentSampleId,
            externalSampleCode,
            sampleType,
            matrix,
            collectionLocation,
            collectedAtUtc,
            collectionNote,
            containerCode,
            sealNumber,
            createdByUserId);
    }

    public static BiologicalSample CreateUnknown(
        string sampleCode,
        Guid? parentSampleId,
        string? externalSampleCode,
        string? sampleType,
        string? matrix,
        string? collectionLocation,
        DateTime? collectedAtUtc,
        string? collectionNote,
        string? containerCode,
        string? sealNumber,
        Guid createdByUserId) =>
        CreateCore(
            sampleCode,
            SampleContext.Unknown,
            null,
            null,
            parentSampleId,
            externalSampleCode,
            sampleType,
            matrix,
            collectionLocation,
            collectedAtUtc,
            collectionNote,
            containerCode,
            sealNumber,
            createdByUserId);

    public void SetStatus(SampleStatus status)
    {
        Status = status;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    private static BiologicalSample CreateCore(
        string sampleCode,
        SampleContext sampleContext,
        Guid? caseId,
        Guid? subjectId,
        Guid? parentSampleId,
        string? externalSampleCode,
        string? sampleType,
        string? matrix,
        string? collectionLocation,
        DateTime? collectedAtUtc,
        string? collectionNote,
        string? containerCode,
        string? sealNumber,
        Guid createdByUserId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sampleCode);
        if (createdByUserId == Guid.Empty)
        {
            throw new ArgumentException("Creator identity cannot be empty.", nameof(createdByUserId));
        }

        if (parentSampleId == Guid.Empty)
        {
            throw new ArgumentException("Parent sample identity cannot be empty.", nameof(parentSampleId));
        }

        return new BiologicalSample
        {
            Id = Guid.CreateVersion7(),
            SampleCode = sampleCode.Trim(),
            ExternalSampleCode = Normalize(externalSampleCode),
            SampleContext = sampleContext,
            CaseId = caseId,
            SubjectId = subjectId,
            ParentSampleId = parentSampleId,
            SampleType = Normalize(sampleType),
            Matrix = Normalize(matrix),
            CollectionLocation = Normalize(collectionLocation),
            CollectedAtUtc = collectedAtUtc,
            CollectionNote = Normalize(collectionNote),
            ContainerCode = Normalize(containerCode),
            SealNumber = Normalize(sealNumber),
            Status = SampleStatus.Registered,
            CreatedByUserId = createdByUserId,
            CreatedAtUtc = DateTime.UtcNow,
        };
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
