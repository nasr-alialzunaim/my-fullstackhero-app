namespace FSH.Modules.Cases.Contracts.Dtos;

public sealed record CaseDto(
    Guid Id,
    string Number,
    string Title,
    string? Description,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);