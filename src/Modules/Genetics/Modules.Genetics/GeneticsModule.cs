using Asp.Versioning;
using FSH.Framework.Persistence;
using FSH.Framework.Shared.Constants;
using FSH.Framework.Web.Modules;
using FSH.Modules.Genetics.Contracts.Authorization;
using FSH.Modules.Genetics.Data;
using FSH.Modules.Genetics.Features.v1.Profiles.CreateGeneticProfile;
using FSH.Modules.Genetics.Features.v1.Profiles.GetGeneticProfileById;
using FSH.Modules.Genetics.Features.v1.Profiles.SearchGeneticProfiles;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

[assembly: FshModule(typeof(FSH.Modules.Genetics.GeneticsModule), 1030)]

namespace FSH.Modules.Genetics;

public sealed class GeneticsModule : IModule
{
    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        PermissionConstants.Register(GeneticsPermissions.All);
        builder.Services.AddHeroDbContext<GeneticsDbContext>();
        builder.Services.AddScoped<IDbInitializer, GeneticsDbInitializer>();
        builder.Services.AddHealthChecks().AddDbContextCheck<GeneticsDbContext>(
            name: "db:genetics",
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

        var group = endpoints.MapGroup("api/v{version:apiVersion}/genetics/profiles")
            .WithTags("Genetic Profiles")
            .WithApiVersionSet(versionSet)
            .RequireAuthorization();

        group.MapCreateGeneticProfileEndpoint();
        group.MapGetGeneticProfileByIdEndpoint();
        group.MapSearchGeneticProfilesEndpoint();
    }
}
