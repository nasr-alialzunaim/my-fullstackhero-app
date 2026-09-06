using FSH.Framework.Core.Exceptions;
using FSH.Modules.FrequencyTables.Contracts.v1.Tables;
using FSH.Modules.FrequencyTables.Data;
using FSH.Modules.FrequencyTables.Domain;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.FrequencyTables.Features.v1.Tables.ToggleFrequencyTableActive;

public sealed class ToggleFrequencyTableActiveCommandHandler(FrequencyTablesDbContext dbContext)
    : ICommandHandler<ToggleFrequencyTableActiveCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        ToggleFrequencyTableActiveCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        FrequencyTable table = await dbContext.FrequencyTables
            .FirstOrDefaultAsync(x => x.Id == command.TableId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException($"Frequency table {command.TableId} not found.");

        table.ToggleActive();
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return table.Id;
    }
}
