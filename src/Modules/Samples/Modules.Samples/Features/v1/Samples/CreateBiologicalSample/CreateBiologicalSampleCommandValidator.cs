using FluentValidation;
using FSH.Modules.Samples.Contracts.v1.Samples;
using FSH.Modules.Samples.Domain;

namespace FSH.Modules.Samples.Features.v1.Samples.CreateBiologicalSample;

public sealed class CreateBiologicalSampleCommandValidator : AbstractValidator<CreateBiologicalSampleCommand>
{
    public CreateBiologicalSampleCommandValidator()
    {
        RuleFor(x => x.SampleCode).NotEmpty().MaximumLength(128);
        RuleFor(x => x.SampleContext).Must(x => Enum.TryParse<SampleContext>(x, true, out _)).WithMessage("SampleContext must be CaseSample, KnownReference, or Unknown.");
        RuleFor(x => x.ExternalSampleCode).MaximumLength(128);
        RuleFor(x => x.SampleType).MaximumLength(64);
        RuleFor(x => x.Matrix).MaximumLength(64);
        RuleFor(x => x.CollectionNote).MaximumLength(4096);
        RuleFor(x => x.ContainerCode).MaximumLength(128);
        RuleFor(x => x.SealNumber).MaximumLength(128);
    }
}
