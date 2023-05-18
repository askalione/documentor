using Documentor.Services.Authentication;
using Documentor.Services.Caching;
using Documentor.Services.Dumps;
using Documentor.Services.MarkdownConverters;
using Documentor.Services.Navigation;
using Documentor.Services.Pages;

namespace Documentor.Services
{
    public static class ServicesServiceCollectionExtensions
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddScoped<ISignInManager, SignInManager>();
            services.AddSingleton<IMarkdownConverter, MarkdigMarkdownConverter>();
            services.AddScoped<ICacheManager, CacheManager>();
            services.AddScoped<IPageIOManager, PageIOManager>();
            services.AddScoped<INavigator, PerRequestNavigator>();
            services.AddScoped<IPager, Pager>();
            services.AddScoped<IDumpProcessor, DumpProcessor>();

            return services;
        }
    }
}
