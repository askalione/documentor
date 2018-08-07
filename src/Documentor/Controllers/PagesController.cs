using Documentor.Models;
using Documentor.Services;
using Microsoft.AspNetCore.Mvc;
using System;
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

            return View(model: page);
        }
    }
}