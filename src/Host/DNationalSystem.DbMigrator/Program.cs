using System.Reflection;
using DNationalSystem.DbMigrator;
using FSH.Framework.Persistence;
using FSH.Framework.Web;
using FSH.Framework.Web.Modules;
using FSH.Modules.Auditing;
using FSH.Modules.Billing;
using FSH.Modules.Catalog;
using FSH.Modules.Identity;
using FSH.Modules.Identity.Contracts.v1.Tokens.TokenGeneration;
using FSH.Modules.Identity.Features.v1.Tokens.TokenGeneration;
using FSH.Modules.Tickets;
using FSH.Modules.Webhooks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var cli = MigratorCommand.Parse(args);
if (cli.Help)
{
    await Console.Out.WriteLineAsync(MigratorCommand.HelpText).ConfigureAwait(false);
    return 0;
}

var builder = Host.CreateApplicationBuilder(args);
builder.ConfigureContainer(new DefaultServiceProviderFactory(
    new ServiceProviderOptions { ValidateOnBuild = false, ValidateScopes = false }));

builder.Configuration.AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.json"), optional: true);
builder.Configuration.AddJsonFile(
    Path.Combine(AppContext.BaseDirectory, $"appsettings.{builder.Environment.EnvironmentName}.json"),
    optional: true);
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddCommandLine(args);

if (string.IsNullOrWhiteSpace(builder.Configuration["JwtOptions:SigningKey"]))
{
    builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
    {
        ["JwtOptions:SigningKey"] = "fsh-dbmigrator-placeholder-never-mints-tokens-32+",
        ["JwtOptions:Issuer"] = builder.Configuration["JwtOptions:Issuer"] ?? "fsh.local",
        ["JwtOptions:Audience"] = builder.Configuration["JwtOptions:Audience"] ?? "fsh.clients",
    });
}

if (string.IsNullOrWhiteSpace(builder.Configuration["DatabaseOptions:ConnectionString"]))
{
    await Console.Error.WriteLineAsync(
        "[migrator] FAILED: DatabaseOptions:ConnectionString is empty. " +
        "Set DatabaseOptions__ConnectionString before invoking the migrator.")
        .ConfigureAwait(false);
    return 1;
}

builder.Services.AddMediator(o =>
{
    o.ServiceLifetime = ServiceLifetime.Scoped;
    o.Assemblies =
    [
        typeof(GenerateTokenCommand),
        typeof(GenerateTokenCommandHandler),
        typeof(FSH.Modules.Auditing.Contracts.AuditEnvelope),
        typeof(FSH.Modules.Auditing.Persistence.AuditDbContext),
        typeof(FSH.Modules.Webhooks.Contracts.v1.CreateWebhookSubscription.CreateWebhookSubscriptionCommand),
        typeof(FSH.Modules.Webhooks.WebhooksModule),
        typeof(FSH.Modules.Billing.Contracts.BillingContractsMarker),
        typeof(FSH.Modules.Billing.BillingModule),
        typeof(FSH.Modules.Catalog.Contracts.CatalogContractsMarker),
        typeof(FSH.Modules.Catalog.CatalogModule),
        typeof(FSH.Modules.Cases.Contracts.CasesContractsMarker),
        typeof(FSH.Modules.Cases.CasesModule),
        typeof(FSH.Modules.Evidence.Contracts.EvidenceContractsMarker),
        typeof(FSH.Modules.Evidence.EvidenceModule),
        typeof(FSH.Modules.Samples.Contracts.SamplesContractsMarker),
        typeof(FSH.Modules.Samples.SamplesModule),
        typeof(FSH.Modules.Genetics.Contracts.GeneticsContractsMarker),
        typeof(FSH.Modules.Genetics.GeneticsModule),
        typeof(FSH.Modules.Tickets.Contracts.TicketsContractsMarker),
        typeof(FSH.Modules.Tickets.TicketsModule),
        typeof(FSH.Modules.Files.Contracts.v1.Commands.RequestUploadUrlCommand),
        typeof(FSH.Modules.Files.FilesModule),
        typeof(FSH.Modules.Chat.Contracts.v1.Commands.CreateChannelCommand),
        typeof(FSH.Modules.Chat.ChatModule),
        typeof(FSH.Modules.Notifications.Contracts.v1.Commands.MarkNotificationReadCommand),
        typeof(FSH.Modules.Notifications.NotificationsModule),
    ];
});

