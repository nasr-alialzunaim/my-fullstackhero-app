using Asp.Versioning;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.AspNetCore.Extensions;
using Finbuckle.MultiTenant.EntityFrameworkCore.Stores;
using Finbuckle.MultiTenant.Extensions;
using Finbuckle.MultiTenant.Stores;
using FSH.Framework.Core.Exceptions;
using FSH.Framework.Eventing.Abstractions;
using FSH.Framework.Persistence;
using FSH.Framework.Shared.Constants;
using FSH.Framework.Shared.Multitenancy;
using FSH.Framework.Web.Modules;
using FSH.Modules.Multitenancy.Contracts;
using FSH.Modules.Multitenancy.Data;
using FSH.Modules.Multitenancy.Features.v1.AdjustTenantValidity;
using FSH.Modules.Multitenancy.Features.v1.ChangeTenantActivation;
using FSH.Modules.Multitenancy.Features.v1.CreateTenant;
using FSH.Modules.Multitenancy.Features.v1.GetMyTenantStatus;
using FSH.Modules.Multitenancy.Features.v1.GetTenantMigrations;
using FSH.Modules.Multitenancy.Features.v1.GetTenants;
using FSH.Modules.Multitenancy.Features.v1.GetTenantStatus;
using FSH.Modules.Multitenancy.Features.v1.GetTenantTheme;
using FSH.Modules.Multitenancy.Features.v1.ResetTenantTheme;
using FSH.Modules.Multitenancy.Features.v1.TenantProvisioning.GetTenantProvisioningStatus;
using FSH.Modules.Multitenancy.Features.v1.TenantProvisioning.RetryTenantProvisioning;
using FSH.Modules.Multitenancy.Features.v1.RenewTenant;
using FSH.Modules.Multitenancy.Features.v1.UpdateTenantTheme;
using FSH.Modules.Multitenancy.Provisioning;
using FSH.Modules.Multitenancy.Services;
using Hangfire;
using Hangfire.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace FSH.Modules.Multitenancy;

public sealed class MultitenancyModule : IModule
{
    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        FSH.Framework.Shared.Constants.PermissionConstants.Register(
            FSH.Modules.Multitenancy.Contracts.Authorization.MultitenancyPermissions.All);

        builder.Services.Configure<TenantBillingOptions>(
            builder.Configuration.GetSection(TenantBillingOptions.SectionName));

        builder.Services.AddScoped<ITenantService, TenantService>();
        builder.Services.AddScoped<ITenantThemeService, TenantThemeService>();
        builder.Services.AddTransient<IConnectionStringValidator, ConnectionStringValidator>();
        builder.Services.AddScoped<ITenantProvisioningService, TenantProvisioningService>();
        builder.Services.AddTransient<TenantProvisioningJob>();
        builder.Services.AddTransient<TenantExpiryScanJob>();

        // Singleton — the buffer survives the request scope that calls Store(...)
        // so the background Hangfire-scheduled seed scope can still TryConsume(...).
        builder.Services.AddSingleton<
            FSH.Framework.Shared.Multitenancy.ITenantInitialPasswordBuffer,
            Services.TenantInitialPasswordBuffer>();

        builder.Services.AddHeroDbContext<TenantDbContext>();

        // Replace (not Add) the no-op event tenant scope with a Finbuckle-backed one so background
        // event dispatch establishes the tenant before tenant-filtered handler DbContexts are built.
        builder.Services.Replace(
            ServiceDescriptor.Singleton<IEventTenantScope, FinbuckleEventTenantScope>());

        // P0 single-tenant transition: keep the Finbuckle compatibility layer only until
        // persistence/identity are fully detached from it. Resolution is hard-pinned to the
        // root installation; request headers, claims and query strings can no longer select
        // or override a tenant.
        builder.Services
            .AddMultiTenant<AppTenantInfo>()
            .WithDelegateStrategy(_ => Task.FromResult<string?>(MultitenancyConstants.Root.Id))
            .WithStore<EFCoreStore<TenantDbContext, AppTenantInfo>>(ServiceLifetime.Scoped);

        builder.Services.AddHealthChecks()
            .AddDbContextCheck<TenantDbContext>(
                name: "db:multitenancy",
                failureStatus: HealthStatus.Unhealthy)
            .AddCheck<TenantMigrationsHealthCheck>(
                name: "db:tenants-migrations",
                failureStatus: HealthStatus.Unhealthy);
    }

    public void ConfigureMiddleware(IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        // Single-tenant mode: no tenant override, activation, expiry, or subscription
        // middleware is allowed to influence request routing.
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        // Single-tenant installations do not expose tenant lifecycle, provisioning,
        // migration-status, subscription, or tenant-theme administration endpoints.
    }

}