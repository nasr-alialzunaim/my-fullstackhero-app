using FSH.Framework.Eventing.Inbox;
using FSH.Framework.Eventing.Outbox;
using FSH.Modules.Identity.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Identity.Data;

public class IdentityDbContext :
    Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext<
        FshUser,
        FshRole,
        string,
        IdentityUserClaim<string>,
        IdentityUserRole<string>,
        IdentityUserLogin<string>,
        FshRoleClaim,
        IdentityUserToken<string>,
        IdentityUserPasskey<string>>
{
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();
    public DbSet<PasswordHistory> PasswordHistories => Set<PasswordHistory>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<GroupRole> GroupRoles => Set<GroupRole>();
    public DbSet<UserGroup> UserGroups => Set<UserGroup>();
    public DbSet<ImpersonationGrant> ImpersonationGrants => Set<ImpersonationGrant>();

    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
        builder.ApplyConfiguration(new OutboxMessageConfiguration(IdentityModuleConstants.SchemaName));
        builder.ApplyConfiguration(new InboxMessageConfiguration(IdentityModuleConstants.SchemaName));
    }
}
