using Asp.Versioning;
using FSH.Framework.Persistence;
using FSH.Framework.Shared.Constants;
using FSH.Framework.Web.Modules;
using FSH.Modules.Matching.Contracts.Authorization;
using FSH.Modules.Matching.Data;
using FSH.Modules.Matching.Features.v1.Matching;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

[assembly: FshModule(typeof(FSH.Modules.Matching.MatchingModule), 1060)]

namespace FSH.Modules.Matching;

public sealed class MatchingModule : IModule
{
    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        PermissionConstants.Register(MatchingPermissions.All);
        builder.Services.AddHeroDbContext<MatchingDbContext>();
        builder.Services.AddScoped<IDbInitializer, MatchingDbInitializer>();
        builder.Services.AddHealthChecks().AddDbContextCheck<MatchingDbContext>(
            name: "db:matching",
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

        var group = endpoints.MapGroup("api/v{version:apiVersion}/matching")
            .WithTags("DNA Matching")
            .WithApiVersionSet(versionSet)
            .RequireAuthorization();

        group.MapMatchingEndpoints();
    }
}
