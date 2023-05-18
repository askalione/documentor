namespace Documentor.Framework.Versioning
{
    public static class VersioningServiceCollectionExtensions
    {
        public static IServiceCollection AddAppVersioning(this IServiceCollection services)
        {
            Ensure.NotNull(services, nameof(services));

            services.AddSingleton<IAppVersionProvider, AppVersionProvider>();

            return services;
        }
    }
}
