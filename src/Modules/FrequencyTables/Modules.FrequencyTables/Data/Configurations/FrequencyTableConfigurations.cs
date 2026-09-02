using FSH.Modules.FrequencyTables.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Modules.FrequencyTables.Data.Configurations;

public sealed class FrequencyTableConfiguration : IEntityTypeConfiguration<FrequencyTable>
{
    public void Configure(EntityTypeBuilder<FrequencyTable> builder)
    {
        builder.ToTable("FrequencyTables");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Model).IsRequired().HasMaxLength(32);
        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => x.SupersedesTableId);
        builder.HasIndex(x => x.IsDefault);
        builder.Ignore(x => x.DomainEvents);
    }
}

public sealed class FrequencyEntryConfiguration : IEntityTypeConfiguration<FrequencyEntry>
{
    public void Configure(EntityTypeBuilder<FrequencyEntry> builder)
    {
        builder.ToTable("FrequencyEntries");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Marker).IsRequired().HasMaxLength(64);
        builder.Property(x => x.Allele).IsRequired().HasMaxLength(64);
        builder.HasIndex(x => new { x.FrequencyTableId, x.Marker, x.Allele }).IsUnique();
        builder.HasIndex(x => new { x.FrequencyTableId, x.Marker });
        builder.HasOne<FrequencyTable>().WithMany().HasForeignKey(x => x.FrequencyTableId).OnDelete(DeleteBehavior.Cascade);
    }
}
