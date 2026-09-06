using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Samples.Contracts.Authorization;
using FSH.Modules.Samples.Contracts.v1.Samples;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Samples.Features.v1.Samples.SearchBiologicalSamples;

public static class SearchBiologicalSamplesEndpoint
{
    internal static RouteHandlerBuilder MapSearchBiologicalSamplesEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        return endpoints.MapGet("",
                (
                    string? search,
                    string? sampleContext,
                    Guid? caseId,
                    Guid? subjectId,
                    string? status,
                    int pageNumber,
                    int pageSize,
                    IMediator mediator,
                    CancellationToken ct) =>
                    mediator.Send(
                        new SearchBiologicalSamplesQuery(
                            search,
                            sampleContext,
                            caseId,
                            subjectId,
                            status,
                            pageNumber == 0 ? 1 : pageNumber,
                            pageSize == 0 ? 20 : pageSize),
                        ct))
            .WithName("SearchBiologicalSamples")
            .WithSummary("Search biological samples")
            .RequirePermission(SamplesPermissions.View);
    }
}
