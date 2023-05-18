using Microsoft.Extensions.Options;

namespace Documentor.Framework.Options
{
    public static class OptionsServiceCollectionExtensions
    {
        public static void ConfigureModifier<T>(this IServiceCollection services,
            IConfigurationSection section,
            string file = "appsettings.json") where T : class, new()
        {
            services.Configure<T>(section);
            services.AddTransient<IOptionsModifier<T>>(provider =>
            {
                return new OptionsModifier<T>(provider.GetRequiredService<IHostEnvironment>(),
                    provider.GetRequiredService<IOptionsMonitor<T>>(),
                    section.Key,
                    file);
            });
        }
    }
}
