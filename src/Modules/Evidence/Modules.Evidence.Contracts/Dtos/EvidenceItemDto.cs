namespace FSH.Modules.Evidence.Contracts.Dtos;

public sealed record EvidenceItemDto(
    Guid Id,
    Guid CaseId,
    string? ExternalReference,
    string? Description,
    DateTime CreatedAtUtc);
