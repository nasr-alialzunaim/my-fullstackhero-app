using FSH.Framework.Shared.Constants;
using FSH.Modules.Catalog.Contracts.Authorization;
using FSH.Modules.Identity.Authorization;
using FSH.Modules.Identity.Data;
using FSH.Modules.Identity.Domain;
using Integration.Tests.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Integration.Tests.Tests.Catalog;

/// <summary>
/// Verifies that role-permission synchronization restores permission claims that may
/// be introduced by later releases of the installation.
/// </summary>
[Collection(FshCollectionDefinition.Name)]
public sealed class RolePermissionSyncerTests
{
    private readonly FshWebApplicationFactory _factory;

    public RolePermissionSyncerTests(FshWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task SyncAsync_Should_Restore_Missing_Permission_Claims_For_Admin_Role()
    {
        var catalogPermissions = CatalogPermissions.All
            .Select(p => p.Name)
            .ToHashSet(StringComparer.Ordinal);

        await WipeClaimsAsync("Admin", catalogPermissions);

        var afterWipe = await GetClaimsAsync("Admin");
        afterWipe.Intersect(catalogPermissions).ShouldBeEmpty(
            "Pre-condition: Admin must not have any Catalog claims after the wipe.");

        using (var scope = _factory.Services.CreateScope())
        {
            var syncer = scope.ServiceProvider.GetRequiredService<RolePermissionSyncer>();
            await syncer.SyncAsync(CancellationToken.None);
        }

        var afterSync = await GetClaimsAsync("Admin");
        var missing = catalogPermissions.Where(p => !afterSync.Contains(p)).ToList();
        missing.ShouldBeEmpty(
            $"Syncer failed to restore {missing.Count} catalog permission(s): " +
            $"[{string.Join(", ", missing)}]");
    }

    [Fact]
    public async Task SyncAsync_Should_Be_Idempotent_When_Claims_Already_Exist()
    {
        var before = await GetClaimsAsync("Admin");

        using (var scope = _factory.Services.CreateScope())
        {
            var syncer = scope.ServiceProvider.GetRequiredService<RolePermissionSyncer>();
            await syncer.SyncAsync(CancellationToken.None);
            await syncer.SyncAsync(CancellationToken.None);
        }

        var after = await GetClaimsAsync("Admin");
        after.Count.ShouldBe(before.Count, "Syncer must not duplicate existing permission claims.");
    }

    private async Task WipeClaimsAsync(string roleName, IReadOnlyCollection<string> claimValues)
    {
        using var scope = _factory.Services.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<FshRole>>();
        var role = await roleManager.Roles.SingleAsync(r => r.Name == roleName);

        var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        var values = claimValues.ToHashSet(StringComparer.Ordinal);

        var toRemove = await db.RoleClaims
            .Where(rc => rc.RoleId == role.Id && rc.ClaimType == ClaimConstants.Permission)
            .ToListAsync();

        db.RoleClaims.RemoveRange(
            toRemove.Where(rc => rc.ClaimValue is not null && values.Contains(rc.ClaimValue)));

        await db.SaveChangesAsync();
    }

    private async Task<HashSet<string>> GetClaimsAsync(string roleName)
    {
        using var scope = _factory.Services.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<FshRole>>();
        var role = await roleManager.Roles.SingleAsync(r => r.Name == roleName);

        var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        var claims = await db.RoleClaims
            .Where(rc => rc.RoleId == role.Id && rc.ClaimType == ClaimConstants.Permission)
            .Select(rc => rc.ClaimValue!)
            .ToListAsync();

        return claims.ToHashSet(StringComparer.Ordinal);
    }
}
