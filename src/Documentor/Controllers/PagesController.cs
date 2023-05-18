using Documentor.Framework.Constants;
using Documentor.Framework.ExceptionHandling;
using Documentor.Framework.Navigation;
using Documentor.Framework.Notifications;
using Documentor.Models.Pages;
using Documentor.Services.Navigation;
using Documentor.Services.Pages;
using Microsoft.AspNetCore.Authorization;
using SmartBreadcrumbs;

namespace Documentor.Controllers
{
    [Authorize]
    public class PagesController : AppController
    {
        private readonly IPager _pager;
        private readonly INavigator _navigator;

        public PagesController(IPager pager, INavigator navigator)
        {
            if (pager == null)
                throw new ArgumentNullException(nameof(pager));
            if (navigator == null)
                throw new ArgumentNullException(nameof(navigator));

            _pager = pager;
            _navigator = navigator;
        }

        [Breadcrumb("Pages")]
        public async Task<IActionResult> Index()
        {
            Nav nav = await _navigator.GetNavAsync();
            return View(nav);
        }

        [AllowAnonymous]
        [DefaultBreadcrumb("Home")]
        public async Task<IActionResult> Page(string virtualPath)
        {
            Page? page = await _pager.GetPageAsync(virtualPath);
            if (page == null)
            {
                if (string.IsNullOrWhiteSpace(virtualPath) && User.Identity!.IsAuthenticated == false)
                {
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    return PageNotFound();
                }
            }

            BuildBreadcrumbs(virtualPath, await _navigator.GetNavAsync());
            ViewBag.IsPage = true;
            return View(page);
        }

        public async Task<IActionResult> Edit(string virtualPath, string markdown)
        {
            Page? page = await _pager.GetPageAsync(virtualPath);
            if (page == null)
                return PageNotFound();

            if (Request.Method.Equals(HttpMethod.Post.Method))
            {
                page = await _pager.EditPageAsync(page.Context.Path, markdown);
                return Redirect("/" + virtualPath)
                    .Notify(NotificationType.Success, "Changes saved");
            }

            BuildBreadcrumbs(virtualPath, await _navigator.GetNavAsync());
            ViewBag.IsPage = true;
            return View(page);
        }

        [HttpGet]
        [Breadcrumb("Add page", FromAction = "Pages.Index")]
        public IActionResult Add(string p)
        {
            PageAddCommand addCommand = new PageAddCommand
            {
                ParentVirtualPath = p
            };
            return View(addCommand);
        }

        [HttpPost]
        [Breadcrumb("Add page", FromAction = "Pages.Index")]
        [HandleException]
        public async Task<IActionResult> Add(PageAddCommand addCommand)
        {
            if (ModelState.IsValid)
            {
                await _pager.AddPageAsync(addCommand);
                return RedirectToAction(nameof(Index))
                    .Notify(NotificationType.Success, $"Page \"{addCommand.Title}\" added");
            }
            return View(addCommand);
        }

        [HttpGet]
        [Breadcrumb("Modify page", FromAction = "Pages.Index")]
        public async Task<IActionResult> Modify(string p)
        {
            PageContext pageContext = await _pager.GetPageContextAsync(p);
            PageModifyCommand modifyCommand = new PageModifyCommand
            {
                VirtualPath = pageContext.Path.Location.GetVirtualPath(),
                VirtualName = pageContext.Path.Location.GetDestinationFolder()!.VirtualName,
                Title = pageContext.Metadata.Title,
                Description = pageContext.Metadata.Description
            };
            return View(modifyCommand);
        }

        [HttpPost]
        [Breadcrumb("Modify page", FromAction = "Pages.Index")]
        [HandleException]
        public async Task<IActionResult> Modify(PageModifyCommand modifyCommand)
        {
            if (ModelState.IsValid)
            {
                await _pager.ModifyPageAsync(modifyCommand);
                string newVirtualPath = modifyCommand.VirtualPath.Remove(modifyCommand.VirtualPath.LastIndexOf(Separator.Path) + 1) + modifyCommand.VirtualName;
                return RedirectToAction(nameof(Index))
                    .Notify(NotificationType.Success, $"Page \"{modifyCommand.Title}\" modified");
            }
            return View(modifyCommand);
        }

        [HttpPost]
        [HandleException]
        public IActionResult Delete(string virtualPath)
        {
            _pager.DeletePage(virtualPath);
            return Json(new JsonResponse(true))
                .Notify(NotificationType.Success, $"Page with virtual path \"{virtualPath}\" deleted");
        }

        [HttpPost]
        [HandleException]
        public IActionResult Move(PageMoveCommand moveCommand)
        {
            _pager.MovePage(moveCommand);
            return Json(new JsonResponse(true))
                .Notify(NotificationType.Success, $"Page \"{moveCommand.VirtualPath.Substring(moveCommand.VirtualPath.LastIndexOf(Separator.Path) + 1)}\" moved");
        }

        private void BuildBreadcrumbs(string virtualPath, Nav nav)
        {
            string? action = RouteData.Values["action"]!.ToString();
            string? controller = RouteData.Values["controller"]!.ToString();

            string? requestPath = virtualPath?.Trim(Separator.Path);

            BreadcrumbNode? breadcrumbNode = null;
            if (!string.IsNullOrWhiteSpace(requestPath))
            {
                Stack<string> virtualNamesForScan = new Stack<string>(requestPath.Split(Separator.Path).Reverse());

                BreadcrumbNode? ScanNavItem(string virtualNameForScan, IEnumerable<NavItem> navItems, BreadcrumbNode? parent = null)
                {
                    if (!string.IsNullOrWhiteSpace(virtualNameForScan) &&
                        navItems.Count() > 0)
                    {
                        NavItem? navItem = navItems.FirstOrDefault(x => x.VirtualPath.Equals(virtualNameForScan, StringComparison.OrdinalIgnoreCase));
                        if (navItem != null)
                        {
                            BreadcrumbNode node = new BreadcrumbNode(navItem.DisplayName, "Page", "Pages", new { navItem.VirtualPath }, parent);
                            if (virtualNamesForScan.Count > 0 && navItem.Children.Count() > 0)
                                return ScanNavItem(virtualNameForScan + Separator.Path + virtualNamesForScan.Pop(), navItem.Children, node);
                            else
                                return node;
                        }
                    }

                    return parent;
                }

                breadcrumbNode = ScanNavItem(virtualNamesForScan.Pop(), nav.Items);
            }
            else
            {
                if (nav.Items.Count() > 0)
                {
                    NavItem navItem = nav.Items.First();
                    breadcrumbNode = new BreadcrumbNode(navItem.DisplayName, "Page", "Pages", new { navItem.VirtualPath });
                }
            }

            if (breadcrumbNode != null)
                ViewData["BreadcrumbNode"] = breadcrumbNode;
        }
    }
}
