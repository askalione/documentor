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
        public static AuthenticationBuilder AddGoogleIfConfigured(this AuthenticationBuilder builder,
            IConfiguration configuration)
        {
            AddOAuthIfConfigured("Google", 
                (clinetId, clientSecret) =>
                {
                    builder.AddGoogle(
                     options =>
                     {
                         options.ClientId = clinetId;
                         options.ClientSecret = clientSecret;
                     });
                }, configuration);

            return builder;
        }

        public static AuthenticationBuilder AddGitHubIfConfigured(this AuthenticationBuilder builder,
            IConfiguration configuration)
        {
            AddOAuthIfConfigured("GitHub",
                (clinetId, clientSecret) =>
                {
                    builder.AddGitHub(
                     options =>
                     {
                         options.ClientId = clinetId;
                         options.ClientSecret = clientSecret;
                         options.Scope.Add("user:email");
                     });
                }, configuration);

            return builder;
        }

        public static AuthenticationBuilder AddFacebookIfConfigured(this AuthenticationBuilder builder,
            IConfiguration configuration)
        {
            AddOAuthIfConfigured("Facebook",
                (clinetId, clientSecret) =>
                {
                    builder.AddFacebook(
                     options =>
                     {
                         options.ClientId = clinetId;
                         options.ClientSecret = clientSecret;
                     });
                }, configuration);

            return builder;
        }

        public static AuthenticationBuilder AddYandexIfConfigured(this AuthenticationBuilder builder,
            IConfiguration configuration)
        {
            AddOAuthIfConfigured("Yandex",
                (clinetId, clientSecret) =>
                {
                    builder.AddYandex(
                     options =>
                     {
                         options.ClientId = clinetId;
                         options.ClientSecret = clientSecret;
                     });
                }, configuration);

            return builder;
        }

        public static AuthenticationBuilder AddVkontakteIfConfigured(this AuthenticationBuilder builder,
            IConfiguration configuration)
        {
            AddOAuthIfConfigured("Vkontakte",
                (clinetId, clientSecret) =>
                {
                    builder.AddVkontakte(
                     options =>
                     {
                         options.ClientId = clinetId;
                         options.ClientSecret = clientSecret;
                     });
                }, configuration);

            return builder;
        }

        private static void AddOAuthIfConfigured(
            string configurationSectionName,
            Action<string, string> registration,
            IConfiguration configuration)
        {
            string clienId = configuration[$"Authentication:{configurationSectionName}:ClientId"];
            string clientSecret = configuration[$"Authentication:{configurationSectionName}:ClientSecret"];

            if (!string.IsNullOrWhiteSpace(clienId) &&
                !string.IsNullOrWhiteSpace(clientSecret))
                registration(clienId, clientSecret);
        }
    }
}
