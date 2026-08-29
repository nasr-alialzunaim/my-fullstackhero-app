using FSH.Framework.Shared.Quota;

namespace FSH.Framework.Shared.Installation;

/// <summary>
/// Immutable-in-scope information for the one locally installed DNationalSystem instance.
/// </summary>
public sealed class InstallationInfo
{
    public string Id { get; init; } = InstallationConstants.Id;
    public string Name { get; init; } = InstallationConstants.Name;
    public string AdminEmail { get; init; } = InstallationConstants.AdminEmail;
    public string Issuer { get; init; } = InstallationConstants.Issuer;
    public string? Plan { get; set; }
    public Dictionary<QuotaResource, long> QuotaLimits { get; } = new();
}
