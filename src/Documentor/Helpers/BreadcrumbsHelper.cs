using Documentor.Constants;
using Documentor.Models;
using Markdig.Helpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using SmartBreadcrumbs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Documentor.Helpers
{
    public static class BreadcrumbsHelper
    {
        //public static IHtmlContent Breadcrumbs(this IHtmlHelper html, Nav nav)
        //{
        //    RouteData routeData = html.ViewContext.RouteData;
        //    string action = routeData.Values["action"].ToString();
        //    string controller = routeData.Values["controller"].ToString();

        //    if ((!action.Equals("Page", StringComparison.OrdinalIgnoreCase) && !action.Equals("Edit", StringComparison.OrdinalIgnoreCase)) ||
        //        !controller.Equals("Pages", StringComparison.OrdinalIgnoreCase))
        //        return HtmlString.Empty;

        //    List<NavItem> breadcrumbsNavItems = new List<NavItem>();

        //    string requestPath = html.ViewContext.HttpContext.Request.Path.ToString().Trim(Separator.Path);
        //    string editPart = "Edit";
        //    if (requestPath.StartsWith(editPart, StringComparison.OrdinalIgnoreCase))
        //        requestPath = requestPath.Remove(0, editPart.Length).Trim(Separator.Path);
        //    if (!String.IsNullOrWhiteSpace(requestPath))
        //    {
        //        Stack<string> virtualNamesForScan = new Stack<string>(requestPath.Split(Separator.Path).Reverse());

        //        void ScanNavItem(string virtualNameForScan, IEnumerable<NavItem> navItems)
        //        {
        //            if (String.IsNullOrWhiteSpace(virtualNameForScan) ||
        //                navItems.Count() == 0)
        //                return;

        //            NavItem navItem = navItems.FirstOrDefault(x => x.VirtualPath.Equals(virtualNameForScan, StringComparison.OrdinalIgnoreCase));
        //            if (navItem != null)
        //            {
        //                breadcrumbsNavItems.Add(navItem);
        //                if (virtualNamesForScan.Count > 0 && navItem.Children.Count() > 0)
        //                    ScanNavItem(virtualNameForScan + Separator.Path + virtualNamesForScan.Pop(), navItem.Children);
        //            }
        //        }

        //        ScanNavItem(virtualNamesForScan.Pop(), nav.Items);
        //    }
        //    else
        //    {
        //        if (nav.Items.Count() > 0)
        //            breadcrumbsNavItems.Add(nav.Items.First());
        //    }

        //    TagBuilder breadcrumbs = new TagBuilder("div");
        //    breadcrumbs.AddCssClass("breadcrumbs");

        //    TagBuilder breadcrumbsHomeIcon = new TagBuilder("i");
        //    breadcrumbsHomeIcon.AddCssClass("la la-home breadcrumbs__home-icon");
        //    breadcrumbs.InnerHtml.AppendHtml(breadcrumbsHomeIcon);

        //    if (breadcrumbsNavItems.Count > 0)
        //    {

        //        TagBuilder breadcrumbsItems = new TagBuilder("ul");
        //        breadcrumbsItems.AddCssClass("breadcrumbs__items");

        //        for (var i = 0; i < breadcrumbsNavItems.Count; i++)
        //        {
        //            NavItem breadcrumbNavItem = breadcrumbsNavItems[i];

        //            TagBuilder breadcrumbItem = new TagBuilder("li");
        //            breadcrumbItem.AddCssClass("breadcrumbs__item");

        //            if (i != breadcrumbsNavItems.Count - 1)
        //            {
        //                TagBuilder breadcrumbLink = new TagBuilder("a");
        //                breadcrumbLink.AddCssClass("breadcrumbs__link");
        //                breadcrumbLink.Attributes.Add("href", Separator.Path + breadcrumbNavItem.VirtualPath);
        //                breadcrumbLink.InnerHtml.Append(breadcrumbNavItem.DisplayName);
        //                breadcrumbItem.InnerHtml.AppendHtml(breadcrumbLink);
        //            }
        //            else
        //            {
        //                TagBuilder breadcrumbText = new TagBuilder("span");
        //                breadcrumbText.AddCssClass("breadcrumbs__text");
        //                breadcrumbText.InnerHtml.Append(breadcrumbNavItem.DisplayName);
        //                breadcrumbItem.InnerHtml.AppendHtml(breadcrumbText);
        //            }

        //            breadcrumbsItems.InnerHtml.AppendHtml(breadcrumbItem);
        //        }

        //        breadcrumbs.InnerHtml.AppendHtml(breadcrumbsItems);
        //    }

        //    return breadcrumbs;
        //}

        public static IHtmlContent Breadcrumbs(this IHtmlHelper html)
        {
            ViewContext viewContext = html.ViewContext;
            IServiceProvider servicdeProvider = viewContext.HttpContext.RequestServices;
            BreadcrumbsManager breadcrumbsManager = servicdeProvider.GetService<BreadcrumbsManager>();
            IUrlHelperFactory urlHelperFactory = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            UrlHelper urlHelper = urlHelperFactory.GetUrlHelper(html.ViewContext) as UrlHelper;

            string action = viewContext.ActionDescriptor.RouteValues["action"];
            string controller = viewContext.ActionDescriptor.RouteValues["controller"];

            var nodeKey = $"{controller}.{action}";
            var node = viewContext.ViewData["BreadcrumbNode"] as BreadcrumbNode ?? breadcrumbsManager.GetNode(nodeKey);

            TagBuilder breadcrumbs = new TagBuilder("div");
            breadcrumbs.AddCssClass("breadcrumbs");

            TagBuilder breadcrumbsHomeIcon = new TagBuilder("i");
            breadcrumbsHomeIcon.AddCssClass("la la-home breadcrumbs__home-icon");
            breadcrumbs.InnerHtml.AppendHtml(breadcrumbsHomeIcon);

            if (node != null)
            {
                if (node.CacheTitle && node.Title.StartsWith("ViewData."))
                    node.Title = ExtractTitle(viewContext, node.Title);

                TagBuilder breadcrumbsItems = new TagBuilder("ul");
                breadcrumbsItems.AddCssClass("breadcrumbs__items");

                List<TagBuilder> items = new List<TagBuilder>();

                TagBuilder breadcrumbItem = new TagBuilder("li");
                breadcrumbItem.AddCssClass("breadcrumbs__item");
                TagBuilder breadcrumbText = new TagBuilder("span");
                breadcrumbText.AddCssClass("breadcrumbs__text");
                breadcrumbText.InnerHtml.Append(node.Title);
                breadcrumbItem.InnerHtml.AppendHtml(breadcrumbText);
                items.Add(breadcrumbItem);

                node = node.Parent;

                while (node != null && node != breadcrumbsManager.DefaultNode)
                {
                    breadcrumbItem = new TagBuilder("li");
                    breadcrumbItem.AddCssClass("breadcrumbs__item");

                    TagBuilder breadcrumbLink = new TagBuilder("a");
                    breadcrumbLink.AddCssClass("breadcrumbs__link");

                    RouteValueDictionary routes = new RouteValueDictionary(node.RouteValues);
                    if (routes.ContainsKey("virtualPath"))
                        breadcrumbLink.Attributes.Add("href", "/" + routes["virtualPath"]); // Tip: Otherwise '/' escaped
                    else
                        breadcrumbLink.Attributes.Add("href", node.GetUrl(urlHelper));
                    breadcrumbLink.InnerHtml.Append(node.Title);
                    breadcrumbItem.InnerHtml.AppendHtml(breadcrumbLink);
                    items.Add(breadcrumbItem);

                    node = node.Parent;
                }

                items.Reverse();
                items.ForEach(item =>
                {
                    breadcrumbsItems.InnerHtml.AppendHtml(item);
                });

                breadcrumbs.InnerHtml.AppendHtml(breadcrumbsItems);
            }

            return breadcrumbs;
        }

        private static string ExtractTitle(ViewContext viewContext, string title)
        {
            if (!title.StartsWith("ViewData."))
                return title;

            string key = title.Substring(9);
            return viewContext.ViewData.ContainsKey(key) ? viewContext.ViewData[key].ToString() : key;
        }
    }
}
