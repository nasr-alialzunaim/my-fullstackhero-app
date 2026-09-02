using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Evidence.Contracts.Authorization;
using FSH.Modules.Evidence.Contracts.v1.Evidence;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Evidence.Features.v1.Evidence.GetEvidenceItemById;

public static class GetEvidenceItemByIdEndpoint
{
    internal static RouteHandlerBuilder MapGetEvidenceItemByIdEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/{evidenceItemId:guid}",
                (Guid evidenceItemId, IMediator mediator, CancellationToken ct) =>
                    mediator.Send(new GetEvidenceItemByIdQuery(evidenceItemId), ct))
            .WithName("GetEvidenceItemById")
            .WithSummary("Get an evidence item by id")
            .RequirePermission(EvidencePermissions.View);
    }
}
