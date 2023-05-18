using Documentor.Models.Errors;

namespace Documentor.Controllers
{
    public class ErrorsController : AppController
    {
        public IActionResult Error()
        {
            ViewBag.FullWidth = true;
            var error = new Error
            {
                Code = 500,
                Message = "Something goes wrong"
            };
            return View(error);
        }
    }
}
