using Documentor.Framework.Notifications;
using Documentor.Framework.Options;
using Documentor.Models.Users;
using Documentor.Settings;
using Microsoft.AspNetCore.Authorization;
using SmartBreadcrumbs;

namespace Documentor.Controllers
{
    [Authorize]
    public class UsersController : AppController
    {
        private readonly IOptionsModifier<AuthorizationSettings> _authorizationSettingsModifier;
        private AuthorizationSettings _authorizationSettings => _authorizationSettingsModifier.Value;

        public UsersController(IOptionsModifier<AuthorizationSettings> settingsModifier)
        {
            if (settingsModifier == null)
                throw new ArgumentNullException(nameof(settingsModifier));

            _authorizationSettingsModifier = settingsModifier;
        }

        [HttpGet]
        [Breadcrumb("Users")]
        public IActionResult Index()
        {
            UsersEditCommand usersEditCommand = new UsersEditCommand
            {
                Emails = _authorizationSettings.Emails
            };
            return View(usersEditCommand);
        }

        [HttpPost]
        public IActionResult Add(string email)
        {
            if (string.IsNullOrEmpty(email))
                return Json(new JsonResponse(false, "Email required"));
            if (!IsValidEmail(email))
                return Json(new JsonResponse(false, "Email not valid"));

            email = email.Trim();

            if (!_authorizationSettings.Emails.Any(x => x.Equals(email, StringComparison.OrdinalIgnoreCase)))
            {
                List<string> emails = _authorizationSettings.Emails.ToList();
                emails.Add(email);
                _authorizationSettingsModifier.Update(x => x.Emails = emails);
            }

            return Json(new JsonResponse(true))
                .Notify(NotificationType.Success, $"{email} added");
        }

        [HttpPost]
        public IActionResult Remove(string email)
        {
            List<string> emails = _authorizationSettings.Emails.ToList();
            if (emails.Remove(email))
            {
                _authorizationSettingsModifier.Update(x => x.Emails = emails);
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
