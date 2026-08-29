using FSH.Framework.Quota;
using FSH.Framework.Shared.Installation;
using FSH.Framework.Shared.Quota;
using FSH.Modules.Identity.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Identity.Services;

/// <summary>
/// Reports the live user count for the single installation.
/// </summary>
internal sealed class UserCountQuotaGaugeProvider : IQuotaGaugeProvider
{
    private readonly UserManager<FshUser> _userManager;

    public UserCountQuotaGaugeProvider(UserManager<FshUser> userManager)
    {
        ArgumentNullException.ThrowIfNull(userManager);
        _userManager = userManager;
    }

    public QuotaResource Resource => QuotaResource.Users;

    public async ValueTask<long> GetCurrentAsync(string installationId, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(installationId);

        if (!string.Equals(installationId, InstallationConstants.Id, StringComparison.Ordinal))
        {
            return 0;
        }

        return await _userManager.Users
            .CountAsync(ct)
            .ConfigureAwait(false);
    }
}
