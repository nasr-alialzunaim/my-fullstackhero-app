using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Cases.Contracts.Authorization;
using FSH.Modules.Cases.Contracts.v1.Cases;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Cases.Features.v1.Cases.UpdateCase;

public static class UpdateCaseEndpoint
{
    internal static RouteHandlerBuilder MapUpdateCaseEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPut("/{caseId:guid}",
                async (Guid caseId, UpdateCaseCommand body, IMediator mediator, CancellationToken ct) =>
                {
                    ArgumentNullException.ThrowIfNull(body);
                    UpdateCaseCommand command = body with { CaseId = caseId };
                    return Results.Ok(await mediator.Send(command, ct));
                })
            .WithName("UpdateCase")
            .WithSummary("Update a forensic case")
            .RequirePermission(CasesPermissions.Update);
    }
}