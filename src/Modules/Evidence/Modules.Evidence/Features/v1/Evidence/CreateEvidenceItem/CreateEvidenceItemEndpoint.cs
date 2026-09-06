using FSH.Framework.Shared.Identity.Authorization;
using FSH.Framework.Web.Idempotency;
using FSH.Modules.Evidence.Contracts.Authorization;
using FSH.Modules.Evidence.Contracts.v1.Evidence;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Evidence.Features.v1.Evidence.CreateEvidenceItem;

public static class CreateEvidenceItemEndpoint
{
    internal static RouteHandlerBuilder MapCreateEvidenceItemEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("",
                async (CreateEvidenceItemCommand command, IMediator mediator, CancellationToken ct) =>
                    Results.Ok(await mediator.Send(command, ct)))
            .WithName("CreateEvidenceItem")
            .WithSummary("Register an evidence item under a forensic case")
            .RequirePermission(EvidencePermissions.Create)
            .WithIdempotency();
    }
}
