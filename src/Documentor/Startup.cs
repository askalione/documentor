﻿using Documentor.Config;
using Documentor.Infrastructure;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace Documentor
{
    public class Startup
    {
        public const string RoutePrefix = "m";
        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        public Startup(IHostingEnvironment environment)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
            Environment = environment;
            Bootstrap.Configure();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.Configure<AppConfig>(Configuration.GetSection("App"));
            services.Configure<IOConfig>(Configuration.GetSection("IO"));
            services.ConfigureModifier<AuthorizationConfig>(Configuration.GetSection("Authorization"), Environment.IsDevelopment() ? $"appsettings.{Environment.EnvironmentName}.json" : "appsettings.json");
            services.AddScoped<ISignInManager, SignInManager>();
            services.AddSingleton<IMarkdownConverter, MarkdigConverter>();
            services.AddScoped<ICacheManager, CacheManager>();
            services.AddScoped<IPageIOManager, PageIOManager>();
            services.AddScoped<INavigator, PerRequestNavigator>();
            services.AddScoped<IPager, Pager>();
            services.AddScoped<IDumpProcessor, DumpProcessor>();

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                })
                .AddCookie(IdentityConstants.ApplicationScheme, options =>
                {
                    options.LoginPath = new PathString($"/{RoutePrefix}/Account/Login");
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
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });
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

                routes.MapRoute("manage",
                    RoutePrefix + "/{controller}/{action}",
                    new { action = "Index" },
                    new { isPage = new PageConstraint() }
                );

                routes.MapRoute("edit-page",
                    RoutePrefix + "/Edit/{*virtualPath}",
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
