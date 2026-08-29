namespace FSH.Framework.Shared.Installation;

public static class InstallationConstants
{
    /// <summary>
    /// Stable identifier retained as "root" for backward compatibility with existing
    /// rows, claims, audit records, quota keys, and seeded data.
    /// </summary>
    public const string Id = "root";
    public const string Name = "DNationalSystem";
    public const string AdminEmail = "admin@root.com";
    public const string DefaultProfilePicture = "assets/defaults/profile-picture.webp";
    public const string Issuer = "dnationalsystem";
}
