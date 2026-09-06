using FSH.Framework.Shared.Constants;

namespace FSH.Modules.Genetics.Contracts.Authorization;

public static class GeneticsPermissions
{
    public const string Resource = "GeneticProfiles";
    public const string View = $"Permissions.{Resource}.View";
    public const string Create = $"Permissions.{Resource}.Create";

    public static IReadOnlyList<FshPermission> All { get; } =
    [
        new("View Genetic Profiles", ActionConstants.View, Resource, IsBasic: true),
        new("Create Genetic Profile Version", ActionConstants.Create, Resource),
    ];
}
