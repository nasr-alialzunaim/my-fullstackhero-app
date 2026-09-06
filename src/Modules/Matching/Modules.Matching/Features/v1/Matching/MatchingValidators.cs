using FluentValidation;
using FSH.Modules.Matching.Contracts.v1.Matching;

namespace FSH.Modules.Matching.Features.v1.Matching;

public sealed class CreateProfileCategoryCommandValidator
    : AbstractValidator<CreateProfileCategoryCommand>
{
    public CreateProfileCategoryCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(128);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.AnalysisTypeId).GreaterThanOrEqualTo(0);
    }
}

public sealed class CreateMatchingRuleCommandValidator
    : AbstractValidator<CreateMatchingRuleCommand>
{
    private static readonly HashSet<string> Algorithms =
        new(StringComparer.Ordinal) { "ENFSI", "GENIS_MM" };

    public CreateMatchingRuleCommandValidator()
    {
        RuleFor(x => x.SourceCategoryId).NotEmpty();
        RuleFor(x => x.Type).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CategoryRelated).NotEmpty().MaximumLength(128);
        RuleFor(x => x.MinimumStringency)
            .Must(GenisMatchingRuleEvaluator.IsValidStringency)
            .WithMessage("Minimum stringency must be a GENis Stringency value.");
        RuleFor(x => x.MatchingAlgorithm)
            .Must(Algorithms.Contains)
            .WithMessage("Matching algorithm must be ENFSI or GENIS_MM.");
        RuleFor(x => x.MinLocusMatch).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MismatchsAllowed).GreaterThanOrEqualTo(0);
    }
}

public sealed class ConfigureProfileMatchingCommandValidator
    : AbstractValidator<ConfigureProfileMatchingCommand>
{
    public ConfigureProfileMatchingCommandValidator()
    {
        RuleFor(x => x.GeneticProfileId).NotEmpty();
        RuleFor(x => x.CategoryId).NotEmpty();
    }
}

public sealed class RunAutosomalDatabaseSearchCommandValidator
    : AbstractValidator<RunAutosomalDatabaseSearchCommand>
{
    public RunAutosomalDatabaseSearchCommandValidator()
    {
        RuleFor(x => x.QueryProfileId).NotEmpty();
        RuleFor(x => x.MatchingRuleId).NotEmpty();
    }
}

public sealed class ReviewMatchHitCommandValidator
    : AbstractValidator<ReviewMatchHitCommand>
{
    public ReviewMatchHitCommandValidator()
    {
        RuleFor(x => x.HitId).NotEmpty();
        RuleFor(x => x.Status).NotEmpty().MaximumLength(32);
        RuleFor(x => x.ReviewNote).MaximumLength(4096);
    }
}
