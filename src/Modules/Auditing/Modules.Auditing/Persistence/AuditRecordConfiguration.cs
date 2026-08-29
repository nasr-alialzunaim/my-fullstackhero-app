using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Modules.Auditing.Persistence;

public class AuditRecordConfiguration : IEntityTypeConfiguration<AuditRecord>
{
    public void Configure(EntityTypeBuilder<AuditRecord> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("AuditRecords", "audit");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EventType).HasConversion<int>();
        builder.Property(x => x.Severity).HasConversion<byte>();
        builder.Property(x => x.Tags).HasConversion<long>();
        builder.Property(x => x.PayloadJson).HasColumnType("jsonb");

        builder.HasIndex(x => new { x.TenantId, x.OccurredAtUtc })
            .IsDescending(false, true)
            .HasDatabaseName("IX_AuditRecords_Tenant_OccurredAt");

        builder.HasIndex(x => new { x.TenantId, x.EventType, x.OccurredAtUtc })
            .IsDescending(false, false, true)
            .HasDatabaseName("IX_AuditRecords_Tenant_EventType_OccurredAt");

        builder.HasIndex(x => x.CorrelationId)
            .HasDatabaseName("IX_AuditRecords_CorrelationId");
        builder.HasIndex(x => x.TraceId)
            .HasDatabaseName("IX_AuditRecords_TraceId");

        builder.HasIndex(x => x.Source)
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops")
            .HasDatabaseName("IX_AuditRecords_Source_trgm");
        builder.HasIndex(x => x.UserName)
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops")
            .HasDatabaseName("IX_AuditRecords_UserName_trgm");

        builder.HasIndex(x => x.PayloadJson)
            .HasMethod("gin")
            .HasOperators("jsonb_path_ops")
            .HasDatabaseName("IX_AuditRecords_PayloadJson_gin");
    }
}
