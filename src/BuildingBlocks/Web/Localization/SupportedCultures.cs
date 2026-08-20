namespace FSH.Framework.Web.Localization;

/// <summary>
/// Canonical cultures supported by the platform-level localization layer.
/// Keep this list deliberately small and extend it only together with resource catalogs and tests.
/// </summary>
public static class SupportedCultures
{
    public const string Default = "en-US";
    public const string Arabic = "ar-SA";

    public static readonly string[] Tags = [Default, Arabic];

    public static bool IsSupported(string? value) =>
        !string.IsNullOrWhiteSpace(value) &&
        Tags.Contains(value, StringComparer.OrdinalIgnoreCase);
}
