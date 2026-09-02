using Asp.Versioning;
using FSH.Framework.Persistence;
using FSH.Framework.Shared.Constants;
using FSH.Framework.Web.Modules;
using FSH.Modules.Evidence.Contracts.Authorization;
using FSH.Modules.Evidence.Data;
using FSH.Modules.Evidence.Features.v1.Evidence.CreateEvidenceItem;
using FSH.Modules.Evidence.Features.v1.Evidence.GetEvidenceItemById;
using FSH.Modules.Evidence.Features.v1.Evidence.SearchEvidenceItems;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

[assembly: FshModule(typeof(FSH.Modules.Evidence.EvidenceModule), 1010)]

namespace FSH.Modules.Evidence;

public sealed class EvidenceModule : IModule
{
    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        PermissionConstants.Register(EvidencePermissions.All);
        builder.Services.AddHeroDbContext<EvidenceDbContext>();
        builder.Services.AddScoped<IDbInitializer, EvidenceDbInitializer>();
        builder.Services.AddHealthChecks().AddDbContextCheck<EvidenceDbContext>(
            name: "db:evidence",
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

        var group = endpoints.MapGroup("api/v{version:apiVersion}/evidence/items")
            .WithTags("Evidence")
            .WithApiVersionSet(versionSet)
            .RequireAuthorization();

        group.MapCreateEvidenceItemEndpoint();
        group.MapGetEvidenceItemByIdEndpoint();
        group.MapSearchEvidenceItemsEndpoint();
    }
}
