using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Cases.Contracts.Authorization;
using FSH.Modules.Cases.Contracts.v1.Cases;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Cases.Features.v1.Cases.GetCaseById;

public static class GetCaseByIdEndpoint
{
    internal static RouteHandlerBuilder MapGetCaseByIdEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/{caseId:guid}",
                (Guid caseId, IMediator mediator, CancellationToken ct) =>
                    mediator.Send(new GetCaseByIdQuery(caseId), ct))
            .WithName("GetCaseById")
            .WithSummary("Get a forensic case by id")
            .RequirePermission(CasesPermissions.View);
    }
}