using FSH.Framework.Quota;
using FSH.Framework.Shared.Installation;
using FSH.Framework.Shared.Quota;
using FSH.Modules.Billing.Data;
using FSH.Modules.Billing.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FSH.Modules.Billing.Services;

public sealed class UsageReporter : IUsageReporter
{
    private readonly BillingDbContext _db;
    private readonly IQuotaService _quotas;
    private readonly QuotaPlanResolver _planResolver;
    private readonly IInstallationContext _installationContext;
    private readonly ILogger<UsageReporter> _logger;

    public UsageReporter(
        BillingDbContext db,
        IQuotaService quotas,
        QuotaPlanResolver planResolver,
        IInstallationContext installationContext,
        ILogger<UsageReporter> logger)
    {
        _db = db;
        _quotas = quotas;
        _planResolver = planResolver;
        _installationContext = installationContext;
        _logger = logger;
    }

    public async Task<IReadOnlyList<UsageSnapshot>> CaptureForPeriodAsync(
        string tenantId,
        int periodYear,
        int periodMonth,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tenantId);

        tenantId = InstallationConstants.Id;
        var installation = _installationContext.Current;
        var existing = await _db.UsageSnapshots
            .Where(s => s.TenantId == tenantId && s.PeriodYear == periodYear && s.PeriodMonth == periodMonth)
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        var snapshots = new List<UsageSnapshot>(capacity: 4);
        foreach (var resource in Enum.GetValues<QuotaResource>())
        {
            var already = existing.FirstOrDefault(s => s.Resource == resource);
            if (already is not null)
            {
                snapshots.Add(already);
                continue;
            }

            var used = await _quotas.GetCurrentAsync(tenantId, resource, cancellationToken).ConfigureAwait(false);
            var limit = _planResolver.ResolveLimit(installation, resource);
            var snap = UsageSnapshot.Capture(tenantId, periodYear, periodMonth, resource, used, limit);
            _db.UsageSnapshots.Add(snap);
            snapshots.Add(snap);
        }

        await _db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("[Billing] captured {Count} usage snapshots for installation {InstallationId} period {Year}-{Month:00}",
                snapshots.Count, tenantId, periodYear, periodMonth);
        }
        return snapshots;
    }
}
