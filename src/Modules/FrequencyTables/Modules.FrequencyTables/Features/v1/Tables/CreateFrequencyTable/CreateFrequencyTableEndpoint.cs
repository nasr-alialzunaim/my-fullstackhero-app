using FSH.Framework.Shared.Identity.Authorization;
using FSH.Framework.Web.Idempotency;
using FSH.Modules.FrequencyTables.Contracts.Authorization;
using FSH.Modules.FrequencyTables.Contracts.v1.Tables;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.FrequencyTables.Features.v1.Tables.CreateFrequencyTable;

public static class CreateFrequencyTableEndpoint
{
    internal static RouteHandlerBuilder MapCreateFrequencyTableEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("",
                async (CreateFrequencyTableCommand command, IMediator mediator, CancellationToken ct) =>
                    Results.Ok(await mediator.Send(command, ct)))
            .WithName("CreateFrequencyTable")
            .WithSummary("Create an immutable population-frequency table version")
            .RequirePermission(FrequencyTablesPermissions.Create)
            .WithIdempotency();
    }
}
