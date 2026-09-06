using FSH.Modules.Samples.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Modules.Samples.Data.Configurations;

public sealed class SampleCustodyEventConfiguration : IEntityTypeConfiguration<SampleCustodyEvent>
{
    public void Configure(EntityTypeBuilder<SampleCustodyEvent> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("SampleCustodyEvents");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.EventType).HasConversion<string>().HasMaxLength(32);
        builder.Property(x => x.FromLocation).HasColumnType("text");
        builder.Property(x => x.ToLocation).HasColumnType("text");
        builder.Property(x => x.ContainerCode).HasMaxLength(128);
        builder.Property(x => x.SealNumber).HasMaxLength(128);
        builder.Property(x => x.Reason).HasColumnType("text");
        builder.Property(x => x.Notes).HasColumnType("text");
        builder.Property(x => x.PreviousEventHash).HasMaxLength(64);
        builder.Property(x => x.EventHash).HasMaxLength(64);
        builder.HasIndex(x => new { x.SampleId, x.OccurredAtUtc });
        builder.HasOne<BiologicalSample>()
            .WithMany()
            .HasForeignKey(x => x.SampleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class SampleProcessingEventConfiguration : IEntityTypeConfiguration<SampleProcessingEvent>
{
    public void Configure(EntityTypeBuilder<SampleProcessingEvent> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("SampleProcessingEvents");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.EventType).HasMaxLength(32);
        builder.Property(x => x.Method).HasMaxLength(128);
        builder.Property(x => x.BatchCode).HasMaxLength(128);
        builder.Property(x => x.ResultSummary).HasColumnType("text");
        builder.Property(x => x.ResultJson).HasColumnType("jsonb");
        builder.HasIndex(x => new { x.SampleId, x.StartedAtUtc });
        builder.HasIndex(x => x.KitId);
        builder.HasOne<BiologicalSample>()
            .WithMany()
            .HasForeignKey(x => x.SampleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class SampleAttachmentConfiguration : IEntityTypeConfiguration<SampleAttachment>
{
    public void Configure(EntityTypeBuilder<SampleAttachment> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("SampleAttachments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.AttachmentType).HasMaxLength(32);
        builder.Property(x => x.Description).HasColumnType("text");
        builder.HasIndex(x => x.SampleId);
        builder.HasIndex(x => x.FileAssetId);
        builder.HasOne<BiologicalSample>()
            .WithMany()
            .HasForeignKey(x => x.SampleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
