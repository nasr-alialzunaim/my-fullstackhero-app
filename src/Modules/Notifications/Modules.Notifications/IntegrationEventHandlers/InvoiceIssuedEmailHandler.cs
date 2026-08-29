using FSH.Framework.Eventing.Abstractions;
using FSH.Framework.Mailing.Services;
using FSH.Framework.Shared.Multitenancy;
using FSH.Modules.Billing.Contracts.Events;
using Microsoft.Extensions.Logging;

namespace FSH.Modules.Notifications.IntegrationEventHandlers;

/// <summary>Emails the installation admin when an invoice is issued.</summary>
public sealed class InvoiceIssuedEmailHandler(
    Finbuckle.MultiTenant.Abstractions.IMultiTenantContextAccessor<AppTenantInfo> tenantAccessor,
    IMailService mailService,
    ILogger<InvoiceIssuedEmailHandler> logger)
    : IIntegrationEventHandler<InvoiceIssuedIntegrationEvent>
{
    public async Task HandleAsync(InvoiceIssuedIntegrationEvent @event, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(@event);
        if (string.IsNullOrWhiteSpace(@event.TenantId))
        {
            return;
        }

        var tenant = tenantAccessor.MultiTenantContext.TenantInfo;
        if (tenant is null)
        {
            return;
        }

        var (subject, body) = BillingEmailBodies.InvoiceIssued(
            @event.InvoiceNumber, @event.Amount, @event.Currency, @event.DueAtUtc);
        await BillingEmailSender.SendAsync(mailService, logger, tenant.AdminEmail, subject, body, "invoice-issued", ct)
            .ConfigureAwait(false);
    }
}
