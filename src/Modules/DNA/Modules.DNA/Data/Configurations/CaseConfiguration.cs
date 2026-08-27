using FSH.Modules.DNA.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Modules.DNA.Data.Configurations;

public sealed class CaseConfiguration : IEntityTypeConfiguration<DnaCase>
{
    public void Configure(EntityTypeBuilder<DnaCase> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Cases");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CaseNumber).IsRequired().HasMaxLength(64);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(256);
        builder.Property(x => x.Description).HasMaxLength(4000);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(32);
        builder.Property(x => x.DeletedBy).HasMaxLength(64);

        builder.HasIndex(x => x.CaseNumber)
            .IsUnique()
            .HasFilter("\"IsDeleted\" = FALSE");
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.IsDeleted);
        builder.Ignore(x => x.DomainEvents);
    }
}
