using Documentor.Framework.Authentication;
using Documentor.Framework.Breadcrumbs;
using Documentor.Framework.Options;
using Documentor.Framework.Versioning;
using Documentor.Services;
using Documentor.Settings;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using WebMarkupMin.AspNetCore7;

namespace Documentor
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApp(this IServiceCollection services,
            ConfigurationManager configuration,
            IWebHostEnvironment environment)
        {
            services.Configure<AppSettings>(configuration.GetSection("App"));
            services.Configure<IOSettings>(configuration.GetSection("IO"));
            services.ConfigureModifier<AuthorizationSettings>(configuration.GetSection("Authorization"), environment.IsDevelopment() ? $"appsettings.{environment.EnvironmentName}.json" : "appsettings.json");

            services.AddBreadcrumbs();
            services.AddOptions();

            services.AddAppVersioning();
            services.AddAppServices();

            services.AddAspNetCore();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
                .AddCookie(IdentityConstants.ApplicationScheme, options =>
                {
                    options.LoginPath = new PathString($"/m/Account/Login");
                })
                .AddCookie(IdentityConstants.ExternalScheme, options =>
                {
                    options.Cookie.Name = IdentityConstants.ExternalScheme;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                })
                .AddGoogleIfConfigured(configuration)
                .AddGitHubIfConfigured(configuration)
                .AddYandexIfConfigured(configuration)
                .AddVkontakteIfConfigured(configuration);

            IMvcBuilder mvcBuilder = services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });

#if DEBUG
            if (environment.IsDevelopment())
            {
                mvcBuilder.AddRazorRuntimeCompilation();
            }
#endif
            services.Configure<RouteOptions>(options =>
            {
                options.AppendTrailingSlash = false;
                options.LowercaseUrls = true;
            });
            services.AddAntiforgery(options =>
            {
                options.HeaderName = "XSRF-TOKEN";
                options.Cookie.Name = "Documentor.Antiforgery";
            });
            services.Configure<CookieTempDataProviderOptions>(options =>
            {
                options.Cookie.Name = "Documentor.CookieTempDataProvider";
            });
            services.AddWebEncoders(options =>
            {
                options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic);
            });
            services.AddWebMarkupMin(options =>
            {
                options.AllowMinificationInDevelopmentEnvironment = false;
                options.AllowCompressionInDevelopmentEnvironment = false;
                options.DisablePoweredByHttpHeaders = true;
            })
            .AddHtmlMinification(options =>
            {
                options.MinificationSettings.RemoveRedundantAttributes = true;
                options.MinificationSettings.RemoveEmptyAttributes = false;
                options.MinificationSettings.RemoveTagsWithoutContent = false;
                options.MinificationSettings.RemoveHttpProtocolFromAttributes = false;
                options.MinificationSettings.RemoveHttpsProtocolFromAttributes = false;
                options.MinificationSettings.RemoveOptionalEndTags = false;
                options.MinificationSettings.RemoveHtmlComments = true;
            })
            .AddHttpCompression();

            return services;
        }
    }
}
