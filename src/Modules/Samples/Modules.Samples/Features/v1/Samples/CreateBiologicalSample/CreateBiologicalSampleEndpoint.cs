using FSH.Framework.Shared.Identity.Authorization;
using FSH.Framework.Web.Idempotency;
using FSH.Modules.Samples.Contracts.Authorization;
using FSH.Modules.Samples.Contracts.v1.Samples;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Samples.Features.v1.Samples.CreateBiologicalSample;

public static class CreateBiologicalSampleEndpoint
{
    internal static RouteHandlerBuilder MapCreateBiologicalSampleEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("",
                async (CreateBiologicalSampleCommand command, IMediator mediator, CancellationToken ct) =>
                    Results.Ok(await mediator.Send(command, ct)))
            .WithName("CreateBiologicalSample")
            .WithSummary("Register a biological sample")
            .RequirePermission(SamplesPermissions.Create)
            .WithIdempotency();
    }
}
