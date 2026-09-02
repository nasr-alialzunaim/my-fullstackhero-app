using FSH.Framework.Core.Exceptions;
using FSH.Modules.FrequencyTables.Contracts.v1.Tables;
using FSH.Modules.FrequencyTables.Data;
using FSH.Modules.FrequencyTables.Domain;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.FrequencyTables.Features.v1.Tables.GetGenisFrequencyTable;

public sealed class GetGenisFrequencyTableQueryHandler(FrequencyTablesDbContext dbContext)
    : IQueryHandler<GetGenisFrequencyTableQuery, Dictionary<string, Dictionary<string, double>>>
{
    public async ValueTask<Dictionary<string, Dictionary<string, double>>> Handle(
        GetGenisFrequencyTableQuery query,
        CancellationToken cancellationToken)
    {
        bool exists = await dbContext.FrequencyTables.AsNoTracking()
            .AnyAsync(x => x.Id == query.TableId, cancellationToken).ConfigureAwait(false);
        if (!exists)
        {
            throw new NotFoundException($"Frequency table {query.TableId} not found.");
        }

        List<FrequencyEntry> entries = await dbContext.FrequencyEntries.AsNoTracking()
            .Where(x => x.FrequencyTableId == query.TableId)
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        return entries
            .GroupBy(x => x.Marker, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group.ToDictionary(x => x.Allele, x => x.Frequency, StringComparer.Ordinal),
                StringComparer.Ordinal);
    }
}
