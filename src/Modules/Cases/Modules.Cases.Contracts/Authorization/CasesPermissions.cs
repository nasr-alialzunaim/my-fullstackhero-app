using FSH.Framework.Shared.Constants;

namespace FSH.Modules.Cases.Contracts.Authorization;

public static class CasesPermissions
{
    public const string Resource = "Cases";
    public const string View = $"Permissions.{Resource}.View";
    public const string Create = $"Permissions.{Resource}.Create";
    public const string Update = $"Permissions.{Resource}.Update";

    public static IReadOnlyList<FshPermission> All { get; } =
    [
        new("View Cases", ActionConstants.View, Resource, IsBasic: true),
        new("Create Cases", ActionConstants.Create, Resource),
        new("Update Cases", ActionConstants.Update, Resource),
    ];
}