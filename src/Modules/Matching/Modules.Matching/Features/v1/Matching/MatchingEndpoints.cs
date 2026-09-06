using FSH.Framework.Shared.Identity.Authorization;
using FSH.Framework.Web.Idempotency;
using FSH.Modules.Matching.Contracts.Authorization;
using FSH.Modules.Matching.Contracts.v1.Matching;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Matching.Features.v1.Matching;

public static class MatchingEndpoints
{
    internal static void MapMatchingEndpoints(this IEndpointRouteBuilder group)
    {
        group.MapGet("/categories",
                (IMediator mediator, CancellationToken ct) =>
                    mediator.Send(new ListProfileCategoriesQuery(), ct))
            .WithName("ListProfileCategories")
            .RequirePermission(MatchingPermissions.View);

        group.MapPost("/categories",
                async (CreateProfileCategoryCommand command, IMediator mediator, CancellationToken ct) =>
                    Results.Ok(await mediator.Send(command, ct)))
            .WithName("CreateProfileCategory")
            .RequirePermission(MatchingPermissions.Configure)
            .WithIdempotency();

        group.MapGet("/rules",
                (Guid? sourceCategoryId, IMediator mediator, CancellationToken ct) =>
                    mediator.Send(new ListMatchingRulesQuery(sourceCategoryId), ct))
            .WithName("ListMatchingRules")
            .RequirePermission(MatchingPermissions.View);

        group.MapPost("/rules",
                async (CreateMatchingRuleCommand command, IMediator mediator, CancellationToken ct) =>
                    Results.Ok(await mediator.Send(command, ct)))
            .WithName("CreateMatchingRule")
            .RequirePermission(MatchingPermissions.Configure)
            .WithIdempotency();

        group.MapGet("/profiles/{profileId:guid}/configuration",
                (Guid profileId, IMediator mediator, CancellationToken ct) =>
                    mediator.Send(new GetProfileMatchingConfigurationQuery(profileId), ct))
            .WithName("GetProfileMatchingConfiguration")
            .RequirePermission(MatchingPermissions.View);

        group.MapPut("/profiles/{profileId:guid}/configuration",
                async (
                    Guid profileId,
                    ConfigureProfileMatchingCommand body,
                    IMediator mediator,
                    CancellationToken ct) =>
                {
                    ConfigureProfileMatchingCommand command = body with { GeneticProfileId = profileId };
                    return Results.Ok(await mediator.Send(command, ct));
                })
            .WithName("ConfigureProfileMatching")
            .RequirePermission(MatchingPermissions.Configure);

        group.MapPost("/autosomal/search",
                async (RunAutosomalDatabaseSearchCommand command, IMediator mediator, CancellationToken ct) =>
                    Results.Ok(await mediator.Send(command, ct)))
            .WithName("RunAutosomalDatabaseSearch")
            .RequirePermission(MatchingPermissions.Run)
            .WithIdempotency();

        group.MapGet("/autosomal/searches/{searchId:guid}",
                (Guid searchId, IMediator mediator, CancellationToken ct) =>
                    mediator.Send(new GetAutosomalMatchSearchQuery(searchId), ct))
            .WithName("GetAutosomalMatchSearch")
            .RequirePermission(MatchingPermissions.View);

        group.MapPut("/hits/{hitId:guid}/review",
                async (
                    Guid hitId,
                    ReviewMatchHitCommand body,
                    IMediator mediator,
                    CancellationToken ct) =>
                {
                    ReviewMatchHitCommand command = body with { HitId = hitId };
                    return Results.Ok(await mediator.Send(command, ct));
                })
            .WithName("ReviewMatchHit")
            .RequirePermission(MatchingPermissions.Review);
    }
}
