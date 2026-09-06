using FSH.Framework.Persistence.Context;
using FSH.Modules.Subjects.Domain;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Subjects.Data;

public sealed class SubjectsDbContext : BaseDbContext
{
    public const string Schema = "subjects";
    public SubjectsDbContext(DbContextOptions<SubjectsDbContext> options) : base(options) { }
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<PersonIdentity> PersonIdentities => Set<PersonIdentity>();
    public DbSet<SubjectAlias> SubjectAliases => Set<SubjectAlias>();
    public DbSet<SubjectExternalIdentifier> SubjectExternalIdentifiers => Set<SubjectExternalIdentifier>();
    public DbSet<SubjectLegalReference> SubjectLegalReferences => Set<SubjectLegalReference>();
    protected override void OnModelCreating(ModelBuilder modelBuilder) { ArgumentNullException.ThrowIfNull(modelBuilder); modelBuilder.HasDefaultSchema(Schema); modelBuilder.ApplyConfigurationsFromAssembly(typeof(SubjectsDbContext).Assembly); base.OnModelCreating(modelBuilder); }
}
