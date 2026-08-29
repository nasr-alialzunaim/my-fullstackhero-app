using FSH.Framework.Shared.Persistence;
using FSH.Modules.Identity.Contracts.DTOs;
using Mediator;

namespace FSH.Modules.Identity.Contracts.v1.Sessions.GetAllSessions;

/// <summary>
/// Returns all sessions in this installation, paged and optionally filtered.
/// Used by the admin system-sessions surface.
/// </summary>
public sealed record GetAllSessionsQuery : IQuery<PagedResponse<UserSessionDto>>
{
    public bool IncludeInactive { get; init; }
    public string? Search { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}
