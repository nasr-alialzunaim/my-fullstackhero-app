using System.Net;
using FSH.Framework.Core.Exceptions;
using FSH.Modules.Evidence.Contracts.v1.Evidence;
using FSH.Modules.Samples.Contracts.v1.Samples;
using FSH.Modules.Samples.Data;
using FSH.Modules.Samples.Domain;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Samples.Features.v1.Samples.CreateBiologicalSample;

public sealed class CreateBiologicalSampleCommandHandler(
    SamplesDbContext dbContext,
    IMediator mediator)
    : ICommandHandler<CreateBiologicalSampleCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        CreateBiologicalSampleCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        _ = await mediator.Send(
            new GetEvidenceItemByIdQuery(command.EvidenceItemId),
            cancellationToken).ConfigureAwait(false);

        if (command.ParentSampleId.HasValue)
        {
            BiologicalSample parent = await dbContext.BiologicalSamples
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == command.ParentSampleId.Value,
                    cancellationToken)
                .ConfigureAwait(false)
                ?? throw new NotFoundException(
                    $"Parent sample {command.ParentSampleId.Value} not found.");

            if (parent.EvidenceItemId != command.EvidenceItemId)
            {
                throw new CustomException(
                    "A derived sample must retain the same evidence item as its immediate parent.",
                    (IEnumerable<string>?)null,
                    HttpStatusCode.BadRequest);
            }
        }

        BiologicalSample entity = BiologicalSample.Create(
            command.EvidenceItemId,
            command.ParentSampleId,
            command.ExternalSampleCode,
            command.CollectedAtUtc,
            command.CollectionNote);

        dbContext.BiologicalSamples.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return entity.Id;
    }
}
