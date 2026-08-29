using FSH.Framework.Shared.Installation;
using FSH.Modules.Billing.Contracts;
using FSH.Modules.Billing.Contracts.v1.Usage.GetUsageSnapshots;
using FSH.Modules.Billing.Data;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Billing.Features.v1.Usage.GetUsageSnapshots;

public sealed class GetUsageSnapshotsQueryHandler(BillingDbContext db)
    : IQueryHandler<GetUsageSnapshotsQuery, IReadOnlyList<UsageSnapshotDto>>
{
    public async ValueTask<IReadOnlyList<UsageSnapshotDto>> Handle(
        GetUsageSnapshotsQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var q = db.UsageSnapshots
            .AsNoTracking()
            .Where(s => s.TenantId == InstallationConstants.Id);

        if (query.PeriodYear is not null)
        {
            q = q.Where(s => s.PeriodYear == query.PeriodYear.Value);
        }

        if (query.PeriodMonth is not null)
        {
            q = q.Where(s => s.PeriodMonth == query.PeriodMonth.Value);
        }

        return await q
            .OrderByDescending(s => s.PeriodYear)
            .ThenByDescending(s => s.PeriodMonth)
            .ThenBy(s => s.Resource)
            .Select(s => new UsageSnapshotDto(
                s.Id, s.TenantId, s.PeriodYear, s.PeriodMonth,
                s.Resource, s.UsedUnits, s.LimitUnits, s.Overage, s.CapturedAtUtc))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
