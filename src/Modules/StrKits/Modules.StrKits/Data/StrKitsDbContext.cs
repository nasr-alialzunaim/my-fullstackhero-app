using FSH.Framework.Persistence.Context;
using FSH.Modules.StrKits.Domain;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.StrKits.Data;

public sealed class StrKitsDbContext : BaseDbContext
{
    public const string Schema = "str_kits";
    public StrKitsDbContext(DbContextOptions<StrKitsDbContext> options) : base(options) { }

    public DbSet<StrKit> StrKits => Set<StrKit>();
    public DbSet<StrKitAlias> StrKitAliases => Set<StrKitAlias>();
    public DbSet<StrKitLocus> StrKitLoci => Set<StrKitLocus>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StrKitsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
