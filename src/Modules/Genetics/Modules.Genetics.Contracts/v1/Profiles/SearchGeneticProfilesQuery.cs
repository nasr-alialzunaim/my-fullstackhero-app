using FSH.Framework.Shared.Persistence;
using FSH.Modules.Genetics.Contracts.Dtos;
using Mediator;

namespace FSH.Modules.Genetics.Contracts.v1.Profiles;

public sealed record SearchGeneticProfilesQuery(
    Guid? SampleId = null,
    string? Search = null,
    int PageNumber = 1,
    int PageSize = 20) : IQuery<PagedResponse<GeneticProfileSummaryDto>>;
