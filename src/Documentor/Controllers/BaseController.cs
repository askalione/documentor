using Documentor.Infrastructure;
using Documentor.Models;
using Microsoft.AspNetCore.Mvc;

namespace Documentor.Controllers
{
    public abstract class BaseController : Controller
    {
        public virtual IActionResult PageNotFound()
        {
            return View("Error", new Error { Code = 404, Message = "Page not found" });
        }
    }
}