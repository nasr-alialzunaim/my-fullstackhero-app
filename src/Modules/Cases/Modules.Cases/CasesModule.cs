using Asp.Versioning;
using FSH.Framework.Persistence;
using FSH.Framework.Shared.Constants;
using FSH.Framework.Web.Modules;
using FSH.Modules.Cases.Contracts.Authorization;
using FSH.Modules.Cases.Data;
using FSH.Modules.Cases.Features.v1.Cases.CreateCase;
using FSH.Modules.Cases.Features.v1.Cases.GetCaseById;
using FSH.Modules.Cases.Features.v1.Cases.SearchCases;
using FSH.Modules.Cases.Features.v1.Cases.UpdateCase;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

[assembly: FshModule(typeof(FSH.Modules.Cases.CasesModule), 1000)]

namespace FSH.Modules.Cases;

public sealed class CasesModule : IModule
{
    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        PermissionConstants.Register(CasesPermissions.All);
        builder.Services.AddHeroDbContext<CasesDbContext>();
        builder.Services.AddScoped<IDbInitializer, CasesDbInitializer>();

        builder.Services.AddHealthChecks()
            .AddDbContextCheck<CasesDbContext>(
                name: "db:cases",
                failureStatus: HealthStatus.Unhealthy);
    }

    public void ConfigureMiddleware(IApplicationBuilder app)
    {
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var versionSet = endpoints.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        var group = endpoints
            .MapGroup("api/v{version:apiVersion}/cases")
            .WithTags("Cases")
            .WithApiVersionSet(versionSet)
            .RequireAuthorization();

        group.MapCreateCaseEndpoint();
        group.MapUpdateCaseEndpoint();
        group.MapGetCaseByIdEndpoint();
        group.MapSearchCasesEndpoint();
    }
}