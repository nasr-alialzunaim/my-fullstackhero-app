using System.Net;
using FSH.Framework.Core.Exceptions;
using FSH.Modules.StrKits.Contracts.v1.Kits;
using FSH.Modules.StrKits.Data;
using FSH.Modules.StrKits.Domain;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.StrKits.Features.v1.Kits.CreateStrKit;

public sealed class CreateStrKitCommandHandler(StrKitsDbContext dbContext)
    : ICommandHandler<CreateStrKitCommand, Guid>
{
    public async ValueTask<Guid> Handle(CreateStrKitCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        int versionNumber = 1;
        if (command.SupersedesKitId.HasValue)
        {
            StrKit prior = await dbContext.StrKits.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == command.SupersedesKitId.Value, cancellationToken)
                .ConfigureAwait(false)
                ?? throw new NotFoundException($"STR kit {command.SupersedesKitId.Value} not found.");

            if (!string.Equals(prior.KitCode, command.KitCode.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                throw new CustomException(
                    "A new STR kit version must retain the same kit code as the version it supersedes.",
                    (IEnumerable<string>?)null,
                    HttpStatusCode.BadRequest);
            }

            versionNumber = checked(prior.VersionNumber + 1);
        }

        StrKit kit = StrKit.Create(
            command.KitCode,
            command.Name,
            command.AnalysisTypeId,
            command.RepresentativeParameter,
            versionNumber,
            command.SupersedesKitId);

        dbContext.StrKits.Add(kit);

        foreach (string alias in command.Aliases.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            dbContext.StrKitAliases.Add(StrKitAlias.Create(kit.Id, alias));
        }

        foreach (StrKitLocusInput input in command.Loci)
        {
            dbContext.StrKitLoci.Add(StrKitLocus.Create(
                kit.Id,
                input.Marker,
                input.Chromosome,
                input.MinimumAllelesQty,
                input.MaximumAllelesQty,
                input.Fluorophore,
                input.Order,
                input.Required,
                input.AlleleRangeMin,
                input.AlleleRangeMax));
        }

        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return kit.Id;
    }
}
