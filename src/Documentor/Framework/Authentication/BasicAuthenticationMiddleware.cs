using Documentor.Settings;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;

namespace Documentor.Framework.Authentication
{
    public class BasicAuthenticationMiddleware : IMiddleware
    {
        private readonly IOptionsSnapshot<BasicAuthenticationSettings> _settingsOptions;

        public BasicAuthenticationMiddleware(IOptionsSnapshot<BasicAuthenticationSettings> settingsOptions)
        {
            Ensure.NotNull(settingsOptions, nameof(settingsOptions));

            _settingsOptions = settingsOptions;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            BasicAuthenticationSettings settings = _settingsOptions.Value;

            if (context.User.Identity!.IsAuthenticated == false
                && string.IsNullOrWhiteSpace(settings.Login) == false
                && string.IsNullOrWhiteSpace(settings.Password) == false)
            {
                string? authHeader = context.Request.Headers["Authorization"];
                if (authHeader != null && authHeader.StartsWith("Basic "))
                {
                    string encodedUsernamePassword = authHeader.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[1]?.Trim()!;
                    string decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));

                    string login = decodedUsernamePassword.Split(':', 2)[0];
                    string password = decodedUsernamePassword.Split(':', 2)[1];

                    if (settings.Login == login && settings.Password == password)
                    {
                        await next.Invoke(context);
                        return;
                    }
                }

                context.Response.Headers["WWW-Authenticate"] = "Basic";

                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            else
            {
                await next.Invoke(context);
            }
        }
    }
}
