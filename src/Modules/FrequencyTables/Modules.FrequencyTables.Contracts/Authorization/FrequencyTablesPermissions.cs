using FSH.Framework.Shared.Constants;

namespace FSH.Modules.FrequencyTables.Contracts.Authorization;

public static class FrequencyTablesPermissions
{
    public const string Resource = "FrequencyTables";
    public const string View = $"Permissions.{Resource}.View";
    public const string Create = $"Permissions.{Resource}.Create";
    public const string Manage = $"Permissions.{Resource}.Manage";

    public static IReadOnlyList<FshPermission> All { get; } =
    [
        new("View Frequency Tables", ActionConstants.View, Resource, IsBasic: true),
        new("Create Frequency Table Version", ActionConstants.Create, Resource),
        new("Manage Frequency Table Selection", "Manage", Resource),
    ];
}
