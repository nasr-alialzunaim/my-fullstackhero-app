using FluentValidation;
using FSH.Modules.Subjects.Contracts.v1.Subjects;
using FSH.Modules.Subjects.Domain;

namespace FSH.Modules.Subjects.Features.v1.Subjects;

public sealed class CreateSubjectCommandValidator : AbstractValidator<CreateSubjectCommand>
{
    public CreateSubjectCommandValidator() { RuleFor(x => x.SubjectCode).NotEmpty().MaximumLength(64); RuleFor(x => x.SubjectType).Must(x => Enum.TryParse<SubjectType>(x, true, out _)).WithMessage("SubjectType must be Person, MissingPerson, UnidentifiedRemains, or UnknownPerson."); }
}
public sealed class SearchSubjectsQueryValidator : AbstractValidator<SearchSubjectsQuery>
{
    public SearchSubjectsQueryValidator() { RuleFor(x => x.PageNumber).GreaterThan(0); RuleFor(x => x.PageSize).InclusiveBetween(1, 200); }
}
public sealed class UpsertPersonIdentityCommandValidator : AbstractValidator<UpsertPersonIdentityCommand>
{
    public UpsertPersonIdentityCommandValidator() { RuleFor(x => x.SubjectId).NotEmpty(); RuleFor(x => x.NationalityCode).MaximumLength(8); RuleFor(x => x.Sex).MaximumLength(16); }
}
