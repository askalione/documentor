using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using SmartBreadcrumbs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Documentor.TagHelpers
{
    [HtmlTargetElement("breadcrumbs")]
    public class BreadcrumbsTagHelper : TagHelper
    {
        public bool Show { get; set; } = true;

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        private readonly BreadcrumbsManager _breadcrumbsManager;
        private readonly UrlHelper _urlHelper;

        public BreadcrumbsTagHelper(BreadcrumbsManager breadcrumbsManager,
            IActionContextAccessor actionContextAccessor)
        {
            if (breadcrumbsManager == null)
                throw new ArgumentNullException(nameof(breadcrumbsManager));
            if (actionContextAccessor == null)
                throw new ArgumentNullException(nameof(actionContextAccessor));

            _breadcrumbsManager = breadcrumbsManager;
            _urlHelper = new UrlHelper(actionContextAccessor.ActionContext);
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!Show)
            {
                output.SuppressOutput();
                return;
            }

            string action = ViewContext.ActionDescriptor.RouteValues["action"];
            string controller = ViewContext.ActionDescriptor.RouteValues["controller"];

            var nodeKey = $"{controller}.{action}";
            var node = ViewContext.ViewData["BreadcrumbNode"] as BreadcrumbNode ?? _breadcrumbsManager.GetNode(nodeKey);

            if (node == null)
            {
                output.SuppressOutput();
                return;
            }

            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("class", "breadcrumbs");

            TagBuilder breadcrumbsHomeIcon = new TagBuilder("i");
            breadcrumbsHomeIcon.AddCssClass("la la-home breadcrumbs__home-icon");
            output.Content.AppendHtml(breadcrumbsHomeIcon);

            if (node.CacheTitle && node.Title.StartsWith("ViewData."))
                node.Title = ExtractTitle(node.Title);

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

            while (node != null && node != _breadcrumbsManager.DefaultNode)
            {
                breadcrumbItem = new TagBuilder("li");
                breadcrumbItem.AddCssClass("breadcrumbs__item");

                TagBuilder breadcrumbLink = new TagBuilder("a");
                breadcrumbLink.AddCssClass("breadcrumbs__link");

                RouteValueDictionary routes = new RouteValueDictionary(node.RouteValues);
                if (routes.ContainsKey("virtualPath"))
                    breadcrumbLink.Attributes.Add("href", "/" + routes["virtualPath"]); // Tip: Otherwise '/' escaped
                else
                    breadcrumbLink.Attributes.Add("href", node.GetUrl(_urlHelper));
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

            output.Content.AppendHtml(breadcrumbsItems);
        }

        private string ExtractTitle(string title)
        {
            if (!title.StartsWith("ViewData."))
                return title;

            string key = title.Substring(9);
            return ViewContext.ViewData.ContainsKey(key) ? ViewContext.ViewData[key].ToString() : key;
        }
    }
}
