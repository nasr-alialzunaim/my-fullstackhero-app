using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.DNA.Contracts.Authorization;
using FSH.Modules.DNA.Contracts.v1.Cases;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.DNA.Features.v1.Cases.ListCases;

public static class ListCasesEndpoint
{
    internal static RouteHandlerBuilder MapListCasesEndpoint(this RouteGroupBuilder endpoints)
    {
        return endpoints.MapGet("/cases", async (IMediator mediator, CancellationToken ct) =>
                Results.Ok(await mediator.Send(new ListCasesQuery(), ct)))
            .WithName("ListDnaCases")
            .WithSummary("List DNA cases")
            .RequirePermission(DnaPermissions.ModuleAccess.View);
    }
}
