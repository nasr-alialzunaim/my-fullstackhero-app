using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.FrequencyTables.Contracts.Authorization;
using FSH.Modules.FrequencyTables.Contracts.v1.Tables;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.FrequencyTables.Features.v1.Tables.SearchFrequencyTables;

public static class SearchFrequencyTablesEndpoint
{
    internal static RouteHandlerBuilder MapSearchFrequencyTablesEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("",
                (string? search, int pageNumber, int pageSize, IMediator mediator, CancellationToken ct) =>
                    mediator.Send(new SearchFrequencyTablesQuery(
                        search,
                        pageNumber == 0 ? 1 : pageNumber,
                        pageSize == 0 ? 20 : pageSize), ct))
            .WithName("SearchFrequencyTables")
            .WithSummary("Search frequency table versions")
            .RequirePermission(FrequencyTablesPermissions.View);
    }
}
