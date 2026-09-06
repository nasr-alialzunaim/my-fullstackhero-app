using FSH.Framework.Core.Exceptions;
using FSH.Modules.StrKits.Contracts.Dtos;
using FSH.Modules.StrKits.Contracts.v1.Kits;
using FSH.Modules.StrKits.Data;
using FSH.Modules.StrKits.Domain;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.StrKits.Features.v1.Kits.GetStrKitById;

public sealed class GetStrKitByIdQueryHandler(StrKitsDbContext dbContext)
    : IQueryHandler<GetStrKitByIdQuery, StrKitDto>
{
    public async ValueTask<StrKitDto> Handle(
        GetStrKitByIdQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        StrKit kit = await dbContext.StrKits.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == query.KitId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException($"STR kit {query.KitId} not found.");

        List<string> aliases = await dbContext.StrKitAliases.AsNoTracking()
            .Where(x => x.StrKitId == kit.Id)
            .OrderBy(x => x.Alias)
            .Select(x => x.Alias)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        List<StrKitLocusDto> loci = await dbContext.StrKitLoci.AsNoTracking()
            .Where(x => x.StrKitId == kit.Id)
            .OrderBy(x => x.Order)
            .Select(x => new StrKitLocusDto(
                x.Id,
                x.Marker,
                x.Chromosome,
                x.MinimumAllelesQty,
                x.MaximumAllelesQty,
                x.Fluorophore,
                x.Order,
                x.Required,
                x.AlleleRangeMin,
                x.AlleleRangeMax))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return new StrKitDto(
            kit.Id,
            kit.KitCode,
            kit.Name,
            kit.AnalysisTypeId,
            kit.RepresentativeParameter,
            kit.VersionNumber,
            kit.SupersedesKitId,
            aliases,
            loci,
            kit.CreatedAtUtc);
    }
}
