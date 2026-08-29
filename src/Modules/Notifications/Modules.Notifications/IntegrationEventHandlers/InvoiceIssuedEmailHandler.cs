using FSH.Framework.Eventing.Abstractions;
using FSH.Framework.Mailing.Services;
using FSH.Framework.Shared.Installation;
using FSH.Modules.Billing.Contracts.Events;
using Microsoft.Extensions.Logging;

namespace FSH.Modules.Notifications.IntegrationEventHandlers;

/// <summary>Emails the installation administrator when an invoice is issued.</summary>
public sealed class InvoiceIssuedEmailHandler(
    IMailService mailService,
    ILogger<InvoiceIssuedEmailHandler> logger)
    : IIntegrationEventHandler<InvoiceIssuedIntegrationEvent>
{
    public async Task HandleAsync(InvoiceIssuedIntegrationEvent @event, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(@event);

        var (subject, body) = BillingEmailBodies.InvoiceIssued(
            @event.InvoiceNumber,
            @event.Amount,
            @event.Currency,
            @event.DueAtUtc);

        await BillingEmailSender.SendAsync(
                mailService,
                logger,
                InstallationConstants.AdminEmail,
                subject,
                body,
                "invoice-issued",
                ct)
            .ConfigureAwait(false);
    }
}
