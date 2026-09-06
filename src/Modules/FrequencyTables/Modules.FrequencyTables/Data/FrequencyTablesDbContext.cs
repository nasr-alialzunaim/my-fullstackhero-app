using FSH.Framework.Persistence.Context;
using FSH.Modules.FrequencyTables.Domain;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.FrequencyTables.Data;

public sealed class FrequencyTablesDbContext : BaseDbContext
{
    public const string Schema = "frequency_tables";
    public FrequencyTablesDbContext(DbContextOptions<FrequencyTablesDbContext> options) : base(options) { }
    public DbSet<FrequencyTable> FrequencyTables => Set<FrequencyTable>();
    public DbSet<FrequencyEntry> FrequencyEntries => Set<FrequencyEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FrequencyTablesDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
