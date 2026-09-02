using FSH.Framework.Shared.Identity.Authorization;
using FSH.Framework.Web.Idempotency;
using FSH.Modules.StrKits.Contracts.Authorization;
using FSH.Modules.StrKits.Contracts.v1.Kits;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.StrKits.Features.v1.Kits.CreateStrKit;

public static class CreateStrKitEndpoint
{
    internal static RouteHandlerBuilder MapCreateStrKitEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("",
                async (CreateStrKitCommand command, IMediator mediator, CancellationToken ct) =>
                    Results.Ok(await mediator.Send(command, ct)))
            .WithName("CreateStrKit")
            .WithSummary("Create an immutable STR kit version")
            .RequirePermission(StrKitsPermissions.Create)
            .WithIdempotency();
    }
}
