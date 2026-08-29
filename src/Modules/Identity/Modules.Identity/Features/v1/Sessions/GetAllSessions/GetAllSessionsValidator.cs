using FluentValidation;
using FSH.Modules.Identity.Contracts.v1.Sessions.GetAllSessions;

namespace FSH.Modules.Identity.Features.v1.Sessions.GetAllSessions;

public sealed class GetAllSessionsValidator : AbstractValidator<GetAllSessionsQuery>
{
    public GetAllSessionsValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("Page number must be greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("Page size must be greater than or equal to 1.");
    }
}
