using FSH.Framework.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FSH.Modules.DNA.Data;

public sealed class DnaDbInitializer(
    DnaDbContext dbContext,
    ILogger<DnaDbInitializer> logger) : IDbInitializer
{
    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        if ((await dbContext.Database.GetPendingMigrationsAsync(cancellationToken).ConfigureAwait(false)).Any())
        {
            await dbContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
            logger.LogInformation("[DNA] applied migrations");
        }
    }

    public Task SeedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
