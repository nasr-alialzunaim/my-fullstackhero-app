using FSH.Framework.Persistence.Context;
using FSH.Modules.Genetics.Domain;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Genetics.Data;

public sealed class GeneticsDbContext : BaseDbContext
{
    public const string Schema = "genetics";

    public GeneticsDbContext(DbContextOptions<GeneticsDbContext> options)
        : base(options)
    {
    }

    public DbSet<GeneticProfile> GeneticProfiles => Set<GeneticProfile>();
    public DbSet<ProfileLocus> ProfileLoci => Set<ProfileLocus>();
    public DbSet<AlleleCall> AlleleCalls => Set<AlleleCall>();
    public DbSet<PeakObservation> PeakObservations => Set<PeakObservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GeneticsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
