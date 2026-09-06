namespace FSH.Modules.Samples.Domain;

public enum SampleCustodyEventType
{
    None = 0,
    Registered = 1,
    Received = 2,
    Transferred = 3,
    Opened = 4,
    Processed = 5,
    Resealed = 6,
    Stored = 7,
    Released = 8,
    Disposed = 9,
}

public sealed class SampleCustodyEvent
{
    public Guid Id { get; private set; }
    public Guid SampleId { get; private set; }
    public SampleCustodyEventType EventType { get; private set; }
    public Guid? FromCustodianUserId { get; private set; }
    public Guid? ToCustodianUserId { get; private set; }
    public string? FromLocation { get; private set; }
    public string? ToLocation { get; private set; }
    public string? ContainerCode { get; private set; }
    public string? SealNumber { get; private set; }
    public string? Reason { get; private set; }
    public string? Notes { get; private set; }
    public Guid PerformedByUserId { get; private set; }
    public DateTime OccurredAtUtc { get; private set; }
    public string? PreviousEventHash { get; private set; }
    public string EventHash { get; private set; } = default!;

    private SampleCustodyEvent()
    {
    }

    public static SampleCustodyEvent Create(
        Guid sampleId,
        SampleCustodyEventType eventType,
        Guid? fromCustodianUserId,
        Guid? toCustodianUserId,
        string? fromLocation,
        string? toLocation,
        string? containerCode,
        string? sealNumber,
        string? reason,
        string? notes,
        Guid performedByUserId,
        DateTime occurredAtUtc,
        string? previousEventHash,
        string eventHash)
    {
        if (sampleId == Guid.Empty)
        {
            throw new ArgumentException("Sample identity cannot be empty.", nameof(sampleId));
        }

        if (eventType is SampleCustodyEventType.None)
        {
            throw new ArgumentOutOfRangeException(nameof(eventType), "A custody event type must be specified.");
        }

        if (performedByUserId == Guid.Empty)
        {
            throw new ArgumentException("Performer identity cannot be empty.", nameof(performedByUserId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(eventHash);
        return new SampleCustodyEvent
        {
            Id = Guid.CreateVersion7(),
            SampleId = sampleId,
            EventType = eventType,
            FromCustodianUserId = fromCustodianUserId,
            ToCustodianUserId = toCustodianUserId,
            FromLocation = Normalize(fromLocation),
            ToLocation = Normalize(toLocation),
            ContainerCode = Normalize(containerCode),
            SealNumber = Normalize(sealNumber),
            Reason = Normalize(reason),
            Notes = Normalize(notes),
            PerformedByUserId = performedByUserId,
            OccurredAtUtc = occurredAtUtc,
            PreviousEventHash = Normalize(previousEventHash),
            EventHash = eventHash.Trim(),
        };
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed class SampleProcessingEvent
{
    public Guid Id { get; private set; }
    public Guid SampleId { get; private set; }
    public string EventType { get; private set; } = default!;
    public string? Method { get; private set; }
    public Guid? KitId { get; private set; }
    public string? BatchCode { get; private set; }
    public Guid PerformedByUserId { get; private set; }
    public DateTime? StartedAtUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }
    public string? ResultSummary { get; private set; }
    public string? ResultJson { get; private set; }

    private SampleProcessingEvent()
    {
    }

    public static SampleProcessingEvent Create(
        Guid sampleId,
        string eventType,
        string? method,
        Guid? kitId,
        string? batchCode,
        Guid performedByUserId,
        DateTime? startedAtUtc,
        DateTime? completedAtUtc,
        string? resultSummary,
        string? resultJson)
    {
        if (sampleId == Guid.Empty)
        {
            throw new ArgumentException("Sample identity cannot be empty.", nameof(sampleId));
        }

        if (performedByUserId == Guid.Empty)
        {
            throw new ArgumentException("Performer identity cannot be empty.", nameof(performedByUserId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(eventType);
        if (completedAtUtc.HasValue && startedAtUtc.HasValue && completedAtUtc < startedAtUtc)
        {
            throw new ArgumentException(
                "Processing completion cannot precede its start.",
                nameof(completedAtUtc));
        }

        return new SampleProcessingEvent
        {
            Id = Guid.CreateVersion7(),
            SampleId = sampleId,
            EventType = eventType.Trim(),
            Method = Normalize(method),
            KitId = kitId,
            BatchCode = Normalize(batchCode),
            PerformedByUserId = performedByUserId,
            StartedAtUtc = startedAtUtc,
            CompletedAtUtc = completedAtUtc,
            ResultSummary = Normalize(resultSummary),
            ResultJson = Normalize(resultJson),
        };
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed class SampleAttachment
{
    public Guid Id { get; private set; }
    public Guid SampleId { get; private set; }
    public Guid FileAssetId { get; private set; }
    public string AttachmentType { get; private set; } = default!;
    public string? Description { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private SampleAttachment()
    {
    }

    public static SampleAttachment Create(
        Guid sampleId,
        Guid fileAssetId,
        string attachmentType,
        string? description,
        Guid createdByUserId)
    {
        if (sampleId == Guid.Empty)
        {
            throw new ArgumentException("Sample identity cannot be empty.", nameof(sampleId));
        }

        if (fileAssetId == Guid.Empty)
        {
            throw new ArgumentException("File asset identity cannot be empty.", nameof(fileAssetId));
        }

        if (createdByUserId == Guid.Empty)
        {
            throw new ArgumentException("Creator identity cannot be empty.", nameof(createdByUserId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(attachmentType);
        return new SampleAttachment
        {
            Id = Guid.CreateVersion7(),
            SampleId = sampleId,
            FileAssetId = fileAssetId,
            AttachmentType = attachmentType.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            CreatedByUserId = createdByUserId,
            CreatedAtUtc = DateTime.UtcNow,
        };
    }
}
