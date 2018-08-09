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

        public PagesController(IPager pager)
        {
            if (pager == null)
                throw new ArgumentNullException(nameof(pager));

            _pager = pager;
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