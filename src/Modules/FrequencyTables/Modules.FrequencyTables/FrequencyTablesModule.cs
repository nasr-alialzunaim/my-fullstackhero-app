using Asp.Versioning;
using FSH.Framework.Persistence;
using FSH.Framework.Shared.Constants;
using FSH.Framework.Web.Modules;
using FSH.Modules.FrequencyTables.Contracts.Authorization;
using FSH.Modules.FrequencyTables.Data;
using FSH.Modules.FrequencyTables.Features.v1.Tables.CreateFrequencyTable;
using FSH.Modules.FrequencyTables.Features.v1.Tables.GetFrequencyTableById;
using FSH.Modules.FrequencyTables.Features.v1.Tables.GetGenisFrequencyTable;
using FSH.Modules.FrequencyTables.Features.v1.Tables.SearchFrequencyTables;
using FSH.Modules.FrequencyTables.Features.v1.Tables.SetDefaultFrequencyTable;
using FSH.Modules.FrequencyTables.Features.v1.Tables.ToggleFrequencyTableActive;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

[assembly: FshModule(typeof(FSH.Modules.FrequencyTables.FrequencyTablesModule), 1050)]

namespace FSH.Modules.FrequencyTables;

public sealed class FrequencyTablesModule : IModule
{
    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        PermissionConstants.Register(FrequencyTablesPermissions.All);
        builder.Services.AddHeroDbContext<FrequencyTablesDbContext>();
        builder.Services.AddScoped<IDbInitializer, FrequencyTablesDbInitializer>();
        builder.Services.AddHealthChecks().AddDbContextCheck<FrequencyTablesDbContext>(
            name: "db:frequency-tables",
            failureStatus: HealthStatus.Unhealthy);
    }

    public void ConfigureMiddleware(IApplicationBuilder app) { }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);
        var versionSet = endpoints.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        var group = endpoints.MapGroup("api/v{version:apiVersion}/frequency-tables")
            .WithTags("Frequency Tables")
            .WithApiVersionSet(versionSet)
            .RequireAuthorization();

        group.MapCreateFrequencyTableEndpoint();
        group.MapSearchFrequencyTablesEndpoint();
        group.MapGetGenisFrequencyTableEndpoint();
        group.MapSetDefaultFrequencyTableEndpoint();
        group.MapToggleFrequencyTableActiveEndpoint();
        group.MapGetFrequencyTableByIdEndpoint();
    }
}
