using FSH.Framework.Core.Exceptions;
using FSH.Modules.Samples.Contracts.Dtos;
using FSH.Modules.Samples.Contracts.v1.Samples;
using FSH.Modules.Samples.Data;
using FSH.Modules.Samples.Domain;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Samples.Features.v1.Samples.GetBiologicalSampleById;

public sealed class GetBiologicalSampleByIdQueryHandler(SamplesDbContext dbContext)
    : IQueryHandler<GetBiologicalSampleByIdQuery, BiologicalSampleDto>
{
    public async ValueTask<BiologicalSampleDto> Handle(
        GetBiologicalSampleByIdQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        BiologicalSample entity = await dbContext.BiologicalSamples
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == query.SampleId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException($"Biological sample {query.SampleId} not found.");

        return new BiologicalSampleDto(
            entity.Id,
            entity.EvidenceItemId,
            entity.ParentSampleId,
            entity.ExternalSampleCode,
            entity.CollectedAtUtc,
            entity.CollectionNote,
            entity.CreatedAtUtc);
    }
}
