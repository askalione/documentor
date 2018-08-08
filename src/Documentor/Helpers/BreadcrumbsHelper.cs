using Documentor.Constants;
using Documentor.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Documentor.Helpers
{
    public static class BreadcrumbsHelper
    {
        public static IHtmlContent Breadcrumbs(this IHtmlHelper html, Nav nav)
        {
            RouteData routeData = html.ViewContext.RouteData;
            string action = routeData.Values["action"].ToString();
            string controller = routeData.Values["controller"].ToString();

            if (!action.Equals("Page", StringComparison.OrdinalIgnoreCase) ||
                !controller.Equals("Pages", StringComparison.OrdinalIgnoreCase))
                return HtmlString.Empty;

            List<NavItem> breadcrumbsNavItems = new List<NavItem>();

            string request = html.ViewContext.HttpContext.Request.Path.ToString().Trim(Separator.Path);
            if (!String.IsNullOrWhiteSpace(request))
            {
                Stack<string> virtualNamesForScan = new Stack<string>(request.Split(Separator.Path).Reverse());

                void ScanNavItem(string virtualNameForScan, IEnumerable<NavItem> navItems)
                {
                    if (String.IsNullOrWhiteSpace(virtualNameForScan) ||
                        navItems.Count() == 0)
                        return;

                    NavItem navItem = navItems.FirstOrDefault(x => x.VirtualPath.Equals(virtualNameForScan, StringComparison.OrdinalIgnoreCase));
                    if (navItem != null)
                    {
                        breadcrumbsNavItems.Add(navItem);
                        if (virtualNamesForScan.Count > 0 && navItem.Children.Count() > 0)
                            ScanNavItem(virtualNameForScan + Separator.Path + virtualNamesForScan.Pop(), navItem.Children);
                    }
                }

                ScanNavItem(virtualNamesForScan.Pop(), nav.Items);
            }
            else
            {
                if (nav.Items.Count() > 0)
                    breadcrumbsNavItems.Add(nav.Items.First());
            }

            TagBuilder breadcrumbs = new TagBuilder("div");
            breadcrumbs.AddCssClass("breadcrumbs");

            TagBuilder breadcrumbsHomeIcon = new TagBuilder("i");
            breadcrumbsHomeIcon.AddCssClass("la la-home breadcrumbs__home-icon");
            breadcrumbs.InnerHtml.AppendHtml(breadcrumbsHomeIcon);

            if (breadcrumbsNavItems.Count > 0)
            {

                TagBuilder breadcrumbsItems = new TagBuilder("ul");
                breadcrumbsItems.AddCssClass("breadcrumbs__items");

                for (var i = 0; i < breadcrumbsNavItems.Count; i++)
                {
                    NavItem breadcrumbNavItem = breadcrumbsNavItems[i];

                    TagBuilder breadcrumbItem = new TagBuilder("li");
                    breadcrumbItem.AddCssClass("breadcrumbs__item");

                    if (i != breadcrumbsNavItems.Count - 1)
                    {
                        TagBuilder breadcrumbLink = new TagBuilder("a");
                        breadcrumbLink.AddCssClass("breadcrumbs__link");
                        breadcrumbLink.Attributes.Add("href", Separator.Path + breadcrumbNavItem.VirtualPath);
                        breadcrumbLink.InnerHtml.Append(breadcrumbNavItem.DisplayName);
                        breadcrumbItem.InnerHtml.AppendHtml(breadcrumbLink);
                    }
                    else
                    {
                        TagBuilder breadcrumbText = new TagBuilder("span");
                        breadcrumbText.AddCssClass("breadcrumbs__text");
                        breadcrumbText.InnerHtml.Append(breadcrumbNavItem.DisplayName);
                        breadcrumbItem.InnerHtml.AppendHtml(breadcrumbText);
                    }

                    breadcrumbsItems.InnerHtml.AppendHtml(breadcrumbItem);
                }

                breadcrumbs.InnerHtml.AppendHtml(breadcrumbsItems);
            }

            return breadcrumbs;
        }
    }
}
