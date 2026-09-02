using FSH.Modules.Genetics.Contracts.Dtos;
using Mediator;

namespace FSH.Modules.Genetics.Contracts.v1.Profiles;

public sealed record GetGeneticProfilesByIdsQuery(IReadOnlyList<Guid> ProfileIds)
    : IQuery<IReadOnlyList<GeneticProfileDto>>;
