using Microsoft.AspNetCore.Authentication;

namespace Documentor.Services.Authentication
{
    public interface ISignInManager
    {
        Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync();
        AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string? redirectUrl);
        Task<SignInResult> TrySignInAsync(bool isPersistent);
        Task SignOutAsync();
    }
}
