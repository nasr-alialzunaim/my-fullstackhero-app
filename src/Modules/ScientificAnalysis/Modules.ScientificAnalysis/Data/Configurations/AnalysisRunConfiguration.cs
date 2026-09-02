using FSH.Modules.ScientificAnalysis.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Modules.ScientificAnalysis.Data.Configurations;

public sealed class AnalysisRunConfiguration : IEntityTypeConfiguration<AnalysisRun>
{
    public void Configure(EntityTypeBuilder<AnalysisRun> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("AnalysisRuns");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.AlgorithmId).IsRequired().HasMaxLength(64);
        builder.Property(x => x.EngineName).IsRequired().HasMaxLength(128);
        builder.Property(x => x.EngineVersion).IsRequired().HasMaxLength(32);
        builder.Property(x => x.UpstreamCommit).IsRequired().HasMaxLength(64);
        builder.Property(x => x.RequestJson).IsRequired().HasColumnType("text");
        builder.Property(x => x.ResponseJson).HasColumnType("text");
        builder.Property(x => x.RequestSha256).IsRequired().HasMaxLength(64);
        builder.Property(x => x.ResponseSha256).HasMaxLength(64);
        builder.Property(x => x.Outcome).IsRequired().HasMaxLength(32);
        builder.HasIndex(x => x.AlgorithmId);
        builder.HasIndex(x => x.StartedAtUtc);
        builder.HasIndex(x => x.InitiatedByUserId);
        builder.Ignore(x => x.DomainEvents);
    }
}
