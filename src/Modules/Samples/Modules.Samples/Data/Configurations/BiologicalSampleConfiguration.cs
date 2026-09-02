using FSH.Modules.Samples.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Modules.Samples.Data.Configurations;

public sealed class BiologicalSampleConfiguration
    : IEntityTypeConfiguration<BiologicalSample>
{
    public void Configure(EntityTypeBuilder<BiologicalSample> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("BiologicalSamples");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.ExternalSampleCode).HasMaxLength(128);
        builder.Property(x => x.CollectionNote).HasMaxLength(4096);
        builder.HasIndex(x => x.EvidenceItemId);
        builder.HasIndex(x => x.ParentSampleId);
        builder.HasIndex(x => x.ExternalSampleCode);
        builder.Ignore(x => x.DomainEvents);
    }
}
