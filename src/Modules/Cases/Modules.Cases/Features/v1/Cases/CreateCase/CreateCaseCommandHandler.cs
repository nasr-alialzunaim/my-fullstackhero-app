using System.Net;
using FSH.Framework.Core.Exceptions;
using FSH.Modules.Cases.Contracts.v1.Cases;
using FSH.Modules.Cases.Data;
using FSH.Modules.Cases.Domain;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Cases.Features.v1.Cases.CreateCase;

public sealed class CreateCaseCommandHandler(CasesDbContext dbContext)
    : ICommandHandler<CreateCaseCommand, Guid>
{
    public async ValueTask<Guid> Handle(CreateCaseCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        string number = command.Number.Trim();
        bool numberTaken = await dbContext.Cases
            .AnyAsync(x => x.Number == number, cancellationToken)
            .ConfigureAwait(false);

        if (numberTaken)
        {
            throw new CustomException(
                $"Case number '{number}' already exists.",
                (IEnumerable<string>?)null,
                HttpStatusCode.Conflict);
        }

        ForensicCase entity = ForensicCase.Create(command.Number, command.Title, command.Description);
        dbContext.Cases.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return entity.Id;
    }
}