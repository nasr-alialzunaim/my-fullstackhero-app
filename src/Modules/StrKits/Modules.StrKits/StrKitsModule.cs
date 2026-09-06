using Asp.Versioning;
using FSH.Framework.Persistence;
using FSH.Framework.Shared.Constants;
using FSH.Framework.Web.Modules;
using FSH.Modules.StrKits.Contracts.Authorization;
using FSH.Modules.StrKits.Data;
using FSH.Modules.StrKits.Features.v1.Kits.CreateStrKit;
using FSH.Modules.StrKits.Features.v1.Kits.GetStrKitById;
using FSH.Modules.StrKits.Features.v1.Kits.SearchStrKits;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

[assembly: FshModule(typeof(FSH.Modules.StrKits.StrKitsModule), 1040)]

namespace FSH.Modules.StrKits;

public sealed class StrKitsModule : IModule
{
    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        PermissionConstants.Register(StrKitsPermissions.All);
        builder.Services.AddHeroDbContext<StrKitsDbContext>();
        builder.Services.AddScoped<IDbInitializer, StrKitsDbInitializer>();
        builder.Services.AddHealthChecks().AddDbContextCheck<StrKitsDbContext>(
            name: "db:str-kits",
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

        var group = endpoints.MapGroup("api/v{version:apiVersion}/str-kits")
            .WithTags("STR Kits")
            .WithApiVersionSet(versionSet)
            .RequireAuthorization();

        group.MapCreateStrKitEndpoint();
        group.MapGetStrKitByIdEndpoint();
        group.MapSearchStrKitsEndpoint();
    }
}
