using Documentor.Framework.Notifications;
using Documentor.Services.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Documentor.Controllers
{
    public class AccountController : AppController
    {
        private readonly ISignInManager _signInManager;

        public AccountController(ISignInManager signInManager)
        {
            if (signInManager == null)
                throw new ArgumentNullException(nameof(signInManager));

            _signInManager = signInManager;
        }

        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            if (User.Identity!.IsAuthenticated)
                return RedirectToAction("Page", "Pages");

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            if (!string.IsNullOrWhiteSpace(remoteError))
                return RedirectToAction(nameof(Login))
                    .Notify(NotificationType.Error, "Remote error");

            Services.Authentication.SignInResult signInResult = await _signInManager.TrySignInAsync(true);

            IActionResult result = RedirectToAction(nameof(Login));

            switch (signInResult)
            {
                case Services.Authentication.SignInResult.Successfully:
                    if (!string.IsNullOrWhiteSpace(returnUrl))
                        return RedirectToLocal(returnUrl);
                    return RedirectToAction("Page", "Pages");
                case Services.Authentication.SignInResult.Failure:
                    result = result.Notify(NotificationType.Error, "Login failed");
                    break;
                case Services.Authentication.SignInResult.AccessDenied:
                    result = result.Notify(NotificationType.Error, "Access denied");
                    break;
            }

            return result;
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Page", "Pages");
        }
    }
}
