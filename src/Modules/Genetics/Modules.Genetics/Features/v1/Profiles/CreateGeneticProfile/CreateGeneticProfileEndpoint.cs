using FSH.Framework.Shared.Identity.Authorization;
using FSH.Framework.Web.Idempotency;
using FSH.Modules.Genetics.Contracts.Authorization;
using FSH.Modules.Genetics.Contracts.v1.Profiles;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Genetics.Features.v1.Profiles.CreateGeneticProfile;

public static class CreateGeneticProfileEndpoint
{
    internal static RouteHandlerBuilder MapCreateGeneticProfileEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("",
                async (CreateGeneticProfileCommand command, IMediator mediator, CancellationToken ct) =>
                    Results.Ok(await mediator.Send(command, ct)))
            .WithName("CreateGeneticProfile")
            .WithSummary("Create an immutable genetic profile version")
            .RequirePermission(GeneticsPermissions.Create)
            .WithIdempotency();
    }
}
