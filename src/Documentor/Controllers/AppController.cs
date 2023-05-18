using Documentor.Models.Errors;

namespace Documentor.Controllers
{
    public abstract class AppController : Controller
    {
        public virtual IActionResult PageNotFound()
        {
            return View("Error", new Error { Code = 404, Message = "Page not found" });
        }
    }
}
