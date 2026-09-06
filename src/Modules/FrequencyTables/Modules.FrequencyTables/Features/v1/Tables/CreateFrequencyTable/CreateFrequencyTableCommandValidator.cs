using FluentValidation;
using FSH.Modules.FrequencyTables.Contracts.v1.Tables;

namespace FSH.Modules.FrequencyTables.Features.v1.Tables.CreateFrequencyTable;

public sealed class CreateFrequencyTableCommandValidator : AbstractValidator<CreateFrequencyTableCommand>
{
    private static readonly HashSet<string> Models =
        new(StringComparer.Ordinal) { "HardyWeinberg", "NRCII41", "NRCII410" };

    public CreateFrequencyTableCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Model).Must(Models.Contains)
            .WithMessage("Model must be HardyWeinberg, NRCII41, or NRCII410.");
        RuleFor(x => x.Theta).InclusiveBetween(0, 1);
        RuleFor(x => x.Entries).NotNull().NotEmpty();
        RuleFor(x => x.Entries).Must(UniqueEntries)
            .WithMessage("Each marker/allele pair may appear only once.");
        RuleFor(x => x.Entries).Must(EveryMarkerHasFmin)
            .WithMessage("Every marker must contain an fmin entry using allele -1 or -1.0.");

        RuleForEach(x => x.Entries).ChildRules(entry =>
        {
            entry.RuleFor(x => x.Marker).NotEmpty().MaximumLength(64);
            entry.RuleFor(x => x.Allele).NotEmpty().MaximumLength(64);
            entry.RuleFor(x => x.Frequency).GreaterThan(0).LessThanOrEqualTo(1);
        });
    }

    private static bool UniqueEntries(IReadOnlyList<FrequencyEntryInput> entries) =>
        entries.Select(x => $"{x.Marker.Trim().ToUpperInvariant()}|{NormalizeAllele(x.Allele)}")
            .Distinct(StringComparer.Ordinal)
            .Count() == entries.Count;

    private static bool EveryMarkerHasFmin(IReadOnlyList<FrequencyEntryInput> entries)
    {
        return entries
            .GroupBy(x => x.Marker.Trim(), StringComparer.OrdinalIgnoreCase)
            .All(group => group.Any(x => NormalizeAllele(x.Allele) == "-1"));
    }

    private static string NormalizeAllele(string allele)
    {
        string value = allele.Trim().Replace(',', '.');
        return value == "-1.0" ? "-1" : value;
    }
}
