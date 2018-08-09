using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Documentor.Models;
using Documentor.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Documentor.Controllers
{
    public class AccountController : BaseController
    {
        private readonly ISignInManager _signInManager;
        
        [TempData]
        public string ErrorMessage { get; set; }

        public AccountController(ISignInManager signInManager)
        {
            if (signInManager == null)
                throw new ArgumentNullException(nameof(signInManager));

            _signInManager = signInManager;
        }

        public async Task<IActionResult> Login(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
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
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (!String.IsNullOrWhiteSpace(remoteError))
                return RedirectToAction(nameof(Login));

            Services.SignInResult result = await _signInManager.TrySignInAsync(true);

            switch(result)
            {
                case Services.SignInResult.Successfully:
                    if (!String.IsNullOrWhiteSpace(returnUrl))
                        return RedirectToLocal(returnUrl);
                    return RedirectToAction("Page", "Pages");
                case Services.SignInResult.Failure:
                    ErrorMessage = "Login have been failed";
                    break;
                case Services.SignInResult.AccessDenied:
                    ErrorMessage = "Access denied";
                    break;
            }
            
            return RedirectToAction(nameof(Login));
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Page", "Pages");
        }
    }
}