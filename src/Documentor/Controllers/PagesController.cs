using Documentor.Constants;
using Documentor.Extensions;
using Documentor.Infrastructure;
using Documentor.Models;
using Documentor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Documentor.Controllers
{
    [Authorize]
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
            Page page = await _pager.GetPageAsync(virtualPath);
            if (page == null)
                return PageNotFound();

            BuildBreadcrumbs(virtualPath, await _navigator.GetNavAsync());
            ViewBag.IsPage = true;
            return View(page);
        }

        public async Task<IActionResult> Edit(string virtualPath, string markdown)
        {
            Page page = await _pager.GetPageAsync(virtualPath);
            if (page == null)
                return PageNotFound();

            if (Request.Method.Equals(HttpMethod.Post.Method))
            {
                page = await _pager.EditPageAsync(page.Context.Path, markdown);
                return RedirectToAction(nameof(Page), new { virtualPath })
                    .Notify(NotificationType.Success, "Changes have been saved");
            }

            BuildBreadcrumbs(virtualPath, await _navigator.GetNavAsync());
            ViewBag.IsPage = true;
            return View(page);
        }

        private void BuildBreadcrumbs(string virtualPath, Nav nav)
        {
            string action = RouteData.Values["action"].ToString();
            string controller = RouteData.Values["controller"].ToString();

            string requestPath = virtualPath?.Trim(Separator.Path);

            BreadcrumbNode breadcrumbNode = null;
            if (!String.IsNullOrWhiteSpace(requestPath))
            {
                Stack<string> virtualNamesForScan = new Stack<string>(requestPath.Split(Separator.Path).Reverse());

                BreadcrumbNode ScanNavItem(string virtualNameForScan, IEnumerable<NavItem> navItems, BreadcrumbNode parent = null)
                {
                    if (!String.IsNullOrWhiteSpace(virtualNameForScan) &&
                        navItems.Count() > 0)
                    {
                        NavItem navItem = navItems.FirstOrDefault(x => x.VirtualPath.Equals(virtualNameForScan, StringComparison.OrdinalIgnoreCase));
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
                return RedirectToAction(nameof(Page), new { virtualPath = addCommand.ParentVirtualPath + Separator.Path + addCommand.VirtualName })
                    .Notify(NotificationType.Success, $"Page \"{addCommand.Title}\" has been added");
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
                VirtualName = pageContext.Path.Location.GetDestinationFolder().VirtualName,
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
                return RedirectToAction(nameof(Page), new { virtualPath = newVirtualPath })
                    .Notify(NotificationType.Success, $"Page \"{modifyCommand.Title}\" has been modified");
            }
            return View(modifyCommand);
        }

        [HttpPost]
        [HandleException]
        public IActionResult Remove(string virtualPath)
        {
            _pager.RemovePage(virtualPath);
            return Json(new JsonResponse(true))
                .Notify(NotificationType.Success, $"Page with virtual path \"{virtualPath}\" has been deleted");
        }
    }
}