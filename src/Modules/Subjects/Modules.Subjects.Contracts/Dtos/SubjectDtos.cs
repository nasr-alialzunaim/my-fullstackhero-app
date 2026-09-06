namespace FSH.Modules.Subjects.Contracts.Dtos;

public sealed record SubjectDto(Guid Id, string SubjectCode, string SubjectType, string Status, DateTime CreatedAtUtc, DateTime? UpdatedAtUtc);

public sealed record PersonIdentityDto(Guid SubjectId, string? FirstName, string? MiddleName, string? LastName, DateOnly? DateOfBirth, string? Sex, string? NationalityCode, bool IdentityVerified, DateTime? VerifiedAtUtc);
