using Documentor.Framework.Constants;
using Documentor.Framework.Navigation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Documentor.TagHelpers
{
    [HtmlTargetElement(Attributes = "nav-item")]
    public class NavItemTagHelper : TagHelper
    {
        public NavItem NavItem { get; set; } = default!;
        public bool IsFirst { get; set; }
        public bool IsSubItem { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } = default!;

        private const string _expandedItem = "sidebar-nav__item--expanded";
        private const string _activeItem = "sidebar-nav__item--active";

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var child = await output.GetChildContentAsync();

            string requestPath = ViewContext.HttpContext.Request.Path.ToString().Trim(Separator.Path);
            string editPart = "m/Edit";
            if (requestPath.StartsWith(editPart, StringComparison.OrdinalIgnoreCase))
                requestPath = requestPath.Remove(0, editPart.Length).Trim(Separator.Path);
            int level = NavItem.VirtualPath.Split(Separator.Path).Length;

            bool expanded = false;
            bool active = false;

            bool isPage = ViewContext.ViewBag?.IsPage ?? false;

            if (isPage)
            {
                if (!string.IsNullOrWhiteSpace(requestPath))
                {
                    expanded = requestPath.StartsWith(NavItem.VirtualPath, StringComparison.OrdinalIgnoreCase);
                    active = requestPath.Equals(NavItem.VirtualPath, StringComparison.OrdinalIgnoreCase);
                }
                else
                {
                    if (level == 1 && IsFirst)
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

            output.TagName = "li";
            output.Attributes.Add("class", $"sidebar-nav__{(IsSubItem ? "subitem" : "item")}" + navItemState);
            output.Content.AppendHtml(child);
        }
    }
}
