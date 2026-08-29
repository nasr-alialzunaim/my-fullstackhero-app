using FSH.Framework.Shared.Installation;
using FSH.Framework.Shared.Quota;

namespace FSH.Framework.Quota;

/// <summary>
/// Resolves the effective quota limit for this installation.
/// Installation-local overrides take precedence over configured plan defaults.
/// </summary>
public sealed class QuotaPlanResolver
{
    private readonly QuotaOptions _options;

    public QuotaPlanResolver(QuotaOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _options = options;
    }

    public long ResolveLimit(InstallationInfo? installation, QuotaResource resource)
    {
        if (installation is not null
            && installation.QuotaLimits.TryGetValue(resource, out var installationLimit))
        {
            return NormalizeLimit(installationLimit);
        }

        var planName = !string.IsNullOrWhiteSpace(installation?.Plan)
            ? installation!.Plan!
            : _options.DefaultPlan;

        if (_options.Plans.TryGetValue(planName, out var plan)
            && plan.TryGetValue(resource, out var planLimit))
        {
            return NormalizeLimit(planLimit);
        }

        if (!string.Equals(planName, _options.DefaultPlan, StringComparison.OrdinalIgnoreCase)
            && _options.Plans.TryGetValue(_options.DefaultPlan, out var defaultPlan)
            && defaultPlan.TryGetValue(resource, out var defaultLimit))
        {
            return NormalizeLimit(defaultLimit);
        }

        return long.MaxValue;
    }

    private static long NormalizeLimit(long value) => value < 0 ? long.MaxValue : value;
}
