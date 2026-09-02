using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Samples.Contracts.Authorization;
using FSH.Modules.Samples.Contracts.v1.Samples;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Samples.Features.v1.Samples.GetBiologicalSampleById;

public static class GetBiologicalSampleByIdEndpoint
{
    internal static RouteHandlerBuilder MapGetBiologicalSampleByIdEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/{sampleId:guid}",
                (Guid sampleId, IMediator mediator, CancellationToken ct) =>
                    mediator.Send(new GetBiologicalSampleByIdQuery(sampleId), ct))
            .WithName("GetBiologicalSampleById")
            .WithSummary("Get a biological sample by id")
            .RequirePermission(SamplesPermissions.View);
    }
}
