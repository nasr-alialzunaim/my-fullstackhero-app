using FSH.Framework.Persistence.Context;
using FSH.Modules.Tickets.Domain;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Tickets.Data;

public sealed class TicketsDbContext : BaseDbContext
{
    public const string Schema = "tickets";

    public TicketsDbContext(DbContextOptions<TicketsDbContext> options) : base(options) { }

    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<TicketComment> TicketComments => Set<TicketComment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TicketsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
