using FSH.Framework.Eventing.Abstractions;
using FSH.Framework.Shared.Installation;
using FSH.Modules.Webhooks.Data;
using FSH.Modules.Webhooks.Domain;
using FSH.Modules.Webhooks.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Webhooks.Tests.Services;

public sealed class WebhookFanoutHandlerTests
{
    private const string EventType = nameof(FakeIntegrationEvent);

    private readonly IWebhookDispatcher _dispatcher = Substitute.For<IWebhookDispatcher>();
    private readonly IEventSerializer _serializer = Substitute.For<IEventSerializer>();
    private readonly ILogger<WebhookFanoutHandler<FakeIntegrationEvent>> _logger =
        Substitute.For<ILogger<WebhookFanoutHandler<FakeIntegrationEvent>>>();

    public WebhookFanoutHandlerTests()
    {
        _serializer.Serialize(Arg.Any<IIntegrationEvent>()).Returns("{\"serialized\":true}");
    }

    [Fact]
    public async Task HandleAsync_Should_Enqueue_Delivery_When_Subscription_Matches_Exact_Event()
    {
        await using var db = CreateContext();
        Guid subId = await SeedSubscriptionAsync(db, [EventType], isActive: true);

        var handler = CreateHandler(db);
        await handler.HandleAsync(new FakeIntegrationEvent("legacy-tenant-value"));

        await _dispatcher.Received(1).EnqueueAsync(
            InstallationConstants.Id,
            subId,
            EventType,
            "{\"serialized\":true}",
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_Should_Enqueue_Delivery_When_Subscription_Uses_Wildcard()
    {
        await using var db = CreateContext();
        Guid subId = await SeedSubscriptionAsync(db, ["*"], isActive: true);

        var handler = CreateHandler(db);
        await handler.HandleAsync(new FakeIntegrationEvent(null));

        await _dispatcher.Received(1).EnqueueAsync(
            InstallationConstants.Id,
            subId,
            EventType,
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_Should_Enqueue_For_Each_Matching_Subscription()
    {
        await using var db = CreateContext();
        await SeedSubscriptionAsync(db, [EventType], isActive: true);
        await SeedSubscriptionAsync(db, ["*"], isActive: true);

        var handler = CreateHandler(db);
        await handler.HandleAsync(new FakeIntegrationEvent("   "));

        await _dispatcher.Received(2).EnqueueAsync(
            InstallationConstants.Id,
            Arg.Any<Guid>(),
            EventType,
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_Should_Not_Enqueue_When_No_Subscription_Matches_Event()
    {
        await using var db = CreateContext();
        await SeedSubscriptionAsync(db, ["some.other.event"], isActive: true);

        var handler = CreateHandler(db);
        await handler.HandleAsync(new FakeIntegrationEvent(null));

        await _dispatcher.DidNotReceiveWithAnyArgs()
            .EnqueueAsync(default!, default, default!, default!, default);
        _serializer.DidNotReceiveWithAnyArgs().Serialize(default!);
    }

    [Fact]
    public async Task HandleAsync_Should_Not_Enqueue_When_Matching_Subscription_Is_Inactive()
    {
        await using var db = CreateContext();
        await SeedSubscriptionAsync(db, [EventType], isActive: false);

        var handler = CreateHandler(db);
        await handler.HandleAsync(new FakeIntegrationEvent(null));

        await _dispatcher.DidNotReceiveWithAnyArgs()
            .EnqueueAsync(default!, default, default!, default!, default);
    }

    [Fact]
    public async Task HandleAsync_Should_Continue_Fanout_When_One_Enqueue_Throws()
    {
        await using var db = CreateContext();
        Guid first = await SeedSubscriptionAsync(db, [EventType], isActive: true);
        Guid second = await SeedSubscriptionAsync(db, [EventType], isActive: true);

        _dispatcher
            .EnqueueAsync(
                InstallationConstants.Id,
                first,
                EventType,
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(_ => throw new InvalidOperationException("transient enqueue failure"));

        var handler = CreateHandler(db);

        await Should.NotThrowAsync(
            async () => await handler.HandleAsync(new FakeIntegrationEvent(null)));

        await _dispatcher.Received(1).EnqueueAsync(
            InstallationConstants.Id,
            second,
            EventType,
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_Should_Throw_When_Event_Is_Null()
    {
        await using var db = CreateContext();
        var handler = CreateHandler(db);

        await Should.ThrowAsync<ArgumentNullException>(
            async () => await handler.HandleAsync(null!));
    }

    private WebhookFanoutHandler<FakeIntegrationEvent> CreateHandler(WebhookDbContext db) =>
        new(db, _dispatcher, _serializer, _logger);

    private static WebhookDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<WebhookDbContext>()
            .UseInMemoryDatabase($"webhooks-{Guid.NewGuid():N}")
            .Options;

        return new WebhookDbContext(options);
    }

    private static async Task<Guid> SeedSubscriptionAsync(
        WebhookDbContext db,
        string[] events,
        bool isActive)
    {
        WebhookSubscription sub = WebhookSubscription.Create(
            "https://example.com/hook",
            events,
            "hash");

        if (!isActive)
        {
            sub.Deactivate();
        }

        db.Subscriptions.Add(sub);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        return sub.Id;
    }
}

public sealed record FakeIntegrationEvent(string? TenantId) : IIntegrationEvent
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
    public string CorrelationId { get; } = Guid.CreateVersion7().ToString();
    public string Source { get; } = "Tests";
}
