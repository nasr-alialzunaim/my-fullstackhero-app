using FluentValidation;
using FSH.Modules.StrKits.Contracts.v1.Kits;

namespace FSH.Modules.StrKits.Features.v1.Kits.CreateStrKit;

public sealed class CreateStrKitCommandValidator : AbstractValidator<CreateStrKitCommand>
{
    public CreateStrKitCommandValidator()
    {
        RuleFor(x => x.KitCode).NotEmpty().MaximumLength(128);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.AnalysisTypeId).GreaterThanOrEqualTo(0);
        RuleFor(x => x.RepresentativeParameter).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Loci).NotNull().NotEmpty();
        RuleForEach(x => x.Aliases).NotEmpty().MaximumLength(128);
        RuleFor(x => x.Loci).Must(UniqueMarkers).WithMessage("STR kit marker names must be unique.");
        RuleFor(x => x.Loci).Must(UniqueOrders).WithMessage("STR kit locus order values must be unique.");

        RuleForEach(x => x.Loci).ChildRules(locus =>
        {
            locus.RuleFor(x => x.Marker).NotEmpty().MaximumLength(64);
            locus.RuleFor(x => x.Chromosome).MaximumLength(32);
            locus.RuleFor(x => x.Fluorophore).MaximumLength(64);
            locus.RuleFor(x => x.MinimumAllelesQty).GreaterThanOrEqualTo(0);
            locus.RuleFor(x => x.MaximumAllelesQty).GreaterThanOrEqualTo(x => x.MinimumAllelesQty);
            locus.RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
            locus.RuleFor(x => x)
                .Must(x => !x.AlleleRangeMin.HasValue || !x.AlleleRangeMax.HasValue ||
                    x.AlleleRangeMin.Value <= x.AlleleRangeMax.Value)
                .WithMessage("Allele range min must not exceed max.");
        });
    }

    private static bool UniqueMarkers(IReadOnlyList<StrKitLocusInput> loci) =>
        loci.Select(x => x.Marker.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).Count() == loci.Count;

    private static bool UniqueOrders(IReadOnlyList<StrKitLocusInput> loci) =>
        loci.Select(x => x.Order).Distinct().Count() == loci.Count;
}
