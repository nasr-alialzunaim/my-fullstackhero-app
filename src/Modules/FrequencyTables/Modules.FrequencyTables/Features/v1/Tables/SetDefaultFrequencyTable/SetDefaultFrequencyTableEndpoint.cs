using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.FrequencyTables.Contracts.Authorization;
using FSH.Modules.FrequencyTables.Contracts.v1.Tables;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.FrequencyTables.Features.v1.Tables.SetDefaultFrequencyTable;

public static class SetDefaultFrequencyTableEndpoint
{
    internal static RouteHandlerBuilder MapSetDefaultFrequencyTableEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/{tableId:guid}/default",
                async (Guid tableId, IMediator mediator, CancellationToken ct) =>
                    Results.Ok(await mediator.Send(new SetDefaultFrequencyTableCommand(tableId), ct)))
            .WithName("SetDefaultFrequencyTable")
            .WithSummary("Set the installation default frequency table")
            .RequirePermission(FrequencyTablesPermissions.Manage);
    }
}
