using FSH.Framework.Core.Context;
using FSH.Framework.Core.Exceptions;
using FSH.Framework.Shared.Persistence;
using FSH.Modules.Subjects.Contracts.Dtos;
using FSH.Modules.Subjects.Contracts.v1.Subjects;
using FSH.Modules.Subjects.Data;
using FSH.Modules.Subjects.Domain;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Subjects.Features.v1.Subjects;

public sealed class CreateSubjectCommandHandler(SubjectsDbContext dbContext) : ICommandHandler<CreateSubjectCommand, Guid>
{
    public async ValueTask<Guid> Handle(CreateSubjectCommand command, CancellationToken cancellationToken) { ArgumentNullException.ThrowIfNull(command); if (!Enum.TryParse(command.SubjectType, true, out SubjectType subjectType)) throw new ArgumentException("Unsupported subject type.", nameof(command)); Subject entity = Subject.Create(command.SubjectCode, subjectType); dbContext.Subjects.Add(entity); await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false); return entity.Id; }
}
public sealed class GetSubjectByIdQueryHandler(SubjectsDbContext dbContext) : IQueryHandler<GetSubjectByIdQuery, SubjectDto>
{
    public async ValueTask<SubjectDto> Handle(GetSubjectByIdQuery query, CancellationToken cancellationToken) { ArgumentNullException.ThrowIfNull(query); Subject entity = await dbContext.Subjects.AsNoTracking().FirstOrDefaultAsync(x => x.Id == query.SubjectId, cancellationToken).ConfigureAwait(false) ?? throw new NotFoundException($"Subject {query.SubjectId} not found."); return new SubjectDto(entity.Id, entity.SubjectCode, entity.SubjectType.ToString(), entity.Status.ToString(), entity.CreatedAtUtc, entity.UpdatedAtUtc); }
}
public sealed class SearchSubjectsQueryHandler(SubjectsDbContext dbContext) : IQueryHandler<SearchSubjectsQuery, PagedResponse<SubjectDto>>
{
    public async ValueTask<PagedResponse<SubjectDto>> Handle(SearchSubjectsQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query); int page = query.PageNumber < 1 ? 1 : query.PageNumber; int size = query.PageSize is < 1 or > 200 ? 20 : query.PageSize; IQueryable<Subject> subjects = dbContext.Subjects.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(query.Search)) { string term = query.Search.Trim(); subjects = subjects.Where(x => EF.Functions.ILike(x.SubjectCode, $"%{term}%")); }
        if (!string.IsNullOrWhiteSpace(query.SubjectType) && Enum.TryParse(query.SubjectType, true, out SubjectType subjectType)) subjects = subjects.Where(x => x.SubjectType == subjectType);
        if (!string.IsNullOrWhiteSpace(query.Status) && Enum.TryParse(query.Status, true, out SubjectStatus status)) subjects = subjects.Where(x => x.Status == status);
        long total = await subjects.LongCountAsync(cancellationToken).ConfigureAwait(false); List<Subject> rows = await subjects.OrderByDescending(x => x.CreatedAtUtc).Skip((page - 1) * size).Take(size).ToListAsync(cancellationToken).ConfigureAwait(false);
        return new PagedResponse<SubjectDto> { Items = rows.Select(x => new SubjectDto(x.Id, x.SubjectCode, x.SubjectType.ToString(), x.Status.ToString(), x.CreatedAtUtc, x.UpdatedAtUtc)).ToList(), PageNumber = page, PageSize = size, TotalCount = total, TotalPages = (int)Math.Ceiling(total / (double)size) };
    }
}
public sealed class UpsertPersonIdentityCommandHandler(SubjectsDbContext dbContext, ICurrentUser currentUser) : ICommandHandler<UpsertPersonIdentityCommand, Unit>
{
    public async ValueTask<Unit> Handle(UpsertPersonIdentityCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command); bool subjectExists = await dbContext.Subjects.AnyAsync(x => x.Id == command.SubjectId, cancellationToken).ConfigureAwait(false); if (!subjectExists) throw new NotFoundException($"Subject {command.SubjectId} not found.");
        PersonIdentity? identity = await dbContext.PersonIdentities.FirstOrDefaultAsync(x => x.SubjectId == command.SubjectId, cancellationToken).ConfigureAwait(false); Guid? verifier = command.IdentityVerified ? currentUser.GetUserId() : null;
        if (identity is null) { identity = PersonIdentity.Create(command.SubjectId, command.NationalId, command.FirstName, command.MiddleName, command.LastName, command.DateOfBirth, command.Sex, command.NationalityCode, command.IdentityVerified, verifier); dbContext.PersonIdentities.Add(identity); }
        else identity.Update(command.NationalId, command.FirstName, command.MiddleName, command.LastName, command.DateOfBirth, command.Sex, command.NationalityCode, command.IdentityVerified, verifier);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false); return Unit.Value;
    }
}
