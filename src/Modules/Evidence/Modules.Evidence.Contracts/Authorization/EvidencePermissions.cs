using FSH.Framework.Shared.Constants;

namespace FSH.Modules.Evidence.Contracts.Authorization;

public static class EvidencePermissions
{
    public const string Resource = "Evidence";
    public const string View = $"Permissions.{Resource}.View";
    public const string Create = $"Permissions.{Resource}.Create";

    public static IReadOnlyList<FshPermission> All { get; } =
    [
        new("View Evidence", ActionConstants.View, Resource, IsBasic: true),
        new("Register Evidence", ActionConstants.Create, Resource),
    ];
}
