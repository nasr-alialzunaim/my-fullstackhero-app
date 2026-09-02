using FSH.Modules.Samples.Contracts.Dtos;
using Mediator;

namespace FSH.Modules.Samples.Contracts.v1.Samples;

public sealed record GetBiologicalSampleByIdQuery(Guid SampleId)
    : IQuery<BiologicalSampleDto>;
