using Documentor.Models;
using Documentor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Documentor.Controllers
{
    public class PagesController : BaseController
    {
        private readonly IPager _pager;
        private readonly INavigator _navigator;

        public PagesController(IPager pager,
            INavigator navigator)
        {
            if (pager == null)
                throw new ArgumentNullException(nameof(pager));
            if (navigator == null)
                throw new ArgumentNullException(nameof(navigator));

            _pager = pager;
            _navigator = navigator;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            Nav nav = await _navigator.GetNavAsync();            
            return View(nav);
        }

        public async Task<IActionResult> Page(string virtualPath)
        {
            Page page = await _pager.GetPageAsync(virtualPath);
            if (page == null)
                return PageNotFound();

            ViewBag.IsPage = true;

            return View(page);
        }
        
        [Authorize]
        public async Task<IActionResult> Edit(string virtualPath, string markdown)
        {
            Page page = await _pager.GetPageAsync(virtualPath);
            if (page == null)
                return PageNotFound();

            if (Request.Method.Equals(HttpMethod.Post.Method))
            {
                page = await _pager.EditPageAsync(page.Path, markdown);
                return RedirectToAction(nameof(Page), new { virtualPath });
            }

            ViewBag.IsPage = true;

            return View(page);
        }
    }
}