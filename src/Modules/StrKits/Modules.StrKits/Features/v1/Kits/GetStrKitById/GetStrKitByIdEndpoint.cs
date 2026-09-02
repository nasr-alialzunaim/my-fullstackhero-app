using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.StrKits.Contracts.Authorization;
using FSH.Modules.StrKits.Contracts.v1.Kits;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.StrKits.Features.v1.Kits.GetStrKitById;

public static class GetStrKitByIdEndpoint
{
    internal static RouteHandlerBuilder MapGetStrKitByIdEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/{kitId:guid}",
                (Guid kitId, IMediator mediator, CancellationToken ct) =>
                    mediator.Send(new GetStrKitByIdQuery(kitId), ct))
            .WithName("GetStrKitById")
            .WithSummary("Get an STR kit version")
            .RequirePermission(StrKitsPermissions.View);
    }
}
