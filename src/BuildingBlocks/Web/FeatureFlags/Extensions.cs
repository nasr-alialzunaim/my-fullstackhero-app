using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

namespace FSH.Framework.Web.FeatureFlags;

public static class Extensions
{
    /// <summary>
    /// Adds installation-wide feature management from the "FeatureManagement" configuration section.
    /// </summary>
    public static IServiceCollection AddHeroFeatureFlags(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddFeatureManagement(configuration.GetSection("FeatureManagement"));
        return services;
    }
}
