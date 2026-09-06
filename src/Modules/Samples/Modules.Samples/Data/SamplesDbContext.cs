using FSH.Framework.Persistence.Context;
using FSH.Modules.Samples.Domain;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Samples.Data;

public sealed class SamplesDbContext : BaseDbContext
{
    public const string Schema = "samples";

    public SamplesDbContext(DbContextOptions<SamplesDbContext> options)
        : base(options)
    {
    }

    public DbSet<BiologicalSample> BiologicalSamples => Set<BiologicalSample>();
    public DbSet<SampleCustodyEvent> SampleCustodyEvents => Set<SampleCustodyEvent>();
    public DbSet<SampleProcessingEvent> SampleProcessingEvents => Set<SampleProcessingEvent>();
    public DbSet<SampleAttachment> SampleAttachments => Set<SampleAttachment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SamplesDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
