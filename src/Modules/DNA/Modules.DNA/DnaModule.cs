using Asp.Versioning;
using FSH.Framework.Persistence;
using FSH.Framework.Shared.Constants;
using FSH.Framework.Shared.Identity.Authorization;
using FSH.Framework.Web.Modules;
using FSH.Modules.DNA.Contracts.Authorization;
using FSH.Modules.DNA.Data;
using FSH.Modules.DNA.Features.v1.Cases.CreateCase;
using FSH.Modules.DNA.Features.v1.Cases.ListCases;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

[assembly: FshModule(typeof(FSH.Modules.DNA.DnaModule), 650)]

namespace FSH.Modules.DNA;

public sealed class DnaModule : IModule
{
    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        PermissionConstants.Register(DnaPermissions.All);
        builder.Services.AddHeroDbContext<DnaDbContext>();
        builder.Services.AddScoped<IDbInitializer, DnaDbInitializer>();
        builder.Services.AddHealthChecks()
            .AddDbContextCheck<DnaDbContext>(
                name: "db:dna",
                failureStatus: HealthStatus.Unhealthy);
    }

    public void ConfigureMiddleware(IApplicationBuilder app)
    {
        // DNA has no custom middleware in this phase.
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var versionSet = endpoints.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        var dna = endpoints.MapGroup("api/v{version:apiVersion}/dna")
            .WithTags("DNA")
            .WithApiVersionSet(versionSet)
            .RequireAuthorization();

        dna.MapGet("", () => Results.Ok(new
            {
                module = "DNA",
                status = "ready",
                message = "DNA module is registered and ready for Cases."
            }))
            .WithName("GetDnaModuleStatus")
            .WithSummary("Get DNA module status")
            .RequirePermission(DnaPermissions.ModuleAccess.View);

        dna.MapCreateCaseEndpoint();
        dna.MapListCasesEndpoint();
    }
}
