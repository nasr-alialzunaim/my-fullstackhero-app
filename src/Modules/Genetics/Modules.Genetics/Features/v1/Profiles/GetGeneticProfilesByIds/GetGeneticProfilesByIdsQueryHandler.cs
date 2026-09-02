using FSH.Modules.Genetics.Contracts.Dtos;
using FSH.Modules.Genetics.Contracts.v1.Profiles;
using Mediator;

namespace FSH.Modules.Genetics.Features.v1.Profiles.GetGeneticProfilesByIds;

public sealed class GetGeneticProfilesByIdsQueryHandler(IMediator mediator)
    : IQueryHandler<GetGeneticProfilesByIdsQuery, IReadOnlyList<GeneticProfileDto>>
{
    public async ValueTask<IReadOnlyList<GeneticProfileDto>> Handle(
        GetGeneticProfilesByIdsQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var ids = query.ProfileIds.Distinct().ToArray();
        if (ids.Length > 1000)
        {
            throw new ArgumentOutOfRangeException(
                nameof(query),
                "At most 1000 profiles may be loaded in one scientific batch.");
        }

        var profiles = new List<GeneticProfileDto>(ids.Length);
        foreach (Guid id in ids)
        {
            profiles.Add(await mediator.Send(
                new GetGeneticProfileByIdQuery(id),
                cancellationToken).ConfigureAwait(false));
        }

        return profiles;
    }
}
