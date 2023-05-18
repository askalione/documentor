namespace Documentor.Framework.Authentication
{
    public static class AuthenticationApplicationBuilderExceptions
    {
        public static IApplicationBuilder UseBasicAuthentication(this IApplicationBuilder app)
        {
            app.UseMiddleware<BasicAuthenticationMiddleware>();
            return app;
        }
    }
}
