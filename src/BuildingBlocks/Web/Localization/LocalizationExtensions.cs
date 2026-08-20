using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;

namespace FSH.Framework.Web.Localization;

public sealed class FshLocalizationOptions
{
    public string DefaultCulture { get; set; } =
        global::FSH.Framework.Web.Localization.SupportedCultures.Default;

    public string[] SupportedCultureTags { get; set; } =
        global::FSH.Framework.Web.Localization.SupportedCultures.Tags;
}


public static class LocalizationExtensions
{
    public static IHostApplicationBuilder AddHeroLocalization(
        this IHostApplicationBuilder builder,
        Action<FshLocalizationOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var options = new FshLocalizationOptions();
        builder.Configuration.GetSection("LocalizationOptions").Bind(options);
        configure?.Invoke(options);

        var supported = options.SupportedCultureTags

            .Where(SupportedCultures.IsSupported)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (supported.Length == 0)
        {
            supported = SupportedCultures.Tags;
        }

        var defaultCulture = SupportedCultures.IsSupported(options.DefaultCulture)
            ? options.DefaultCulture
            : SupportedCultures.Default;

        builder.Services.AddLocalization(configuration =>
        {
            configuration.ResourcesPath = "Localization/Resources";
        });

        builder.Services.Configure<RequestLocalizationOptions>(localization =>
        {
            // The API keeps numeric/date formatting invariant. Only UI resources negotiate culture.
            localization.DefaultRequestCulture = new RequestCulture(
                CultureInfo.InvariantCulture,
                new CultureInfo(defaultCulture));
            localization.SupportedCultures = null;
            localization.SupportedUICultures = supported
                .Select(static tag => new CultureInfo(tag))
                .ToList();
            localization.ApplyCurrentCultureToResponseHeaders = true;

            // The API receives the browser/user preference through Accept-Language.
            // Cookie negotiation is intentionally omitted until a server-side preference endpoint exists.
            localization.RequestCultureProviders =
            [
                new AcceptLanguageHeaderRequestCultureProvider()
            ];
        });

        return builder;
    }

    public static IApplicationBuilder UseHeroLocalization(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
        return app.UseRequestLocalization(
            app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
    }
}
