using Microsoft.Extensions.DependencyInjection;

namespace FSH.Framework.Shared.Installation;

public sealed class SingleInstallationContext : IInstallationContext
{
    public InstallationInfo Current { get; } = new();
}

public static class InstallationContextExtensions
{
    public static IServiceCollection AddSingleInstallationContext(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddSingleton<IInstallationContext, SingleInstallationContext>();
        return services;
    }
}
