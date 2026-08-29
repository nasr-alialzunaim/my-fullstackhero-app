using FSH.Framework.Core.Context;
using FSH.Framework.Core.Exceptions;
using FSH.Framework.Shared.Constants;
using FSH.Framework.Shared.Installation;
using FSH.Modules.Auditing.Contracts;
using FSH.Modules.Identity.Contracts.Services;
using FSH.Modules.Identity.Contracts.v1.Impersonation;
using FSH.Modules.Identity.Contracts.v1.Impersonation.StartImpersonation;
using Mediator;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FSH.Modules.Identity.Features.v1.Impersonation.StartImpersonation;

public sealed class StartImpersonationCommandHandler
    : ICommandHandler<StartImpersonationCommand, ImpersonationResponse>
{
    private readonly IIdentityService _identityService;
    private readonly ITokenService _tokenService;
    private readonly ISecurityAudit _securityAudit;
    private readonly ICurrentUser _currentUser;
    private readonly IRequestContext _requestContext;
    private readonly IImpersonationGrantService _grantService;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<StartImpersonationCommandHandler> _logger;

    public StartImpersonationCommandHandler(
        IIdentityService identityService,
        ITokenService tokenService,
        ISecurityAudit securityAudit,
        ICurrentUser currentUser,
        IRequestContext requestContext,
        IImpersonationGrantService grantService,
        TimeProvider timeProvider,
        ILogger<StartImpersonationCommandHandler> logger)
    {
        _identityService = identityService;
        _tokenService = tokenService;
        _securityAudit = securityAudit;
        _currentUser = currentUser;
        _requestContext = requestContext;
        _grantService = grantService;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async ValueTask<ImpersonationResponse> Handle(
        StartImpersonationCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!_currentUser.IsAuthenticated())
        {
            throw new UnauthorizedException();
        }

        var actorUserId = _currentUser.GetUserId().ToString();
        const string installationId = InstallationConstants.Id;
        var actorUserName = _currentUser.Name;

        if (string.Equals(actorUserId, request.TargetUserId, StringComparison.Ordinal))
        {
            throw new CustomException(
                "cannot impersonate yourself",
                errors: null,
                System.Net.HttpStatusCode.BadRequest);
        }

        var callerClaims = _currentUser.GetUserClaims();
        if (callerClaims is not null
            && callerClaims.Any(c => c.Type == ClaimConstants.ActorSubject))
        {
            throw new CustomException(
                "end current impersonation before starting a new one",
                errors: null,
                System.Net.HttpStatusCode.BadRequest);
        }

        var targetClaimsResult = await _identityService
            .BuildClaimsForUserAsync(request.TargetUserId, installationId, cancellationToken);

        if (targetClaimsResult is null)
        {
            throw new NotFoundException("target user not found");
        }

        var (subject, claims) = targetClaimsResult.Value;
        var targetUserName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value
            ?? claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name)?.Value;

        var jti = Guid.NewGuid().ToString("N");
        var impersonationClaims = claims
            .Where(c => c.Type != JwtRegisteredClaimNames.Jti)
            .Concat(
            [
                new Claim(JwtRegisteredClaimNames.Jti, jti),
                new Claim(ClaimConstants.ActorSubject, actorUserId),
                new Claim(ClaimConstants.ActorTenant, installationId)
            ])
            .ToList();

        var lifetime = request.DurationMinutes is { } minutes
            ? TimeSpan.FromMinutes(
                Math.Clamp(minutes, 1, StartImpersonationCommandValidator.MaxImpersonationMinutes))
            : (TimeSpan?)null;

        var startedAtUtc = _timeProvider.GetUtcNow().UtcDateTime;
        var (accessToken, expiresAt) = await _tokenService.IssueAccessOnlyAsync(
            subject,
            impersonationClaims,
            lifetime,
            cancellationToken);

        await _grantService.CreateAsync(new CreateGrantInput(
            Jti: jti,
            ActorUserId: actorUserId,
            ActorUserName: actorUserName,
            ActorTenantId: installationId,
            ImpersonatedUserId: subject,
            ImpersonatedUserName: targetUserName,
            ImpersonatedTenantId: installationId,
            Reason: request.Reason ?? string.Empty,
            StartedAtUtc: startedAtUtc,
            ExpiresAtUtc: expiresAt,
            ClientId: _requestContext.ClientId,
            IpAddress: _requestContext.IpAddress,
            UserAgent: _requestContext.UserAgent), cancellationToken);

        await _securityAudit.ImpersonationStartedAsync(
            actorUserId: actorUserId,
            actorTenantId: installationId,
            targetUserId: subject,
            targetTenantId: installationId,
            clientId: _requestContext.ClientId ?? "unknown",
            ip: _requestContext.IpAddress ?? "unknown",
            userAgent: _requestContext.UserAgent ?? "unknown",
            reason: request.Reason ?? string.Empty,
            ct: cancellationToken);

        _logger.LogWarning(
            "Impersonation started: actor {ActorUserId} -> target {TargetUserId} jti={Jti}",
            actorUserId,
            subject,
            jti);

        return new ImpersonationResponse(
            AccessToken: accessToken,
            AccessTokenExpiresAt: expiresAt,
            ActorUserId: actorUserId,
            ActorTenantId: installationId,
            ImpersonatedUserId: subject,
            ImpersonatedTenantId: installationId);
    }
}
