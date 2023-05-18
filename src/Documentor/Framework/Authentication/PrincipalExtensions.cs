using System.Security.Claims;

namespace Documentor.Framework.Authentication
{
    public static class PrincipalExtensions
    {
        public static string? GetName(this ClaimsPrincipal principal)
        {
            string? givenName = principal.FindFirstValue(ClaimTypes.GivenName);
            if (!string.IsNullOrWhiteSpace(givenName))
                return givenName;

            return principal.FindFirstValue(ClaimTypes.Name);
        }
    }
}
