using FSH.Framework.Shared.Installation;
using FSH.Modules.Billing.Contracts.Dtos;
using FSH.Modules.Billing.Contracts.v1.Subscriptions;
using FSH.Modules.Billing.Data;
using FSH.Modules.Billing.Domain;
using Microsoft.EntityFrameworkCore;

using Mediator;

namespace FSH.Modules.Billing.Features.v1.Subscriptions.AssignSubscription;

public sealed class AssignSubscriptionCommandHandler(BillingDbContext db)
    : ICommandHandler<AssignSubscriptionCommand, SubscriptionDto>
{
    public async ValueTask<SubscriptionDto> Handle(
        AssignSubscriptionCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        const string installationId = InstallationConstants.Id;

        var plan = await db.Plans
            .FirstOrDefaultAsync(p => p.Id == command.PlanId && p.IsActive, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new InvalidOperationException($"Active plan {command.PlanId} was not found.");

        var current = await db.Subscriptions
            .FirstOrDefaultAsync(
                s => s.TenantId == installationId && s.Status == SubscriptionStatus.Active,
                cancellationToken)
            .ConfigureAwait(false);

        current?.Cancel();

        var now = DateTime.UtcNow;
        var subscription = Subscription.Create(installationId, plan.Id, now);
        db.Subscriptions.Add(subscription);
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return new SubscriptionDto(
            subscription.Id,
            subscription.TenantId,
            subscription.PlanId,
            plan.Key,
            subscription.StartUtc,
            subscription.EndUtc,
            subscription.Status);
    }
}
