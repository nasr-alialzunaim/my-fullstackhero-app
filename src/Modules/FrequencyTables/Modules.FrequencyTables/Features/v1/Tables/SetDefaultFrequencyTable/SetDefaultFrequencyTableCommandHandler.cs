using FSH.Framework.Core.Exceptions;
using FSH.Modules.FrequencyTables.Contracts.v1.Tables;
using FSH.Modules.FrequencyTables.Data;
using FSH.Modules.FrequencyTables.Domain;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.FrequencyTables.Features.v1.Tables.SetDefaultFrequencyTable;

public sealed class SetDefaultFrequencyTableCommandHandler(FrequencyTablesDbContext dbContext)
    : ICommandHandler<SetDefaultFrequencyTableCommand, Guid>
{
    public async ValueTask<Guid> Handle(SetDefaultFrequencyTableCommand command, CancellationToken cancellationToken)
    {
        FrequencyTable target = await dbContext.FrequencyTables
            .FirstOrDefaultAsync(x => x.Id == command.TableId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException($"Frequency table {command.TableId} not found.");

        if (!target.IsActive)
        {
            throw new InvalidOperationException("An inactive frequency table cannot be the default.");
        }

        List<FrequencyTable> currentDefaults = await dbContext.FrequencyTables
            .Where(x => x.IsDefault && x.Id != target.Id)
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        foreach (FrequencyTable current in currentDefaults)
        {
            current.SetDefault(false);
        }

        target.SetDefault(true);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return target.Id;
    }
}
