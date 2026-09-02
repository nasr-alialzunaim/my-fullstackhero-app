using Asp.Versioning;
using FSH.Framework.Persistence;
using FSH.Framework.Shared.Constants;
using FSH.Framework.Web.Modules;
using FSH.Modules.ScientificAnalysis.Contracts;
using FSH.Modules.ScientificAnalysis.Contracts.Authorization;
using FSH.Modules.ScientificAnalysis.Data;
using FSH.Modules.ScientificAnalysis.Engine;
using FSH.Modules.ScientificAnalysis.Features.v1.Genis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

[assembly: FshModule(typeof(FSH.Modules.ScientificAnalysis.ScientificAnalysisModule), 1100)]

namespace FSH.Modules.ScientificAnalysis;

public sealed class ScientificAnalysisModule : IModule
{
    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        PermissionConstants.Register(ScientificAnalysisPermissions.All);

        builder.Services.Configure<GenisScientificEngineOptions>(
            builder.Configuration.GetSection(GenisScientificEngineOptions.SectionName));

        builder.Services.AddHeroDbContext<ScientificAnalysisDbContext>();
        builder.Services.AddScoped<IDbInitializer, ScientificAnalysisDbInitializer>();

        builder.Services.AddHttpClient<GenisScientificEngineClient>((services, client) =>
        {
            var options = services.GetRequiredService<IOptions<GenisScientificEngineOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl, UriKind.Absolute);
            client.Timeout = TimeSpan.FromSeconds(Math.Max(5, options.TimeoutSeconds));
        });

        builder.Services.AddScoped<GenisAnalysisProxy>();
        builder.Services.AddScoped<IScientificEngineGateway>(
            services => services.GetRequiredService<GenisAnalysisProxy>());

        builder.Services.AddHealthChecks().AddDbContextCheck<ScientificAnalysisDbContext>(
            name: "db:scientific-analysis",
            failureStatus: HealthStatus.Unhealthy);

        if (builder.Configuration.GetValue<bool>(
            $"{GenisScientificEngineOptions.SectionName}:Enabled"))
        {
            builder.Services.AddHealthChecks().AddCheck<GenisScientificEngineHealthCheck>(
                "engine:genis",
                failureStatus: HealthStatus.Unhealthy);
        }
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

        var group = endpoints.MapGroup("api/v{version:apiVersion}/scientific/genis")
            .WithTags("Scientific Analysis · GENis")
            .WithApiVersionSet(versionSet)
            .RequireAuthorization();

        group.MapGenisMetadataEndpoints();
        group.MapGenisCalculationEndpoints();
    }
}