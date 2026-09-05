using FSH.Modules.Cases.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Modules.Cases.Data.Configurations;

public sealed class ForensicCaseConfiguration : IEntityTypeConfiguration<ForensicCase>
{
    public void Configure(EntityTypeBuilder<ForensicCase> builder)
    {
        ArgumentNullException.ThrowIfNull(builder); builder.ToTable("Cases"); builder.HasKey(x => x.Id); builder.Property(x => x.Id).ValueGeneratedNever(); builder.Property(x => x.Number).HasMaxLength(64); builder.Property(x => x.Title).HasMaxLength(200); builder.Property(x => x.Description).HasMaxLength(4096); builder.Property(x => x.CaseType).HasMaxLength(32); builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(32); builder.Property(x => x.Priority).HasMaxLength(32); builder.Property(x => x.JurisdictionCode).HasMaxLength(64); builder.HasIndex(x => x.Number).IsUnique(); builder.HasIndex(x => x.Status); builder.HasIndex(x => x.CaseType); builder.HasIndex(x => x.OpenedAtUtc); builder.Ignore(x => x.DomainEvents);
    }
}

public sealed class CaseAssignmentConfiguration : IEntityTypeConfiguration<CaseAssignment>
{
    public void Configure(EntityTypeBuilder<CaseAssignment> builder) { builder.ToTable("CaseAssignments"); builder.HasKey(x => x.Id); builder.Property(x => x.Id).ValueGeneratedNever(); builder.Property(x => x.AssignmentRole).HasMaxLength(64); builder.HasIndex(x => new { x.CaseId, x.UserId }); builder.HasIndex(x => x.UserId); }
}

public sealed class CaseStatusHistoryConfiguration : IEntityTypeConfiguration<CaseStatusHistory>
{
    public void Configure(EntityTypeBuilder<CaseStatusHistory> builder) { builder.ToTable("CaseStatusHistory"); builder.HasKey(x => x.Id); builder.Property(x => x.Id).ValueGeneratedNever(); builder.Property(x => x.FromStatus).HasConversion<string>().HasMaxLength(32); builder.Property(x => x.ToStatus).HasConversion<string>().HasMaxLength(32); builder.Property(x => x.Reason).HasColumnType("text"); builder.HasIndex(x => new { x.CaseId, x.ChangedAtUtc }); }
}
