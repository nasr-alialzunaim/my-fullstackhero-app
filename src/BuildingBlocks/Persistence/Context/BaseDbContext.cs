using FSH.Framework.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace FSH.Framework.Persistence.Context;

/// <summary>
/// Base database context for the single-installation runtime.
/// Applies cross-cutting persistence behavior that is independent of tenancy.
/// </summary>
public class BaseDbContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);
        modelBuilder.AppendGlobalQueryFilter<ISoftDeletable>(
            QueryFilters.SoftDelete,
            entity => !entity.IsDeleted);
    }
}
