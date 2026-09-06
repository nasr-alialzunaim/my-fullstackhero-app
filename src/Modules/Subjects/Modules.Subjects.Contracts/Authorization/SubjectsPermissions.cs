using FSH.Framework.Shared.Constants;

namespace FSH.Modules.Subjects.Contracts.Authorization;

public static class SubjectsPermissions
{
    public const string Resource = "Subjects";
    public const string View = $"Permissions.{Resource}.View";
    public const string Create = $"Permissions.{Resource}.Create";
    public const string Update = $"Permissions.{Resource}.Update";

    public static IReadOnlyList<FshPermission> All { get; } =
    [
        new("View DNA Subjects", ActionConstants.View, Resource, IsBasic: true),
        new("Create DNA Subjects", ActionConstants.Create, Resource),
        new("Update DNA Subjects", ActionConstants.Update, Resource),
    ];
}
