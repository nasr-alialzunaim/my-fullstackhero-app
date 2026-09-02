using FSH.Modules.Cases.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Modules.Cases.Data.Configurations;

public sealed class ForensicCaseConfiguration : IEntityTypeConfiguration<ForensicCase>
{
    public void Configure(EntityTypeBuilder<ForensicCase> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Cases");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Number).IsRequired().HasMaxLength(64);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(4096);
        builder.HasIndex(x => x.Number).IsUnique();
        builder.Ignore(x => x.DomainEvents);
    }
}