using System.Net;
using FSH.Framework.Core.Context;
using FSH.Framework.Core.Exceptions;
using FSH.Modules.Cases.Contracts.v1.Cases;
using FSH.Modules.Samples.Contracts.v1.Samples;
using FSH.Modules.Samples.Data;
using FSH.Modules.Samples.Domain;
using FSH.Modules.Subjects.Contracts.v1.Subjects;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Samples.Features.v1.Samples.CreateBiologicalSample;

public sealed class CreateBiologicalSampleCommandHandler(
    SamplesDbContext dbContext,
    IMediator mediator,
    ICurrentUser currentUser)
    : ICommandHandler<CreateBiologicalSampleCommand, Guid>
{
    public async ValueTask<Guid> Handle(CreateBiologicalSampleCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (!Enum.TryParse(command.SampleContext, true, out SampleContext context))
        {
            throw new CustomException("Unsupported sample context.", (IEnumerable<string>?)null, HttpStatusCode.BadRequest);
        }

        switch (context)
        {
            case SampleContext.CaseSample:
                if (!command.CaseId.HasValue || command.SubjectId.HasValue)
                    throw InvalidContext();
                _ = await mediator.Send(new GetCaseByIdQuery(command.CaseId.Value), cancellationToken).ConfigureAwait(false);
                break;
            case SampleContext.KnownReference:
                if (command.CaseId.HasValue || !command.SubjectId.HasValue)
                    throw InvalidContext();
                _ = await mediator.Send(new GetSubjectByIdQuery(command.SubjectId.Value), cancellationToken).ConfigureAwait(false);
                break;
            case SampleContext.Unknown:
                if (command.CaseId.HasValue || command.SubjectId.HasValue)
                    throw InvalidContext();
                break;
            default:
                throw InvalidContext();
        }

        if (command.ParentSampleId.HasValue)
        {
            BiologicalSample parent = await dbContext.BiologicalSamples.AsNoTracking().FirstOrDefaultAsync(x => x.Id == command.ParentSampleId.Value, cancellationToken).ConfigureAwait(false)
                ?? throw new NotFoundException($"Parent sample {command.ParentSampleId.Value} not found.");

            if (parent.SampleContext != context || parent.CaseId != command.CaseId || parent.SubjectId != command.SubjectId)
            {
                throw new CustomException("A derived sample must retain the same provenance as its immediate parent.", (IEnumerable<string>?)null, HttpStatusCode.BadRequest);
            }
        }

        Guid userId = currentUser.GetUserId();
        BiologicalSample entity = context switch
        {
            SampleContext.CaseSample => BiologicalSample.CreateCaseSample(command.SampleCode, command.CaseId!.Value, command.ParentSampleId, command.ExternalSampleCode, command.SampleType, command.Matrix, command.CollectionLocation, command.CollectedAtUtc, command.CollectionNote, command.ContainerCode, command.SealNumber, userId),
            SampleContext.KnownReference => BiologicalSample.CreateKnownReference(command.SampleCode, command.SubjectId!.Value, command.ParentSampleId, command.ExternalSampleCode, command.SampleType, command.Matrix, command.CollectionLocation, command.CollectedAtUtc, command.CollectionNote, command.ContainerCode, command.SealNumber, userId),
            SampleContext.Unknown => BiologicalSample.CreateUnknown(command.SampleCode, command.ParentSampleId, command.ExternalSampleCode, command.SampleType, command.Matrix, command.CollectionLocation, command.CollectedAtUtc, command.CollectionNote, command.ContainerCode, command.SealNumber, userId),
            _ => throw InvalidContext(),
        };

        dbContext.BiologicalSamples.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return entity.Id;
    }

    private static CustomException InvalidContext() =>
        new("SampleContext requires exactly one valid provenance: CaseSample=CaseId only, KnownReference=SubjectId only, Unknown=neither.", (IEnumerable<string>?)null, HttpStatusCode.BadRequest);
}
