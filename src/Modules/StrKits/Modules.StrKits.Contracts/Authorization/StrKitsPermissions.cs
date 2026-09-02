using FSH.Framework.Shared.Constants;

namespace FSH.Modules.StrKits.Contracts.Authorization;

public static class StrKitsPermissions
{
    public const string Resource = "StrKits";
    public const string View = $"Permissions.{Resource}.View";
    public const string Create = $"Permissions.{Resource}.Create";

    public static IReadOnlyList<FshPermission> All { get; } =
    [
        new("View STR Kits", ActionConstants.View, Resource, IsBasic: true),
        new("Create STR Kit Version", ActionConstants.Create, Resource),
    ];
}
