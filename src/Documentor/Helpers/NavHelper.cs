using Documentor.Constants;
using Documentor.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace Documentor.Helpers
{
    public static class NavHelper
    {
        private const string _expandedItem = "sidebar-nav__item--expanded";
        private const string _activeItem = "sidebar-nav__item--active";

        public static string NavItemState(this IHtmlHelper html, NavItem navItem, bool firstNavItem = false)
        {
            string requestPath = html.ViewContext.HttpContext.Request.Path.ToString().Trim(Separator.Path);
            int level = navItem.VirtualPath.Split(Separator.Path).Length;

            bool expanded = false;
            bool active = false;

            bool isPage = html.ViewBag?.IsPage ?? false;

            if (isPage)
            {
                if (!String.IsNullOrWhiteSpace(requestPath))
                {
                    expanded = requestPath.StartsWith(navItem.VirtualPath, StringComparison.OrdinalIgnoreCase);
                    active = requestPath.Equals(navItem.VirtualPath, StringComparison.OrdinalIgnoreCase);
                }
                else
                {
                    if (level == 1 && firstNavItem)
                    {
                        expanded = true;
                        active = true;
                    }
                }
            }

            string navItemState = "";
            if (expanded)
                navItemState += " " + _expandedItem;
            if (active)
                navItemState += " " + _activeItem;

            return navItemState;
        }
    }
}
