using FSH.Framework.Persistence.Context;
using FSH.Modules.ScientificAnalysis.Domain;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.ScientificAnalysis.Data;

public sealed class ScientificAnalysisDbContext : BaseDbContext
{
    public const string Schema = "scientific_analysis";

    public ScientificAnalysisDbContext(
        DbContextOptions<ScientificAnalysisDbContext> options)
        : base(options)
    {
    }

    public DbSet<AnalysisRun> AnalysisRuns => Set<AnalysisRun>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ScientificAnalysisDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
