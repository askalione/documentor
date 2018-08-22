using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Documentor.Config;
using Documentor.Extensions;
using Documentor.Infrastructure;
using Documentor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmartBreadcrumbs;

namespace Documentor.Controllers
{
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IOptionsModifier<AuthorizationConfig> _authorizationConfigModifier;
        private AuthorizationConfig _authorizationConfig => _authorizationConfigModifier.Value;

        public UsersController(IOptionsModifier<AuthorizationConfig> configModifier)
        {
            if (configModifier == null)
                throw new ArgumentNullException(nameof(configModifier));

            _authorizationConfigModifier = configModifier;
        }

        [HttpGet]
        [Breadcrumb("Users")]
        public IActionResult Index()
        {
            UsersEditCommand usersEditCommand = new UsersEditCommand
            {
                Emails = _authorizationConfig.Emails
            };
            return View(usersEditCommand);
        }

        [HttpPost]
        public IActionResult Add(string email)
        {
            if (String.IsNullOrEmpty(email))
                return Json(new JsonResponse(false, "Email required"));
            if (!IsValidEmail(email))
                return Json(new JsonResponse(false, "Email not valid"));

            email = email.Trim();

            if (!_authorizationConfig.Emails.Any(x => x.Equals(email, StringComparison.OrdinalIgnoreCase)))
            {
                List<string> emails = _authorizationConfig.Emails.ToList();
                emails.Add(email);
                _authorizationConfigModifier.Update(x => x.Emails = emails.ToArray());
            }

            return Json(new JsonResponse(true))
                .Notify(NotificationType.Success, $"{email} added");
        }

        [HttpPost]
        public IActionResult Remove(string email)
        {
            List<string> emails = _authorizationConfig.Emails.ToList();
            if (emails.Remove(email))
            {
                _authorizationConfigModifier.Update(x => x.Emails = emails.ToArray());
                return Json(new JsonResponse(true))
                    .Notify(NotificationType.Success, $"{email} removed");
            }
            else
            {
                return Json(new JsonResponse(false, "Email not found"));
            }
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}