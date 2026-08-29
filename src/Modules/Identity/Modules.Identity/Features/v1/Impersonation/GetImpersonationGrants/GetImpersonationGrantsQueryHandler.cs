using FSH.Framework.Shared.Installation;
using FSH.Modules.Identity.Contracts.Services;
using FSH.Modules.Identity.Contracts.v1.Impersonation;
using FSH.Modules.Identity.Contracts.v1.Impersonation.GetImpersonationGrants;
using Mediator;

namespace FSH.Modules.Identity.Features.v1.Impersonation.GetImpersonationGrants;

public sealed class GetImpersonationGrantsQueryHandler(IImpersonationGrantService grantService)
    : IQueryHandler<GetImpersonationGrantsQuery, IReadOnlyList<ImpersonationGrantDto>>
{
    public async ValueTask<IReadOnlyList<ImpersonationGrantDto>> Handle(
        GetImpersonationGrantsQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        return await grantService.ListAsync(
            status: request.Status,
            impersonatedTenantId: InstallationConstants.Id,
            actorUserId: request.ActorUserId,
            take: request.Take,
            ct: cancellationToken).ConfigureAwait(false);
    }
}
