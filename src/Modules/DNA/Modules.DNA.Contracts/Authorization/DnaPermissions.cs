using FSH.Framework.Shared.Constants;

namespace FSH.Modules.DNA.Contracts.Authorization;

public static class DnaPermissions
{
    public static class ModuleAccess
    {
        public const string Resource = "DNA.Module";
        public const string View = $"Permissions.{Resource}.View";
    }

    public static IReadOnlyList<FshPermission> All { get; } =
    [
        new("View DNA module", ActionConstants.View, ModuleAccess.Resource, IsBasic: true),
    ];
}
