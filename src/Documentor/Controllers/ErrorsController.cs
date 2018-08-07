using Documentor.Models;
using Microsoft.AspNetCore.Mvc;

namespace Documentor.Controllers
{
    public class ErrorsController : BaseController
    {
        public IActionResult Error()
        {
            return View("Error", new Error { Code = 500, Message = "Something goes wrong" });
        }
    }
}