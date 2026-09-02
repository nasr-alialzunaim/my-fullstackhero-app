using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.FrequencyTables.Contracts.Authorization;
using FSH.Modules.FrequencyTables.Contracts.v1.Tables;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.FrequencyTables.Features.v1.Tables.GetFrequencyTableById;

public static class GetFrequencyTableByIdEndpoint
{
    internal static RouteHandlerBuilder MapGetFrequencyTableByIdEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/{tableId:guid}",
                (Guid tableId, IMediator mediator, CancellationToken ct) =>
                    mediator.Send(new GetFrequencyTableByIdQuery(tableId), ct))
            .WithName("GetFrequencyTableById")
            .WithSummary("Get a frequency table version")
            .RequirePermission(FrequencyTablesPermissions.View);
    }
}
