using FSH.Modules.DNA.Contracts.v1.Cases;
using FluentValidation;

namespace FSH.Modules.DNA.Features.v1.Cases.CreateCase;

public sealed class CreateCaseCommandValidator : AbstractValidator<CreateCaseCommand>
{
    public CreateCaseCommandValidator()
    {
        RuleFor(x => x.CaseNumber)
            .NotEmpty()
            .MaximumLength(64);

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Description)
            .MaximumLength(4000);
    }
}
