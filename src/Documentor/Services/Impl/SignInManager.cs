using Documentor.Config;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Documentor.Services.Impl
{
    public class SignInManager : ISignInManager
    {
        private const string _loginProviderKey = "LoginProvider";
        private const string _xsrfKey = "XsrfId";

        private readonly AuthorizationConfig _authorizationConfig;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IAuthenticationSchemeProvider _schemes;

        public SignInManager(IOptionsSnapshot<AuthorizationConfig> configOptions,
            IHttpContextAccessor contextAccessor,
            IAuthenticationSchemeProvider schemes)
        {
            if (configOptions == null)
                throw new ArgumentNullException(nameof(configOptions));
            if (contextAccessor == null)
                throw new ArgumentNullException(nameof(contextAccessor));
            if (schemes == null)
                throw new ArgumentNullException(nameof(schemes));

            _authorizationConfig = configOptions.Value;
            _contextAccessor = contextAccessor;
            _schemes = schemes;
        }

        public async Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync()
        {
            return (await _schemes.GetAllSchemesAsync()).Where(s => !string.IsNullOrEmpty(s.DisplayName));
        }

        public AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl)
        {
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            properties.Items[_loginProviderKey] = provider;
            return properties;
        }

        public async Task<SignInResult> TrySignInAsync(bool isPersistent)
        {
            ClaimsPrincipal externalClaimsPrincipal = await GetExternalClaimsPrincipal();
            if (externalClaimsPrincipal == null)
                return SignInResult.Failure;

            string email = externalClaimsPrincipal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrWhiteSpace(email) || !_authorizationConfig.Emails.Contains(email))
                return SignInResult.AccessDenied;

            ClaimsPrincipal userPrincipal = CreateClaimsPrincipal(externalClaimsPrincipal);
            AuthenticationProperties authenticationProperties = new AuthenticationProperties { IsPersistent = isPersistent };
            await _contextAccessor.HttpContext.SignInAsync(IdentityConstants.ApplicationScheme,
                userPrincipal,
                authenticationProperties);
            return SignInResult.Successfully;
        }

        public virtual async Task SignOutAsync()
        {
            var context = _contextAccessor.HttpContext;
            await context.SignOutAsync(IdentityConstants.ApplicationScheme);
            await context.SignOutAsync(IdentityConstants.ExternalScheme);
        }

        private async Task<ClaimsPrincipal> GetExternalClaimsPrincipal()
        {
            var auth = await _contextAccessor.HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
            var items = auth?.Properties?.Items;
            if (auth?.Principal == null || items == null || !items.ContainsKey(_loginProviderKey))
                return null;
            
            var providerKey = auth.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var provider = items[_loginProviderKey] as string;
            if (providerKey == null || provider == null)
                return null;

            return auth.Principal;
        }

        private ClaimsPrincipal CreateClaimsPrincipal(ClaimsPrincipal externalClaimsPrincipal)
        {
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(IdentityConstants.ApplicationScheme, 
                ClaimTypes.Name,
                ClaimTypes.Role);

            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, externalClaimsPrincipal.FindFirstValue(ClaimTypes.Email)));            
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, externalClaimsPrincipal.FindFirstValue(ClaimTypes.Name)));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, externalClaimsPrincipal.FindFirstValue(ClaimTypes.Email)));

            string givenName = externalClaimsPrincipal.FindFirstValue(ClaimTypes.GivenName);
            if (!string.IsNullOrEmpty(givenName))
                claimsIdentity.AddClaim(new Claim(ClaimTypes.GivenName, givenName));

            return new ClaimsPrincipal(claimsIdentity);
        }
    }
}
