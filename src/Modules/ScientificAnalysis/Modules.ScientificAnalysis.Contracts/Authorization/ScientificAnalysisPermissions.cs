using FSH.Framework.Shared.Constants;

namespace FSH.Modules.ScientificAnalysis.Contracts.Authorization;

public static class ScientificAnalysisPermissions
{
    public const string Resource = "ScientificAnalysis";
    public const string View = $"Permissions.{Resource}.View";
    public const string Run = $"Permissions.{Resource}.Run";

    public static IReadOnlyList<FshPermission> All { get; } =
    [
        new("View Scientific Analysis", ActionConstants.View, Resource, IsBasic: true),
        new("Run Scientific Analysis", "Run", Resource),
    ];
}
