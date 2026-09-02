using System.Net;
using FSH.Framework.Core.Exceptions;
using FSH.Modules.Genetics.Contracts.v1.Profiles;
using FSH.Modules.Genetics.Data;
using FSH.Modules.Genetics.Domain;
using FSH.Modules.Samples.Contracts.v1.Samples;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Genetics.Features.v1.Profiles.CreateGeneticProfile;

public sealed class CreateGeneticProfileCommandHandler(
    GeneticsDbContext dbContext,
    IMediator mediator)
    : ICommandHandler<CreateGeneticProfileCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        CreateGeneticProfileCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        _ = await mediator.Send(
            new GetBiologicalSampleByIdQuery(command.SampleId),
            cancellationToken).ConfigureAwait(false);

        int versionNumber = 1;

        if (command.SupersedesProfileId.HasValue)
        {
            GeneticProfile previous = await dbContext.GeneticProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == command.SupersedesProfileId.Value,
                    cancellationToken)
                .ConfigureAwait(false)
                ?? throw new NotFoundException(
                    $"Superseded profile {command.SupersedesProfileId.Value} not found.");

            if (previous.SampleId != command.SampleId)
            {
                throw new CustomException(
                    "A profile version may only supersede a profile from the same biological sample.",
                    (IEnumerable<string>?)null,
                    HttpStatusCode.BadRequest);
            }

            versionNumber = checked(previous.VersionNumber + 1);
        }

        GeneticProfile profile = GeneticProfile.Create(
            command.SampleId,
            command.ExternalProfileCode,
            versionNumber,
            command.SupersedesProfileId,
            command.AnalysisTypeId,
            command.IsReference);

        dbContext.GeneticProfiles.Add(profile);

        foreach (ProfileLocusInput locusInput in command.Loci)
        {
            ProfileLocus locus = ProfileLocus.Create(profile.Id, locusInput.Marker);
            dbContext.ProfileLoci.Add(locus);

            for (int i = 0; i < locusInput.Alleles.Count; i++)
            {
                dbContext.AlleleCalls.Add(
                    AlleleCall.Create(locus.Id, locusInput.Alleles[i], i));
            }

            for (int i = 0; i < locusInput.Peaks.Count; i++)
            {
                PeakObservationInput peak = locusInput.Peaks[i];
                dbContext.PeakObservations.Add(
                    PeakObservation.Create(
                        locus.Id,
                        peak.AlleleValue,
                        peak.HeightRfu,
                        peak.SizeBp,
                        peak.Channel,
                        i));
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return profile.Id;
    }
}
