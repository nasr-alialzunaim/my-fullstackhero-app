using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Genetics.Contracts.Authorization;
using FSH.Modules.Genetics.Contracts.v1.Profiles;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Genetics.Features.v1.Profiles.GetGeneticProfileById;

public static class GetGeneticProfileByIdEndpoint
{
    internal static RouteHandlerBuilder MapGetGeneticProfileByIdEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/{profileId:guid}",
                (Guid profileId, IMediator mediator, CancellationToken ct) =>
                    mediator.Send(new GetGeneticProfileByIdQuery(profileId), ct))
            .WithName("GetGeneticProfileById")
            .WithSummary("Get an immutable genetic profile version")
            .RequirePermission(GeneticsPermissions.View);
    }
}
