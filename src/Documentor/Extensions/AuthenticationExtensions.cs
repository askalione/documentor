using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AuthenticationExtensions
    {
        public static AuthenticationBuilder AddGoogleIfConfigured(this AuthenticationBuilder builder, IConfiguration configuration)
        {
            string clienId = configuration["Authentication:Google:ClientId"];
            string clientSecret = configuration["Authentication:Google:ClientSecret"];

            if (!String.IsNullOrWhiteSpace(clienId) &&
                !String.IsNullOrWhiteSpace(clientSecret))
            {
                builder.AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = clienId;
                    googleOptions.ClientSecret = clientSecret;
                });
            }

            return builder;
        }
    }
}
