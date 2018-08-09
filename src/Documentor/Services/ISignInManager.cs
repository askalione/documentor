using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Documentor.Services
{
    public enum SignInResult
    {
        Failure,
        Successfully,
        AccessDenied
    }

    public interface ISignInManager
    {
        Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync();
        AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl);
        Task<SignInResult> TrySignInAsync(bool isPersistent);
        Task SignOutAsync();
    }
}
