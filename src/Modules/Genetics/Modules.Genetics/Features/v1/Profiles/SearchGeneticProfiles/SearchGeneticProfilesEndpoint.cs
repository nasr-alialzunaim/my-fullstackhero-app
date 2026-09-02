using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Genetics.Contracts.Authorization;
using FSH.Modules.Genetics.Contracts.v1.Profiles;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Genetics.Features.v1.Profiles.SearchGeneticProfiles;

public static class SearchGeneticProfilesEndpoint
{
    internal static RouteHandlerBuilder MapSearchGeneticProfilesEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("",
                (
                    Guid? sampleId,
                    string? search,
                    int pageNumber,
                    int pageSize,
                    IMediator mediator,
                    CancellationToken ct) =>
                    mediator.Send(
                        new SearchGeneticProfilesQuery(
                            sampleId,
                            search,
                            pageNumber == 0 ? 1 : pageNumber,
                            pageSize == 0 ? 20 : pageSize),
                        ct))
            .WithName("SearchGeneticProfiles")
            .WithSummary("Search genetic profile versions")
            .RequirePermission(GeneticsPermissions.View);
    }
}
