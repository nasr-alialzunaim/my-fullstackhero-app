using FSH.Modules.Subjects.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Modules.Subjects.Data;

public sealed class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.ToTable("Subjects"); builder.HasKey(x => x.Id); builder.Property(x => x.Id).ValueGeneratedNever(); builder.Property(x => x.SubjectCode).HasMaxLength(64); builder.Property(x => x.SubjectType).HasConversion<string>().HasMaxLength(32); builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(32); builder.HasIndex(x => x.SubjectCode).IsUnique(); builder.HasIndex(x => x.SubjectType); builder.HasIndex(x => x.Status); builder.Ignore(x => x.DomainEvents);
    }
}

public sealed class PersonIdentityConfiguration : IEntityTypeConfiguration<PersonIdentity>
{
    public void Configure(EntityTypeBuilder<PersonIdentity> builder)
    {
        builder.ToTable("PersonIdentities"); builder.HasKey(x => x.SubjectId); builder.Property(x => x.NationalIdHash).HasMaxLength(64); builder.Property(x => x.NationalIdProtected).HasColumnType("text"); builder.Property(x => x.FirstName).HasMaxLength(128); builder.Property(x => x.MiddleName).HasMaxLength(128); builder.Property(x => x.LastName).HasMaxLength(128); builder.Property(x => x.Sex).HasMaxLength(16); builder.Property(x => x.NationalityCode).HasMaxLength(8); builder.HasIndex(x => x.NationalIdHash); builder.HasOne<Subject>().WithOne().HasForeignKey<PersonIdentity>(x => x.SubjectId).OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class SubjectAliasConfiguration : IEntityTypeConfiguration<SubjectAlias>
{
    public void Configure(EntityTypeBuilder<SubjectAlias> builder)
    {
        builder.ToTable("SubjectAliases"); builder.HasKey(x => x.Id); builder.Property(x => x.Id).ValueGeneratedNever(); builder.Property(x => x.AliasType).HasMaxLength(32); builder.Property(x => x.AliasValue).HasMaxLength(256); builder.HasIndex(x => new { x.SubjectId, x.AliasType, x.AliasValue }).IsUnique(); builder.HasOne<Subject>().WithMany().HasForeignKey(x => x.SubjectId).OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class SubjectExternalIdentifierConfiguration : IEntityTypeConfiguration<SubjectExternalIdentifier>
{
    public void Configure(EntityTypeBuilder<SubjectExternalIdentifier> builder)
    {
        builder.ToTable("SubjectExternalIdentifiers"); builder.HasKey(x => x.Id); builder.Property(x => x.Id).ValueGeneratedNever(); builder.Property(x => x.IdentifierType).HasMaxLength(32); builder.Property(x => x.ValueProtected).HasColumnType("text"); builder.Property(x => x.ValueHash).HasMaxLength(64); builder.Property(x => x.Issuer).HasMaxLength(128); builder.HasIndex(x => x.ValueHash); builder.HasIndex(x => new { x.SubjectId, x.IdentifierType }); builder.HasOne<Subject>().WithMany().HasForeignKey(x => x.SubjectId).OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class SubjectLegalReferenceConfiguration : IEntityTypeConfiguration<SubjectLegalReference>
{
    public void Configure(EntityTypeBuilder<SubjectLegalReference> builder)
    {
        builder.ToTable("SubjectLegalReferences"); builder.HasKey(x => x.Id); builder.Property(x => x.Id).ValueGeneratedNever(); builder.Property(x => x.ReferenceType).HasMaxLength(64); builder.Property(x => x.ReferenceNumber).HasMaxLength(128); builder.Property(x => x.Authority).HasMaxLength(256); builder.Property(x => x.Description).HasColumnType("text"); builder.HasIndex(x => x.SubjectId); builder.HasOne<Subject>().WithMany().HasForeignKey(x => x.SubjectId).OnDelete(DeleteBehavior.Cascade);
    }
}
