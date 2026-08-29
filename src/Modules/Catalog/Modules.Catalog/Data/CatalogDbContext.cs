using FSH.Framework.Persistence.Context;
using FSH.Modules.Catalog.Domain;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Catalog.Data;

public sealed class CatalogDbContext : BaseDbContext
{
    public const string Schema = "catalog";

    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }

    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
