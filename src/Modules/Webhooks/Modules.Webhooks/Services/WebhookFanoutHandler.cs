using FSH.Framework.Eventing.Abstractions;
using FSH.Framework.Shared.Installation;
using FSH.Modules.Webhooks.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FSH.Modules.Webhooks.Services;

/// <summary>
/// Fans every published integration event out to active webhook subscriptions for
/// the single local installation, then enqueues a delivery job per subscription.
/// </summary>
public sealed class WebhookFanoutHandler<TEvent> : IIntegrationEventHandler<TEvent>
    where TEvent : IIntegrationEvent
{
    private readonly WebhookDbContext _db;
    private readonly IWebhookDispatcher _dispatcher;
    private readonly IEventSerializer _serializer;
    private readonly ILogger<WebhookFanoutHandler<TEvent>> _logger;

    public WebhookFanoutHandler(
        WebhookDbContext db,
        IWebhookDispatcher dispatcher,
        IEventSerializer serializer,
        ILogger<WebhookFanoutHandler<TEvent>> logger)
    {
        _db = db;
        _dispatcher = dispatcher;
        _serializer = serializer;
        _logger = logger;
    }

    public async Task HandleAsync(TEvent @event, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(@event);

        const string installationId = InstallationConstants.Id;
        var eventType = typeof(TEvent).Name;

        var subscriptions = await _db.Subscriptions
            .AsNoTracking()
            .Where(s => s.IsActive)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        var matching = subscriptions.Where(s => s.MatchesEvent(eventType)).ToList();
        if (matching.Count == 0)
        {
            return;
        }

        var payload = _serializer.Serialize(@event);
        foreach (var subscription in matching)
        {
            try
            {
                await _dispatcher
                    .EnqueueAsync(installationId, subscription.Id, eventType, payload, ct)
                    .ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogWarning(
                    ex,
                    "Failed to enqueue webhook delivery for subscription {SubscriptionId} (installation {InstallationId}, event {EventType})",
                    subscription.Id,
                    installationId,
                    eventType);
            }
        }
    }
}
