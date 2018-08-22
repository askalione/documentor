using Documentor.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Documentor.TagHelpers
{
    [HtmlTargetElement("external-links")]
    public class ExternalLinksTagHelper : TagHelper
    {
        private readonly ExternalLinksSettings _settings;
        private readonly UrlHelper _urlHelper;

        public ExternalLinksTagHelper(IOptionsSnapshot<AppConfig> configOptions,
            IActionContextAccessor actionContextAccessor)
        {
            if (configOptions == null)
                throw new ArgumentNullException(nameof(configOptions));
            if (actionContextAccessor == null)
                throw new ArgumentNullException(nameof(actionContextAccessor));

            _settings = configOptions.Value.ExternalLinks;
            _urlHelper = new UrlHelper(actionContextAccessor.ActionContext);
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            List<ExternalLink> links = new List<ExternalLink>()
            {
                new ExternalLink { Url = _settings.Github, Name = nameof(_settings.Github), UseImage = false },
                new ExternalLink { Url = _settings.Bitbucket, Name = nameof(_settings.Bitbucket), UseImage = false },
                new ExternalLink { Url = _settings.Nuget, Name = nameof(_settings.Nuget), UseImage = true },
                new ExternalLink { Url = _settings.Bower, Name = nameof(_settings.Bower), UseImage = true },
                new ExternalLink { Url = _settings.Npm, Name = nameof(_settings.Npm), UseImage = true },
                new ExternalLink { Url = _settings.Pypi, Name = nameof(_settings.Pypi), UseImage = true }
            };

            links = links
                .Where(x => !string.IsNullOrWhiteSpace(x.Url))
                .ToList();

            if (links.Count == 0)
            {
                output.SuppressOutput();
                return;
            }

            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("class", "share scroller__section");

            TagBuilder inner = new TagBuilder("div");
            inner.AddCssClass("share__inner");

            TagBuilder items = new TagBuilder("ul");
            items.AddCssClass("share__items");

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
                    itemIcon.Attributes.Add("src", _urlHelper.Content("~/images/icon-" + link.Name.ToLower() + ".svg"));
                else
                    itemIcon.AddCssClass("la la-" + link.Name.ToLower());

                itemLink.InnerHtml.AppendHtml(itemIcon);
                item.InnerHtml.AppendHtml(itemLink);
                items.InnerHtml.AppendHtml(item);
            }

            inner.InnerHtml.AppendHtml(items);
            output.Content.AppendHtml(inner);
        }

        class ExternalLink
        {
            public string Url { get; set; }
            public string Name { get; set; }
            public bool UseImage { get; set; }
        }
    }
}
