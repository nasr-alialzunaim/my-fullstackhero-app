using FSH.Framework.Shared.Constants;

namespace FSH.Modules.Matching.Contracts.Authorization;

public static class MatchingPermissions
{
    public const string Resource = "Matching";
    public const string View = $"Permissions.{Resource}.View";
    public const string Configure = $"Permissions.{Resource}.Configure";
    public const string Run = $"Permissions.{Resource}.Run";
    public const string Review = $"Permissions.{Resource}.Review";

    public static IReadOnlyList<FshPermission> All { get; } =
    [
        new("View DNA Matching", ActionConstants.View, Resource, IsBasic: true),
        new("Configure DNA Matching", "Configure", Resource),
        new("Run DNA Matching", "Run", Resource),
        new("Review DNA Hits", "Review", Resource),
    ];
}
