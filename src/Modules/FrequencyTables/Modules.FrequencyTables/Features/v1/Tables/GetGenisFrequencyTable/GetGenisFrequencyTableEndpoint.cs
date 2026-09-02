using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.FrequencyTables.Contracts.Authorization;
using FSH.Modules.FrequencyTables.Contracts.v1.Tables;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.FrequencyTables.Features.v1.Tables.GetGenisFrequencyTable;

public static class GetGenisFrequencyTableEndpoint
{
    internal static RouteHandlerBuilder MapGetGenisFrequencyTableEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/{tableId:guid}/genis",
                (Guid tableId, IMediator mediator, CancellationToken ct) =>
                    mediator.Send(new GetGenisFrequencyTableQuery(tableId), ct))
            .WithName("GetGenisFrequencyTable")
            .WithSummary("Export a stored table using the exact GENis FrequencyTable JSON shape")
            .RequirePermission(FrequencyTablesPermissions.View);
    }
}
