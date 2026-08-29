using FSH.Framework.Shared.Persistence;
using FSH.Modules.Identity.Contracts.DTOs;
using FSH.Modules.Identity.Contracts.Services;
using FSH.Modules.Identity.Contracts.v1.Sessions.GetAllSessions;
using Mediator;

namespace FSH.Modules.Identity.Features.v1.Sessions.GetAllSessions;

public sealed class GetAllSessionsQueryHandler(ISessionService sessionService)
    : IQueryHandler<GetAllSessionsQuery, PagedResponse<UserSessionDto>>
{
    public async ValueTask<PagedResponse<UserSessionDto>> Handle(
        GetAllSessionsQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        int page = query.PageNumber < 1 ? 1 : query.PageNumber;
        int size = query.PageSize is < 1 or > 200 ? 50 : query.PageSize;

        var (items, total) = await sessionService.GetAllSessionsAsync(
            includeInactive: query.IncludeInactive,
            search: query.Search,
            skip: (page - 1) * size,
            take: size,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return new PagedResponse<UserSessionDto>
        {
            Items = items,
            PageNumber = page,
            PageSize = size,
            TotalCount = total,
            TotalPages = (int)Math.Ceiling(total / (double)size),
        };
    }
}
