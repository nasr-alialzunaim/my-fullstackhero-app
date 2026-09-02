using FSH.Modules.Genetics.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Modules.Genetics.Data.Configurations;

public sealed class AlleleCallConfiguration
    : IEntityTypeConfiguration<AlleleCall>
{
    public void Configure(EntityTypeBuilder<AlleleCall> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("AlleleCalls");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Value).IsRequired().HasMaxLength(64);
        builder.HasIndex(x => x.ProfileLocusId);
        builder.HasOne<ProfileLocus>()
            .WithMany()
            .HasForeignKey(x => x.ProfileLocusId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
