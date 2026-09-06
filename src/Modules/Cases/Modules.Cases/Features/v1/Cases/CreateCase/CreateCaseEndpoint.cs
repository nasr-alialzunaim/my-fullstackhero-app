using FSH.Framework.Shared.Identity.Authorization;
using FSH.Framework.Web.Idempotency;
using FSH.Modules.Cases.Contracts.Authorization;
using FSH.Modules.Cases.Contracts.v1.Cases;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Cases.Features.v1.Cases.CreateCase;

public static class CreateCaseEndpoint
{
    internal static RouteHandlerBuilder MapCreateCaseEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("",
                async (CreateCaseCommand command, IMediator mediator, CancellationToken ct) =>
                    Results.Ok(await mediator.Send(command, ct)))
            .WithName("CreateCase")
            .WithSummary("Create a forensic case")
            .RequirePermission(CasesPermissions.Create)
            .WithIdempotency();
    }
}