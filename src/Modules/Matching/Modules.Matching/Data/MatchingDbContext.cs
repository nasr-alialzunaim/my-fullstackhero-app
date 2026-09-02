using FSH.Framework.Persistence.Context;
using FSH.Modules.Matching.Domain;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Matching.Data;

public sealed class MatchingDbContext : BaseDbContext
{
    public const string Schema = "matching";
    public MatchingDbContext(DbContextOptions<MatchingDbContext> options) : base(options) { }

    public DbSet<ProfileCategory> ProfileCategories => Set<ProfileCategory>();
    public DbSet<MatchingRule> MatchingRules => Set<MatchingRule>();
    public DbSet<ProfileMatchingConfiguration> ProfileConfigurations => Set<ProfileMatchingConfiguration>();
    public DbSet<AutosomalMatchSearch> AutosomalMatchSearches => Set<AutosomalMatchSearch>();
    public DbSet<AutosomalMatchResult> AutosomalMatchResults => Set<AutosomalMatchResult>();
    public DbSet<MatchHit> MatchHits => Set<MatchHit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MatchingDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
