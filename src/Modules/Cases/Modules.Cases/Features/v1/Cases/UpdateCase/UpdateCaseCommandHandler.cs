using System.Net;
using FSH.Framework.Core.Exceptions;
using FSH.Modules.Cases.Contracts.v1.Cases;
using FSH.Modules.Cases.Data;
using FSH.Modules.Cases.Domain;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Cases.Features.v1.Cases.UpdateCase;

public sealed class UpdateCaseCommandHandler(CasesDbContext dbContext)
    : ICommandHandler<UpdateCaseCommand, Guid>
{
    public async ValueTask<Guid> Handle(UpdateCaseCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        ForensicCase entity = await dbContext.Cases
            .FirstOrDefaultAsync(x => x.Id == command.CaseId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException($"Case {command.CaseId} not found.");

        string number = command.Number.Trim();
        bool numberTaken = await dbContext.Cases
            .AnyAsync(x => x.Number == number && x.Id != entity.Id, cancellationToken)
            .ConfigureAwait(false);

        if (numberTaken)
        {
            throw new CustomException(
                $"Case number '{number}' is already used by another case.",
                (IEnumerable<string>?)null,
                HttpStatusCode.Conflict);
        }

        entity.Update(command.Number, command.Title, command.Description);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return entity.Id;
    }
}