using FSH.Framework.Core.Exceptions;
using FSH.Modules.FrequencyTables.Contracts.Dtos;
using FSH.Modules.FrequencyTables.Contracts.v1.Tables;
using FSH.Modules.FrequencyTables.Data;
using FSH.Modules.FrequencyTables.Domain;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.FrequencyTables.Features.v1.Tables.GetFrequencyTableById;

public sealed class GetFrequencyTableByIdQueryHandler(FrequencyTablesDbContext dbContext)
    : IQueryHandler<GetFrequencyTableByIdQuery, FrequencyTableDto>
{
    public async ValueTask<FrequencyTableDto> Handle(GetFrequencyTableByIdQuery query, CancellationToken cancellationToken)
    {
        FrequencyTable table = await dbContext.FrequencyTables.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == query.TableId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException($"Frequency table {query.TableId} not found.");

        List<FrequencyEntryDto> entries = await dbContext.FrequencyEntries.AsNoTracking()
            .Where(x => x.FrequencyTableId == table.Id)
            .OrderBy(x => x.Marker).ThenBy(x => x.Allele)
            .Select(x => new FrequencyEntryDto(x.Id, x.Marker, x.Allele, x.Frequency))
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        return new FrequencyTableDto(
            table.Id, table.Name, table.Model, table.Theta, table.VersionNumber,
            table.SupersedesTableId, table.IsActive, table.IsDefault, entries, table.CreatedAtUtc);
    }
}
