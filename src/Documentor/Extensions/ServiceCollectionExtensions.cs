using Documentor;
using Documentor.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SmartBreadcrumbs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddBreadcrumbs(this IServiceCollection services)
        {
            BreadcrumbsManager breadcrumbsManager = new BreadcrumbsManager();
            breadcrumbsManager.Initialize(typeof(Startup).Assembly, new BreadcrumbOptions());
            services.AddSingleton(breadcrumbsManager);
        }

        public static void ConfigureModifier<T>(this IServiceCollection services,
            IConfigurationSection section,
            string file = "appsettings.json") where T : class, new()
        {
            services.Configure<T>(section);
            services.AddTransient<IOptionsModifier<T>>(provider =>
            {
                return new OptionsModifier<T>(provider.GetService<IHostingEnvironment>(), 
                    provider.GetService<IOptionsMonitor<T>>(), 
                    section.Key, 
                    file);
            });
        }
    }
}
