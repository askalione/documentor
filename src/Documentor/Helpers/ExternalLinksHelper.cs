using Documentor.Config;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Documentor.Helpers
{
    public static class ExternalLinksHelper
    {
        class ExternalLink
        {
            public string Url { get; set; }
            public string Name { get; set; }
            public bool UseImage { get; set; }
        }

        public static IHtmlContent ExternalLinks(this IHtmlHelper html, ExternalLinksSettings settings)
        {
            List<ExternalLink> links = new List<ExternalLink>()
            {
                new ExternalLink { Url = settings.Github, Name = nameof(settings.Github), UseImage = false },
                new ExternalLink { Url = settings.Bitbucket, Name = nameof(settings.Bitbucket), UseImage = false },
                new ExternalLink { Url = settings.Nuget, Name = nameof(settings.Nuget), UseImage = true },
                new ExternalLink { Url = settings.Bower, Name = nameof(settings.Bower), UseImage = true },
                new ExternalLink { Url = settings.Npm, Name = nameof(settings.Npm), UseImage = true },
                new ExternalLink { Url = settings.Pypi, Name = nameof(settings.Pypi), UseImage = true }
            };

            links = links
                .Where(x => !String.IsNullOrWhiteSpace(x.Url))
                .ToList();

            if (links.Count == 0)
                return HtmlString.Empty;

            TagBuilder wrapper = new TagBuilder("div");
            wrapper.AddCssClass("share scroller__section");

            TagBuilder inner = new TagBuilder("div");
            inner.AddCssClass("share__inner");

            TagBuilder items = new TagBuilder("ul");
            items.AddCssClass("share__items");

            var urlHelperFactory = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            var urlHelper = urlHelperFactory.GetUrlHelper(html.ViewContext);

            foreach (ExternalLink link in links)
            {
                TagBuilder item = new TagBuilder("li");
                item.AddCssClass("share__item");

                TagBuilder itemLink = new TagBuilder("a");
                itemLink.AddCssClass("share__link");
                itemLink.Attributes.Add("href", link.Url);
                itemLink.Attributes.Add("title", link.Name);

                TagBuilder itemIcon = new TagBuilder(link.UseImage ? "img" : "i");
                itemIcon.AddCssClass("share__icon share__icon--" + (link.UseImage ? "image" : "font"));
                if (link.UseImage)
                    itemIcon.Attributes.Add("src", urlHelper.Content("~/images/icon-" + link.Name.ToLower() + ".svg"));
                else
                    itemIcon.AddCssClass("la la-" + link.Name.ToLower());

                itemLink.InnerHtml.AppendHtml(itemIcon);
                item.InnerHtml.AppendHtml(itemLink);
                items.InnerHtml.AppendHtml(item);
            }

            inner.InnerHtml.AppendHtml(items);
            wrapper.InnerHtml.AppendHtml(inner);

            return wrapper;
        }
    }
}
