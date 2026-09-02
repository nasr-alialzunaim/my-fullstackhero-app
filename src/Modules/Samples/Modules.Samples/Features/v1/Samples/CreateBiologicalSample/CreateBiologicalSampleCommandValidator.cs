using FluentValidation;
using FSH.Modules.Samples.Contracts.v1.Samples;

namespace FSH.Modules.Samples.Features.v1.Samples.CreateBiologicalSample;

public sealed class CreateBiologicalSampleCommandValidator
    : AbstractValidator<CreateBiologicalSampleCommand>
{
    public CreateBiologicalSampleCommandValidator()
    {
        RuleFor(x => x.EvidenceItemId).NotEmpty();
        RuleFor(x => x.ExternalSampleCode).MaximumLength(128);
        RuleFor(x => x.CollectionNote).MaximumLength(4096);
    }
}
