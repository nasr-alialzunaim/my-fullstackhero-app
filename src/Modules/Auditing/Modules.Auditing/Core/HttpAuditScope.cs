using FSH.Framework.Core.Context;
using FSH.Framework.Shared.Installation;
using FSH.Modules.Auditing.Contracts;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Security.Claims;

namespace FSH.Modules.Auditing;

/// <summary>
/// Ambient-aware audit scope for the single installation. HTTP data is used when
/// available; background work falls back to the scoped current-user abstraction.
/// </summary>
public sealed class HttpAuditScope : IAuditScope
{
    private readonly IHttpContextAccessor _http;
    private readonly ICurrentUser? _currentUser;

    public HttpAuditScope(
        IHttpContextAccessor httpContextAccessor,
        ICurrentUser? currentUser = null)
    {
        _http = httpContextAccessor;
        _currentUser = currentUser;
    }

    public string? TenantId => InstallationConstants.Id;

    public string? UserId =>
        _http.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? _http.HttpContext?.User?.FindFirstValue("sub")
        ?? NullIfEmpty(_currentUser?.GetUserId().ToString());

    public string? UserName =>
        _http.HttpContext?.User?.Identity?.Name
        ?? _http.HttpContext?.User?.FindFirstValue("name")
        ?? _currentUser?.Name;

    public string? TraceId => Activity.Current?.TraceId.ToString();
    public string? SpanId => Activity.Current?.SpanId.ToString();

    public string? CorrelationId =>
        _http.HttpContext?.TraceIdentifier
        ?? Activity.Current?.RootId;

    public string? RequestId =>
        _http.HttpContext?.TraceIdentifier
        ?? Activity.Current?.Id;

    public string? Source =>
        _http.HttpContext?.GetEndpoint()?.DisplayName
        ?? Activity.Current?.OperationName
        ?? "background";

    public AuditTag Tags => AuditTag.None;

    public IAuditScope WithTags(AuditTag tags) => this;

    public IAuditScope WithProperties(
        string? tenantId = null,
        string? userId = null,
        string? userName = null,
        string? traceId = null,
        string? spanId = null,
        string? correlationId = null,
        string? requestId = null,
        string? source = null,
        AuditTag? tags = null) => this;

    private static string? NullIfEmpty(string? value) =>
        string.IsNullOrEmpty(value) || value == Guid.Empty.ToString() ? null : value;
}
