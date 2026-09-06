using FSH.Framework.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FSH.Modules.Cases.Data;

public sealed class CasesDbInitializer(
    CasesDbContext dbContext,
    ILogger<CasesDbInitializer> logger) : IDbInitializer
{
    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        if ((await dbContext.Database.GetPendingMigrationsAsync(cancellationToken).ConfigureAwait(false)).Any())
        {
            await dbContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
            logger.LogInformation("[Cases] applied migrations");
        }
    }

    public Task SeedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}