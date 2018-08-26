using Documentor.Config;
using Documentor.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Documentor.Infrastructure
{
    public class RestrictedDemoAttribute : ActionFilterAttribute
    {
        private readonly AuthorizationConfig _authorizationConfig;

        public RestrictedDemoAttribute(IOptionsSnapshot<AuthorizationConfig> configOptions)
        {
            if (configOptions == null)
                throw new ArgumentNullException(nameof(configOptions));

            _authorizationConfig = configOptions.Value;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;
            var user = httpContext.User;

            if (httpContext.Request.Method == HttpMethod.Post.Method &&
                user.Identity.IsAuthenticated &&
                _authorizationConfig.Emails.Contains(user.FindFirstValue(ClaimTypes.Email)))
            {
                context.Result = IsAjaxRequest(httpContext.Request) ?
                    new JsonResult(new JsonResponse(false, "Restricted demo")) :
                    new RedirectResult(httpContext.Request.GetDisplayUrl())
                    .Notify(Models.NotificationType.Error, "Restricted demo");
            }

        }

        private static bool IsAjaxRequest(HttpRequest httpRequest)
        {
            return httpRequest.Headers["x-requested-with"] == "XMLHttpRequest";
        }
    }
}
