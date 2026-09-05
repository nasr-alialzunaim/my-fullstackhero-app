using FSH.Modules.Samples.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Modules.Samples.Data.Configurations;

public sealed class BiologicalSampleConfiguration : IEntityTypeConfiguration<BiologicalSample>
{
    public void Configure(EntityTypeBuilder<BiologicalSample> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("BiologicalSamples", table =>
            table.HasCheckConstraint(
                "CK_BiologicalSamples_Context",
                "(\"SampleContext\" = 'CaseSample' AND \"CaseId\" IS NOT NULL AND \"SubjectId\" IS NULL) OR " +
                "(\"SampleContext\" = 'KnownReference' AND \"CaseId\" IS NULL AND \"SubjectId\" IS NOT NULL) OR " +
                "(\"SampleContext\" = 'Unknown' AND \"CaseId\" IS NULL AND \"SubjectId\" IS NULL)"));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.SampleCode).HasMaxLength(128);
        builder.Property(x => x.ExternalSampleCode).HasMaxLength(128);
        builder.Property(x => x.SampleContext).HasConversion<string>().HasMaxLength(32);
        builder.Property(x => x.SampleType).HasMaxLength(64);
        builder.Property(x => x.Matrix).HasMaxLength(64);
        builder.Property(x => x.CollectionLocation).HasColumnType("text");
        builder.Property(x => x.CollectionNote).HasMaxLength(4096);
        builder.Property(x => x.ContainerCode).HasMaxLength(128);
        builder.Property(x => x.SealNumber).HasMaxLength(128);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(32);
        builder.HasIndex(x => x.SampleCode).IsUnique();
        builder.HasIndex(x => x.ExternalSampleCode);
        builder.HasIndex(x => x.SampleContext);
        builder.HasIndex(x => x.CaseId);
        builder.HasIndex(x => x.SubjectId);
        builder.HasIndex(x => x.ParentSampleId);
        builder.HasIndex(x => x.Status);
        builder.Ignore(x => x.DomainEvents);
    }
}
