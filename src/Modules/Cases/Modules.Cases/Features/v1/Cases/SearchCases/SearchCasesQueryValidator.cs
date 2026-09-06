using FluentValidation;
using FSH.Modules.Cases.Contracts.v1.Cases;

namespace FSH.Modules.Cases.Features.v1.Cases.SearchCases;

public sealed class SearchCasesQueryValidator : AbstractValidator<SearchCasesQuery>
{
    public SearchCasesQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
        RuleFor(x => x.Search).MaximumLength(200);
    }
}