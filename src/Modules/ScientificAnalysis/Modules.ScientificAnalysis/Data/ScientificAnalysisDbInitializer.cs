using FSH.Framework.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FSH.Modules.ScientificAnalysis.Data;

public sealed class ScientificAnalysisDbInitializer(
    ScientificAnalysisDbContext dbContext,
    ILogger<ScientificAnalysisDbInitializer> logger) : IDbInitializer
{
    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        if ((await dbContext.Database.GetPendingMigrationsAsync(cancellationToken)
            .ConfigureAwait(false)).Any())
        {
            await dbContext.Database.MigrateAsync(cancellationToken)
                .ConfigureAwait(false);
            logger.LogInformation("[ScientificAnalysis] applied migrations");
        }
    }

    public Task SeedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
