using FSH.Modules.Evidence.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Modules.Evidence.Data.Configurations;

public sealed class EvidenceItemConfiguration : IEntityTypeConfiguration<EvidenceItem>
{
    public void Configure(EntityTypeBuilder<EvidenceItem> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("EvidenceItems");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.ExternalReference).HasMaxLength(128);
        builder.Property(x => x.Description).HasMaxLength(4096);
        builder.HasIndex(x => x.CaseId);
        builder.HasIndex(x => x.ExternalReference);
        builder.Ignore(x => x.DomainEvents);
    }
}
