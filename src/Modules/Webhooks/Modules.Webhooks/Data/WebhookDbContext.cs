using FSH.Framework.Persistence.Context;
using FSH.Modules.Webhooks.Domain;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Webhooks.Data;

public sealed class WebhookDbContext : BaseDbContext
{
    public WebhookDbContext(DbContextOptions<WebhookDbContext> options) : base(options) { }

    public DbSet<WebhookSubscription> Subscriptions => Set<WebhookSubscription>();
    public DbSet<WebhookDelivery> Deliveries => Set<WebhookDelivery>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.HasDefaultSchema("webhooks");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WebhookDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
