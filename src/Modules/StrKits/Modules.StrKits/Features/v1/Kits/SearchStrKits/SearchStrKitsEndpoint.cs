using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.StrKits.Contracts.Authorization;
using FSH.Modules.StrKits.Contracts.v1.Kits;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.StrKits.Features.v1.Kits.SearchStrKits;

public static class SearchStrKitsEndpoint
{
    internal static RouteHandlerBuilder MapSearchStrKitsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("",
                (string? search, int pageNumber, int pageSize, IMediator mediator, CancellationToken ct) =>
                    mediator.Send(new SearchStrKitsQuery(
                        search,
                        pageNumber == 0 ? 1 : pageNumber,
                        pageSize == 0 ? 20 : pageSize), ct))
            .WithName("SearchStrKits")
            .WithSummary("Search STR kit versions")
            .RequirePermission(StrKitsPermissions.View);
    }
}
