using FSH.Framework.Persistence;
using FSH.Framework.Shared.Constants;
using FSH.Framework.Shared.Multitenancy;
using FSH.Modules.Identity.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FSH.Modules.Identity.Data;

internal sealed class IdentityDbInitializer(
    ILogger<IdentityDbInitializer> logger,
    IdentityDbContext context,
    RoleManager<FshRole> roleManager,
    UserManager<FshUser> userManager,
    TimeProvider timeProvider,
    IConfiguration configuration) : IDbInitializer
{
    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        if ((await context.Database.GetPendingMigrationsAsync(cancellationToken).ConfigureAwait(false)).Any())
        {
            await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("[{Installation}] applied database migrations for identity module", MultitenancyConstants.Root.Id);
            }
        }
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        await SeedRolesAsync(cancellationToken);
        await SeedSystemGroupsAsync(cancellationToken);
        await SeedAdminUserAsync(cancellationToken);
    }

    private async Task SeedRolesAsync(CancellationToken cancellationToken = default)
    {
        foreach (string roleName in RoleConstants.DefaultRoles)
        {
            if (await roleManager.Roles.SingleOrDefaultAsync(r => r.Name == roleName, cancellationToken)
                is not FshRole role)
            {
                // create role
                role = new FshRole(roleName, $"{roleName} role for this installation");
                await roleManager.CreateAsync(role);
            }

            // Assign permissions
            if (roleName == RoleConstants.Basic)
            {
                await AssignPermissionsToRoleAsync(context, PermissionConstants.Basic, role, cancellationToken);
            }
            else if (roleName == RoleConstants.Admin)
            {
                await AssignPermissionsToRoleAsync(context, PermissionConstants.Admin, role, cancellationToken);
                await AssignPermissionsToRoleAsync(context, PermissionConstants.Root, role, cancellationToken);
            }
        }
    }

    private async Task AssignPermissionsToRoleAsync(IdentityDbContext dbContext, IReadOnlyList<FshPermission> permissions, FshRole role, CancellationToken cancellationToken = default)
    {
        var currentClaims = await roleManager.GetClaimsAsync(role);
        var newClaims = permissions
            .Where(permission => !currentClaims.Any(c => c.Type == ClaimConstants.Permission && c.Value == permission.Name))
            .Select(permission => new FshRoleClaim
            {
                RoleId = role.Id,
                ClaimType = ClaimConstants.Permission,
                ClaimValue = permission.Name,
                CreatedBy = "application",
                CreatedOn = timeProvider.GetUtcNow()
            })
            .ToList();

        foreach (var claim in newClaims)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Seeding {Role} permission '{Permission}' for installation '{InstallationId}'.", role.Name, claim.ClaimValue, MultitenancyConstants.Root.Id);
            }
            await dbContext.RoleClaims.AddAsync(claim, cancellationToken);
        }

        // Save changes to the database context
        if (newClaims.Count != 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }

    }

    private async Task SeedSystemGroupsAsync(CancellationToken cancellationToken = default)
    {
        const string tenantId = MultitenancyConstants.Root.Id;

        // Seed "All Users" default group - all new users are automatically added to this group
        const string allUsersGroupName = "All Users";
        var allUsersGroup = await context.Groups
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Name == allUsersGroupName && g.IsSystemGroup, cancellationToken);

        if (allUsersGroup is null)
        {
            allUsersGroup = Group.Create(
                name: allUsersGroupName,
                description: "Default group for all users. New users are automatically added to this group.",
                isDefault: true,
                isSystemGroup: true,
                createdBy: "System");

            await context.Groups.AddAsync(allUsersGroup, cancellationToken);
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Seeding '{GroupName}' system group for '{TenantId}' Tenant.", allUsersGroupName, tenantId);
            }
        }

        // Seed "Administrators" group with Admin role
        const string administratorsGroupName = "Administrators";
        var administratorsGroup = await context.Groups
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Name == administratorsGroupName && g.IsSystemGroup, cancellationToken);

        if (administratorsGroup is null)
        {
            administratorsGroup = Group.Create(
                name: administratorsGroupName,
                description: "System group for administrators with full administrative privileges.",
                isDefault: false,
                isSystemGroup: true,
                createdBy: "System");

            await context.Groups.AddAsync(administratorsGroup, cancellationToken);
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Seeding '{GroupName}' system group for '{TenantId}' Tenant.", administratorsGroupName, tenantId);
            }
        }

        await context.SaveChangesAsync(cancellationToken);

        // Assign Admin role to Administrators group
        var adminRole = await roleManager.FindByNameAsync(RoleConstants.Admin);
        if (adminRole is not null)
        {
            var existingGroupRole = await context.GroupRoles
                .AsNoTracking()
                .FirstOrDefaultAsync(gr => gr.GroupId == administratorsGroup.Id && gr.RoleId == adminRole.Id, cancellationToken);

            if (existingGroupRole is null)
            {
                context.GroupRoles.Add(GroupRole.Create(administratorsGroup.Id, adminRole.Id));

                await context.SaveChangesAsync(cancellationToken);
                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("Assigned Admin role to '{GroupName}' group for '{TenantId}' Tenant.", administratorsGroupName, tenantId);
                }
            }
        }
    }

    private async Task SeedAdminUserAsync(CancellationToken cancellationToken = default)
    {
        const string installationId = MultitenancyConstants.Root.Id;
        const string adminEmail = MultitenancyConstants.Root.EmailAddress;

        if (await userManager.Users
            .FirstOrDefaultAsync(u => u.Email == adminEmail, cancellationToken)
            .ConfigureAwait(false) is not FshUser adminUser)
        {
            string adminUserName = $"{installationId}.{RoleConstants.Admin}".ToUpperInvariant();
            adminUser = new FshUser
            {
                FirstName = MultitenancyConstants.Root.Name,
                LastName = RoleConstants.Admin,
                Email = adminEmail,
                UserName = adminUserName,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                NormalizedEmail = adminEmail.ToUpperInvariant(),
                NormalizedUserName = adminUserName.ToUpperInvariant(),
                ImageUrl = null,
                IsActive = true
            };

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(
                    "Seeding default admin user for installation '{InstallationId}'.",
                    installationId);
            }

            string initialPassword = ResolveInitialAdminPassword(installationId);
            var password = new PasswordHasher<FshUser>();
            adminUser.PasswordHash = password.HashPassword(adminUser, initialPassword);

            IdentityResult createResult = await userManager.CreateAsync(adminUser).ConfigureAwait(false);
            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to seed admin user for installation '{installationId}': "
                    + string.Join("; ", createResult.Errors.Select(e => e.Description)));
            }
        }

        if (!await userManager.IsInRoleAsync(adminUser, RoleConstants.Admin).ConfigureAwait(false))
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(
                    "Assigning Admin role to installation admin '{InstallationId}'.",
                    installationId);
            }

            await userManager.AddToRoleAsync(adminUser, RoleConstants.Admin).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Resolve the initial password for the installation admin.
    /// The single-installation runtime accepts it only from <c>Seed:DefaultAdminPassword</c>.
    /// Operators provide the value through environment variables, user-secrets, or a
    /// production secrets manager. Refusing to seed is safer than minting a predictable secret.
    /// </summary>
    private string ResolveInitialAdminPassword(string tenantId)
    {
        var fromConfig = configuration["Seed:DefaultAdminPassword"];
        if (!string.IsNullOrWhiteSpace(fromConfig))
        {
            return fromConfig;
        }

        throw new InvalidOperationException(
            $"No initial admin password available for installation '{tenantId}'. " +
            "Set 'Seed:DefaultAdminPassword' in configuration for the installation seed.");
    }
}