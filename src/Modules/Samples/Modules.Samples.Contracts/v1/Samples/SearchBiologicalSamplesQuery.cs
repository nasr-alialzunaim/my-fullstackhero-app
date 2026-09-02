using FSH.Framework.Shared.Persistence;
using FSH.Modules.Samples.Contracts.Dtos;
using Mediator;

namespace FSH.Modules.Samples.Contracts.v1.Samples;

public sealed record SearchBiologicalSamplesQuery(
    Guid? EvidenceItemId = null,
    string? Search = null,
    int PageNumber = 1,
    int PageSize = 20) : IQuery<PagedResponse<BiologicalSampleDto>>;
