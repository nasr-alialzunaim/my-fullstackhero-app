using FSH.Modules.Genetics.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Modules.Genetics.Data.Configurations;

public sealed class GeneticProfileConfiguration
    : IEntityTypeConfiguration<GeneticProfile>
{
    public void Configure(EntityTypeBuilder<GeneticProfile> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("GeneticProfiles");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.ExternalProfileCode).HasMaxLength(128);
        builder.HasIndex(x => x.SampleId);
        builder.HasIndex(x => x.SupersedesProfileId);
        builder.HasIndex(x => x.ExternalProfileCode);
        builder.HasIndex(x => new { x.SampleId, x.VersionNumber });
        builder.Ignore(x => x.DomainEvents);
    }
}
