using FSH.Modules.Genetics.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Modules.Genetics.Data.Configurations;

public sealed class PeakObservationConfiguration
    : IEntityTypeConfiguration<PeakObservation>
{
    public void Configure(EntityTypeBuilder<PeakObservation> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("PeakObservations");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.AlleleValue).HasMaxLength(64);
        builder.Property(x => x.Channel).HasMaxLength(64);
        builder.HasIndex(x => x.ProfileLocusId);
        builder.HasOne<ProfileLocus>()
            .WithMany()
            .HasForeignKey(x => x.ProfileLocusId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
