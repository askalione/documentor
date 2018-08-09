using Documentor;
using SmartBreadcrumbs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BreadcrumbsExtensions
    {
        public static void AddBreadcrumbs(this IServiceCollection services)
        {
            BreadcrumbsManager breadcrumbsManager = new BreadcrumbsManager();
            breadcrumbsManager.Initialize(typeof(Startup).Assembly, new BreadcrumbOptions());
            services.AddSingleton(breadcrumbsManager);
        }
    }
}
