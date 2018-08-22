using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System.Security.Claims
{
    public static class PrincipalExtensions
    {
        public static string GetName(this ClaimsPrincipal principal)
        {
            string givenName = principal.FindFirstValue(ClaimTypes.GivenName);
            if (!string.IsNullOrWhiteSpace(givenName))
                return givenName;

            return principal.FindFirstValue(ClaimTypes.Name);
        }
    }
}
