using FluentValidation;
using FSH.Modules.Samples.Contracts.v1.Samples;
using FSH.Modules.Samples.Domain;

namespace FSH.Modules.Samples.Features.v1.Samples.SearchBiologicalSamples;

public sealed class SearchBiologicalSamplesQueryValidator : AbstractValidator<SearchBiologicalSamplesQuery>
{
    public SearchBiologicalSamplesQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
        RuleFor(x => x.SampleContext).Must(x => string.IsNullOrWhiteSpace(x) || Enum.TryParse<SampleContext>(x, true, out _)).WithMessage("Invalid SampleContext.");
        RuleFor(x => x.Status).Must(x => string.IsNullOrWhiteSpace(x) || Enum.TryParse<SampleStatus>(x, true, out _)).WithMessage("Invalid sample status.");
    }
}
