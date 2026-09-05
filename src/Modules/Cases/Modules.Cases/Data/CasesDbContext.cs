using FSH.Framework.Persistence.Context;
using FSH.Modules.Cases.Domain;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Cases.Data;

public sealed class CasesDbContext : BaseDbContext
{
    public const string Schema = "cases";
    public CasesDbContext(DbContextOptions<CasesDbContext> options) : base(options) { }
    public DbSet<ForensicCase> Cases => Set<ForensicCase>();
    public DbSet<CaseAssignment> CaseAssignments => Set<CaseAssignment>();
    public DbSet<CaseStatusHistory> CaseStatusHistory => Set<CaseStatusHistory>();
    protected override void OnModelCreating(ModelBuilder modelBuilder) { ArgumentNullException.ThrowIfNull(modelBuilder); modelBuilder.HasDefaultSchema(Schema); modelBuilder.ApplyConfigurationsFromAssembly(typeof(CasesDbContext).Assembly); base.OnModelCreating(modelBuilder); }
}
