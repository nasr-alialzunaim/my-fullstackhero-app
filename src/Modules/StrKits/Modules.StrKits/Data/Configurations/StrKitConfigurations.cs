using FSH.Modules.StrKits.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Modules.StrKits.Data.Configurations;

public sealed class StrKitConfiguration : IEntityTypeConfiguration<StrKit>
{
    public void Configure(EntityTypeBuilder<StrKit> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("StrKits");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.KitCode).IsRequired().HasMaxLength(128);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.HasIndex(x => x.KitCode);
        builder.HasIndex(x => x.SupersedesKitId);
        builder.Ignore(x => x.DomainEvents);
    }
}

public sealed class StrKitAliasConfiguration : IEntityTypeConfiguration<StrKitAlias>
{
    public void Configure(EntityTypeBuilder<StrKitAlias> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("StrKitAliases");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Alias).IsRequired().HasMaxLength(128);
        builder.HasIndex(x => new { x.StrKitId, x.Alias }).IsUnique();
        builder.HasOne<StrKit>()
            .WithMany()
            .HasForeignKey(x => x.StrKitId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class StrKitLocusConfiguration : IEntityTypeConfiguration<StrKitLocus>
{
    public void Configure(EntityTypeBuilder<StrKitLocus> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("StrKitLoci");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Marker).IsRequired().HasMaxLength(64);
        builder.Property(x => x.Chromosome).HasMaxLength(32);
        builder.Property(x => x.Fluorophore).HasMaxLength(64);
        builder.HasIndex(x => new { x.StrKitId, x.Marker }).IsUnique();
        builder.HasIndex(x => new { x.StrKitId, x.Order }).IsUnique();
        builder.HasOne<StrKit>()
            .WithMany()
            .HasForeignKey(x => x.StrKitId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
