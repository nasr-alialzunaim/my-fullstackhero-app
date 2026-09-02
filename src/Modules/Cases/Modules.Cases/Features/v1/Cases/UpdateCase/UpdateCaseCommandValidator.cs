using FluentValidation;
using FSH.Modules.Cases.Contracts.v1.Cases;

namespace FSH.Modules.Cases.Features.v1.Cases.UpdateCase;

public sealed class UpdateCaseCommandValidator : AbstractValidator<UpdateCaseCommand>
{
    public UpdateCaseCommandValidator()
    {
        RuleFor(x => x.CaseId).NotEmpty();
        RuleFor(x => x.Number).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(4096);
    }
}