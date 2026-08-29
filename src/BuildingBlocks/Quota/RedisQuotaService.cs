using FSH.Framework.Shared.Installation;
using FSH.Framework.Shared.Quota;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace FSH.Framework.Quota;

/// <summary>
/// Redis-backed quota counter. Counter-based resources use atomic <c>INCRBY</c> on a key of the
/// form <c>quota:{installationId}:{resource}:{YYYYMM}</c> with a TTL that expires shortly after the
/// billing period boundary. Gauge-based resources delegate to <see cref="IQuotaGaugeProvider"/>
/// instances that modules register to report live usage from their own state stores.
/// </summary>
public sealed class RedisQuotaService : IQuotaService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly QuotaOptions _options;
    private readonly QuotaPlanResolver _planResolver;
    private readonly IInstallationContext _installationContext;
    private readonly Dictionary<QuotaResource, IQuotaGaugeProvider> _gauges;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<RedisQuotaService> _logger;

    public RedisQuotaService(
        IConnectionMultiplexer redis,
        QuotaOptions options,
        QuotaPlanResolver planResolver,
        IEnumerable<IQuotaGaugeProvider> gauges,
        TimeProvider timeProvider,
        ILogger<RedisQuotaService> logger,
        IInstallationContext installationContext)
    {
        ArgumentNullException.ThrowIfNull(redis);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(planResolver);
        ArgumentNullException.ThrowIfNull(gauges);
        ArgumentNullException.ThrowIfNull(timeProvider);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(installationContext);

        _redis = redis;
        _options = options;
        _planResolver = planResolver;
        _installationContext = installationContext;
        _timeProvider = timeProvider;
        _logger = logger;

        // Fail fast on duplicate gauge registrations — two providers for the same resource is a bug.
        _gauges = gauges.ToDictionary(g => g.Resource);
    }

    public async ValueTask<QuotaCheckResult> CheckAsync(string installationId, QuotaResource resource, long amount, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(installationId);

        var (limit, exempt) = ResolveLimit(installationId, resource);
        var current = await GetCurrentAsync(installationId, resource, ct).ConfigureAwait(false);

        if (exempt || limit == long.MaxValue)
        {
            return QuotaCheckResult.Unlimited(resource, current);
        }

        var allowed = current + amount <= limit;
        return new QuotaCheckResult(allowed, resource, current, limit, GetPeriodResetUtc(resource));
    }

    public async ValueTask<long> RecordAsync(string installationId, QuotaResource resource, long amount, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(installationId);

        if (!IsCounterResource(resource))
        {
            // Gauges are read from module state; we have no counter to increment here.
            return await GetCurrentAsync(installationId, resource, ct).ConfigureAwait(false);
        }

        var db = _redis.GetDatabase();
        var key = BuildCounterKey(installationId, resource);
        var newValue = await db.StringIncrementAsync(key, amount).ConfigureAwait(false);

        // Set a TTL aligned to the period boundary the first time we touch this key. KeyExpireAsync
        // is a no-op if the key already has a TTL, so this is safe to call on every increment.
        var reset = GetPeriodResetUtc(resource);
        if (reset is not null)
        {
            await db.KeyExpireAsync(key, reset.Value.UtcDateTime, ExpireWhen.HasNoExpiry).ConfigureAwait(false);
        }

        return newValue;
    }

    public async ValueTask<QuotaCheckResult> CheckAndRecordAsync(string installationId, QuotaResource resource, long amount, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(installationId);

        var (limit, exempt) = ResolveLimit(installationId, resource);

        if (exempt || limit == long.MaxValue)
        {
            var after = await RecordAsync(installationId, resource, amount, ct).ConfigureAwait(false);
            return QuotaCheckResult.Unlimited(resource, after);
        }

        if (!IsCounterResource(resource))
        {
            // Gauges are not counters — we can't "record" them, so delegate to CheckAsync.
            return await CheckAsync(installationId, resource, amount, ct).ConfigureAwait(false);
        }

        var db = _redis.GetDatabase();
        var key = BuildCounterKey(installationId, resource);
        var newValue = await db.StringIncrementAsync(key, amount).ConfigureAwait(false);
        var reset = GetPeriodResetUtc(resource);
        if (reset is not null)
        {
            await db.KeyExpireAsync(key, reset.Value.UtcDateTime, ExpireWhen.HasNoExpiry).ConfigureAwait(false);
        }

        if (newValue <= limit)
        {
            return new QuotaCheckResult(true, resource, newValue, limit, reset);
        }

        // Overshoot: roll the increment back so repeated checks don't keep inflating the counter.
        await db.StringIncrementAsync(key, -amount).ConfigureAwait(false);

        if (_logger.IsEnabled(LogLevel.Warning))
        {
            _logger.LogWarning(
                "Quota exceeded for installation {InstallationId} resource {Resource}: {Current}/{Limit}",
                installationId, resource, newValue, limit);
        }

        return new QuotaCheckResult(false, resource, newValue - amount, limit, reset);
    }

    public async ValueTask<long> GetCurrentAsync(string installationId, QuotaResource resource, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(installationId);

        if (!IsCounterResource(resource))
        {
            if (_gauges.TryGetValue(resource, out var provider))
            {
                return await provider.GetCurrentAsync(installationId, ct).ConfigureAwait(false);
            }

            return 0;
        }

        var db = _redis.GetDatabase();
        var value = await db.StringGetAsync(BuildCounterKey(installationId, resource)).ConfigureAwait(false);
        return value.TryParse(out long parsed) ? parsed : 0;
    }

    private (long Limit, bool Exempt) ResolveLimit(string installationId, QuotaResource resource)
    {
        if (_options.ExemptInstallation
            && string.Equals(installationId, InstallationConstants.Id, StringComparison.Ordinal))
        {
            return (long.MaxValue, true);
        }

        return (_planResolver.ResolveLimit(_installationContext.Current, resource), false);
    }

    private static bool IsCounterResource(QuotaResource resource) => resource switch
    {
        QuotaResource.ApiCalls => true,
        QuotaResource.StorageBytes => true,
        _ => false
    };

    // Periodic counters reset at the billing period boundary (monthly).
    // Perpetual counters (e.g. StorageBytes) accumulate until explicitly decremented.
    private static bool IsPeriodic(QuotaResource resource) => resource switch
    {
        QuotaResource.ApiCalls => true,
        _ => false
    };

    private string BuildCounterKey(string installationId, QuotaResource resource)
    {
        if (!IsPeriodic(resource))
        {
            return $"quota:{installationId}:{resource}";
        }

        var now = _timeProvider.GetUtcNow();
        // Monthly billing period is the coarsest useful window for SaaS; hourly/daily windows can be
        // added as additional QuotaResource values if needed later.
        var period = $"{now.Year:D4}{now.Month:D2}";
        return $"quota:{installationId}:{resource}:{period}";
    }

    private DateTimeOffset? GetPeriodResetUtc(QuotaResource resource)
    {
        if (!IsPeriodic(resource))
        {
            return null;
        }

        var now = _timeProvider.GetUtcNow();
        // Reset at the first moment of the next UTC month.
        var nextMonth = now.Month == 12
            ? new DateTimeOffset(now.Year + 1, 1, 1, 0, 0, 0, TimeSpan.Zero)
            : new DateTimeOffset(now.Year, now.Month + 1, 1, 0, 0, 0, TimeSpan.Zero);
        return nextMonth;
    }
}
