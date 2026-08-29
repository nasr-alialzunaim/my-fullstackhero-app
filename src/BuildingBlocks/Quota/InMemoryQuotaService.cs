using System.Collections.Concurrent;
using FSH.Framework.Shared.Installation;
using FSH.Framework.Shared.Quota;

namespace FSH.Framework.Quota;

/// <summary>
/// Per-process quota counter. Suitable for development and tests; not shared across instances.
/// </summary>
public sealed class InMemoryQuotaService : IQuotaService
{
    private readonly ConcurrentDictionary<string, long> _counters;
    private readonly QuotaOptions _options;
    private readonly QuotaPlanResolver _planResolver;
    private readonly IInstallationContext _installationContext;
    private readonly Dictionary<QuotaResource, IQuotaGaugeProvider> _gauges;
    private readonly TimeProvider _timeProvider;

    internal InMemoryQuotaService(
        InMemoryQuotaStore store,
        QuotaOptions options,
        QuotaPlanResolver planResolver,
        IEnumerable<IQuotaGaugeProvider> gauges,
        TimeProvider timeProvider,
        IInstallationContext installationContext)
    {
        ArgumentNullException.ThrowIfNull(store);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(planResolver);
        ArgumentNullException.ThrowIfNull(gauges);
        ArgumentNullException.ThrowIfNull(timeProvider);
        ArgumentNullException.ThrowIfNull(installationContext);

        _counters = store.Counters;
        _options = options;
        _planResolver = planResolver;
        _installationContext = installationContext;
        _timeProvider = timeProvider;
        _gauges = gauges.ToDictionary(g => g.Resource);
    }

    public ValueTask<QuotaCheckResult> CheckAsync(string installationId, QuotaResource resource, long amount, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(installationId);
        var (limit, exempt) = ResolveLimit(installationId, resource);
        var current = GetCounter(installationId, resource);

        if (exempt || limit == long.MaxValue)
        {
            return ValueTask.FromResult(QuotaCheckResult.Unlimited(resource, current));
        }

        return ValueTask.FromResult(new QuotaCheckResult(
            current + amount <= limit,
            resource,
            current,
            limit,
            GetPeriodResetUtc(resource)));
    }

    public ValueTask<long> RecordAsync(string installationId, QuotaResource resource, long amount, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(installationId);
        if (!IsCounterResource(resource))
        {
            return GetCurrentAsync(installationId, resource, ct);
        }

        var key = BuildCounterKey(installationId, resource);
        return ValueTask.FromResult(_counters.AddOrUpdate(key, amount, (_, value) => value + amount));
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
            return await CheckAsync(installationId, resource, amount, ct).ConfigureAwait(false);
        }

        var key = BuildCounterKey(installationId, resource);
        var newValue = _counters.AddOrUpdate(key, amount, (_, value) => value + amount);

        if (newValue <= limit)
        {
            return new QuotaCheckResult(true, resource, newValue, limit, GetPeriodResetUtc(resource));
        }

        _counters.AddOrUpdate(key, 0, (_, value) => value - amount);
        return new QuotaCheckResult(false, resource, newValue - amount, limit, GetPeriodResetUtc(resource));
    }

    public ValueTask<long> GetCurrentAsync(string installationId, QuotaResource resource, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(installationId);

        if (!IsCounterResource(resource))
        {
            if (_gauges.TryGetValue(resource, out var provider))
            {
                return provider.GetCurrentAsync(installationId, ct);
            }

            return ValueTask.FromResult(0L);
        }

        return ValueTask.FromResult(GetCounter(installationId, resource));
    }

    private long GetCounter(string installationId, QuotaResource resource) =>
        _counters.TryGetValue(BuildCounterKey(installationId, resource), out var value) ? value : 0;

    private (long Limit, bool Exempt) ResolveLimit(string installationId, QuotaResource resource)
    {
        if (_options.ExemptInstallation
            && string.Equals(installationId, InstallationConstants.Id, StringComparison.Ordinal))
        {
            return (long.MaxValue, true);
        }

        var installation = string.Equals(installationId, _installationContext.Current.Id, StringComparison.Ordinal)
            ? _installationContext.Current
            : null;

        return (_planResolver.ResolveLimit(installation, resource), false);
    }

    private static bool IsCounterResource(QuotaResource resource) =>
        resource is QuotaResource.ApiCalls or QuotaResource.StorageBytes;

    private static bool IsPeriodic(QuotaResource resource) => resource == QuotaResource.ApiCalls;

    private string BuildCounterKey(string installationId, QuotaResource resource)
    {
        if (!IsPeriodic(resource))
        {
            return $"quota:{installationId}:{resource}";
        }

        var now = _timeProvider.GetUtcNow();
        return $"quota:{installationId}:{resource}:{now.Year:D4}{now.Month:D2}";
    }

    private DateTimeOffset? GetPeriodResetUtc(QuotaResource resource)
    {
        if (!IsPeriodic(resource))
        {
            return null;
        }

        var now = _timeProvider.GetUtcNow();
        return now.Month == 12
            ? new DateTimeOffset(now.Year + 1, 1, 1, 0, 0, 0, TimeSpan.Zero)
            : new DateTimeOffset(now.Year, now.Month + 1, 1, 0, 0, 0, TimeSpan.Zero);
    }
}
