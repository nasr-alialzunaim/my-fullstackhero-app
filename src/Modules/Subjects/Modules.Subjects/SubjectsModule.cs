using Asp.Versioning;
using FSH.Framework.Persistence;
using FSH.Framework.Shared.Constants;
using FSH.Framework.Web.Modules;
using FSH.Modules.Subjects.Contracts.Authorization;
using FSH.Modules.Subjects.Data;
using FSH.Modules.Subjects.Features.v1.Subjects;
using FSH.Modules.Subjects.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

[assembly: FshModule(typeof(FSH.Modules.Subjects.SubjectsModule), 1015)]

namespace FSH.Modules.Subjects;

public sealed class SubjectsModule : IModule
{
    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        PermissionConstants.Register(SubjectsPermissions.All);
        builder.Services.AddHeroDbContext<SubjectsDbContext>();
        builder.Services.AddScoped<IDbInitializer, SubjectsDbInitializer>();
        builder.Services.AddScoped<ISubjectSensitiveDataProtector, SubjectSensitiveDataProtector>();
        builder.Services.AddHealthChecks().AddDbContextCheck<SubjectsDbContext>(
            name: "db:subjects",
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

        var group = endpoints.MapGroup("api/v{version:apiVersion}/subjects")
            .WithTags("DNA Subjects")
            .WithApiVersionSet(versionSet)
            .RequireAuthorization();

        group.MapSubjectEndpoints();
    }
}
