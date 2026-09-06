namespace FSH.Modules.Samples.Contracts.Dtos;

public sealed record BiologicalSampleDto(
    Guid Id,
    string SampleCode,
    string? ExternalSampleCode,
    string SampleContext,
    Guid? CaseId,
    Guid? SubjectId,
    Guid? ParentSampleId,
    string? SampleType,
    string? Matrix,
    string? CollectionLocation,
    DateTime? CollectedAtUtc,
    string? CollectionNote,
    string? ContainerCode,
    string? SealNumber,
    string Status,
    Guid CreatedByUserId,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);
