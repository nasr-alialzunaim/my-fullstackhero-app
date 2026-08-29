using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FSH.Modules.Identity.Authorization;

/// <summary>
/// Runs once on host startup and synchronizes registered permissions into the installation roles.
/// The operation is idempotent and best-effort; database migration/seeding is performed by DbMigrator.
/// </summary>
internal sealed class RolePermissionSyncHostedService(
    IServiceProvider serviceProvider,
    ILogger<RolePermissionSyncHostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var syncer = scope.ServiceProvider.GetRequiredService<RolePermissionSyncer>();
            await syncer.SyncAsync(stoppingToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Normal host shutdown.
        }
        catch (Exception ex)
        {
            // Permission sync is best-effort; DbMigrator/seed remains the authoritative setup path.
            logger.LogError(
                ex,
                "Role permission sync failed; new permissions may not be available until the next startup.");
        }
    }
}
