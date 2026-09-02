namespace FSH.Modules.Samples.Contracts.Dtos;

public sealed record BiologicalSampleDto(
    Guid Id,
    Guid EvidenceItemId,
    Guid? ParentSampleId,
    string? ExternalSampleCode,
    DateTime? CollectedAtUtc,
    string? CollectionNote,
    DateTime CreatedAtUtc);
