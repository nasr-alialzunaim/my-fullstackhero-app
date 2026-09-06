using FluentValidation;
using FSH.Modules.Evidence.Contracts.v1.Evidence;

namespace FSH.Modules.Evidence.Features.v1.Evidence.CreateEvidenceItem;

public sealed class CreateEvidenceItemCommandValidator
    : AbstractValidator<CreateEvidenceItemCommand>
{
    public CreateEvidenceItemCommandValidator()
    {
        RuleFor(x => x.CaseId).NotEmpty();
        RuleFor(x => x.ExternalReference).MaximumLength(128);
        RuleFor(x => x.Description).MaximumLength(4096);
    }
}
