using System.Net;
using FSH.Framework.Core.Exceptions;
using FSH.Modules.DNA.Contracts.v1.Cases;
using FSH.Modules.DNA.Data;
using FSH.Modules.DNA.Domain;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.DNA.Features.v1.Cases.CreateCase;

public sealed class CreateCaseCommandHandler(DnaDbContext dbContext)
    : ICommandHandler<CreateCaseCommand, Guid>
{
    public async ValueTask<Guid> Handle(CreateCaseCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var entity = DnaCase.Create(command.CaseNumber, command.Title, command.Description);
        var exists = await dbContext.Cases
            .AnyAsync(x => x.CaseNumber == entity.CaseNumber, cancellationToken)
            .ConfigureAwait(false);

        if (exists)
        {
            throw new CustomException(
                $"A case with number '{entity.CaseNumber}' already exists.",
                (IEnumerable<string>?)null,
                HttpStatusCode.Conflict);
        }

        dbContext.Cases.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return entity.Id;
    }
}