var moduleAssemblies = new Assembly[]
{
    typeof(IdentityModule).Assembly,
    typeof(AuditingModule).Assembly,
    typeof(FSH.Modules.Files.FilesModule).Assembly,
    typeof(WebhooksModule).Assembly,
    typeof(BillingModule).Assembly,
    typeof(CatalogModule).Assembly,
    typeof(FSH.Modules.Cases.CasesModule).Assembly,
    typeof(FSH.Modules.Evidence.EvidenceModule).Assembly,
    typeof(FSH.Modules.Samples.SamplesModule).Assembly,
    typeof(FSH.Modules.Genetics.GeneticsModule).Assembly,
    typeof(FSH.Modules.ScientificAnalysis.ScientificAnalysisModule).Assembly,
    typeof(TicketsModule).Assembly,
    typeof(FSH.Modules.Chat.ChatModule).Assembly,
    typeof(FSH.Modules.Notifications.NotificationsModule).Assembly,
};

builder.AddHeroPlatform(o =>
{
    o.EnableOpenTelemetry = false;
    o.EnableCors = false;
    o.EnableOpenApi = false;
    o.EnableJobs = false;
    o.EnableMailing = false;
    o.EnableSse = false;
    o.EnableRealtime = false;
    o.EnableQuotas = false;
    o.EnableFeatureFlags = false;
    o.EnableIdempotency = false;
    o.EnableCaching = true;
});

builder.AddModules(moduleAssemblies);
builder.Services.AddSingleton<FSH.Framework.Jobs.Services.IJobService, NoOpJobService>();

foreach (var descriptor in builder.Services
    .Where(d => d.ServiceType == typeof(IHostedService)
        && typeof(BackgroundService).IsAssignableFrom(d.ImplementationType))
    .ToList())
{
    builder.Services.Remove(descriptor);
}

using var host = builder.Build();
var logger = host.Services.GetRequiredService<ILogger<MigratorCommand>>();
await host.StartAsync().ConfigureAwait(false);

try
{
    var connectionString = host.Services.GetRequiredService<IConfiguration>()["DatabaseOptions:ConnectionString"]
        ?? throw new InvalidOperationException("DatabaseOptions:ConnectionString is not configured.");

    await Console.Out.WriteLineAsync("[migrator] waiting for postgres...").ConfigureAwait(false);
    await PostgresMigratorLock.WaitForDatabaseAsync(connectionString, logger, CancellationToken.None)
        .ConfigureAwait(false);
    await Console.Out.WriteLineAsync("[migrator] postgres ready").ConfigureAwait(false);

    await using var migratorLock = await PostgresMigratorLock
        .AcquireAsync(connectionString, logger, CancellationToken.None)
        .ConfigureAwait(false);

    using var scope = host.Services.CreateScope();
    var initializers = scope.ServiceProvider.GetServices<IDbInitializer>().ToList();
    if (initializers.Count == 0)
    {
        throw new InvalidOperationException("No database initializers were registered.");
    }

    switch (cli.Command)
    {
        case "apply":
            await Console.Out.WriteLineAsync(
                $"[migrator] applying migrations for {initializers.Count} module database context(s)...")
                .ConfigureAwait(false);
            foreach (var initializer in initializers)
            {
                await initializer.MigrateAsync(CancellationToken.None).ConfigureAwait(false);
            }

            if (cli.SeedAfter)
            {
                foreach (var initializer in initializers)
                {
                    await initializer.SeedAsync(CancellationToken.None).ConfigureAwait(false);
                }
            }
            break;

        case "seed":
            foreach (var initializer in initializers)
            {
                await initializer.SeedAsync(CancellationToken.None).ConfigureAwait(false);
            }
            break;

        default:
            await Console.Error.WriteLineAsync(
                $"[migrator] Unsupported command '{cli.Command}'. Use apply or seed.")
                .ConfigureAwait(false);
            return 1;
    }

    await Console.Out.WriteLineAsync("[migrator] finished successfully.").ConfigureAwait(false);
    return 0;
}
#pragma warning disable CA1031 // Top-level operator CLI intentionally converts any migration failure to exit code 1.
catch (Exception ex)
#pragma warning restore CA1031
{
    logger.LogError(ex, "DbMigrator failed");
    await Console.Error.WriteLineAsync(
        $"[migrator] FAILED: {ex.GetType().Name}: {ex.Message}")
        .ConfigureAwait(false);
    return 1;
}
finally
{
    await host.StopAsync().ConfigureAwait(false);
}