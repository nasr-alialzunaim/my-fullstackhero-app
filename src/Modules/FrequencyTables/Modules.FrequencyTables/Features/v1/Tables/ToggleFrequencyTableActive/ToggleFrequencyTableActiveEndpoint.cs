using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.FrequencyTables.Contracts.Authorization;
using FSH.Modules.FrequencyTables.Contracts.v1.Tables;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.FrequencyTables.Features.v1.Tables.ToggleFrequencyTableActive;

public static class ToggleFrequencyTableActiveEndpoint
{
    internal static RouteHandlerBuilder MapToggleFrequencyTableActiveEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/{tableId:guid}/toggle-active",
                async (Guid tableId, IMediator mediator, CancellationToken ct) =>
                    Results.Ok(await mediator.Send(new ToggleFrequencyTableActiveCommand(tableId), ct)))
            .WithName("ToggleFrequencyTableActive")
            .WithSummary("Enable or disable a frequency table version")
            .RequirePermission(FrequencyTablesPermissions.Manage);
    }
}
