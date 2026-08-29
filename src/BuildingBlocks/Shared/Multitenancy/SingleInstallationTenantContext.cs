using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Framework.Shared.Multitenancy;

/// <summary>
/// Compatibility context for the single-installation runtime.
/// Legacy components that still consume Finbuckle abstractions always observe the
/// installation root; callers cannot select, replace, or mutate the active tenant.
/// No tenant store, tenant database, middleware, or tenant lifecycle is involved.
/// </summary>
public sealed class SingleInstallationTenantContext :
    IMultiTenantContextAccessor<AppTenantInfo>,
    IMultiTenantContextSetter
{
    private static readonly IMultiTenantContext<AppTenantInfo> RootContext =
        new MultiTenantContext<AppTenantInfo>(CreateInstallation());

    public IMultiTenantContext<AppTenantInfo> MultiTenantContext => RootContext;

    IMultiTenantContext IMultiTenantContextAccessor.MultiTenantContext => RootContext;

#pragma warning disable S2376 // Finbuckle's IMultiTenantContextSetter contract is intentionally setter-only.
    IMultiTenantContext IMultiTenantContextSetter.MultiTenantContext
    {
        set
        {
            // Intentionally ignored. Background jobs/event handlers written against
            // the former multitenant API cannot switch a single-installation process.
        }
    }
#pragma warning restore S2376

    private static AppTenantInfo CreateInstallation() =>
        new(MultitenancyConstants.Root.Id, MultitenancyConstants.Root.Id, MultitenancyConstants.Root.Name)
        {
            AdminEmail = MultitenancyConstants.Root.EmailAddress,
            IsActive = true,
            ValidUpto = DateTime.MaxValue,
            Issuer = MultitenancyConstants.Root.Issuer
        };
}

public static class SingleInstallationTenantContextExtensions
{
    public static IServiceCollection AddSingleInstallationTenantContext(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        var accessor = new SingleInstallationTenantContext();
        services.AddSingleton<IMultiTenantContextAccessor<AppTenantInfo>>(accessor);
        services.AddSingleton<IMultiTenantContextSetter>(accessor);
        return services;
    }
}
