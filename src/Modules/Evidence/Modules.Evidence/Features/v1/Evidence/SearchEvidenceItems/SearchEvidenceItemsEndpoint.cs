using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Evidence.Contracts.Authorization;
using FSH.Modules.Evidence.Contracts.v1.Evidence;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Evidence.Features.v1.Evidence.SearchEvidenceItems;

public static class SearchEvidenceItemsEndpoint
{
    internal static RouteHandlerBuilder MapSearchEvidenceItemsEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("",
                (
                    Guid? caseId,
                    string? search,
                    int pageNumber,
                    int pageSize,
                    IMediator mediator,
                    CancellationToken ct) =>
                    mediator.Send(
                        new SearchEvidenceItemsQuery(
                            caseId,
                            search,
                            pageNumber == 0 ? 1 : pageNumber,
                            pageSize == 0 ? 20 : pageSize),
                        ct))
            .WithName("SearchEvidenceItems")
            .WithSummary("Search evidence items")
            .RequirePermission(EvidencePermissions.View);
    }
}
