using Documentor.Framework.Notifications;
using Documentor.Models.Robots;
using Microsoft.AspNetCore.Authorization;
using SmartBreadcrumbs;
using System.Text;

namespace Documentor.Controllers
{
    [Authorize]
    public class RobotsController : AppController
    {
        private const string _robotsTxtFilename = "robots.txt";
        private readonly IWebHostEnvironment _hostEnvironment;

        public RobotsController(IWebHostEnvironment hostEnvironment)
        {
            if (hostEnvironment == null)
                throw new ArgumentNullException(nameof(hostEnvironment));

            _hostEnvironment = hostEnvironment;
        }

        [HttpGet]
        [Breadcrumb("Robots")]
        public async Task<IActionResult> Index()
        {
            RobotsEditCommand robotsEditCommand = new RobotsEditCommand();
            FileInfo robotsFile = GetRobotsTxtFile();
            if (robotsFile.Exists)
                robotsEditCommand.Content = await System.IO.File.ReadAllTextAsync(robotsFile.FullName);
            return View(robotsEditCommand);
        }

        [HttpPost]
        [Breadcrumb("Robots")]
        public async Task<IActionResult> Index(RobotsEditCommand robotsEditCommand)
        {
            if (ModelState.IsValid)
            {
                await System.IO.File.WriteAllTextAsync(GetRobotsTxtFile().FullName, robotsEditCommand.Content, Encoding.UTF8);
                return RedirectToAction(nameof(Index))
                    .Notify(NotificationType.Success, "Changes in robots.txt saved");
            }
            return View();
        }

        private FileInfo GetRobotsTxtFile()
        {
            return new FileInfo(Path.Combine(_hostEnvironment.WebRootPath, _robotsTxtFilename));
        }
    }
}
