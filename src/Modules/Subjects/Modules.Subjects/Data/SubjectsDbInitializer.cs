using FSH.Framework.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FSH.Modules.Subjects.Data;

public sealed class SubjectsDbInitializer(SubjectsDbContext dbContext, ILogger<SubjectsDbInitializer> logger) : IDbInitializer
{
    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        if ((await dbContext.Database.GetPendingMigrationsAsync(cancellationToken).ConfigureAwait(false)).Any()) { await dbContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(false); logger.LogInformation("[Subjects] applied migrations"); }
    }
    public Task SeedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
