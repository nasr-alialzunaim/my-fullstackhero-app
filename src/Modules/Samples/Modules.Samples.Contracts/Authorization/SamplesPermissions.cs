using FSH.Framework.Shared.Constants;

namespace FSH.Modules.Samples.Contracts.Authorization;

public static class SamplesPermissions
{
    public const string Resource = "Samples";
    public const string View = $"Permissions.{Resource}.View";
    public const string Create = $"Permissions.{Resource}.Create";

    public static IReadOnlyList<FshPermission> All { get; } =
    [
        new("View Biological Samples", ActionConstants.View, Resource, IsBasic: true),
        new("Register Biological Samples", ActionConstants.Create, Resource),
    ];
}
