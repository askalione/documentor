using Documentor.Config;
using Documentor.Services;
using Documentor.Services.Impl;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.Configure<AppConfig>(Configuration.GetSection("App"));
            services.Configure<AuthorizationConfig>(Configuration.GetSection("Authorization"));
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
                .AddCookie(IdentityConstants.ApplicationScheme, options =>
                {
                    options.LoginPath = new PathString("/Login");
                })
                .AddCookie(IdentityConstants.ExternalScheme, options =>
                {
                    options.Cookie.Name = IdentityConstants.ExternalScheme;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                })
                .AddGoogleIfConfigured(Configuration)
                .AddGitHubIfConfigured(Configuration)
                .AddFacebookIfConfigured(Configuration)
                .AddYandexIfConfigured(Configuration)
                .AddVkontakteIfConfigured(Configuration);

            services.AddOptions();
            services.AddBreadcrumbs();
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
                OnPrepareResponse = context =>
                {
                    ResponseHeaders headers = context.Context.Response.GetTypedHeaders();
                    if (env.IsDevelopment())
                    {
                        headers.CacheControl = new CacheControlHeaderValue()
                        {
                            NoCache = true,
                            NoStore = true
                        };
                        headers.Expires = DateTime.Now.AddDays(-1);
                    }
                    else
                    {                        
                        headers.CacheControl = new CacheControlHeaderValue()
                        {
                            Public = true,
                            MaxAge = TimeSpan.FromDays(365)
                        };
                    } 
                }
            });

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute("error",
                    "Error",
                    new { controller = "Errors", action = "Error" }
                );

                routes.MapRoute("login",
                    "Login",
                    new { controller = "Account", action = "Login" }
                );

                routes.MapRoute("logout",
                    "Logout",
                    new { controller = "Account", action = "Logout" }
                );

                routes.MapRoute("external-login",
                    "ExternalLogin",
                    new { controller = "Account", action = "ExternalLogin" }
                );

                routes.MapRoute("external-login-callback",
                    "ExternalLoginCallback",
                    new { controller = "Account", action = "ExternalLoginCallback" }
                );

                routes.MapRoute("pages",
                    "Pages",
                    new { controller = "Pages", action = "Index" }
                );

                routes.MapRoute("edit-page",
                    "Edit/{*virtualPath}",
                    new { controller = "Pages", action = "Edit" }
                );

                routes.MapRoute("page",
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
