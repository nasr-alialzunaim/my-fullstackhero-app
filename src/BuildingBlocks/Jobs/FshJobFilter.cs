using FSH.Framework.Core.Common;
using FSH.Framework.Shared.Identity.Claims;
using Hangfire.Client;
using Hangfire.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Framework.Jobs;

public class FshJobFilter : IClientFilter
{
    private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();
    private readonly IServiceProvider _services;

    public FshJobFilter(IServiceProvider services) => _services = services;

    public void OnCreating(CreatingContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        Logger.InfoFormat(
            "Set UserId parameter for job {0}.{1}...",
            context.Job.Method.ReflectedType?.FullName,
            context.Job.Method.Name);

        using var scope = _services.CreateScope();
        var httpContext = scope.ServiceProvider.GetService<IHttpContextAccessor>()?.HttpContext;
        if (httpContext is null)
        {
            Logger.WarnFormat(
                "No HttpContext available for job {0}.{1}; skipping user parameter.",
                context.Job.Method.ReflectedType?.FullName,
                context.Job.Method.Name);
            return;
        }

        var userId = httpContext.User.GetUserId();
        if (!string.IsNullOrEmpty(userId))
        {
            context.SetJobParameter(QueryStringKeys.UserId, userId);
        }
    }

    public void OnCreated(CreatedContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        Logger.InfoFormat(
            "Job created with parameters {0}",
            context.Parameters.Count == 0
                ? "<none>"
                : string.Join(";", context.Parameters.Select(x => x.Key + "=" + x.Value)));
    }
}