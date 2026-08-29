using FSH.Framework.Persistence.Context;
using FSH.Modules.Notifications.Domain;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Notifications.Data;

public sealed class NotificationsDbContext : BaseDbContext
{
    public const string Schema = "notifications";

    public NotificationsDbContext(DbContextOptions<NotificationsDbContext> options) : base(options) { }

    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NotificationsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
