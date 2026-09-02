using Asp.Versioning;
using FSH.Framework.Persistence;
using FSH.Framework.Shared.Constants;
using FSH.Framework.Web.Modules;
using FSH.Modules.Samples.Contracts.Authorization;
using FSH.Modules.Samples.Data;
using FSH.Modules.Samples.Features.v1.Samples.CreateBiologicalSample;
using FSH.Modules.Samples.Features.v1.Samples.GetBiologicalSampleById;
using FSH.Modules.Samples.Features.v1.Samples.SearchBiologicalSamples;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

[assembly: FshModule(typeof(FSH.Modules.Samples.SamplesModule), 1020)]

namespace FSH.Modules.Samples;

public sealed class SamplesModule : IModule
{
    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        PermissionConstants.Register(SamplesPermissions.All);
        builder.Services.AddHeroDbContext<SamplesDbContext>();
        builder.Services.AddScoped<IDbInitializer, SamplesDbInitializer>();
        builder.Services.AddHealthChecks().AddDbContextCheck<SamplesDbContext>(
            name: "db:samples",
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

        var group = endpoints.MapGroup("api/v{version:apiVersion}/samples")
            .WithTags("Biological Samples")
            .WithApiVersionSet(versionSet)
            .RequireAuthorization();

        group.MapCreateBiologicalSampleEndpoint();
        group.MapGetBiologicalSampleByIdEndpoint();
        group.MapSearchBiologicalSamplesEndpoint();
    }
}
