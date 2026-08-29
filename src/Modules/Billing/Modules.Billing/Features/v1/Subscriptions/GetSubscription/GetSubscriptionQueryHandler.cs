using FSH.Framework.Shared.Installation;
using FSH.Modules.Billing.Contracts.Dtos;
using FSH.Modules.Billing.Contracts.v1.Subscriptions;
using FSH.Modules.Billing.Data;
using Microsoft.EntityFrameworkCore;

using Mediator;

namespace FSH.Modules.Billing.Features.v1.Subscriptions.GetSubscription;

public sealed class GetSubscriptionQueryHandler(BillingDbContext db)
    : IQueryHandler<GetSubscriptionQuery, SubscriptionDto?>
{
    public async ValueTask<SubscriptionDto?> Handle(
        GetSubscriptionQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        return await (
            from s in db.Subscriptions.AsNoTracking()
            join p in db.Plans.AsNoTracking() on s.PlanId equals p.Id
            where s.TenantId == InstallationConstants.Id
                && s.Status == SubscriptionStatus.Active
            orderby s.StartUtc descending
            select new SubscriptionDto(
                s.Id, s.TenantId, s.PlanId, p.Key, s.StartUtc, s.EndUtc, s.Status))
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
