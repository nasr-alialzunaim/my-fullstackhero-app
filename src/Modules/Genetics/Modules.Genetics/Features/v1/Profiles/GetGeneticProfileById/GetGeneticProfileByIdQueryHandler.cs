using FSH.Framework.Core.Exceptions;
using FSH.Modules.Genetics.Contracts.Dtos;
using FSH.Modules.Genetics.Contracts.v1.Profiles;
using FSH.Modules.Genetics.Data;
using FSH.Modules.Genetics.Domain;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Genetics.Features.v1.Profiles.GetGeneticProfileById;

public sealed class GetGeneticProfileByIdQueryHandler(GeneticsDbContext dbContext)
    : IQueryHandler<GetGeneticProfileByIdQuery, GeneticProfileDto>
{
    public async ValueTask<GeneticProfileDto> Handle(
        GetGeneticProfileByIdQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        GeneticProfile profile = await dbContext.GeneticProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == query.ProfileId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException($"Genetic profile {query.ProfileId} not found.");

        List<ProfileLocus> loci = await dbContext.ProfileLoci
            .AsNoTracking()
            .Where(x => x.GeneticProfileId == profile.Id)
            .OrderBy(x => x.Marker)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        Guid[] locusIds = loci.Select(x => x.Id).ToArray();

        List<AlleleCall> alleles = await dbContext.AlleleCalls
            .AsNoTracking()
            .Where(x => locusIds.Contains(x.ProfileLocusId))
            .OrderBy(x => x.SortOrder)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        List<PeakObservation> peaks = await dbContext.PeakObservations
            .AsNoTracking()
            .Where(x => locusIds.Contains(x.ProfileLocusId))
            .OrderBy(x => x.SortOrder)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        IReadOnlyList<ProfileLocusDto> locusDtos = loci.Select(locus =>
            new ProfileLocusDto(
                locus.Id,
                locus.Marker,
                alleles
                    .Where(x => x.ProfileLocusId == locus.Id)
                    .Select(x => new AlleleCallDto(x.Id, x.Value, x.SortOrder))
                    .ToList(),
                peaks
                    .Where(x => x.ProfileLocusId == locus.Id)
                    .Select(x => new PeakObservationDto(
                        x.Id,
                        x.AlleleValue,
                        x.HeightRfu,
                        x.SizeBp,
                        x.Channel,
                        x.SortOrder))
                    .ToList()))
            .ToList();

        return new GeneticProfileDto(
            profile.Id,
            profile.SampleId,
            profile.ExternalProfileCode,
            profile.VersionNumber,
            profile.SupersedesProfileId,
            profile.AnalysisTypeId,
            profile.IsReference,
            profile.CreatedAtUtc,
            locusDtos);
    }
}
