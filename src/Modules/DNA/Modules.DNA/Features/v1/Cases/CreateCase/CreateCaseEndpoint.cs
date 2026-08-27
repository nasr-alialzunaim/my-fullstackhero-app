using FSH.Framework.Web.Idempotency;
using FSH.Modules.DNA.Contracts.Authorization;
using FSH.Modules.DNA.Contracts.v1.Cases;
using FSH.Framework.Shared.Identity.Authorization;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.DNA.Features.v1.Cases.CreateCase;

public static class CreateCaseEndpoint
{
    internal static RouteHandlerBuilder MapCreateCaseEndpoint(
        this RouteGroupBuilder endpoints)
    {
        return endpoints.MapPost("/cases",
                async (CreateCaseCommand command, IMediator mediator, CancellationToken ct) =>
                    Results.Ok(await mediator.Send(command, ct)))
            .WithName("CreateDnaCase")
            .WithSummary("Create a DNA case")
            .RequirePermission(DnaPermissions.Cases.Create)
            .WithIdempotency();
    }
}
