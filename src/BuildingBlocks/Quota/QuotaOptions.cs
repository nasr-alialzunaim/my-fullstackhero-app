using FSH.Framework.Shared.Quota;

namespace FSH.Framework.Quota;

/// <summary>
/// Quota plan catalog for this installation. Installation-local overrides take precedence
/// over configured plan defaults when present.
/// </summary>
public sealed class QuotaOptions
{
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Redis connection string. When empty, the in-memory quota service is used instead (suitable
    /// for development/tests only — counters are per-process and not shared across instances).
    /// </summary>
    public string? Redis { get; set; }

    public string DefaultPlan { get; set; } = "free";

    /// <summary>Plan name → per-resource limit map. Use -1 or long.MaxValue for "unlimited".</summary>
    public Dictionary<string, Dictionary<QuotaResource, long>> Plans { get; } = new();

    /// <summary>
    /// Whether this local installation is exempt from quota enforcement.
    /// Offline deployments normally keep this enabled.
    /// </summary>
    public bool ExemptInstallation { get; set; } = true;
}
