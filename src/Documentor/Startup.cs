using Documentor.Config;
using Documentor.Services;
using Documentor.Services.Impl;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;

namespace Documentor
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _env;

        public Startup(IHostingEnvironment env)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            
            _configuration = builder.Build();
            _env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddOptions();
            services.Configure<AppConfig>(_configuration.GetSection("App"));
            services.Configure<IOConfig>(_configuration.GetSection("IO"));
            services.PostConfigure<IOConfig>(config =>
            {
                if (!String.IsNullOrWhiteSpace(config.Pages.Path))
                    config.Pages.Path = Path.Combine(_env.ContentRootPath, config.Pages.Path);

                if (!String.IsNullOrWhiteSpace(config.Cache.Path))
                    config.Cache.Path = Path.Combine(_env.ContentRootPath, config.Cache.Path);
            });
            services.AddSingleton<IMarkdownConverter, MarkdigConverter>();
            services.AddScoped<IPager, Pager>();
            services.AddScoped<INavigator, PerRequestNavigator>();
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

            app.UseMvc(routes =>
            {

                routes.MapRoute("error",
                    "Error",
                    new { controller = "Errors", action = "Error" }
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
