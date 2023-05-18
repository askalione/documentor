using SmartBreadcrumbs;

namespace Documentor.Framework.Breadcrumbs
{
    public static class BreadcrumbsServiceCollectionExtensions
    {
        public static void AddBreadcrumbs(this IServiceCollection services)
        {
            BreadcrumbsManager breadcrumbsManager = new BreadcrumbsManager();
            breadcrumbsManager.Initialize(typeof(Program).Assembly, new BreadcrumbOptions());
            services.AddSingleton(breadcrumbsManager);
        }
    }
}
