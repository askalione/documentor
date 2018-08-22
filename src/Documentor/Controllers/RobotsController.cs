using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Documentor.Extensions;
using Documentor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs;

namespace Documentor.Controllers
{
    [Authorize]
    public class RobotsController : BaseController
    {
        private const string _robotsTxtFilename = "robots.txt";
        private readonly IHostingEnvironment _hostingEnvironment;

        public RobotsController(IHostingEnvironment hostingEnvironment)
        {
            if (hostingEnvironment == null)
                throw new ArgumentNullException(nameof(hostingEnvironment));

            _hostingEnvironment = hostingEnvironment;
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
            return new FileInfo(Path.Combine(_hostingEnvironment.WebRootPath, _robotsTxtFilename));
        }
    }
}