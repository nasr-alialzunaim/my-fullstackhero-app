using FSH.Modules.Cases.Contracts.v1.Cases;
using FSH.Modules.Evidence.Contracts.v1.Evidence;
using FSH.Modules.Evidence.Data;
using FSH.Modules.Evidence.Domain;
using Mediator;

namespace FSH.Modules.Evidence.Features.v1.Evidence.CreateEvidenceItem;

public sealed class CreateEvidenceItemCommandHandler(
    EvidenceDbContext dbContext,
    IMediator mediator)
    : ICommandHandler<CreateEvidenceItemCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        CreateEvidenceItemCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        _ = await mediator.Send(
            new GetCaseByIdQuery(command.CaseId),
            cancellationToken).ConfigureAwait(false);

        EvidenceItem entity = EvidenceItem.Create(
            command.CaseId,
            command.ExternalReference,
            command.Description);

        dbContext.EvidenceItems.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return entity.Id;
    }
}
