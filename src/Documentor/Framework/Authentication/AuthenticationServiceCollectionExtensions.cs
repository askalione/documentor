namespace Documentor.Framework.Authentication
{
    public static class AuthenticationServiceCollectionExtensions
    {
        public static IServiceCollection AddBasicAuthentication(this IServiceCollection services)
        {
            services.AddTransient<BasicAuthenticationMiddleware>();

            return services;
        }
    }
}
