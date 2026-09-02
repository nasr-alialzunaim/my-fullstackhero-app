using FSH.Framework.Persistence.Context;
using FSH.Modules.Evidence.Domain;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Evidence.Data;

public sealed class EvidenceDbContext : BaseDbContext
{
    public const string Schema = "evidence";

    public EvidenceDbContext(DbContextOptions<EvidenceDbContext> options)
        : base(options)
    {
    }

    public DbSet<EvidenceItem> EvidenceItems => Set<EvidenceItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EvidenceDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
