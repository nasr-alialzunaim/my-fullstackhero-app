using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Cases.Contracts.Authorization;
using FSH.Modules.Cases.Contracts.v1.Cases;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Cases.Features.v1.Cases.SearchCases;

public static class SearchCasesEndpoint
{
    internal static RouteHandlerBuilder MapSearchCasesEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("",
                (
                    string? search,
                    int pageNumber,
                    int pageSize,
                    string? sortBy,
                    string? sortDir,
                    IMediator mediator,
                    CancellationToken ct) =>
                    mediator.Send(
                        new SearchCasesQuery(
                            search,
                            pageNumber == 0 ? 1 : pageNumber,
                            pageSize == 0 ? 20 : pageSize,
                            sortBy,
                            sortDir),
                        ct))
            .WithName("SearchCases")
            .WithSummary("Search forensic cases")
            .RequirePermission(CasesPermissions.View);
    }
}