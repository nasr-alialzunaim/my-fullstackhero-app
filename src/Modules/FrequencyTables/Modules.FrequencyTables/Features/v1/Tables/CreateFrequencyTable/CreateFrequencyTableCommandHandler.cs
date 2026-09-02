using System.Net;
using FSH.Framework.Core.Exceptions;
using FSH.Modules.FrequencyTables.Contracts.v1.Tables;
using FSH.Modules.FrequencyTables.Data;
using FSH.Modules.FrequencyTables.Domain;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.FrequencyTables.Features.v1.Tables.CreateFrequencyTable;

public sealed class CreateFrequencyTableCommandHandler(FrequencyTablesDbContext dbContext)
    : ICommandHandler<CreateFrequencyTableCommand, Guid>
{
    public async ValueTask<Guid> Handle(CreateFrequencyTableCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        int versionNumber = 1;
        if (command.SupersedesTableId.HasValue)
        {
            FrequencyTable prior = await dbContext.FrequencyTables.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == command.SupersedesTableId.Value, cancellationToken)
                .ConfigureAwait(false)
                ?? throw new NotFoundException($"Frequency table {command.SupersedesTableId.Value} not found.");

            if (!string.Equals(prior.Name, command.Name.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                throw new CustomException(
                    "A frequency table version must retain the same name as the version it supersedes.",
                    (IEnumerable<string>?)null,
                    HttpStatusCode.BadRequest);
            }
            versionNumber = checked(prior.VersionNumber + 1);
        }

        FrequencyTable table = FrequencyTable.Create(
            command.Name, command.Model, command.Theta, versionNumber, command.SupersedesTableId);

        dbContext.FrequencyTables.Add(table);
        foreach (FrequencyEntryInput input in command.Entries)
        {
            dbContext.FrequencyEntries.Add(
                FrequencyEntry.Create(table.Id, input.Marker, input.Allele, input.Frequency));
        }

        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return table.Id;
    }
}
