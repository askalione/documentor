using Documentor.Config;
using Documentor.Models;
using Documentor.Services;
using Documentor.Services.Impl;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System;

namespace Documentor
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.Configure<AppConfig>(Configuration.GetSection("App"));
            services.Configure<IOConfig>(Configuration.GetSection("IO"));
            services.AddScoped<ISignInManager, SignInManager>();
            services.AddSingleton<IMarkdownConverter, MarkdigConverter>();
            services.AddScoped<ICacheManager, CacheManager>();
            services.AddScoped<IPageManager, PageManager>();
            services.AddScoped<INavigator, PerRequestNavigator>();
            services.AddScoped<IPager, Pager>();

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                })
                .AddCookie(IdentityConstants.ApplicationScheme, o =>
                {
                    o.LoginPath = new PathString("/Account/Login");
                    //o.Events = new CookieAuthenticationEvents
                    //{
                    //    OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync
                    //};
                })
                .AddCookie(IdentityConstants.ExternalScheme, o =>
                {
                    o.Cookie.Name = IdentityConstants.ExternalScheme;
                    o.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                })
                .AddGoogleIfConfigured(Configuration);

            services.AddOptions();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.Map("/favicon.ico", delegate { });

            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    var headers = ctx.Context.Response.GetTypedHeaders();
                    headers.CacheControl = new CacheControlHeaderValue()
                    {
                        Public = true,
                        MaxAge = TimeSpan.FromDays(365)
                    };
                }
            });

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute("error",
                    "Error",
                    new { controller = "Errors", action = "Error" }
                );

                routes.MapRoute("account",
                    "Account/{action}",
                    new { controller = "Account", action = "Login" }
                );

                routes.MapRoute("pages",
                    "{*virtualPath}",
                    new { controller = "Pages", action = "Page" }
                );

                routes.MapRoute("default",
                    "",
                    new { controller = "Pages", action = "Page" }
                );
            });
        }
    }
}
