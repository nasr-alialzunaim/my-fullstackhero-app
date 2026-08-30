using FSH.Framework.Web.Modules;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;

[assembly: FshModule(typeof(FSH.Modules.Cases.CasesModule), 1000)]

namespace FSH.Modules.Cases;

/// <summary>
/// Forensic case boundary. P1 registers the boundary without persistence or endpoints.
/// </summary>
public sealed class CasesModule : IModule
{
    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);
    }
}
