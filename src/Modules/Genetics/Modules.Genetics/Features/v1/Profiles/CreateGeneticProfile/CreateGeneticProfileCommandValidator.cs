using FluentValidation;
using FSH.Modules.Genetics.Contracts.v1.Profiles;

namespace FSH.Modules.Genetics.Features.v1.Profiles.CreateGeneticProfile;

public sealed class CreateGeneticProfileCommandValidator
    : AbstractValidator<CreateGeneticProfileCommand>
{
    public CreateGeneticProfileCommandValidator()
    {
        RuleFor(x => x.SampleId).NotEmpty();
        RuleFor(x => x.ExternalProfileCode).MaximumLength(128);
        RuleFor(x => x.Loci).NotNull().NotEmpty();
        RuleFor(x => x.Loci)
            .Must(HaveUniqueMarkers)
            .WithMessage("Marker names must be unique within one genetic profile version.");

        RuleForEach(x => x.Loci).ChildRules(locus =>
        {
            locus.RuleFor(x => x.Marker).NotEmpty().MaximumLength(64);
            locus.RuleFor(x => x.Alleles).NotNull();
            locus.RuleFor(x => x.Peaks).NotNull();
            locus.RuleForEach(x => x.Alleles)
                .NotEmpty()
                .MaximumLength(64);
            locus.RuleForEach(x => x.Peaks).ChildRules(peak =>
            {
                peak.RuleFor(x => x.AlleleValue).MaximumLength(64);
                peak.RuleFor(x => x.Channel).MaximumLength(64);
                peak.RuleFor(x => x.HeightRfu)
                    .GreaterThanOrEqualTo(0)
                    .When(x => x.HeightRfu.HasValue);
                peak.RuleFor(x => x.SizeBp)
                    .GreaterThanOrEqualTo(0)
                    .When(x => x.SizeBp.HasValue);
            });
        });
    }

    private static bool HaveUniqueMarkers(IReadOnlyList<ProfileLocusInput> loci)
    {
        return loci
            .Select(x => x.Marker.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count() == loci.Count;
    }
}
