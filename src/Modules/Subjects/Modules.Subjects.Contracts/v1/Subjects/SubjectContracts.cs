using FSH.Framework.Shared.Persistence;
using FSH.Modules.Subjects.Contracts.Dtos;
using Mediator;

namespace FSH.Modules.Subjects.Contracts.v1.Subjects;

public sealed record CreateSubjectCommand(string SubjectCode, string SubjectType) : ICommand<Guid>;
public sealed record GetSubjectByIdQuery(Guid SubjectId) : IQuery<SubjectDto>;
public sealed record SearchSubjectsQuery(string? Search = null, string? SubjectType = null, string? Status = null, int PageNumber = 1, int PageSize = 20) : IQuery<PagedResponse<SubjectDto>>;
public sealed record UpsertPersonIdentityCommand(Guid SubjectId, string? NationalId, string? FirstName, string? MiddleName, string? LastName, DateOnly? DateOfBirth, string? Sex, string? NationalityCode, bool IdentityVerified = false) : ICommand<Unit>;
