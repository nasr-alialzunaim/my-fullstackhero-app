using FSH.Framework.Persistence.Context;
using FSH.Modules.Files.Domain;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Files.Data;

public sealed class FilesDbContext : BaseDbContext
{
    public const string Schema = "files";

    public FilesDbContext(DbContextOptions<FilesDbContext> options) : base(options) { }

    public DbSet<FileAsset> FileAssets => Set<FileAsset>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FilesDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
