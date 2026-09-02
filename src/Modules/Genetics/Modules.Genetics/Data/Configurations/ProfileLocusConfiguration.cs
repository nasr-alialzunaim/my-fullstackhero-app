using FSH.Modules.Genetics.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Modules.Genetics.Data.Configurations;

public sealed class ProfileLocusConfiguration
    : IEntityTypeConfiguration<ProfileLocus>
{
    public void Configure(EntityTypeBuilder<ProfileLocus> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("ProfileLoci");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Marker).IsRequired().HasMaxLength(64);
        builder.HasIndex(x => x.GeneticProfileId);
        builder.HasIndex(x => new { x.GeneticProfileId, x.Marker }).IsUnique();
        builder.HasOne<GeneticProfile>()
            .WithMany()
            .HasForeignKey(x => x.GeneticProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
