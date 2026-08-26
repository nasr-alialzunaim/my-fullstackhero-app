using Finbuckle.MultiTenant.Abstractions;
using FSH.Framework.Persistence.Context;
using FSH.Framework.Shared.Multitenancy;
using FSH.Framework.Shared.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace FSH.Modules.DNA.Data;

public sealed class DnaDbContext : BaseDbContext
{
    public const string Schema = "dna";

    public DnaDbContext(
        IMultiTenantContextAccessor<AppTenantInfo> multiTenantContextAccessor,
        DbContextOptions<DnaDbContext> options,
        IOptions<DatabaseOptions> settings,
        IHostEnvironment environment)
        : base(multiTenantContextAccessor, options, settings, environment)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DnaDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
