using FSH.Framework.Shared.Identity.Authorization;
using FSH.Framework.Shared.Persistence;
using FSH.Modules.Identity.Contracts.Authorization;
using FSH.Modules.Identity.Contracts.DTOs;
using FSH.Modules.Identity.Contracts.v1.Sessions.GetAllSessions;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Identity.Features.v1.Sessions.GetAllSessions;

public static class GetAllSessionsEndpoint
{
    internal static RouteHandlerBuilder MapGetAllSessionsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/sessions",
                async (
                    bool? includeInactive,
                    string? search,
                    int? pageNumber,
                    int? pageSize,
                    IMediator mediator,
                    CancellationToken ct) =>
                {
                    var query = new GetAllSessionsQuery
                    {
                        IncludeInactive = includeInactive ?? false,
                        Search = search,
                        PageNumber = pageNumber ?? 1,
                        PageSize = pageSize ?? 50,
                    };
                    return TypedResults.Ok(await mediator.Send(query, ct));
                })
            .WithName("GetAllSessions")
            .WithSummary("List all sessions in this installation (Admin)")
            .WithDescription("Returns paged sessions across the installation, filterable by active state and free-text search across user name, email, and IP address.")
            .RequirePermission(IdentityPermissions.Sessions.ViewAll)
            .Produces<PagedResponse<UserSessionDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);
    }
}
